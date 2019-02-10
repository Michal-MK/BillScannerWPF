using System;
using System.Collections.Generic;
using Core.Database;

namespace BillScannerCore {

	/// <summary>
	/// Class representing a singe purchase
	/// </summary>
	[Serializable]
	public class Purchase {

		/// <summary>
		/// Unique identifier of this purchase
		/// </summary>
		public int ID { get; private set; }

		/// <summary>
		/// The date this purchase was made
		/// </summary>
		public DateTime date { get; }

		/// <summary>
		/// Total amount paid for this purchase
		/// </summary>
		public int totalCost { get; private set; } = 0;

		/// <summary>
		/// List of item GUIDs that were bought
		/// </summary>
		public Item[] purchasedItems => DatabaseAccess.access.GetItems(ID);

		/// <summary>
		/// The Shop this purchase was made in
		/// </summary>
		public Shop shop { get; }

		private DbItemPurchase[] purchasedDbItems { get; set; }

		public Purchase(DbPurchase current) {
			ID = current.ID;
			date = DateTime.Parse(current.Date);
			shop = (Shop)current.ShopID;
		}


		/// <summary>
		/// Create a new <see cref="Purchase"/>
		/// </summary>
		/// <param name="date">The date this purchase was made</param>
		public Purchase(DateTime date, ItemPurchaseData[] collection) {
			this.date = date;
			purchasedDbItems = new DbItemPurchase[collection.Length];
			for (int i = 0; i < collection.Length; i++) {
				purchasedDbItems[i] = new DbItemPurchase() { Amount = collection[i].amount, ValuePerItem = collection[i].item.currentPrice };
			}
		}


		/// <summary>
		/// Preforms the actual writing to the database
		/// </summary>
		public void FinalizePurchase() {
			for (int i = 0; i < purchasedItems.Length; i++) {
				DatabaseAccess.access.AddNewPurchaseForItemToDatabase(purchasedDbItems[i].ItemID, new ItemPurchaseHistory(ID, purchasedDbItems[i].Amount, purchasedDbItems[i].ValuePerItem),date);
				totalCost += purchasedDbItems[i].Amount * purchasedDbItems[i].ValuePerItem;
			}
			DatabaseAccess.access.WriteNewPurchaseInstance(this);
		}
	}
}
