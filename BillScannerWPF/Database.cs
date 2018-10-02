using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BillScannerWPF {
	public class Database {

		protected Dictionary<string, Item> itemDatabase;
		protected JArray itemDatabaseJson;
		protected Dictionary<string, Shopping> purchaseDatabase;
		protected JToken shoppingDatabaseJson;

		internal FileInfo itemDatabaseFile { get; }
		internal FileInfo selectedShopDBFile { get; }

		private Shop selectedShop;

		internal Database(Shop shop) {
			FolderInit.Initialize(shop);
			selectedShop = shop;
			selectedShopDBFile = new FileInfo(WPFHelper.dataPath + selectedShop.ToString() + "_purchasedb.json");
			itemDatabaseFile = new FileInfo(WPFHelper.dataPath + selectedShop.ToString() + "_itemsdb.json");
			LoadItemDatabase();
			LoadPurchaseDatabase();
		}

		private void LoadItemDatabase() {
			itemDatabase = new Dictionary<string, Item>();
			using (StreamReader sr = File.OpenText(itemDatabaseFile.FullName))
			using (JsonTextReader jr = new JsonTextReader(sr)) {
				itemDatabaseJson = JArray.Load(jr);
				for (int i = 0; i < itemDatabaseJson.Count; i++) {
					Item item = JsonConvert.DeserializeObject<Item>(itemDatabaseJson[i].ToString());
					AddToDB(item);
				}
			}
		}

		private void LoadPurchaseDatabase() {
			purchaseDatabase = new Dictionary<string, Shopping>();
			using (StreamReader sr = File.OpenText(selectedShopDBFile.FullName))
			using (JsonTextReader jr = new JsonTextReader(sr)) {
				shoppingDatabaseJson = JToken.ReadFrom(jr);
				JArray array = ((JArray)shoppingDatabaseJson[nameof(Shopping.purchasedItems)]);
				for (int i = 0; i < array.Count; i++) {
					Shopping item = array[i].ToObject<Shopping>();
					purchaseDatabase.Add(item.GUIDString, item);
				}
			}
		}

		private void AddToDB(Item item) {
			if (itemDatabase.ContainsKey(item.identifier)) {
				throw new Exception("What ? " + item.identifier);
			}
			itemDatabase.Add(item.identifier, item);
		}
	}

	public enum Shop {
		NotSelected,
		Lidl,
		Albert,
		Penny,
		Billa,
		McDonalds
	}
}