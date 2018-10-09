using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace BillScannerWPF {
	[Serializable]
	public class Shopping {

		public string GUIDString { get; private set; }
		public DateTime date { get; }
		public decimal totalCost { get; private set; } = 0;
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

		public Shopping(DateTime date, ObservableCollection<UIItem> collection) {
			this.date = date;
			internalItemGUIDs = new List<string>(collection.Count);
			internalItemsBought = new List<long>(collection.Count);
			foreach (UIItem item in collection) {
				internalItemGUIDs.Add(item.asociatedItem.identifier);
				internalItemsBought.Add(item.quantityPurchased);
			}
		}

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
