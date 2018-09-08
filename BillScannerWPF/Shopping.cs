using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BillScannerWPF {
	[Serializable]
	public class Shopping {

		public DateTime date { get; }
		public decimal totalCost { get; private set; } = 0;
		public SimpleItem[] purchasedItems { get; private set; }

		[NonSerialized]
		private List<SimpleItem> internalList = new List<SimpleItem>();

		//public Shopping(DateTime date) {
		//	this.date = date;
		//}

		public Shopping(DateTime date, ICollection<SimpleItem> collection) {
			this.date = date;
			internalList = (List<SimpleItem>)collection;
		}

		//public int itemsPurchased {
		//	get { return purchasedItems.Length; }
		//}

		//public void AddItem(Item i) {
		//	internalList.Add(i);
		//	totalCost += i.currentPrice;
		//}

		//public void AddItems(ICollection<Item> items) {
		//	internalList.AddRange(items);
		//}

		public void FinalizePurchase() {
			purchasedItems = internalList.ToArray();
			foreach (SimpleItem item in purchasedItems) {
				totalCost += item.price * item.amountPurchased;
			}
			//for (int i = 0; i < internalList.Count; i++) { 
			//	purchasedItems[i] = internalList[i];
			//}
			//internalList.Clear();
		}
	}
}
