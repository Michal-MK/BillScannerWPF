using System.Collections.ObjectModel;

namespace BillScannerWPF {

	/// <summary>
	/// Data holder for parsed results
	/// </summary>
	public class ParsingResult {

		/// <summary>
		/// Collection of successfully matched <see cref="Item"/>s
		/// </summary>
		public ObservableCollection<UIItemCreationInfo> parsed { get; }

		/// <summary>
		/// Collection of unknown <see cref="Item"/>s
		/// </summary>
		public ObservableCollection<UIItemCreationInfo> unknown { get; }

		/// <summary>
		/// Raw string lines from the OCR engine
		/// </summary>
		public string[] originalLines { get; }

		/// <summary>
		/// Other information about this paring result
		/// </summary>
		public PurchaseMeta meta { get; }

		/// <summary>
		/// Create new <see cref="ParsingResult"/> with necessary containers and data
		/// </summary>
		public ParsingResult(string[] lines, ObservableCollection<UIItemCreationInfo> matched,
				ObservableCollection<UIItemCreationInfo> unmatched, PurchaseMeta meta) {
			parsed = matched;
			unknown = unmatched;
			originalLines = lines;
			this.meta = meta;
		}
	}
}