namespace Igor.BillScanner.Core.Database {

	/// <summary>
	/// Class representing an Item row in the database
	/// </summary>
	public class DbItem {

		public static string DName => nameof(DbItem).Substring(2);

		/// <summary>
		/// Primary key
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Foreign key to the <see cref="DbShop"/> table
		/// </summary>
		public int ShopID { get; set; }

		/// <summary>
		/// Real name of the item
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The price of the item in the lowest nominal value
		/// </summary>
		public int Value { get; set; }
	}
}
