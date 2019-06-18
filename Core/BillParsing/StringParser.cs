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
			ObservableCollection<UIItemViewModel> matchedItems = new ObservableCollection<UIItemViewModel>();
			ObservableCollection<UIItemViewModel> unmatchedItems = new ObservableCollection<UIItemViewModel>();

			Item[] items = DatabaseAccess.Access.GetItems(rules.Shop);
			DateTime purchaseTime = DateTime.MinValue;
			(split, purchaseTime) = await CropSplit(split);


			for (int i = 0; i < split.Length; i += 1) {

				(Item closest, int ocrLowestIndex, int ocrLowestDist) = FindBestResult(split, i, items);

				if (ocrLowestDist <= 3) {
					(int incrementA, int currentPrice) = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
					(int incrementB, int quantity) = await GetQuantityAsync(split, i, items[ocrLowestIndex].ItemName);
					if (incrementA == 1 || incrementB == 1) {
						i++;
					}

					UIItemViewModel match = new UIItemViewModel(new ItemPurchaseData(items[ocrLowestIndex], quantity), currentPrice, (MatchRating)ocrLowestDist, split[i]);
					matchedItems.Add(match);
					continue;
				}

				if (ocrLowestDist <= 6) {
					Choices selected = (Choices)(-1);
					await Services.Instance.UserInput.PressOneOf(
						$"Found Item named {split[i]} with {ocrLowestDist} character difference, closest: {items[ocrLowestIndex].ItemName}",
						("Match anyway", new Command(() => selected = Choices.MatchAnyway)),
						("Match but dont register this text as valid item name", new Command(() => selected = Choices.MatchWithoutAddingAmbiguities)),
						("This text should be ignored", new Command(() => selected = Choices.NotAnItem)));

					if (selected != Choices.NotAnItem) {
						(int incrementA, int currentPrice) = await GetCurrentPriceAsync(split, i, ocrLowestIndex);
						(int incrementB, int quantity) = await GetQuantityAsync(split, i, items[ocrLowestIndex].ItemName);
						if (incrementA == 1 || incrementB == 1) {
							i++;
						}

						UIItemViewModel creationInfo = new UIItemViewModel(new ItemPurchaseData(items[ocrLowestIndex], quantity), currentPrice, MatchRating.FivePlus, split[i]);
						if (selected == Choices.MatchAnyway) {
							creationInfo.ItemPurchase.Item.OcrNames.Add(split[i]);
						}
						matchedItems.Add(creationInfo);
					}
				}
				else {
					Choices selected = (Choices)(-1);
					await Services.Instance.UserInput.PressOneOf($"This string is something else.. what is it??\n'{split[i]}'",
						("Find the Item in database", new Command(() => selected = Choices.FindExistingItemFromList)),
						("Define new item", new Command(() => selected = Choices.DefineNewItem)),
						("This text should be ignored", new Command(() => selected = Choices.NotAnItem)));

					if (selected == Choices.DefineNewItem) {
						NewItemDefViewModel result = await Services.Instance.UserInput.DefineNewItemAsync();

						if (result == null) {
							i--;
							continue;
						}

						(int increment, int quantity) = await GetQuantityAsync(split, i, result.ItemName);
						if (increment == 1) {
							i++;
						}

						Item newItem = new Item(result.ItemName, result.DatabaseItemValue.Value);
						newItem.AddOCRNameNew(split[i]);

						UIItemViewModel newlyMatched = new UIItemViewModel(new ItemPurchaseData(newItem, quantity), result.DatabaseItemValue.Value, MatchRating.Success, split[i]);
						newlyMatched.ItemPurchase.Item.SetUnitOfMeassure(result.SelectedMeassureUnit);
						newItem.SetNewID(result.AssignedID);
						matchedItems.Add(newlyMatched);
					}
					else if (selected == Choices.FindExistingItemFromList) {
						Item manuallyMatchedItem = await Services.Instance.UserInput.SelectOneItemFromListAsync(DatabaseAccess.Access.GetItems(rules.Shop));

						if (manuallyMatchedItem == null) {
							i--;
							continue;
						}

						(int increment, int quantity) = await GetQuantityAsync(split, i, manuallyMatchedItem.ItemName);
						if (increment == 1) {
							i++;
						}
						UIItemViewModel fromExistingMatched = new UIItemViewModel(
							new ItemPurchaseData(manuallyMatchedItem, quantity), 
							manuallyMatchedItem.CurrentPriceInt, MatchRating.Success, split[i]);

						fromExistingMatched.ItemPurchase.Item.OcrNames.Add(split[i]);
						matchedItems.Add(fromExistingMatched);
					}
					else if (selected == Choices.NotAnItem) {
						unmatchedItems.Add(new UIItemViewModel(new ItemPurchaseData(new Item(split[i], -1), -1), -1, MatchRating.Fail, split[i]));
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
				for (int j = 0; j < rules.StartMarkers.Length; j++) {
					if (rules.StartMarkers[j] == split[i]) {
						startIndex = i;
					}
				}
				for (int j = 0; j < rules.EndMarkers.Length; j++) {
					if (rules.EndMarkers[j] == split[i]) {
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

		private async Task<(int, int)> GetCurrentPriceAsync(string[] split, int splitIndex, int fallbackItemIndex) {
			try {
				return rules.GetPriceOfOne(split, splitIndex);
			}
			catch (PriceParsingException) {
				int? data;
				if (fallbackItemIndex == -1) {
					data = await Services.Instance.UserInput.GetDecimalInputAsIntAsync(
										$"Unable to get current item's price [{split[splitIndex]}]");
				}
				else {
					data = await Services.Instance.UserInput.GetDecimalInputAsIntAsync(
						$"Unable to get current item's price [{split[splitIndex]}]",
						DatabaseAccess.Access.GetItems()[fallbackItemIndex].CurrentPriceInt);
				}

				if (!data.HasValue) {
					return (0, DatabaseAccess.Access.GetItems()[fallbackItemIndex].CurrentPriceInt);
				}
				else {
					return (0, (int)data * 100);
				}
			}
		}

		private async Task<DateTime> GetPurchaseDateAsync() {
			return await Services.Instance.UserInput.GetDateTimeInputAsync(
			"Parser could not find purchase date/time in the bill.", true);
		}

		private async Task<(int, int)> GetQuantityAsync(string[] split, int splitIndex, string itemName) {
			int quantity;
			int increment;
			try {
				(increment, quantity) = rules.GetQuantity(split, splitIndex);
			}
			catch (QuantityParsingException) {
				increment = 0;
				quantity = await Services.Instance.UserInput.GetIntInputAsync($"Unable to get quantity of goods purchased: Item name '{itemName}'");
			}
			return (increment, quantity);
		}
	}
}