using System;

namespace BillScannerWPF.Rules {

	/// <summary>
	/// Exception thrown when the parser can not determine the quantity purchased in this shopping instance.
	/// </summary>
	[Serializable]
	internal class QuantityParsingException : Exception {

		/// <summary>
		/// The line at which the exception occurred
		/// </summary>
		public string line { get; }

		/// <summary>
		/// The index into the OCR'd text array
		/// </summary>
		public int arrayIndex { get; }

		public QuantityParsingException(string message, string line, int arrayIndex) : base(message) {
			this.line = line;
			this.arrayIndex = arrayIndex;
		}
	}
}