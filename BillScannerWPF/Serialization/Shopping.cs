using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace BillScannerWPF {

	/// <summary>
	/// Class representing a singe purchase
	/// </summary>
	[Serializable]
	public class Shopping {

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
		private List<string> internalItemGUIDs;
		[NonSerialized]
		private List<long> internalItemsBought;

		[JsonConstructor]
		public Shopping(string GUIDString, DateTime date, decimal totalCost, string[] purchasedItems) {
			this.GUIDString = GUIDString;
			this.date = date;
			this.totalCost = totalCost;
			this.purchasedItems = purchasedItems;
		}

		/// <summary>
		/// Create a new purchase
		/// </summary>
		/// <param name="date">The date this purchase was made</param>
		/// <param name="collection">The items scanned from a bill visually represented by a <see cref="UIItem"/></param>
		public Shopping(DateTime date, ObservableCollection<UIItem> collection) {
			this.date = date;
			internalItemGUIDs = new List<string>(collection.Count);
			internalItemsBought = new List<long>(collection.Count);
			foreach (UIItem item in collection) {
				internalItemGUIDs.Add(item.asociatedItem.identifier);
				internalItemsBought.Add(item.quantityPurchased);
			}
		}

		/// <summary>
		/// Preforms the actual writing to the database
		/// </summary>
		public void FinalizePurchase() {
			GUIDString = Guid.NewGuid().ToString();
			purchasedItems = internalItemGUIDs.ToArray();
			for (int i = 0; i < purchasedItems.Length; i++) {
				Item item = MainWindow.access.GetItem(purchasedItems[i]);
				MainWindow.access.AddNewPurchaseForItemToDatabase(purchasedItems[i], new PurchaseHistory(GUIDString, internalItemsBought[i], item.currentPrice));
				totalCost += item.currentPrice;
			}
			MainWindow.access.WriteNewShoppingInstance(this);
		}
	}
}
