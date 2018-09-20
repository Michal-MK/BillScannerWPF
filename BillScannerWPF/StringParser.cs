using Igor.TCP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillScannerWPF {
	internal class StringParser {

		private Rules.IRuleset rules;

		private bool foundSomeKindOfMatch = false;

		internal StringParser(Rules.IRuleset rules) {
			this.rules = rules;
		}

		public ParsingResult Parse(string[] split) {
			ObservableCollection<UItemCreationInfo> matchedItems = new ObservableCollection<UItemCreationInfo>();
			ObservableCollection<UItemCreationInfo> unmatchedItems = new ObservableCollection<UItemCreationInfo>();

			//Process line by line
			Item[] items = MainWindow.access.GetItems();
			bool initiated = false;
			bool finalized = false;

			for (int i = 0; i < split.Length; i++) {
				bool matched = false;
				if (string.IsNullOrWhiteSpace(split[i])) {
					continue;
				}
				if (!initiated) {
					initiated = IsInitiatingString(split[i].ToLower().Trim());
					if (initiated) {
						continue;
					}
				}
				else {
					finalized = IsFinalizingString(split[i].ToLower().Trim());
				}
				if (!initiated) {
					continue;
				}
				if (finalized) {
					break;
				}
				int mainLowestDist = int.MaxValue;
				int mainLowestIndex = -1;

				int ocrLowestDist = int.MaxValue;
				int ocrLowestIndex = -1;

				for (int j = 0; j < items.Length; j++) {
					int mainNameDist = WordSimilarity.Compute(split[i], items[j].userFriendlyName);
					if (mainNameDist < mainLowestDist) {
						mainLowestDist = mainNameDist;
						mainLowestIndex = j;
					}
					if (mainNameDist == 0) {
						UItemCreationInfo lowest = new UItemCreationInfo(items[j], true, j, (MatchRating)mainNameDist);
						lowest.item.tirggerForMatch = split[i];
						matchedItems.Add(lowest);
						matched = true;
						break;
					}
				}
				if (matched) {
					continue;
				}
				else if (mainLowestDist <= 3) {
					UItemCreationInfo something = new UItemCreationInfo(items[mainLowestIndex], true, mainLowestIndex, (MatchRating)mainLowestDist);
					something.item.tirggerForMatch = split[i];
					matchedItems.Add(something);
					foundSomeKindOfMatch = true;
				}
				else {
					for (int j = 0; j < items.Length; j++) {
						foreach (string ss in items[j].ocrNames) {
							int currentOCRNameDist = WordSimilarity.Compute(split[i], ss);
							if (currentOCRNameDist < ocrLowestDist) {
								ocrLowestDist = currentOCRNameDist;
								ocrLowestIndex = j;
							}
							if (ocrLowestDist == 0) {
								UItemCreationInfo lowest = new UItemCreationInfo(items[ocrLowestIndex], true, ocrLowestIndex, (MatchRating)ocrLowestDist);
								lowest.item.tirggerForMatch = split[i];
								matchedItems.Add(lowest);
								matched = true;
								break;
							}
						}
					}
					if (matched) {
						continue;
					}
					if (ocrLowestDist <= 3) {
						UItemCreationInfo something = new UItemCreationInfo(items[ocrLowestIndex], true, ocrLowestIndex, (MatchRating)mainLowestDist);
						something.item.tirggerForMatch = split[i];
						matchedItems.Add(something);
						foundSomeKindOfMatch = true;
					}
				}
				if (!foundSomeKindOfMatch) {
					int min = Math.Min(mainLowestDist, ocrLowestDist);
					int selectedIndex = -1;
					if (min == mainLowestDist) {
						selectedIndex = mainLowestIndex;
					}
					else {
						selectedIndex = ocrLowestIndex;
					}

					if (min <= 6) {
						UItemCreationInfo unknown = new UItemCreationInfo(items[selectedIndex], false, selectedIndex, MatchRating.Fail);
						unknown.item.tirggerForMatch = split[i];
						unknown.item.ocrNames.Add(split[i]);
						unknown.item.pricesInThePast.Add(unknown.item.currentPrice);
						unmatchedItems.Add(unknown);
					}
					else {
						try {
							int indexCopy = i;
							UItemCreationInfo unknown = new UItemCreationInfo(new Item(split[i], rules.PriceOfOne(split, ref indexCopy)), false, i, MatchRating.Fail);
							unknown.item.tirggerForMatch = split[i];
							unknown.item.ocrNames.Add(split[i]);
							unknown.item.pricesInThePast.Add(unknown.item.currentPrice);
							unmatchedItems.Add(unknown);
							if(indexCopy != i) {
								i = indexCopy;
							}
						}
						catch (NotImplementedException e) {
							Console.WriteLine(e.Message);
						}
					}
				}
			}
			if (!initiated) {
				throw new ParsingEntryNotFoundException(rules.startMarkers, split);
			}
			return new ParsingResult(split, matchedItems, unmatchedItems);
		}

		private bool IsFinalizingString(string s) {
			for (int i = 0; i < rules.endMarkers.Length; i++) {
				if (s.Contains(rules.endMarkers[i])) {
					return true;
				}
			}
			return false;
		}

		private bool IsInitiatingString(string s) {
			if (rules.startMarkers.Length == 0) {
				Console.WriteLine("This shop does not have any start markers, attempting to match immedaitely");
				return true;
			}
			for (int i = 0; i < rules.startMarkers.Length; i++) {
				if (s.Contains(rules.startMarkers[i])) {
					return true;
				}
			}
			return false;
		}
	}

	public struct UItemCreationInfo {
		internal UItemCreationInfo(Item i, bool isRegistered, int index, MatchRating quality) {
			item = i;
			this.index = index;
			this.quality = quality;
			this.isRegistered = isRegistered;
		}

		internal Item item { get; }
		internal int index { get; }
		internal MatchRating quality { get; }
		internal bool isRegistered { get; private set; }
	}
}