using System;

namespace BillScannerCore {

	/// <summary>
	/// Class holding the information about each purchase of an item.
	/// </summary>
	[Serializable]
	public class ItemPurchaseHistory {

		/// <summary>
		/// Unique identifier of this purchase
		/// </summary>
		public int purchaseID { get; }

		/// <summary>
		/// How many items of this type were bought at that time
		/// </summary>
		public int amount { get; }

		/// <summary>
		/// The amount of money the item cost at that time
		/// </summary>
		public int price { get; }

		public ItemPurchaseHistory(int purchaseID, int amount, int price) {
			this.purchaseID = purchaseID;
			this.amount = amount;
			this.price = price;
		}
	}
}
