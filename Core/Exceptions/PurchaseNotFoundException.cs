using System;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Exception thrown when attempting to find a purchase with ID that does not exist.
	/// </summary>
	[Serializable]
	internal class PurchaseNotFoundException : Exception {
		public PurchaseNotFoundException(string message) : base(message) { }
	}
}