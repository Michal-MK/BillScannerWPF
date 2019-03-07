﻿using System.Collections.ObjectModel;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Data holder for parsed results
	/// </summary>
	public class ParsingResult {

		/// <summary>
		/// Collection of successfully matched <see cref="Item"/>s
		/// </summary>
		public ObservableCollection<UIItemCreationInfo> MachedItems { get; }

		/// <summary>
		/// Collection of unknown <see cref="Item"/>s
		/// </summary>
		public ObservableCollection<UIItemCreationInfo> UnknownItems { get; }

		/// <summary>
		/// Raw string lines from the OCR engine
		/// </summary>
		public string[] OriginalLines { get; }

		/// <summary>
		/// Other information about this paring result
		/// </summary>
		public PurchaseMeta Meta { get; }

		/// <summary>
		/// Create new <see cref="ParsingResult"/> with necessary containers and data
		/// </summary>
		public ParsingResult(string[] originalLines, ObservableCollection<UIItemCreationInfo> matchedItems,
							 ObservableCollection<UIItemCreationInfo> unknownItems, PurchaseMeta meta) {
			MachedItems = matchedItems;
			UnknownItems = unknownItems;
			OriginalLines = originalLines;
			Meta = meta;
		}
	}
}