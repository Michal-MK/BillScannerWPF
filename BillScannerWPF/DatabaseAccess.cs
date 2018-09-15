using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
		/// Get the actual item by name
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


		public Item AddItemDefinitionToDatabase(string name) {
			Item i = new Item(name, (decimal)new Random().NextDouble() * 100);
			itemDatabase.Add(name, i);
			itemDatabaseJson.Add(JObject.FromObject(i));
			File.WriteAllText(WPFHelper.itemsFullPath, itemDatabaseJson.ToString());
			return null;
		}

		public Item WriteItemDefinitionToDatabase(Item newItemDefinition, string modifiedName) {
			JObject obj = JObject.FromObject(newItemDefinition);
			JArray ocrArray = ((JArray)obj[nameof(Item.ocrNames)]);
			obj[nameof(Item.mainName)] = modifiedName;

			itemDatabaseJson.Add(obj);
			File.WriteAllText(WPFHelper.itemsFullPath, itemDatabaseJson.ToString());
			return null;
		}

		public void AddAlternativeOCRNameForItemToDatabase(string originalName, string altName) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.mainName)].Value<string>() == originalName) {
					((JArray)tok[nameof(Item.ocrNames)]).Add(altName);
					break;
				}
			}
			itemDatabase[originalName].ocrNames.Add(altName);
			File.WriteAllText(WPFHelper.itemsFullPath, itemDatabaseJson.ToString());
		}


		public void AddNewPurchaseForItemToDatabase(string itemName, PurchaseHistory history) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.mainName)].Value<string>() == itemName) {
					tok[nameof(Item.totalPurchased)] = tok[nameof(Item.totalPurchased)].Value<long>() + history.amount;
					itemDatabase[itemName].AddAmount(history.amount);
					itemDatabase[itemName].purchaseHistory.Add(history);
					((JArray)tok[nameof(Item.purchaseHistory)]).Add(JObject.FromObject(history));
					bool priceRecorded = false;

					foreach (JToken price in ((JArray)tok[nameof(Item.pricesInThePast)])) {
						if (price.Value<decimal>() == history.price) {
							priceRecorded = true;
							break;
						}
					}
					if (!priceRecorded) {
						((JArray)tok[nameof(Item.pricesInThePast)]).Add(history.price);
						itemDatabase[itemName].pricesInThePast.Add(history.price);
					}
					tok[nameof(Item.currentPrice)] = history.price;
					itemDatabase[itemName].SetNewCurrentPrice(history.price);
					break;
				}
			}
			File.WriteAllText(WPFHelper.itemsFullPath, itemDatabaseJson.ToString());
		}

		internal void RegisterItemFromUI(UIItem currentItemBeingInspected, string modifiedName) {
			Item asociated = currentItemBeingInspected.asociatedItem;
			itemDatabase.Add(asociated.mainName, WriteItemDefinitionToDatabase(asociated, modifiedName));
		}

		public void WriteUnitOfMeassureForItemToDatabase(string itemName, MeassurementUnit unit) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.mainName)].Value<string>() == itemName) {
					tok[nameof(Item.unitOfMeassure)] = unit.ToString();
				}
			}
			File.WriteAllText(WPFHelper.itemsFullPath, itemDatabaseJson.ToString());
			itemDatabase[itemName].SetUnitOfMeassure(unit);
		}

		public void WriteNewShoppingInstance(Shopping purchaseInstance) {
			purchaseInstance.FinalizePurchase();
			((JArray)shoppingDatabaseJson[nameof(Shopping.purchasedItems)]).Add(JObject.FromObject(purchaseInstance));
			purchaseDatabase.Add(purchaseInstance.date, purchaseInstance);
			File.WriteAllText(WPFHelper.dataPath + current.ToString() + "db.json", shoppingDatabaseJson.ToString());
		}
	}
}