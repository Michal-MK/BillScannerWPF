using BillScannerWPF.Rules;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using static BillScannerWPF.ManualResolveChoice;

namespace BillScannerWPF {
	internal class StringParser {

		private IRuleset rules;

		private bool foundSomeKindOfMatch = false;

		internal StringParser(IRuleset rules) {
			this.rules = rules;
		}

		public async Task<ParsingResult> ParseAsync(string[] split) {
			ObservableCollection<UIItemCreationInfo> matchedItems = new ObservableCollection<UIItemCreationInfo>();
			ObservableCollection<UIItemCreationInfo> unmatchedItems = new ObservableCollection<UIItemCreationInfo>();

			Item[] items = MainWindow.access.GetItems();
			bool initiated = false;
			bool finalized = false;
			bool purchaseTimeFound = false;

			bool rulesHandleItemLineSpan = rules.itemLineSpan == -1;

			DateTime purchaseTime = DateTime.MinValue;

			for (int i = 0; i < split.Length; i += rulesHandleItemLineSpan ? 1 : rules.itemLineSpan) {
				bool matched = false;

				if (string.IsNullOrWhiteSpace(split[i])) {
					continue;
				}

				if (!initiated) {
					initiated = IsInitiatingString(split[i].ToLower().Trim(), ref i);
					if (initiated && rules.skipInitiatingString) {
						continue;
					}
				}
				else {
					finalized = IsFinalizingString(split[i].ToLower().Trim());
				}

				if (!purchaseTimeFound) {
					if (rules.dateTimeFormat.IsMatch(split[i])) {
						if (DateTime.TryParseExact(split[i], "hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out purchaseTime)) {
							purchaseTimeFound = true;
						}
					}
					else {
						if (DateTime.TryParseExact(new BaseRuleset().ReplaceAmbiguousToNumber(split[i]), "hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out purchaseTime)) {
							purchaseTimeFound = true;
						}
					}
				}
				if (!initiated || finalized) {
					continue;
				}

				int ocrLowestDist = int.MaxValue;
				int ocrLowestIndex = -1;
				for (int j = 0; j < items.Length; j++) {
					foreach (string ss in items[j].ocrNames) {
						int currentOCRNameDist = WordSimilarity.Compute(split[i], ss);
						if (currentOCRNameDist < ocrLowestDist) {
							ocrLowestDist = currentOCRNameDist;
							ocrLowestIndex = j;
						}
						if (ocrLowestDist == 0) {
							decimal currentPrice = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
							long quantity = await GetQuantityAsync(split, i, items[ocrLowestIndex].userFriendlyName);

							UIItemCreationInfo lowest = new UIItemCreationInfo(items[ocrLowestIndex], true, quantity, ocrLowestIndex, currentPrice, (MatchRating)ocrLowestDist);
							lowest.item.tirggerForMatch = split[i];
							matchedItems.Add(lowest);
							matched = true;
							break;
						}
					}
					if (matched) {
						break;
					}
				}
				if (matched) {
					continue;
				}
				if (ocrLowestDist <= 3) {
					decimal currentPrice = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
					long quantity = await GetQuantityAsync(split, i, items[ocrLowestIndex].userFriendlyName);

					UIItemCreationInfo something = new UIItemCreationInfo(items[ocrLowestIndex], true, quantity, ocrLowestIndex, currentPrice, (MatchRating)ocrLowestDist);
					something.item.tirggerForMatch = split[i];
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
							long quantity = await GetQuantityAsync(split, i, items[ocrLowestIndex].userFriendlyName);
							UIItemCreationInfo creationInfo = new UIItemCreationInfo(items[ocrLowestIndex], true, quantity, ocrLowestIndex, currentPrice, MatchRating.Fail);
							creationInfo.item.tirggerForMatch = split[i];
							if (choice == Choices.MatchAnyway) {
								creationInfo.item.ocrNames.Add(split[i]);
								creationInfo.item.pricesInThePast.Add(creationInfo.item.currentPrice);
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
							(string itemName, decimal itemPrice, MeassurementUnit itemUnitOfMeassure) = await definition.RegisterItemAsync();
							long quantity = await GetQuantityAsync(split, i, itemName);
							Item newItem = new Item(itemName, itemPrice);
							UIItemCreationInfo nowKnown = new UIItemCreationInfo(newItem, false, quantity, i, itemPrice, MatchRating.Success);
							MainWindow.access.WriteItemDefinitionToDatabase(newItem);
							nowKnown.item.tirggerForMatch = split[i];
							nowKnown.item.ocrNames.Add(split[i]);
							nowKnown.item.SetUnitOfMeassure(itemUnitOfMeassure);
							matchedItems.Add(nowKnown);
						}
						else if (c == Choices.FindExistingItemFromList) {
							ItemList list = new ItemList(MainWindow.access.GetItems());
							Item manuallyMatchedItem = await list.SelectItemAsync();
							if (manuallyMatchedItem == null) {
								i -= rulesHandleItemLineSpan ? 1 : rules.itemLineSpan;
								continue;
							}
							long quantity = await GetQuantityAsync(split, i, manuallyMatchedItem.userFriendlyName);
							UIItemCreationInfo nowKnown = new UIItemCreationInfo(manuallyMatchedItem, false, quantity, i, manuallyMatchedItem.currentPrice, MatchRating.Success);
							nowKnown.item.tirggerForMatch = split[i];
							nowKnown.item.ocrNames.Add(split[i]);
							matchedItems.Add(nowKnown);
						}
					}
				}
			}
			if (!initiated) {
				throw new ParsingEntryNotFoundException(rules.startMarkers, split);
			}
			while (!purchaseTimeFound) {
				(bool parsed, DateTime result) = await GetPurchaseDateAsync();
				if (parsed) {
					purchaseTime = result;
					break;
				}
			}
			return new ParsingResult(split, matchedItems, unmatchedItems, new PurchaseMeta(purchaseTime));
		}

		private bool IsFinalizingString(string s) {
			for (int i = 0; i < rules.endMarkers.Length; i++) {
				if (s.Contains(rules.endMarkers[i])) {
					return true;
				}
			}
			return false;
		}

		private bool IsInitiatingString(string s, ref int index) {
			if (rules.startMarkers.Length == 0) {
				Debug.WriteLine("This shop does not have any start markers, attempting to match immediately");
				if (rules.skipInitiatingString) {
					index++;
				}
				return true;
			}
			for (int i = 0; i < rules.startMarkers.Length; i++) {
				if (s.Contains(rules.startMarkers[i])) {
					return true;
				}
			}
			return false;
		}

		private async Task<decimal> GetCurrentPriceAsync(string[] split, int splitIndex, int fallbackItemIndex) {
			try {
				return rules.PriceOfOne(split, ref splitIndex);
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
					return MainWindow.access.GetItems()[fallbackItemIndex].currentPrice;
				}
				else if (choice == Choices.ManuallyEnterPrice) {
					if (decimal.TryParse(res.MANUAL_RESOLUTION_Solution4_Box.Text.Replace(',', '.'), NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
						return result;
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
				if (DateTime.TryParseExact(resolveChoice.MANUAL_RESOLUTION_Solution4_Box.Text, "dd:MM:yyyy hh:mm:ss", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time)) {
					return (true, time);
				}
				else if (DateTime.TryParseExact(resolveChoice.MANUAL_RESOLUTION_Solution4_Box.Text, "dd:MM:yyyy", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time1)) {
					return (true, time1);
				}
				else if (DateTime.TryParseExact(resolveChoice.MANUAL_RESOLUTION_Solution4_Box.Text, "hh:mm:ss", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time2)) {
					return (true, new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, time2.Hour, time2.Minute, time2.Second));
				}
				else {
					return (false, DateTime.MinValue);
				}
			}
		}

		private async Task<long> GetQuantityAsync(string[] split, int index, string itemName) {
			long quantity;
			try {
				quantity = rules.GetQuantity(split, index);
			}
			catch (QuantityParsingException) {
				ManualResolveChoice choice2 = new ManualResolveChoice("Unable to get quantity of goods purchased: Item name '" + itemName + "'", Choices.ManuallyEnterQuantity);
				Choices c2 = await choice2.SelectChoiceAsync();
				while (!long.TryParse(choice2.MANUAL_RESOLUTION_Solution4_Box.Text, out quantity)) {
					await choice2.SelectChoiceAsync();
				}
			}
			return quantity;
		}
	}
}