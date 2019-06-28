using System;
using Igor.BillScanner.Core.Database;

namespace Igor.BillScanner.Core {

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
		public DateTime PurchaseDate { get; }

		/// <summary>
		/// Total amount paid for this purchase
		/// </summary>
		public int TotalCost { get; private set; } = 0;

		/// <summary>
		/// List of items that were bought
		/// </summary>
		public Item[] AsociatedItems {
			get {
				if (PurchasedDbItems == null) {
					return DatabaseAccess.Access.GetItems(ID);
				}
				throw new Exception("This purchase does not yet have any items registered to it. Purchase must be finalized first.");
			}
		}

		/// <summary>
		/// The Shop this purchase was made in
		/// </summary>
		public Shop Shop { get; }

		public ItemPurchaseData[] PurchasedDbItems { get; }

		/// <summary>
		/// Create a purchase from database entry
		/// </summary>
		public Purchase(DbPurchase current) {
			ID = current.ID;
			PurchaseDate = DateTime.Parse(current.Date);
			Shop = (Shop)current.ShopID;
		}


		/// <summary>
		/// Create a new <see cref="Purchase"/>
		/// </summary>
		/// <param name="date">The date this purchase was made</param>
		public Purchase(Shop shop, DateTime date, ItemPurchaseData[] collection) {
			PurchaseDate = date;
			PurchasedDbItems = collection;
			Shop = shop;
			for (int i = 0; i < PurchasedDbItems.Length; i++) {
				TotalCost += PurchasedDbItems[i].Amount * PurchasedDbItems[i].Item.CurrentPriceInt;
			}
		}


		/// <summary>
		/// Preforms the actual writing to the database
		/// </summary>
		public void FinalizePurchase() {
			DatabaseAccess.Access.RecordPurchase(this, PurchasedDbItems);
		}
	}
}
