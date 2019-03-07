using System;

namespace Igor.BillScanner.Core.Rules {

	/// <summary>
	/// Exception thrown when the parser can not determine the price of an Item from text.
	/// </summary>
	[Serializable]
	internal class PriceParsingException : Exception {
		/// <summary>
		/// The line at which the exception occurred
		/// </summary>
		public string Line { get; }

		/// <summary>
		/// The index into the OCR'd text array
		/// </summary>
		public int ArrayIndex { get; }

		/// <summary>
		/// Is this Item split into multiple lines, causing the price to be 'x' lines below the name?
		/// </summary>
		public bool IsSingleLine { get; }

		public PriceParsingException(string line, int arrayIndex, bool isSingleLine) {
			Line = line;
			ArrayIndex = arrayIndex;
			IsSingleLine = isSingleLine;
		}
	}
}