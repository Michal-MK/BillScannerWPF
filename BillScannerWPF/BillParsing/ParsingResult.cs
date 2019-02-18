using BillScannerCore;
using System.Collections.ObjectModel;

namespace BillScannerWPF {

	/// <summary>
	/// Data holder for parsed results
	/// </summary>
	public class ParsingResult {

		/// <summary>
		/// Collection of successfully matched <see cref="Item"/>s
		/// </summary>
		public ObservableCollection<UIItemCreationInfo> matchedItems { get; }

		/// <summary>
		/// Collection of unknown <see cref="Item"/>s
		/// </summary>
		public ObservableCollection<UIItemCreationInfo> unknownItems { get; }

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
		public ParsingResult(string[] originalLines, ObservableCollection<UIItemCreationInfo> matchedItems,
							 ObservableCollection<UIItemCreationInfo> unknownItems, PurchaseMeta meta) {
			this.matchedItems = matchedItems;
			this.unknownItems = unknownItems;
			this.originalLines = originalLines;
			this.meta = meta;
		}
	}
}