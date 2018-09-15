using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BillScannerWPF {
	[Serializable]
	public class Shopping {

		public DateTime date { get; }
		public decimal totalCost { get; private set; } = 0;
		public ItemSlim[] purchasedItems { get; private set; }

		[NonSerialized]
		private List<ItemSlim> internalList = new List<ItemSlim>();

		//public Shopping(DateTime date) {
		//	this.date = date;
		//}

		public Shopping(DateTime date, ICollection<ItemSlim> collection) {
			this.date = date;
			internalList = (List<ItemSlim>)collection;
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
			foreach (ItemSlim item in purchasedItems) {
				totalCost += item.price * item.amountPurchased;
			}
			//for (int i = 0; i < internalList.Count; i++) { 
			//	purchasedItems[i] = internalList[i];
			//}
			//internalList.Clear();
		}
	}
}
