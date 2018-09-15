using System;
using System.IO;

namespace BillScannerWPF {
	internal class FolderInit {
		internal static void Initialize(Shop shop) {
			string shopName = shop.ToString();

			if (!Directory.Exists(WPFHelper.dataPath)) {
				Directory.CreateDirectory(WPFHelper.dataPath);
			}
			if (!File.Exists(WPFHelper.dataPath + shopName + "_itemsdb.json")) {
				using (StreamWriter sw = File.CreateText(WPFHelper.dataPath + shopName + "_itemsdb.json")) {
					sw.WriteLine("[]");
				}
			}
			if (!File.Exists(WPFHelper.dataPath + shopName + "_purchasedb.json")) {
				InitNewShop(shopName);
			}
		}

		internal static FileInfo InitNewShop(string shopName) {
			string shopPath = WPFHelper.dataPath + shopName + "_purchasedb.json";
			if (File.Exists(shopPath)) {
				throw new IOException("File already exists!");
			}
			using (StreamWriter sw = File.CreateText(shopPath)) {
				sw.WriteLine("{");
				sw.WriteLine("\"shopName\": \"{0}\",", shopName);
				sw.WriteLine("\"purchases\": []");
				sw.WriteLine("}");
			}
			return new FileInfo(shopPath);
		}
	}
}