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
		/// Get the actual item by GUID
		/// </summary>
		/// <exception cref="ItemNotDefinedException"></exception>
		/// <param name="guidString">GUID of the item</param>
		public Item GetItem(string guidString) {
			if (itemDatabase.ContainsKey(guidString)) {
				return itemDatabase[guidString];
			}
			throw new ItemNotDefinedException("No item with this name exists!");
		}

		public Item WriteItemDefinitionToDatabase(Item newItemDefinition, string userFriendlyName, decimal finalPrice) {
			JObject obj = JObject.FromObject(newItemDefinition);
			JArray ocrArray = ((JArray)obj[nameof(Item.ocrNames)]);
			obj[nameof(Item.userFriendlyName)] = userFriendlyName;
			obj[nameof(Item.currentPrice)] = finalPrice;
			((JArray)obj[nameof(Item.pricesInThePast)]).Add(finalPrice);

			itemDatabaseJson.Add(obj);
			File.WriteAllText(itemDatabaseFile.FullName, itemDatabaseJson.ToString());
			return newItemDefinition;
		}

		internal void WriteNewCurrentPriceToDatabase(string identifier, decimal price) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.identifier)].Value<string>() == identifier) {
					decimal current = tok[nameof(Item.currentPrice)].Value<decimal>();
					if (price != current) { 
						JArray pastPrices = (JArray)tok[nameof(Item.pricesInThePast)];
						bool priceAlreadyAdded = false;
						foreach (decimal pastPrice in pastPrices.Values<decimal>()) {
							if (current == pastPrice) {
								priceAlreadyAdded = true;
								break;
							}
						}
						if (priceAlreadyAdded) {
							tok[nameof(Item.currentPrice)] = price;
							return;
						}
						else {
							pastPrices.Add(current);
							tok[nameof(Item.currentPrice)] = price;
							return;
						}
					}
				}
			}
		}

		public void AddAlternativeOCRNameForItemToDatabase(string identifier, string altName) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.identifier)].Value<string>() == identifier) {
					((JArray)tok[nameof(Item.ocrNames)]).Add(altName);
					break;
				}
			}
			itemDatabase[identifier].ocrNames.Add(altName);
			File.WriteAllText(itemDatabaseFile.FullName, itemDatabaseJson.ToString());
		}


		public void AddNewPurchaseForItemToDatabase(string identifier, PurchaseHistory history) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.identifier)].Value<string>() == identifier) {
					tok[nameof(Item.totalPurchased)] = tok[nameof(Item.totalPurchased)].Value<long>() + history.amount;
					itemDatabase[identifier].AddAmount(history.amount);
					itemDatabase[identifier].purchaseHistory.Add(history);
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
						itemDatabase[identifier].pricesInThePast.Add(history.price);
					}
					tok[nameof(Item.currentPrice)] = history.price;
					itemDatabase[identifier].SetNewCurrentPrice(history.price);
					break;
				}
			}
			File.WriteAllText(itemDatabaseFile.FullName, itemDatabaseJson.ToString());
		}

		internal void RegisterItemFromUI(UIItem currentItemBeingInspected, string modifiedName, decimal finalPrice) {
			Item asociated = currentItemBeingInspected.asociatedItem;
			itemDatabase.Add(asociated.userFriendlyName, WriteItemDefinitionToDatabase(asociated, modifiedName, finalPrice));
		}

		public void WriteUnitOfMeassureForItemToDatabase(string identifier, MeassurementUnit unit) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.identifier)].Value<string>() == identifier) {
					tok[nameof(Item.unitOfMeassure)] = unit.ToString();
				}
			}
			File.WriteAllText(itemDatabaseFile.FullName, itemDatabaseJson.ToString());
		}

		public void WriteNewShoppingInstance(Shopping purchaseInstance) {
			((JArray)shoppingDatabaseJson[nameof(Shopping.purchasedItems)]).Add(JObject.FromObject(purchaseInstance));
			purchaseDatabase.Add(purchaseInstance.GUIDString, purchaseInstance);
			File.WriteAllText(WPFHelper.dataPath + current.ToString() + "_purchasedb.json", shoppingDatabaseJson.ToString());
		}
	}
}