using System;

namespace Igor.BillScanner.Core.Rules {

	/// <summary>
	/// Exception thrown when the parser can not find an occurrence on a 'start marker' in the OCR'd text array.
	/// </summary>
	[Serializable]
	internal class ParsingEntryNotFoundException : Exception {

		/// <summary>
		/// The 'start markers' for this IRuleset
		/// </summary>
		public string[] InitiatingMarkers { get; }

		/// <summary>
		/// THe OCR'd text array
		/// </summary>
		public string[] OCRdText { get; }

		public ParsingEntryNotFoundException(string[] startMarkers, string[] split) {
			this.InitiatingMarkers = startMarkers;
			this.OCRdText = split;
		}
	}
}