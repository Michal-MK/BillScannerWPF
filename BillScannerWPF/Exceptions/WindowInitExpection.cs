using System;

internal enum InitExpectionType {
	SHOP_NOT_SELECTED
}

namespace BillScannerWPF {
	[Serializable]
	internal class WindowInitExpection : Exception {
		internal InitExpectionType type { get; }

		public WindowInitExpection(InitExpectionType exceptionType) {
			type = exceptionType;
		}
	}
}