using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace BillScannerCore {

	/// <summary>
	/// Class representing a singe purchase
	/// </summary>
	[Serializable]
	public class Purchase {

		/// <summary>
		/// Unique identifier of this purchase
		/// </summary>
		public string GUIDString { get; private set; }

		/// <summary>
		/// The date this purchase was made
		/// </summary>
		public DateTime date { get; }

		/// <summary>
		/// Total amount paid for this purchase
		/// </summary>
		public decimal totalCost { get; private set; } = 0;

		/// <summary>
		/// List of item GUIDs that were bought
		/// </summary>
		public string[] purchasedItems { get; private set; }

		[NonSerialized]
		private List<long> internalItemsBought;

		[JsonConstructor]
		public Purchase(string GUIDString, DateTime date, decimal totalCost, string[] purchasedItems) {
			this.GUIDString = GUIDString;
			this.date = date;
			this.totalCost = totalCost;
			this.purchasedItems = purchasedItems;
		}

		/// <summary>
		/// Create a new <see cref="Purchase"/>
		/// </summary>
		/// <param name="date">The date this purchase was made</param>
		/// <param name="collection">The items scanned from a bill visually represented by a <see cref="UIItem"/></param>
		public Purchase(DateTime date, ItemPurchaseData[] collection) {
			this.date = date;
			purchasedItems = new string[collection.Length];
			internalItemsBought = new List<long>(collection.Length);
			for (int i = 0; i < collection.Length; i++) {
				purchasedItems[i] = collection[i].item.identifier;
				internalItemsBought.Add(collection[i].quantityPurchased);
			}
		}

		/// <summary>
		/// Preforms the actual writing to the database
		/// </summary>
		public void FinalizePurchase() {
			GUIDString = Guid.NewGuid().ToString();
			for (int i = 0; i < purchasedItems.Length; i++) {
				Item item = DatabaseAccess.access.GetItem(purchasedItems[i]);
				DatabaseAccess.access.AddNewPurchaseForItemToDatabase(purchasedItems[i], new ItemPurchaseHistory(GUIDString, internalItemsBought[i], item.currentPrice));
				totalCost += item.currentPrice;
			}
			DatabaseAccess.access.WriteNewPurchaseInstance(this);
		}
	}
}
