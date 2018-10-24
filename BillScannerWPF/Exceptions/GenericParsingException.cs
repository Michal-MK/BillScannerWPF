using System;

namespace BillScannerWPF.Rules {

	/// <summary>
	/// A generic parsing exception, thrown when any other more specific Exception would not make sense, or in rare cases.
	/// </summary>
	[Serializable]
	internal class GenericParsingException : Exception {

		/// <summary>
		/// The text being parsed that raised this exception
		/// </summary>
		public string[] parsedText { get; }

		/// <summary>
		/// The index in a parsed strings array at which the exception occurred
		/// </summary>
		public int index { get; }

		public GenericParsingException(string[] parsedText, int index) {
			this.parsedText = parsedText;
			this.index = index;
		}
	}
}