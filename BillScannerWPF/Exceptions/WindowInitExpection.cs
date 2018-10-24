using System;

/// <summary>
/// Type of exception that can prevent the application from starting correctly
/// </summary>
internal enum InitExpectionType {
	SHOP_NOT_SELECTED
}

namespace BillScannerWPF {

	/// <summary>
	/// Exception thrown on unsuccessful load into the main window
	/// </summary>
	[Serializable]
	internal class WindowInitExpection : Exception {

		/// <summary>
		/// The underlying type of exception that prevented the load
		/// </summary>
		internal InitExpectionType type { get; }

		public WindowInitExpection(InitExpectionType exceptionType) {
			type = exceptionType;
		}
	}
}