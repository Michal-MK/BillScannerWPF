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

		internal StringParser(Rules.IRuleset rules) {
			this.rules = rules;
		}

		public ParsingResult Parse(string OCRText) {
			ObservableCollection<UItemCreationInfo> matchedItems = new ObservableCollection<UItemCreationInfo>();
			ObservableCollection<UItemCreationInfo> unmatchedItems = new ObservableCollection<UItemCreationInfo>();
			string[] split = OCRText.Split('\n');

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
				int lowestD = int.MaxValue;
				int index = -1;
				for (int j = 0; j < items.Length; j++) {
					int mainNameDist = WordSimilarity.Compute(split[i], items[j].mainName);
					if (mainNameDist < lowestD) {
						lowestD = mainNameDist;
						index = j;
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
				if (lowestD <= 5) {
					UItemCreationInfo something = new UItemCreationInfo(items[index], true, index, (MatchRating)lowestD);
					something.item.tirggerForMatch = split[i];
					unmatchedItems.Add(something);
				}
				else {
					for (int j = 0; j < items.Length; j++) {
						foreach (string ss in items[j].ocrNames) {
							int ocrNamesDist = WordSimilarity.Compute(split[i], ss);
							if (ocrNamesDist < lowestD) {
								lowestD = ocrNamesDist;
								index = j;
							}
							if (ocrNamesDist == 0) {
								UItemCreationInfo lowest = new UItemCreationInfo(items[j], true, j, (MatchRating)ocrNamesDist);
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
					if (lowestD <= 5) {
						UItemCreationInfo something = new UItemCreationInfo(items[index], true, index, (MatchRating)lowestD);
						something.item.tirggerForMatch = split[i];
						unmatchedItems.Add(something);
					}
				}
				try {
					UItemCreationInfo unknown = new UItemCreationInfo(new Item(split[i], rules.PriceOfOne(split, i)), false, i, MatchRating.Fail);
					unknown.item.tirggerForMatch = split[i];
					unmatchedItems.Add(unknown);
				}
				catch (NotImplementedException e) {
					Console.WriteLine(e.Message);
					continue;
				}
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
