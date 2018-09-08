using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BillScannerWPF {
	public class DatabaseAccess : Database {

		private Shop current;

		private DatabaseAccess(Shop s) : base(s) {
			string shopName = s.ToString();
			current = s;
		}

		public static DatabaseAccess LoadDatabase(Shop shop) {
			DatabaseAccess access = new DatabaseAccess(shop);
			return access;
		}

		public Item[] GetItems() {
			return itemDatabase.Values.ToArray();
		}

		/// <summary>
		/// Get the actual item from name
		/// </summary>
		/// <exception cref="ItemNotDefinedException"></exception>
		/// <param name="name">Name of the item</param>
		public Item GetItem(string name) {
			if (itemDatabase.ContainsKey(name)) {
				return itemDatabase[name];
			}
			else {
				foreach (KeyValuePair<string, Item> item in itemDatabase) {
					for (int i = 0; i < item.Value.ocrNames.Count; i++) {
						if (item.Value.ocrNames[i] == name) {
							return item.Value;
						}
					}
				}
			}
			throw new ItemNotDefinedException("Item not present under this name");
		}


		public Item WriteItemDefinitionToDatabase(string name, bool finalItem = false) {
			Item i = new Item(name, (decimal)new Random().NextDouble() * 100);
			itemDatabase.Add(name, i);
			itemDatabaseJson.Add(JObject.FromObject(i));
			File.WriteAllText(WPFHelper.dataPath + "itemsdb.json", itemDatabaseJson.ToString());
			return null;
		}

		public void WriteAlternativeOCRNameForItemToDatabase(string originalName, string altName) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok["mainName"].Value<string>() == originalName) {
					((JArray)tok["ocrNames"]).Add(altName);
					break;
				}
			}
			itemDatabase[originalName].ocrNames.Add(altName);
			File.WriteAllText(WPFHelper.dataPath + "itemsdb.json", itemDatabaseJson.ToString());
		}


		public void WriteNewPurchaseForItemToDatabase(string itemName, PurchaseHistory history) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok["mainName"].Value<string>() == itemName) {
					tok["totalPurchased"] = tok["totalPurchased"].Value<long>() + history.amount;
					itemDatabase[itemName].AddAmount(history.amount);
					itemDatabase[itemName].purchaseHistory.Add(history);
					((JArray)tok["purchaseHistory"]).Add(JObject.FromObject(history));
					bool priceRecorded = false;

					foreach (JToken price in ((JArray)tok["pricesInThePast"])) {
						if (price.Value<decimal>() == history.price) {
							priceRecorded = true;
							break;
						}
					}
					if (!priceRecorded) {
						((JArray)tok["pricesInThePast"]).Add(history.price);
						itemDatabase[itemName].pricesInThePast.Add(history.price);
					}
					tok["currentPrice"] = history.price;
					itemDatabase[itemName].SetNewCurrentPrice(history.price);
					break;
				}
			}
			File.WriteAllText(WPFHelper.dataPath + "itemsdb.json", itemDatabaseJson.ToString());
		}

		public void WriteUnitOfMeassureForItemToDatabase(string itemName, MeassurementUnit unit) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok["mainName"].Value<string>() == itemName) {
					tok["unitOfMeassure"] = unit.ToString();
				}
			}
			File.WriteAllText(WPFHelper.dataPath + "itemsdb.json", itemDatabaseJson.ToString());
		}

		public void WriteNewShoppingInstance(Shopping purchaseInstance) {
			purchaseInstance.FinalizePurchase();
			((JArray)shoppingDatabaseJson["purchases"]).Add(JObject.FromObject(purchaseInstance));
			purchaseDatabase.Add(purchaseInstance.date, purchaseInstance);
			File.WriteAllText(WPFHelper.dataPath + current.ToString() + "db.json", shoppingDatabaseJson.ToString());
		}
	}
}