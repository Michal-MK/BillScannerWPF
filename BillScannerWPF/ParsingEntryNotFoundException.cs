using System;

namespace BillScannerWPF {
	[Serializable]
	internal class ParsingEntryNotFoundException : Exception {
		public string[] initiatingMarkers { get; }
		public string[] OCRdText { get; }

		public ParsingEntryNotFoundException(string[] startMarkers, string[] split) {
			this.initiatingMarkers = startMarkers;
			this.OCRdText = split;
		}
	}
}