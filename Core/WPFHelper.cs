using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Class containing useful helper functionality for the application
	/// </summary>
	public static class WPFHelper {

		/// <summary>
		/// Returns an absolute path to this application's path inside the %appdata% (Roaming) folder
		/// </summary>
		public static string DataPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "BillScanner" + Path.DirectorySeparatorChar;

		/// <summary>
		/// Name of the database
		/// </summary>
		public const string DatabaseFileName = "ShoppingDB.db";

		/// <summary>
		/// Uses <see cref="resourcesPath"/> and adds part of the path to point into <see cref="MatchRating"/> images directory
		/// </summary>
		public static string ImageRatingResourcesPath => ResourceNames.RESOURCES + "MatchRating/";
	}
}
