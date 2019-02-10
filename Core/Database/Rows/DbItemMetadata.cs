using BillScannerCore;
using System;

namespace Core.Database {
	/// <summary>
	/// Class representing an Item Meta-data row in the database
	/// </summary>
	public class DbItemMetadata {

		public static string DName => nameof(DbItemMetadata).Substring(2);

		/// <summary>
		/// Primary key, Foreign key to the <see cref="DbItem"/> table
		/// </summary>
		public int ItemID { get; set; }

		/// <summary>
		/// The weight of the item
		/// </summary>
		public float Weight { get; set; }

		/// <summary>
		/// How many calories does a <see cref="DbItem"/> have
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// How many calories does a <see cref="DbItem"/> have
		/// </summary>
		public string UnitOfMeassure { get; set; }

		/// <summary>
		/// Return the enum form of the unit of measure text
		/// </summary>
		internal MeassurementUnit MeassurementUnit => (MeassurementUnit)Enum.Parse(typeof(MeassurementUnit), UnitOfMeassure);
	}
}
