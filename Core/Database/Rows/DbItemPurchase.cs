namespace Core.Database {

	/// <summary>
	/// Class representing an Item purchase row in the database
	/// </summary>
	public class DbItemPurchase {


		public static string DName => nameof(DbItemPurchase).Substring(2);

		/// <summary>
		/// Primary key, Foreign key to the <see cref="DbPurchase"/> table
		/// </summary>
		public int PurchaseID { get; set; }

		/// <summary>
		/// Primary key, Foreign key to the <see cref="DbItem"/> table
		/// </summary>
		public int ItemID { get; set; }

		/// <summary>
		/// How many items were bought
		/// </summary>
		public int Amount { get; set; }

		/// <summary>
		/// How much did one Item cost
		/// </summary>
		public int ValuePerItem { get; set; }
	}
}
