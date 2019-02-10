namespace Core.Database {

	/// <summary>
	/// Class representing a Shop row in the database
	/// </summary>
	public class DdShop {

		public static string DName => nameof(DdShop).Substring(2);


		/// <summary>
		/// Primary key
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// The name of the shop
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Foreign key to the <see cref="DbLocation"/> table
		/// </summary>
		public int LocationID { get; set; }
	}
}
