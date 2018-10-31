using System;
namespace BillScannerCore {

	/// <summary>
	/// Class holding the time at which the purchase was made and the time at which the bill was scanned into the database
	/// </summary>
	public class PurchaseMeta {

		/// <summary>
		/// The date this item was purchased at
		/// </summary>
		public DateTime purchasedAt;

		/// <summary>
		/// The date this item was scanned at
		/// </summary>
		public DateTime scannedAt;

		public PurchaseMeta(DateTime purchaseDate) {
			purchasedAt = purchaseDate;
			scannedAt = DateTime.Now;
		}
	}
}
