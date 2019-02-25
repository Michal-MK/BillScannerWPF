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
		public int PurchaseID { get; }

		/// <summary>
		/// How many items of this type were bought at that time
		/// </summary>
		public int Amount { get; }

		/// <summary>
		/// The amount of money the item cost at that time
		/// </summary>
		public int Price { get; }

		public ItemPurchaseHistory(int purchaseID, int amount, int price) {
			PurchaseID = purchaseID;
			Amount = amount;
			Price = price;
		}
	}
}
