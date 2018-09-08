using System;
using System.IO;

namespace BillScannerWPF {
	internal class FolderInit {
		internal static void Initialize(Shop shop) {
			if (!Directory.Exists(WPFHelper.dataPath)) {
				Directory.CreateDirectory(WPFHelper.dataPath);
			}
			if (!File.Exists(WPFHelper.dataPath + "itemsdb.json")) {
				using (StreamWriter sw = File.CreateText(WPFHelper.dataPath + "itemsdb.json")) {
					sw.WriteLine("[]");
				}
			}
			if(!File.Exists(WPFHelper.dataPath + shop.ToString() + "db.json")) {
				InitNewShop(shop.ToString());
			}
		}

		internal static FileInfo InitNewShop(string shopName) {
			if (File.Exists(WPFHelper.dataPath + shopName + "db.json")) {
				throw new IOException("File already exists!");
			}
			using (StreamWriter sw = File.CreateText(WPFHelper.dataPath + shopName + "db.json")) {
				sw.WriteLine("{");
				sw.WriteLine("\"shopName\": \"{0}\",", shopName);
				sw.WriteLine("\"purchases\": []");
				sw.WriteLine("}");
			}
			return new FileInfo(WPFHelper.dataPath + shopName + "db.json");
		}
	}
}