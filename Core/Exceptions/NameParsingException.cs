using System;

namespace Igor.BillScanner.Core.Rules {

	/// <summary>
	/// Exception thrown when the parser can not determine an items name from a line.
	/// </summary>
	[Serializable]
	internal class NameParsingException : Exception {

		/// <summary>
		/// The line being parsed that threw the exception
		/// </summary>
		public string Line { get; }

		public NameParsingException(string message, string line) : base(message) {
			Line = line;
		}
	}
}