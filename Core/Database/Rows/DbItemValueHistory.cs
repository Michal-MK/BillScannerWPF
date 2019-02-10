namespace Core.Database {

	/// <summary>
	/// Class representing an Item value row in the database
	/// </summary>
	public class DbItemValueHistory {


		public static string DName => nameof(DbItemValueHistory).Substring(2);


		/// <summary>
		/// Primary key, Foreign key to the <see cref="DbItem"/> table
		/// </summary>
		public int ItemID { get; set; }

		/// <summary>
		/// Date when this price was valid, in string format
		/// </summary>
		public string Date { get; set; }

		/// <summary>
		/// The price the item was sold at
		/// </summary>
		public int Value { get; set; }
	}
}
