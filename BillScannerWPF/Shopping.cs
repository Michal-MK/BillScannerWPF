using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace BillScannerWPF {
	[Serializable]
	public class Shopping {

		public DateTime date { get; }
		public decimal totalCost { get; private set; } = 0;
		public string[] purchasedItems { get; private set; }

		[NonSerialized]
		private List<string> internalList = new List<string>();

		public Shopping(DateTime date, ObservableCollection<UIItem> collection) {
			this.date = date;
			internalList = new List<string>();
			foreach (UIItem item in collection) {
				internalList.Add(item.asociatedItem.userFriendlyName);
			}
		}

		public void FinalizePurchase() {
			DatabaseAccess access = MainWindow.access;
			purchasedItems = internalList.ToArray();
			foreach (string userFriendlyName in purchasedItems) {
				Item i = access.GetItem(userFriendlyName);
				totalCost += i.currentPrice;
			}
		}
	}
}
