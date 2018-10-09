using System;
namespace BillScannerWPF {
	public class PurchaseMeta {

		internal DateTime purchasedAt;
		internal DateTime scannedAt;

		public PurchaseMeta(DateTime purchaseDate) {
			purchasedAt = purchaseDate;
			scannedAt = DateTime.Now;
		}
	}
}
