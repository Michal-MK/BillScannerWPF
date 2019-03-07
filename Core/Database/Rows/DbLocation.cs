namespace Igor.BillScanner.Core.Database {

	/// <summary>
	/// Class representing a Location row in the database
	/// </summary>
	public class DbLocation {


		public static string DName => nameof(DbLocation).Substring(2);

		/// <summary>
		/// Primary key
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// The city the shop is in
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// The street the shop is on
		/// </summary>
		public string Street { get; set; }
	}
}
