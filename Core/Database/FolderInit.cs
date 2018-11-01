using System;
using System.IO;

namespace BillScannerCore {

	/// <summary>
	/// Helper class to initialize empty shop JSONs and purchase JSONs
	/// </summary>
	internal class FolderInit {

		/// <summary>
		/// Creates a new JSON file for selected <see cref="Shop"/>'s Items and returns the <see cref="FileInfo"/>
		/// </summary>
		/// <exception cref="IOException"></exception>
		internal static FileInfo Initialize(Shop shop) {
			string shopName = shop.ToString();

			if (!Directory.Exists(WPFHelper.dataPath)) {
				Directory.CreateDirectory(WPFHelper.dataPath);
			}

			FileInfo itemsDB = new FileInfo(WPFHelper.dataPath + shopName + "_itemsdb.json");

			if (!itemsDB.Exists) {
				using (StreamWriter sw = File.CreateText(itemsDB.FullName)) {
					sw.WriteLine("[]");
				}
			}
			if (!File.Exists(WPFHelper.dataPath + shopName + "_purchasedb.json")) {
				InitNewShop(shopName);
			}
			return itemsDB;
		}

		/// <summary>
		/// Creates a new JSON file for selected <see cref="Shop"/>'s purchases and returns the <see cref="FileInfo"/>
		/// </summary>
		/// <exception cref="IOException"></exception>
		internal static FileInfo InitNewShop(string shopName) {
			FileInfo shopPath = new FileInfo(WPFHelper.dataPath + shopName + "_purchasedb.json");
			if (!File.Exists(shopPath.FullName)) {
				using (StreamWriter sw = File.CreateText(shopPath.FullName)) {
					sw.WriteLine("{");
					sw.WriteLine("\"shopName\": \"{0}\",", shopName);
					sw.WriteLine("\""+ nameof(Purchase.purchasedItems) + "\": []");
					sw.WriteLine("}");
				}
			}
			return shopPath;
		}
	}
}