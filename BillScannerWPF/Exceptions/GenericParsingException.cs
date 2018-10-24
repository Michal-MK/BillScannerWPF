using System;

namespace BillScannerWPF.Rules {
	[Serializable]
	internal class GenericParsingException : Exception {
		public string[] parsedText { get; }
		public int index { get; }

		public GenericParsingException(string[] parsedText, int index) {
			this.parsedText = parsedText;
			this.index = index;
		}
	}
}