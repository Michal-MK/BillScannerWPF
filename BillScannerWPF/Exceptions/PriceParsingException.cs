using System;

namespace BillScannerWPF.Rules {
	[Serializable]
	internal class PriceParsingException : Exception {
		public string line { get; }
		public int arrayIndex { get; }
		public bool isSingleLine { get; }
		public PriceParsingException(string line, int arrayIndex, bool isSingleLine) {
			this.line = line;
			this.arrayIndex = arrayIndex;
			this.isSingleLine = isSingleLine;
		}
	}
}