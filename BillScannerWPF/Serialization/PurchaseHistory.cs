using System;

namespace BillScannerWPF {

	/// <summary>
	/// Class holding the information about each purchase of an item.
	/// </summary>
	[Serializable]
	public class PurchaseHistory {

		/// <summary>
		/// Unique identifier of this purchase
		/// </summary>
		public string purchaseGUID { get; }

		/// <summary>
		/// How many items of this type were bought at that time
		/// </summary>
		public long amount { get; }

		/// <summary>
		/// The amount of money the item cost at that time
		/// </summary>
		public decimal price { get; }

		public PurchaseHistory(string purchaseGUID, long amount, decimal price) {
			this.purchaseGUID = purchaseGUID;
			this.amount = amount;
			this.price = price;
		}
	}
}
