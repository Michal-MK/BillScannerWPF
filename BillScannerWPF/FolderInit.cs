﻿using System;
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
			string shopName = shop.ToString();
			if(!File.Exists(WPFHelper.dataPath + shopName + "db.json")) {
				InitNewShop(shopName);
			}
		}

		internal static FileInfo InitNewShop(string shopName) {
			string shopPath = WPFHelper.dataPath + shopName + "db.json";
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