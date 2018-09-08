using System;

namespace BillScannerWPF {
	[Serializable]
	public class PurchaseHistory {
		public DateTime timePurchased { get; }
		public int amount { get; }
		public decimal price { get; }

		public PurchaseHistory(DateTime time, int amount, decimal price) {
			this.timePurchased = time;
			this.amount = amount;
			this.price = price;
		}
	}
}
