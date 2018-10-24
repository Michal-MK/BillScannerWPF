using System;

namespace BillScannerWPF.Rules {
	[Serializable]
	internal class NameParsingException : Exception {
		public string line { get; }

		public NameParsingException(string message, string line) : base(message) {
			this.line = line;
		}
	}
}