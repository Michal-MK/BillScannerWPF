using System.Collections.ObjectModel;

namespace BillScannerWPF {
	public class ParsingResult {
		public ObservableCollection<UIItemCreationInfo> parsed { get; }
		public ObservableCollection<UIItemCreationInfo> unknown { get; }
		public string[] originalLines { get; }
		public PurchaseMeta meta { get; }

		public ParsingResult(string[] lines, ObservableCollection<UIItemCreationInfo> mathched,
				ObservableCollection<UIItemCreationInfo> unmatched, PurchaseMeta meta) {
			parsed = mathched;
			unknown = unmatched;
			originalLines = lines;
			this.meta = meta;
		}
	}
}