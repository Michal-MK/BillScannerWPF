using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BillScannerWPF {
	public class DatabaseAccess : Database {


		Dictionary<string, Item> getShopEntries { get; }
		private Shop current;


		private DatabaseAccess(Shop s) {
			string shopName = s.ToString();
			current = s;
			try {
				getShopEntries = GetShopEntries();
			}
			catch {
				Console.WriteLine("Shop not defined!");
				getShopEntries = new Dictionary<string, Item>();
			}
		}

		public static DatabaseAccess LoadDatabase(Shop shop) {
			DatabaseAccess access = new DatabaseAccess(shop);
			return access;
		}

		public Item[] GetItems() {
			return items[current.ToString()[0]][current.ToString()].Values.ToArray();
		}

		/// <summary>
		/// Get the actual item from name
		/// </summary>
		/// <exception cref="ItemNotDefinedException"></exception>
		/// <param name="name">Name of the item</param>
		public Item GetItem(string name) {
			string currentShopName = current.ToString();
			if (items.ContainsKey(currentShopName[0])) {
				if (items[name[0]].ContainsKey(name)) {
					return items[currentShopName[0]][currentShopName][name];
				}
				else {
					foreach (KeyValuePair<string, Item> item in items[currentShopName[0]][currentShopName]) {
						for (int i = 0; i < item.Value.ocrNames.Length; i++) {
							if (item.Value.ocrNames[i] == name) {
								return item.Value;
							}
						}
					}
				}
			}
			throw new ItemNotDefinedException("Item not present under this name");
		}

		public Item DefineItem() {
			return new Item("Name", 50);
		}

		public void AddNewItem(Item i) {

		}

		public void AddAltNameForItem(string originalName) {

		}

		private Dictionary<string, Item> GetShopEntries() {
			return items[current.ToString()[0]][current.ToString()];
		}

		public void SwitchShop(Shop s) {
			current = s;
		}
	}
}