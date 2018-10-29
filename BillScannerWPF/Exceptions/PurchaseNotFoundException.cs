using System;
using System.Runtime.Serialization;

namespace BillScannerWPF {

	/// <summary>
	/// Exception thrown when atempting to find a purchase with GUID that does not exist.
	/// </summary>
	[Serializable]
	internal class PurchaseNotFoundException : Exception {
		public PurchaseNotFoundException(string message) : base(message) { /*TODO*/ }
	}
}