using System.IO;
using System.Reflection;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Helper class to initialize empty shop JSONs and purchase JSONs
	/// </summary>
	internal class FolderInit {

		/// <summary>
		/// Initializes directories in AppData folder
		/// </summary>
		/// <exception cref="IOException"></exception>
		internal static void Initialize() {
			if (!Directory.Exists(WPFHelper.DataPath)) {
				Directory.CreateDirectory(WPFHelper.DataPath);
			}
		}

		/// <summary>
		/// Copies the database file from this assembly into its proper location
		/// </summary>
		internal static void CopyDatabase() {
			if (!File.Exists(WPFHelper.DataPath + WPFHelper.DatabaseFileName)) {
				using (Stream s = Assembly.GetEntryAssembly().GetManifestResourceStream("Core/Database/" + WPFHelper.DatabaseFileName)) {
					using (FileStream fs = new FileStream(WPFHelper.DataPath + WPFHelper.DatabaseFileName, FileMode.CreateNew)) {
						s.CopyTo(fs);
					}
				}
			}
		}
	}
}