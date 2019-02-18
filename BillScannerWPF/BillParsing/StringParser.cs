using BillScannerCore;
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
		/// Attempts to match items from the <see cref="string"/>[] from the very beginning, causing some unnecessary data to be processed,
		/// but can match items, event though the <see cref="string"/>[] does not contain a start marker.
		/// </summary>
		public bool tryMatchFromBeginning { get; internal set; } = false;

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

				bool foundSomeKindOfMatch = false;

				(Item closest, int ocrLowestIndex, int ocrLowestDist) = FindBestResult(split, i, items);

				if (ocrLowestDist <= 3) {
					decimal currentPrice = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
					int quantity = await GetQuantityAsync(split, i, items[ocrLowestIndex].userFriendlyName);

					UIItemCreationInfo something = new UIItemCreationInfo(items[ocrLowestIndex], true, quantity, currentPrice, (MatchRating)ocrLowestDist, split[i]);
					matchedItems.Add(something);
					foundSomeKindOfMatch = true;
				}

				if (!foundSomeKindOfMatch) {
					if (ocrLowestDist <= 6) {
						ManualResolveChoice resolveChoice = new ManualResolveChoice(
							string.Format("Found Item '{0}' with {1} char differences, closest Item: '{2}'", split[i], ocrLowestIndex, items[ocrLowestIndex].userFriendlyName),
							Choices.MatchAnyway, Choices.MatchWithoutAddingAmbiguities, Choices.NotAnItem);
						Choices choice = await resolveChoice.SelectChoiceAsync();

						if (choice != Choices.NotAnItem) {
							decimal currentPrice = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
							int quantity = await GetQuantityAsync(split, i, items[ocrLowestIndex].userFriendlyName);
							UIItemCreationInfo creationInfo = new UIItemCreationInfo(items[ocrLowestIndex], true, quantity, currentPrice, MatchRating.FivePlus, split[i]);
							if (choice == Choices.MatchAnyway) {
								creationInfo.item.ocrNames.Add(split[i]);
							}
							matchedItems.Add(creationInfo);
						}
					}
					else {
						ManualResolveChoice choice = new ManualResolveChoice("This string is something else.. what is it??\n'" + split[i] + "'",
														Choices.FindExistingItemFromList, Choices.DefineNewItem, Choices.NotAnItem);
						Choices c = await choice.SelectChoiceAsync();
						if (c == Choices.DefineNewItem) {
							NewItemDefinitionPanel definition = new NewItemDefinitionPanel();
							(string itemName, int itemPrice, MeassurementUnit itemUnitOfMeassure) = await definition.RegisterItemAsync();
							int quantity = await GetQuantityAsync(split, i, itemName);
							Item newItem = new Item(itemName, itemPrice);
							newItem.AddOCRName(split[i]);
							UIItemCreationInfo nowKnown = new UIItemCreationInfo(newItem, false, quantity, itemPrice, MatchRating.Success, split[i]);
							DatabaseAccess.access.WriteItemDefinitionToDatabase(newItem, purchaseTime);
							nowKnown.item.SetUnitOfMeassure(itemUnitOfMeassure);
							matchedItems.Add(nowKnown);
						}
						else if (c == Choices.FindExistingItemFromList) {
							ItemList list = new ItemList(DatabaseAccess.access.GetItems());
							Item manuallyMatchedItem = await list.SelectItemAsync();
							if (manuallyMatchedItem == null) {
								i--;
								continue;
							}
							int quantity = await GetQuantityAsync(split, i, manuallyMatchedItem.userFriendlyName);
							UIItemCreationInfo nowKnown = new UIItemCreationInfo(manuallyMatchedItem, false, quantity, manuallyMatchedItem.currentPrice, MatchRating.Success, split[i]);
							nowKnown.item.ocrNames.Add(split[i]);
							matchedItems.Add(nowKnown);
						}
						else if (c == Choices.NotAnItem) {
							unmatchedItems.Add(new UIItemCreationInfo(new Item(split[i], -1), false, -1, -1, MatchRating.Fail, ""));
						}
					}
				}
			}
			return new ParsingResult(split, matchedItems, unmatchedItems, new PurchaseMeta(purchaseTime));
		}

		private (Item, int ocrLowestIndex, int ocrLowestDist) FindBestResult(string[] split, int index, Item[] items) {
			int ocrLowestDist = int.MaxValue;
			int ocrLowestIndex = -1;

			for (int j = 0; j < items.Length; j++) {
				foreach (string ocrName in items[j].ocrNames) {
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
						timeFound = DateTime.TryParseExact(new BaseRuleset().ReplaceAmbiguousToNumber(split[i]), "hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out purchaseTime);
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

		/// <summary>
		/// Helper function to get items price from the string, calls <see cref="IRuleset.GetPriceOfOne(string[], ref int)"/> if it fails, falls back to <see cref="ManualResolveChoice"/>
		/// </summary>
		private async Task<int> GetCurrentPriceAsync(string[] split, int splitIndex, int fallbackItemIndex) {
			try {
				return rules.GetPriceOfOne(split, ref splitIndex);
			}
			catch (PriceParsingException) {
				ManualResolveChoice res;
				if (fallbackItemIndex == -1) {
					res = new ManualResolveChoice("Unable to get current item's price [" + split[splitIndex] + "]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.NOOP, Choices.ManuallyEnterPrice });
				}
				else {
					res = new ManualResolveChoice("Unable to get current item's price [" + split[splitIndex] + "]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.UseLatestValue, Choices.ManuallyEnterPrice });
				}
				Choices choice = await res.SelectChoiceAsync();
				if (choice == Choices.UseLatestValue) {
					return DatabaseAccess.access.GetItems()[fallbackItemIndex].currentPrice;
				}
				else if (choice == Choices.ManuallyEnterPrice) {
					if (decimal.TryParse(res.MANUAL_RESOLUTION_Solution5_Box.Text.Replace(',', '.'), NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
						return (int)result * 100;
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

		/// <summary>
		/// If the parser can not find a date/time of purchase, <see cref="GetPurchaseDateAsync"/> is used to get the date via <see cref="ManualResolveChoice"/>
		/// </summary>
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

		/// <summary>
		/// Helper function to get the number of items bought, calls <see cref="IRuleset.GetQuantity(string[], int)"/> if it fails, falls back to <see cref="ManualResolveChoice"/>
		/// </summary>
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