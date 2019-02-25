using System;
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
				if (_purchasedDbItems == null) {
					return DatabaseAccess.access.GetItems(ID);
				}
				throw new Exception("This purchase does not yet have any items registered to it. Purchase must be finalized first.");
			}
		}

		/// <summary>
		/// The Shop this purchase was made in
		/// </summary>
		public Shop Shop { get; }

		private ItemPurchaseData[] _purchasedDbItems;

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
			_purchasedDbItems = collection;
			Shop = shop;
			for (int i = 0; i < _purchasedDbItems.Length; i++) {
				TotalCost += _purchasedDbItems[i].Amount * _purchasedDbItems[i].Item.CurrentPriceInt;
			}
		}


		/// <summary>
		/// Preforms the actual writing to the database
		/// </summary>
		public void FinalizePurchase() {
			DatabaseAccess.access.RecordPurchase(this, _purchasedDbItems);
		}
	}
}
