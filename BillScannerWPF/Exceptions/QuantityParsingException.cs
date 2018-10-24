using System;

namespace BillScannerWPF.Rules {
	[Serializable]
	internal class QuantityParsingException : Exception {
		public string line { get; }
		public int arrayIndex { get; }

		public QuantityParsingException(string message, string line, int arrayIndex) : base(message) {
			this.line = line;
			this.arrayIndex = arrayIndex;
		}
	}
}