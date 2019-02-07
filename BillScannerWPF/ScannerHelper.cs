using System.Collections.ObjectModel;
using BillScannerCore;

namespace BillScannerWPF {
	internal static class ScannerHelper {
		public static ItemPurchaseData[] Transform(this ObservableCollection<UIItem> items) {
			ItemPurchaseData[] data = new ItemPurchaseData[items.Count];
			for (int i = 0; i < data.Length; i++) {
				data[i] = items[i];
			}
			return data;
		}
	}
}
