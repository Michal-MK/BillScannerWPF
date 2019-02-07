using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace BillScannerCore {

	/// <summary>
	/// JSON data parsed into the program
	/// </summary>
	public class Database {

		/// <summary>
		/// All items from the database indexed by item GUID
		/// </summary>
		protected Dictionary<string, Item> itemDatabase;

		/// <summary>
		/// JSON structure holding all items in the file
		/// </summary>
		protected JArray itemDatabaseJson;

		/// <summary>
		/// All purchases from the database indexed be purchase GUID
		/// </summary>
		protected Dictionary<string, Purchase> purchaseDatabase;

		/// <summary>
		/// JSON structure holding all purchases in the file
		/// </summary>
		protected JToken shoppingDatabaseJson;

		/// <summary>
		/// Current shops item JSON
		/// </summary>
		public FileInfo itemDatabaseFile { get; }

		/// <summary>
		/// Current shops purchase JSON
		/// </summary>
		public FileInfo selectedShopDBFile { get; }

		/// <summary>
		/// Shop selected at startup
		/// </summary>
		private Shop selectedShop;


		/// <summary>
		/// Load a <see cref="Database"/> for selected shop
		/// </summary>
		/// <param name="shop"></param>
		internal Database(Shop shop) {
			FolderInit.Initialize(shop);
			selectedShop = shop;
			selectedShopDBFile = new FileInfo(WPFHelper.dataPath + selectedShop.ToString() + "_purchasedb.json");
			itemDatabaseFile = new FileInfo(WPFHelper.dataPath + selectedShop.ToString() + "_itemsdb.json");
			LoadItemDatabase();
			LoadPurchaseDatabase();

			SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\BillScanner_WPF\Core\Database\Database1.mdf;Integrated Security=True");

			connection.Open();
			Random r = new Random();
			int[] s = new[] { 0, 1, 2, 3, 4, 5 };
			string[] cities = new[] { "Me", "Test", "Brod", "NY", "Some", " Other", "Town", "Please", "Why", " So", "Many", "Words", "AAAAAA" };


			using (SqlCommand command1 = connection.CreateCommand()) {
				command1.CommandText = "SELECT * FROM Item";
				using (SqlDataReader rr = command1.ExecuteReader()) {
					while (rr.Read()) {
						int index = rr.GetInt32(0);
						using (SqlCommand command = connection.CreateCommand()) {
							command.CommandText = string.Format("INSERT INTO ItemOCRNames VALUES ({0},{1})", index, Guid.NewGuid().ToString().Split('-')[0]);
							command.ExecuteNonQuery();
						}
					}
				}
			}
		}

		/// <summary>
		/// Populate internal <see cref="itemDatabase"/> with contents of the JSON
		/// </summary>
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

		/// <summary>
		/// Populate internal <see cref="purchaseDatabase"/> with contents of the JSON
		/// </summary>
		private void LoadPurchaseDatabase() {
			purchaseDatabase = new Dictionary<string, Purchase>();
			using (StreamReader sr = File.OpenText(selectedShopDBFile.FullName))
			using (JsonTextReader jr = new JsonTextReader(sr)) {
				shoppingDatabaseJson = JToken.ReadFrom(jr);
				JArray array = (JArray)shoppingDatabaseJson[nameof(Purchase.purchasedItems)];
				for (int i = 0; i < array.Count; i++) {
					Purchase item = array[i].ToObject<Purchase>();
					purchaseDatabase.Add(item.GUIDString, item);
				}
			}
		}

		/// <summary>
		/// Add item to internal <see cref="itemDatabase"/>
		/// </summary>
		private void AddToDB(Item item) {
			if (itemDatabase.ContainsKey(item.identifier)) {
				throw new Exception("What ? " + item.identifier);
			}
			itemDatabase.Add(item.identifier, item);
		}
	}
}