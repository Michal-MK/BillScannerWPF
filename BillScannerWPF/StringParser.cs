using BillScannerWPF.Rules;
using Igor.TCP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
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
			ObservableCollection<UItemCreationInfo> matchedItems = new ObservableCollection<UItemCreationInfo>();
			ObservableCollection<UItemCreationInfo> unmatchedItems = new ObservableCollection<UItemCreationInfo>();

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
				if (!initiated) {
					continue;
				}
				if (finalized) {
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
							decimal currentPrice = await TryGetCurrentPrice(split, i, ocrLowestIndex);
							UItemCreationInfo lowest = new UItemCreationInfo(items[ocrLowestIndex], true, rules.GetQuantity(split, i), ocrLowestIndex, currentPrice, (MatchRating)ocrLowestDist);
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
					decimal currentPrice = await TryGetCurrentPrice(split, i, ocrLowestIndex);
					UItemCreationInfo something = new UItemCreationInfo(items[ocrLowestIndex], true, rules.GetQuantity(split, i), ocrLowestIndex, currentPrice, (MatchRating)ocrLowestDist);
					something.item.tirggerForMatch = split[i];
					matchedItems.Add(something);
					foundSomeKindOfMatch = true;
				}

				if (!foundSomeKindOfMatch) {
					if (ocrLowestDist <= 6) {
						ManualResolveChoice resolveChoice = new ManualResolveChoice(
							string.Format("Found Item '{0}' with {1} char differences, closest Item: '{2}'", split[i], ocrLowestIndex, items[ocrLowestIndex].userFriendlyName),
							Choices.MatchAnyway, Choices.MatchWithoutAddingAmbiguities, Choices.NotAnItem);
						WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(resolveChoice);
						Choices choice = await resolveChoice.SelectChoice();
						WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(resolveChoice);

						if (choice != Choices.NotAnItem) {
							decimal currentPrice = await TryGetCurrentPrice(split, i, ocrLowestIndex);
							UItemCreationInfo creationInfo = new UItemCreationInfo(items[ocrLowestIndex], true, rules.GetQuantity(split, i), ocrLowestIndex, currentPrice, MatchRating.Fail);
							creationInfo.item.tirggerForMatch = split[i];
							if (choice == Choices.MatchAnyway) {
								creationInfo.item.ocrNames.Add(split[i]);
								creationInfo.item.pricesInThePast.Add(creationInfo.item.currentPrice);
							}
							matchedItems.Add(creationInfo);
						}
					}
					else {
						try {
							int indexCopy = i;
							decimal currentPrice = await TryGetCurrentPrice(split, indexCopy, -1);
							Item newItem = new Item(split[i], currentPrice);
							newItem.isSingleLine = indexCopy == i;
							UItemCreationInfo unknown = new UItemCreationInfo(newItem, false, rules.GetQuantity(split, i), i, currentPrice, MatchRating.Fail);
							unknown.item.tirggerForMatch = split[i];
							unknown.item.ocrNames.Add(split[i]);
							unmatchedItems.Add(unknown);
							if (indexCopy != i) {
								i = indexCopy;
							}
						}
						catch (NotImplementedException e) {
							ManualResolveChoice choice = new ManualResolveChoice("This string is someting else.. what is it??",
															Choices.FindExistingItemFromList, Choices.DefineNewItem, Choices.NotAnItem);
							WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(choice);
							Choices c = await choice.SelectChoice();
							WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(choice);
							if (c == Choices.DefineNewItem) {
								NewItemDefinitionPanel definition = new NewItemDefinitionPanel();
								(string itemName, decimal itemPrice, MeassurementUnit itemUnitOfMeassure) = await definition.RegisterItemAsync();
								UItemCreationInfo nowKnown = new UItemCreationInfo(new Item(itemName,itemPrice), false, rules.GetQuantity(split, i), i, itemPrice, MatchRating.Success);
								nowKnown.item.tirggerForMatch = split[i];
								nowKnown.item.ocrNames.Add(split[i]);
								nowKnown.item.SetUnitOfMeassure(itemUnitOfMeassure);
								matchedItems.Add(nowKnown);
							}
							else if (c == Choices.FindExistingItemFromList) {
								ItemList list = new ItemList(MainWindow.access.GetItems());
								Item manuallyMatchedItem = await list.SelectItemAsync();
								UItemCreationInfo nowKnown = new UItemCreationInfo(manuallyMatchedItem, false, rules.GetQuantity(split, i), i, manuallyMatchedItem.currentPrice, MatchRating.Success);
								nowKnown.item.tirggerForMatch = split[i];
								nowKnown.item.ocrNames.Add(split[i]);
								matchedItems.Add(nowKnown);
							}
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

		private async Task<decimal> TryGetCurrentPrice(string[] split, int i, int fallbackItemIndex) {
			try {
				return rules.PriceOfOne(split, ref i);
			}
			catch (NotImplementedException e) {
				ManualResolveChoice res;
				if (fallbackItemIndex == -1) {
					res = new ManualResolveChoice("Unable to get current item's price [" + split[i] + "]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.NOOP, Choices.ManuallyEnterPrice });
				}
				else {
					res = new ManualResolveChoice("Unable to get current item's price [" + split[i] + "]",
						new Choices[] { Choices.NOOP, Choices.NOOP, Choices.UseLatestValue, Choices.ManuallyEnterPrice });
				}
				WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(res);
				Choices choice = await res.SelectChoice();
				WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(res);
				if (choice == Choices.UseLatestValue) {
					return MainWindow.access.GetItems()[fallbackItemIndex].currentPrice;
				}
				else if (choice == Choices.ManuallyEnterPrice) {
					if (decimal.TryParse(res.MANUAL_RESOLUTION_Solution4_Box.Text.Replace(',', '.'), NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
						return result;
					}
					else {
						return await TryGetCurrentPrice(split, i, fallbackItemIndex);
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
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(resolveChoice);
			Choices choice = await resolveChoice.SelectChoice();
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(resolveChoice);
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
	}

	public struct UItemCreationInfo {
		internal UItemCreationInfo(Item i, bool isRegistered, long quantity, int index, decimal currentPrice, MatchRating quality) {
			item = i;
			this.index = index;
			this.quality = quality;
			this.isRegistered = isRegistered;
			this.quantity = quantity;
			this.currentPrice = currentPrice;
		}

		internal Item item { get; }
		internal int index { get; }
		internal long quantity { get; }
		internal MatchRating quality { get; }
		internal bool isRegistered { get; private set; }
		internal decimal currentPrice { get; }
	}
}