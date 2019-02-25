using System.IO;
using System.Reflection;

namespace BillScannerCore {

	/// <summary>
	/// Helper class to initialize empty shop JSONs and purchase JSONs
	/// </summary>
	internal class FolderInit {

		/// <summary>
		/// Initializes directories in AppData folder
		/// </summary>
		/// <exception cref="IOException"></exception>
		internal static void Initialize() {
			if (!Directory.Exists(WPFHelper.dataPath)) {
				Directory.CreateDirectory(WPFHelper.dataPath);
			}
		}

		/// <summary>
		/// Copies the database file from this assembly into its proper location
		/// </summary>
		internal static void CopyDatabase() {
			if (!File.Exists(WPFHelper.dataPath + WPFHelper.databaseFileName)) {
				using (Stream s = Assembly.GetEntryAssembly().GetManifestResourceStream("Core/Database/ShoppingDB.db")) {
					using (FileStream fs = new FileStream(WPFHelper.dataPath + "ShoppingDB.db", FileMode.CreateNew)) {
						s.CopyTo(fs);
					}
				}
			}
		}
	}
}