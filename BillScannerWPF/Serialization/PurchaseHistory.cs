using System;

namespace BillScannerWPF {
	[Serializable]
	public class PurchaseHistory {
		public string purchaseGUID { get; }
		public long amount { get; }
		public decimal price { get; }

		public PurchaseHistory(string purchaseGUID, long amount, decimal price) {
			this.purchaseGUID = purchaseGUID;
			this.amount = amount;
			this.price = price;
		}
	}
}
