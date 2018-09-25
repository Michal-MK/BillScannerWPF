using System.Collections.ObjectModel;

namespace BillScannerWPF {
	public class ParsingResult {
		public ObservableCollection<UItemCreationInfo> parsed { get; }
		public ObservableCollection<UItemCreationInfo> unknown { get; }
		public string[] originalLines { get; }
		public PurchaseMeta meta { get; }

		public ParsingResult(string[] lines, ObservableCollection<UItemCreationInfo> mathched,
				ObservableCollection<UItemCreationInfo> unmatched, PurchaseMeta meta) {
			parsed = mathched;
			unknown = unmatched;
			originalLines = lines;
			this.meta = meta;
		}
	}
}