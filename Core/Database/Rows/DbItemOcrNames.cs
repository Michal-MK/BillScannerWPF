namespace Igor.BillScanner.Core.Database {
	/// <summary>
	/// Class representing an Item OCR names row in the database
	/// </summary>
	public class DbItemOcrNames {

		public static string DName => nameof(DbItemOcrNames).Substring(2);

		/// <summary>
		/// Primary key, FOreign key to the <see cref="DbItem"/> table
		/// </summary>
		public int ItemID { get; set; }

		/// <summary>
		/// OCR name of the item
		/// </summary>
		public string OcrName { get; set; }
	}
}
