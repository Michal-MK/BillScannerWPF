using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BillScannerWPF {
	public class Database {

		protected Dictionary<string, Item> itemDatabase;
		protected JArray itemDatabaseJson;
		protected Dictionary<DateTime, Shopping> purchaseDatabase;
		protected JToken shoppingDatabaseJson;

		protected FileInfo itemDatabaseFile;
		protected FileInfo selectedShopDBFile;

		private Shop selectedShop;

		internal Database(Shop shop) {
			FolderInit.Initialize(shop);
			selectedShopDBFile = new FileInfo(WPFHelper.dataPath + selectedShop.ToString() + "db.json");
			itemDatabaseFile = new FileInfo(WPFHelper.dataPath + "itemsdb.json");
			selectedShop = shop;
			LoadItemDatabase();
			LoadPurchaseDatabase();
		}

		private void LoadItemDatabase() {
			itemDatabase = new Dictionary<string, Item>();
			using (StreamReader sr = File.OpenText(itemDatabaseFile.FullName)) {
				using (JsonTextReader jr = new JsonTextReader(sr)) {
					itemDatabaseJson = JArray.Load(jr);
					for (int i = 0; i < itemDatabaseJson.Count; i++) {
						Item item = itemDatabaseJson[i].ToObject<Item>();
						AddToDB(item);
					}
				}
			}
		}

		private void LoadPurchaseDatabase() {
			purchaseDatabase = new Dictionary<DateTime, Shopping>();
			using (StreamReader sr = File.OpenText(selectedShopDBFile.FullName)) {
				using (JsonTextReader jr = new JsonTextReader(sr)) {
					shoppingDatabaseJson = JToken.ReadFrom(jr);
					JArray array = ((JArray)shoppingDatabaseJson["purchases"]);
					for (int i = 0; i < array.Count; i++) {
						Shopping item = array[i].ToObject<Shopping>();
						purchaseDatabase.Add(item.date, item);
					}
				}
			}
		}

		private void AddToDB(Item item) {
			if (itemDatabase.ContainsKey(item.mainName)) {
				return;
			}
			itemDatabase.Add(item.mainName, item);
		}
	}

	public enum Shop {
		Lidl,
		Albert,
		Penny,
		Billa,
		McDonalds
	}
}