namespace Igor.BillScanner.Core.Database {

	/// <summary>
	/// Class representing a Purchase row in the database
	/// </summary>
	public class DbPurchase {


		public static string DName => nameof(DbPurchase).Substring(2);


		/// <summary>
		/// Primary key
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Foreign key to the <see cref="DbShop"/> table
		/// </summary>
		public int ShopID { get; set; }

		/// <summary>
		/// The date this purchase was made on, in string format
		/// </summary>
		public string Date { get; set; }

		/// <summary>
		/// Total price of the purchase
		/// </summary>
		public int TotalValue { get; set; }
	}
}
