using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BillScannerCore {

	/// <summary>
	/// Provides methods for database IO.
	/// </summary>
	public class DatabaseAccess : Database {

		/// <summary>
		/// The shop that was selected at startup
		/// </summary>
		private Shop current;

		/// <summary>
		/// Static access to database I/O
		/// </summary>
		public static DatabaseAccess access { get; private set; }

		private DatabaseAccess(Shop s) : base(s) {
			string shopName = s.ToString();
			current = s;
			access = this;
		}

		/// <summary>
		/// Loads internal databases (<see cref="Database.itemDatabase"/> and <see cref="Database.purchaseDatabase"/>) with contents of the JSON files for selected <see cref="Shop"/>
		/// </summary>
		public static DatabaseAccess LoadDatabase(Shop shop) {
			if (access == null || access.current != shop) {
				access = new DatabaseAccess(shop);
			}
			return access;
		}

		/// <summary>
		/// Return all items from the database in form of an array
		/// </summary>
		public Item[] GetItems() {
			return itemDatabase.Values.ToArray();
		}

		/// <summary>
		/// Return all purchases from the database in form of an array
		/// </summary>
		public Purchase[] GetPurchases() {
			return purchaseDatabase.Values.ToArray();
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

		/// <summary>
		/// Get the actual purchase by GUID
		/// </summary>		
		/// <exception cref="PurchaseNotFoundException"></exception>
		/// <param name="guidString">GUID of the purchase</param>
		public Purchase GetPurchase(string guidString) {
			if (purchaseDatabase.ContainsKey(guidString)) {
				return purchaseDatabase[guidString];
			}
			throw new PurchaseNotFoundException("Purchase with GUID:" + guidString + " does not exist!");
		}

		/// <summary>
		/// Write newly defined <see cref="Item"/> into the database
		/// </summary>
		/// <param name="newItemDefinition">The item to write out</param>
		public Item WriteItemDefinitionToDatabase(Item newItemDefinition) {
			return WriteItemDefinitionToDatabase(newItemDefinition, newItemDefinition.userFriendlyName, newItemDefinition.currentPrice);
		}

		/// <summary>
		/// Write newly defined <see cref="Item"/> into the database
		/// </summary>
		/// <param name="newItemDefinition">The item to write out</param>
		/// <param name="userFriendlyName">New user friendly name for the item</param>
		/// <param name="finalPrice">New price, adds the previous one to price history</param>
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

		/// <summary>
		/// Searches the database and updates <see cref="Item"/>'s current price, on change updates price history as well
		/// </summary>
		/// <param name="identifier"><see cref="Item"/>'s GUID</param>
		/// <param name="price">The price to set as new</param>
		public void WriteNewCurrentPriceToDatabase(string identifier, decimal price) {
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

		/// <summary>
		/// Update items list of OCR recognized strings
		/// </summary>
		/// <param name="identifier"><see cref="Item"/>'s GUID</param>
		/// <param name="altName">New alternative OCR name</param>
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

		/// <summary>
		/// Records new purchase for an <see cref="Item"/> into its internal list
		/// </summary>
		/// <param name="identifier"><see cref="Item"/>'s GUID</param>
		/// <param name="history">The purchase history of this <see cref="Item"/></param>
		public void AddNewPurchaseForItemToDatabase(string identifier, ItemPurchaseHistory history) {
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

		/// <summary>
		/// Registers <see cref="Item"/> from UI !writes only into <see cref="Database.itemDatabase"/>!
		/// </summary>
		/// <param name="currentItemBeingInspected">The <see cref="UIItem"/> from which to get <see cref="Item"/> information</param>
		public void RegisterItemFromUI(Item newItem) {
			RegisterNewItem(newItem);
		}

		/// <summary>
		/// Registers <see cref="Item"/> from UI !writes only into <see cref="Database.itemDatabase"/>!
		/// </summary>
		/// <param name="currentItemBeingInspected">The <see cref="ItemPurchaseData"/> from which to get <see cref="Item"/> information</param>
		/// <param name="modifiedName">New user friendly name</param>
		/// <param name="finalPrice">New price for the item</param>
		public void RegisterItemFromUI(ItemPurchaseData currentItemBeingInspected, string modifiedName, decimal finalPrice) {
			RegisterNewItem(currentItemBeingInspected.item, modifiedName, finalPrice);
		}

		/// <summary>
		/// Adds new <see cref="Item"/> definition into internal database !Does not write into files!
		/// </summary>
		/// <param name="item">The <see cref="Item"/> to add</param>
		public void RegisterNewItem(Item item, string modifiedName = null, decimal finalPrice = -1) {
			itemDatabase.Add(item.identifier, WriteItemDefinitionToDatabase(item,
				modifiedName == null ? item.userFriendlyName : modifiedName,
				finalPrice == -1 ? item.currentPrice : finalPrice));
		}

		/// <summary>
		/// Updates <see cref="Item"/>'s <see cref="MeassurementUnit"/>
		/// </summary>
		/// <param name="identifier"><see cref="Item"/>'s GUID</param>
		/// <param name="unit">The units to change to</param>
		public void WriteUnitOfMeassureForItemToDatabase(string identifier, MeassurementUnit unit) {
			foreach (JToken tok in itemDatabaseJson) {
				if (tok[nameof(Item.identifier)].Value<string>() == identifier) {
					tok[nameof(Item.unitOfMeassure)] = unit.ToString();
				}
			}
			File.WriteAllText(itemDatabaseFile.FullName, itemDatabaseJson.ToString());
		}

		/// <summary>
		/// Writes new purchase information into <see cref="Database.purchaseDatabase"/> and into the JSON file
		/// </summary>
		public void WriteNewPurchaseInstance(Purchase purchaseInstance) {
			((JArray)shoppingDatabaseJson[nameof(Purchase.purchasedItems)]).Add(JObject.FromObject(purchaseInstance));
			purchaseDatabase.Add(purchaseInstance.GUIDString, purchaseInstance);
			File.WriteAllText(WPFHelper.dataPath + current.ToString() + "_purchasedb.json", shoppingDatabaseJson.ToString());
		}
	}
}