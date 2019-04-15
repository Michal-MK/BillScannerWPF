using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Igor.BillScanner.Core.Rules;

namespace Igor.BillScanner.Core {

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

					Choices choice = await Services.Instance.UserInput.SelectChoiceAsync(
						$"Found Item named {split[i]} with {ocrLowestDist} character difference, closest: {items[ocrLowestIndex].ItemName}",
						Choices.MatchAnyway, Choices.MatchWithoutAddingAmbiguities, Choices.NotAnItem);

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

					Choices c = await Services.Instance.UserInput.SelectChoiceAsync($"This string is something else.. what is it??\n'{split[i]}'",
						Choices.FindExistingItemFromList, Choices.DefineNewItem, Choices.NotAnItem);

					if (c == Choices.DefineNewItem) {
						(string itemName, int itemPrice, MeassurementUnit itemUnitOfMeassure) = await Services.Instance.UserInput.DefineNewItemAsync(); //TODO Implement
						int quantity = await GetQuantityAsync(split, i, itemName);
						Item newItem = new Item(itemName, itemPrice);
						newItem.AddOCRNameNew(split[i]);

						UIItemCreationInfo newlyMatched = new UIItemCreationInfo(newItem, quantity, itemPrice, MatchRating.Success, split[i]);
						newlyMatched.Item.SetUnitOfMeassure(itemUnitOfMeassure);
						DatabaseAccess.access.WriteItemDefinitionToDatabase(newItem, purchaseTime);
						matchedItems.Add(newlyMatched);
					}
					else if (c == Choices.FindExistingItemFromList) {
						Item manuallyMatchedItem = await Services.Instance.UserInput.SelectOneItemFromListAsync(DatabaseAccess.access.GetItems(rules.shop)); //TODO Implement
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

			if (!timeFound) {
				purchaseTime = await GetPurchaseDateAsync();
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
				decimal data;
				Choices choice;
				if (fallbackItemIndex == -1) {
					(choice, data) = await Services.Instance.UserInput.GetDecimalInputAsIntAsync(
					$"Unable to get current item's price [{split[splitIndex]}]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.NOOP, Choices.ManuallyEnterPrice });
				}
				else {
					(choice, data) = await Services.Instance.UserInput.GetDecimalInputAsIntAsync(
						$"Unable to get current item's price [{split[splitIndex]}]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.UseLatestValue, Choices.ManuallyEnterPrice });
				}

				if (choice == Choices.UseLatestValue) {
					return DatabaseAccess.access.GetItems()[fallbackItemIndex].CurrentPriceInt;
				}
				else {
					return (int)data * 100;
				}
			}
		}

		private async Task<DateTime> GetPurchaseDateAsync() {
			return (await Services.Instance.UserInput.GetDateTimeInputAsync(
			"Parser could not find purchase date/time in the bill.",
								new Choices[] { Choices.NOOP, Choices.NOOP, Choices.UseCurrentTime, Choices.ManuallyEnterDate })).value;
		}

		private async Task<int> GetQuantityAsync(string[] split, int splitIndex, string itemName) {
			int quantity;
			try {
				quantity = rules.GetQuantity(split, splitIndex);
			}
			catch (QuantityParsingException) {
				quantity = (await Services.Instance.UserInput.GetIntInputAsync("Unable to get quantity of goods purchased: Item name '" + itemName + "'", Choices.ManuallyEnterQuantity)).value;
			}
			return quantity;
		}
	}
}