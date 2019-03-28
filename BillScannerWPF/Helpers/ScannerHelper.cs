using System.Collections.ObjectModel;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	internal static class ScannerHelper {
		public static ItemPurchaseData[] Transform(this ObservableCollection<UIItem> items) {
			ItemPurchaseData[] data = new ItemPurchaseData[items.Count];
			for (int i = 0; i < data.Length; i++) {
				data[i] = new ItemPurchaseData(items[i].Item, items[i].AmountPurchased);
			}
			return data;
		}
	}
}
