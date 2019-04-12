using System;

namespace Igor.BillScanner.Core {
	public class ParsingCompleteEventArgs : EventArgs {
		public ParsingCompleteEventArgs(ParsingResult result) {
			Result = result;
		}

		public ParsingResult Result { get; }
	}
}
