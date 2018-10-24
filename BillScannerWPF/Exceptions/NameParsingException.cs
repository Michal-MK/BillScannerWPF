using System;

namespace BillScannerWPF.Rules {

	/// <summary>
	/// Exception thrown when the parser can not determine an items name from a line.
	/// </summary>
	[Serializable]
	internal class NameParsingException : Exception {

		/// <summary>
		/// The line being parsed that threw the exception
		/// </summary>
		public string line { get; }

		public NameParsingException(string message, string line) : base(message) {
			this.line = line;
		}
	}
}