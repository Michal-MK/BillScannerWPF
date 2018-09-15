using System.Collections.ObjectModel;

namespace BillScannerWPF {
	public class ParsingResult {
		public ObservableCollection<UItemCreationInfo> parsed { get; }
		public ObservableCollection<UItemCreationInfo> unknown { get; }
		public string[] originalLines { get; }

		public ParsingResult(string[] lines, ObservableCollection<UItemCreationInfo> mathched, ObservableCollection<UItemCreationInfo> unmatched) {
			parsed = mathched;
			unknown = unmatched;
			originalLines = lines;
		}
	}
}