using System;

namespace Igor.BillScanner.Core.Rules {

	/// <summary>
	/// Exception thrown when the parser can not determine the quantity purchased in this shopping instance.
	/// </summary>
	[Serializable]
	internal class QuantityParsingException : Exception {

		/// <summary>
		/// The line at which the exception occurred
		/// </summary>
		public string Line { get; }

		/// <summary>
		/// The index into the OCR'd text array
		/// </summary>
		public int ArrayIndex { get; }

		public QuantityParsingException(string message, string line, int arrayIndex) : base(message) {
			Line = line;
			ArrayIndex = arrayIndex;
		}
	}
}