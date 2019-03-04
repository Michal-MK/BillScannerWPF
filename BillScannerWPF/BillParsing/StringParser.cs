﻿using BillScannerCore;
using BillScannerWPF.Rules;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace BillScannerWPF {

	/// <summary>
	/// Class parsing the generated string arrays from our OCR engine.
	/// </summary>
	internal class StringParser {

		private IRuleset rules;

		/// <summary>
		/// Create new <see cref="StringParser"/> with selected <see cref="Shop"/>'s rule-set
		/// </summary>
		internal StringParser(IRuleset rules) {
			this.rules = rules;
		}

		/// <summary>
		/// Main parsing function, goes though the entire array and interprets the lines by comparing matches with the database entries
		/// </summary>
		public async Task<ParsingResult> ParseAsync(string[] split) {
			ObservableCollection<UIItemCreationInfo> matchedItems = new ObservableCollection<UIItemCreationInfo>();
			ObservableCollection<UIItemCreationInfo> unmatchedItems = new ObservableCollection<UIItemCreationInfo>();

			Item[] items = DatabaseAccess.access.GetItems(rules.shop);
			DateTime purchaseTime = DateTime.MinValue;
			(split, purchaseTime) = await CropSplit(split);


			for (int i = 0; i < split.Length; i += 1) {

				(Item closest, int ocrLowestIndex, int ocrLowestDist) = FindBestResult(split, i, items);

				if (ocrLowestDist <= 3) {
					int currentPrice = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
					int quantity = await GetQuantityAsync(split, i, items[ocrLowestIndex].ItemName);

					UIItemCreationInfo match = new UIItemCreationInfo(items[ocrLowestIndex], quantity, currentPrice, (MatchRating)ocrLowestDist, split[i]);
					matchedItems.Add(match);
					continue;
				}

				if (ocrLowestDist <= 6) {
					ManualResolveChoice resolveChoice = new ManualResolveChoice(
						$"Found Item named {split[i]} with {ocrLowestIndex} character difference, closest: {items[ocrLowestIndex].ItemName}",
						Choices.MatchAnyway, Choices.MatchWithoutAddingAmbiguities, Choices.NotAnItem);

					Choices choice = await resolveChoice.SelectChoiceAsync();

					if (choice != Choices.NotAnItem) {
						int currentPrice = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
						int quantity = await GetQuantityAsync(split, i, items[ocrLowestIndex].ItemName);
						UIItemCreationInfo creationInfo = new UIItemCreationInfo(items[ocrLowestIndex], quantity, currentPrice, MatchRating.FivePlus, split[i]);
						if (choice == Choices.MatchAnyway) {
							creationInfo.Item.OcrNames.Add(split[i]);
						}
						matchedItems.Add(creationInfo);
					}
				}
				else {
					if (rules.correctCostAndQuantityLine.IsMatch(split[i])) {
						//Found a price/quantity line, safely ignore
						continue;
					}
					ManualResolveChoice choice = new ManualResolveChoice(
						$"This string is something else.. what is it??\n'{split[i]}'",
						Choices.FindExistingItemFromList, Choices.DefineNewItem, Choices.NotAnItem);

					Choices c = await choice.SelectChoiceAsync();

					if (c == Choices.DefineNewItem) {
						NewItemDefinitionPanel definition = new NewItemDefinitionPanel();
						(string itemName, int itemPrice, MeassurementUnit itemUnitOfMeassure) = await definition.RegisterItemAsync();
						int quantity = await GetQuantityAsync(split, i, itemName);
						Item newItem = new Item(itemName, itemPrice);
						newItem.AddOCRNameNew(split[i]);

						UIItemCreationInfo newlyMatched = new UIItemCreationInfo(newItem, quantity, itemPrice, MatchRating.Success, split[i]);
						newlyMatched.Item.SetUnitOfMeassure(itemUnitOfMeassure);
						DatabaseAccess.access.WriteItemDefinitionToDatabase(newItem, purchaseTime);
						matchedItems.Add(newlyMatched);
					}
					else if (c == Choices.FindExistingItemFromList) {
						ItemList list = new ItemList();
						list.AddItems(DatabaseAccess.access.GetItems(rules.shop));
						Item manuallyMatchedItem = await list.SelectItemAsync();
						if (manuallyMatchedItem == null) {
							i--;
							// Basically reprocess this item as if this never happened
							continue;
						}
						int quantity = await GetQuantityAsync(split, i, manuallyMatchedItem.ItemName);
						UIItemCreationInfo fromExistingMatched = new UIItemCreationInfo(manuallyMatchedItem, quantity, manuallyMatchedItem.CurrentPriceInt, MatchRating.Success, split[i]);
						fromExistingMatched.Item.OcrNames.Add(split[i]);
						matchedItems.Add(fromExistingMatched);
					}
					else if (c == Choices.NotAnItem) {
						unmatchedItems.Add(new UIItemCreationInfo(new Item(split[i], -1), -1, -1, MatchRating.Fail, ""));
					}
				}
			}
			return new ParsingResult(split, matchedItems, unmatchedItems, new PurchaseMeta(purchaseTime));
		}

		private (Item, int ocrLowestIndex, int ocrLowestDist) FindBestResult(string[] split, int index, Item[] items) {
			int ocrLowestDist = int.MaxValue;
			int ocrLowestIndex = -1;

			for (int j = 0; j < items.Length; j++) {
				foreach (string ocrName in items[j].OcrNames) {
					int currentOCRNameDist = WordSimilarity.Compute(split[index], ocrName);
					if (currentOCRNameDist < ocrLowestDist) {
						ocrLowestDist = currentOCRNameDist;
						ocrLowestIndex = j;
					}
					if (ocrLowestDist == 0) {
						return (items[ocrLowestIndex], ocrLowestIndex, ocrLowestDist);
					}
				}
			}
			return (items[ocrLowestIndex], ocrLowestIndex, ocrLowestDist);
		}


		/// <summary>
		/// Remove the part of the bill that is not Items
		/// </summary>
		private async Task<(string[] splitModified, DateTime purchaseTime)> CropSplit(string[] split) {
			int startIndex = 0;
			int endIndex = split.Length;
			bool timeFound = false;

			DateTime purchaseTime = DateTime.MinValue;

			for (int i = 0; i < split.Length; i++) {
				for (int j = 0; j < rules.startMarkers.Length; j++) {
					if (rules.startMarkers[j] == split[i]) {
						startIndex = i;
					}
				}
				for (int j = 0; j < rules.endMarkers.Length; j++) {
					if (rules.endMarkers[j] == split[i]) {
						endIndex = i;
					}
				}
				if (!timeFound) {
					timeFound = DateTime.TryParseExact(split[i], "hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out purchaseTime);
					if (!timeFound) {
						timeFound = DateTime.TryParseExact((rules as BaseRuleset).ReplaceAmbiguousToNumber(split[i]), "hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out purchaseTime);
					}
				}
			}
			if (startIndex >= endIndex) {
				purchaseTime = DateTime.MinValue;
				return (split, purchaseTime);
			}

			while (!timeFound) {
				(bool parsed, DateTime result) = await GetPurchaseDateAsync();
				if (parsed) {
					purchaseTime = result;
					break;
				}
			}

			string[] ret = new string[endIndex - startIndex];

			Array.Copy(split, startIndex, ret, 0, ret.Length);
			return (ret, purchaseTime);
		}

		private async Task<int> GetCurrentPriceAsync(string[] split, int splitIndex, int fallbackItemIndex) {
			try {
				return rules.GetPriceOfOne(split, ref splitIndex);
			}
			catch (PriceParsingException) {
				ManualResolveChoice res;
				if (fallbackItemIndex == -1) {
					res = new ManualResolveChoice($"Unable to get current item's price [{split[splitIndex]}]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.NOOP, Choices.ManuallyEnterPrice });
				}
				else {
					res = new ManualResolveChoice($"Unable to get current item's price [{split[splitIndex]}]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.UseLatestValue, Choices.ManuallyEnterPrice });
				}
				Choices choice = await res.SelectChoiceAsync();
				if (choice == Choices.UseLatestValue) {
					return DatabaseAccess.access.GetItems()[fallbackItemIndex].CurrentPriceInt;
				}
				else if (choice == Choices.ManuallyEnterPrice) {
					if (decimal.TryParse(res.MANUAL_RESOLUTION_Solution5_Box.Text, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result)) {
						return (int)(result * 100);
					}
					else {
						return await GetCurrentPriceAsync(split, splitIndex, fallbackItemIndex);
					}
				}
				else {
					throw new Exception("StringParser exception at TryGetCurrentPrice()");
				}
			}
		}

		private async Task<(bool, DateTime)> GetPurchaseDateAsync() {
			ManualResolveChoice resolveChoice = new ManualResolveChoice("Parser could not find purchase date/time in the bill.",
								new Choices[] { Choices.NOOP, Choices.NOOP, Choices.UseCurrentTime, Choices.ManuallyEnterDate });
			Choices choice = await resolveChoice.SelectChoiceAsync();
			if (choice == Choices.UseCurrentTime) {
				return (true, DateTime.Now);
			}
			else {
				if (DateTime.TryParseExact(resolveChoice.MANUAL_RESOLUTION_Solution5_DateBox.DATEBOX_Input_Box.Text, "dd:MM:yyyy HH:mm:ss", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time)) {
					return (true, time);
				}
				else if (DateTime.TryParseExact(resolveChoice.MANUAL_RESOLUTION_Solution5_DateBox.DATEBOX_Input_Box.Text, "dd:MM:yyyy", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time1)) {
					return (true, time1);
				}
				else if (DateTime.TryParseExact(resolveChoice.MANUAL_RESOLUTION_Solution5_Box.Text, "HH:mm:ss", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time2)) {
					return (true, new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, time2.Hour, time2.Minute, time2.Second));
				}
				else {
					return (false, DateTime.MinValue);
				}
			}
		}

		private async Task<int> GetQuantityAsync(string[] split, int splitIndex, string itemName) {
			int quantity;
			try {
				quantity = rules.GetQuantity(split, splitIndex);
			}
			catch (QuantityParsingException) {
				ManualResolveChoice choice2 = new ManualResolveChoice("Unable to get quantity of goods purchased: Item name '" + itemName + "'", Choices.ManuallyEnterQuantity);
				Choices c2 = await choice2.SelectChoiceAsync();
				while (!int.TryParse(choice2.MANUAL_RESOLUTION_Solution5_Box.Text, out quantity)) {
					await choice2.SelectChoiceAsync();
				}
			}
			return quantity;
		}
	}
}