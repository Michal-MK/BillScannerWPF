﻿using System.Linq;
using System.IO;
using System.Data.SQLite;
using Dapper;
using System.Data;
using System;
using Core.Database;
using System.Collections.Generic;

namespace BillScannerCore {

	/// <summary>
	/// Provides methods for database IO.
	/// </summary>
	public class DatabaseAccess {

		/// <summary>
		/// The shop that was selected at startup
		/// </summary>
		private Shop current;

		/// <summary>
		/// Static access to database I/O
		/// </summary>
		public static DatabaseAccess access { get; private set; }


		private string DbConnectionString => $@"Data Source={WPFHelper.dataPath}Database.db;Version=3;";
		private string DbDateTimeFormat => "yyyy-MM-dd hh:mm:ss";

		private DatabaseAccess(Shop s) {
			string shopName = s.ToString();
			current = s;
			access = this;
		}

		public static DatabaseAccess LoadDatabase(Shop shop) {
			if (access == null || access.current != shop) {
				access = new DatabaseAccess(shop);
			}
			return access;
		}

		internal Item[] GetItems(int iD) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Return all items from the database in form of an array
		/// </summary>
		public Item[] GetItems() {
			List<Item> ret = new List<Item>();
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				IEnumerator<DbItem> items = connection.Query<DbItem>($"SELECT * FROM {DbItem.DName}").GetEnumerator();
				while (items.MoveNext()) {
					DbItem current = items.Current;

					IEnumerator<DbItemOcrNames> ocrNames = connection.Query<DbItemOcrNames>($"SELECT * FROM {DbItemOcrNames.DName} " +
																							$"WHERE {DbItemOcrNames.DName}.{nameof(DbItemOcrNames.ItemID)} = {current.ID}").GetEnumerator();

					IEnumerator<DbItemValueHistory> pastValues = connection.Query<DbItemValueHistory>($"SELECT * FROM {DbItemValueHistory.DName} " +
																									  $"WHERE {DbItemValueHistory.DName}.{nameof(DbItemValueHistory.ItemID)} = {current.ID}").GetEnumerator();

					ret.Add(new Item(current, ocrNames, pastValues));
				}
			}
			return ret.ToArray();
		}

		/// <summary>
		/// Return all purchases from the database in form of an array
		/// </summary>
		public Purchase[] GetPurchases() {
			List<Purchase> ret = new List<Purchase>();
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				IEnumerator<DbPurchase> purchases = connection.Query<DbPurchase>($"SELECT * FROM {DbPurchase.DName}").GetEnumerator();
				while (purchases.MoveNext()) {
					ret.Add(new Purchase(purchases.Current));
				}
			}
			return ret.ToArray();

		}



		public List<ItemPurchaseHistory> GetItemPuchaseHistory(int identifier) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the actual item by ID
		/// </summary>
		/// <exception cref="ItemNotDefinedException"></exception>
		/// <param name="ID">ID of the item</param>
		public Item GetItem(int ID) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				DbItem item = connection.QueryFirst<DbItem>($"SELECT * FROM {DbItem.DName}" +
																	 $"WHERE {DbItem.DName}.{nameof(DbItem.ID)} = {ID}");
				if (item == null) {
					throw new ItemNotDefinedException($"Item with ID {ID} is not present in the database");
				}

				IEnumerator<DbItemOcrNames> ocrNames = connection.Query<DbItemOcrNames>($"SELECT * FROM {DbItemOcrNames.DName} " +
																						$"WHERE {DbItemOcrNames.DName}.{nameof(DbItemOcrNames.ItemID)} = {item.ID}").GetEnumerator();

				IEnumerator<DbItemValueHistory> pastValues = connection.Query<DbItemValueHistory>($"SELECT * FROM {DbItemValueHistory.DName} " +
																								  $"WHERE {DbItemValueHistory.DName}.{nameof(DbItemValueHistory.ItemID)} = {item.ID}").GetEnumerator();

				return new Item(item, ocrNames, pastValues);
			}
		}

		/// <summary>
		/// Get the actual purchase by ID
		/// </summary>		
		/// <exception cref="PurchaseNotFoundException"></exception>
		/// <param name="ID">GUID of the purchase</param>
		public Purchase GetPurchase(int ID) {
			using (IDbConnection connection = new SQLiteConnection($@"Data Source={WPFHelper.dataPath}Database.db;Version=3;")) {
				DbPurchase purchase = connection.QueryFirst<DbPurchase>($"SELECT * FROM {DbPurchase.DName}" +
																	 $"WHERE {DbPurchase.DName}.{nameof(DbPurchase.ID)} = {ID}");
				if (purchase == null) {
					throw new PurchaseNotFoundException("Purchase with GUID:" + ID + " does not exist!");
				}
				return new Purchase(purchase);
			}
		}

		/// <summary>
		/// Write newly defined <see cref="Item"/> into the database
		/// </summary>
		/// <param name="newItemDefinition">The item to write out</param>
		/// <param name="userFriendlyName">New user friendly name for the item</param>
		/// <param name="finalPrice">New price, adds the previous one to price history</param>
		public void WriteItemDefinitionToDatabase(Item newItemDefinition, DateTime purchaseTime) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				using (IDbTransaction transaction = connection.BeginTransaction()) {

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItem.DName} ({nameof(DbItem.Name)},{nameof(DbItem.ShopID)}, {nameof(DbItem.Value)}) " +
															 $"VALUES (@{nameof(newItemDefinition.userFriendlyName)}, {current}, @{nameof(newItemDefinition.currentPrice)})", newItemDefinition));

					int latest = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");

					foreach (string ocrName in newItemDefinition.ocrNames) {
						connection.Execute(new CommandDefinition($"INSERT INTO {DbItemOcrNames.DName} ({nameof(DbItemOcrNames.ItemID)},{nameof(DbItemOcrNames.OcrName)}) " +
																 $"VALUES ({latest}, {ocrName})"));
					}

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemValueHistory.DName} ({nameof(DbItemValueHistory.ItemID)},{nameof(DbItemValueHistory.Value)},{nameof(DbItemValueHistory.Date)}) " +
															 $"VALUES ({latest}, {newItemDefinition.currentPrice}, {purchaseTime.ToString(DbDateTimeFormat)})"));

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemMetadata.DName} ({nameof(DbItemMetadata.ItemID)}) " +
															 $"VALUES ({latest})"));
				}
			}
		}

		/// <summary>
		/// Searches the database and updates <see cref="Item"/>'s current price, on change updates price history as well
		/// </summary>
		/// <param name="itemID"><see cref="Item"/>'s primary key</param>
		/// <param name="price">The price to set as new</param>
		public void UpdateNewCurrentPrice(int itemID, int price, DateTime purchaseTime) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				using (IDbTransaction transaction = connection.BeginTransaction()) {

					connection.Execute(new CommandDefinition($"UPDATE {DbItem.DName} SET {DbItem.DName}.{nameof(DbItem.Value)} = {price}"));

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemValueHistory.DName} ({nameof(DbItemValueHistory.ItemID)},{nameof(DbItemValueHistory.Value)},{nameof(DbItemValueHistory.Date)}) " +
															 $"VALUES ({itemID}, {price}, {purchaseTime.ToString(DbDateTimeFormat)})"));
				}
			}
		}

		/// <summary>
		/// Update items list of OCR recognized strings
		/// </summary>
		/// <param name="itemID"><see cref="Item"/>'s GUID</param>
		/// <param name="ocrName">New alternative OCR name</param>
		public void AddOcrName(int itemID, string ocrName) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Execute(new CommandDefinition($"INSERT INTO {DbItemOcrNames.DName} ({nameof(DbItemOcrNames.ItemID)},{nameof(DbItemOcrNames.OcrName)}) " +
														 $"VALUES ({itemID}, {ocrName})"));
			}
		}

		/// <summary>
		/// Records new purchase for an <see cref="Item"/> into its internal list
		/// </summary>
		/// <param name="itemID"><see cref="Item"/>'s ID</param>
		/// <param name="history">The purchase history of this <see cref="Item"/></param>
		public void AddNewPurchaseForItemToDatabase(int itemID, ItemPurchaseHistory history, DateTime purchaseTime) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				using (IDbTransaction transaction = connection.BeginTransaction()) {
					DbItem selected = connection.QueryFirst<DbItem>(new CommandDefinition($"SELECT {DbItem.DName}.{nameof(DbItem.Value)} FROM {DbItem.DName} WHERE {DbItem.DName}.{nameof(DbItem.ID)} = {itemID}"));

					if (selected.Value != history.price) {
						UpdateNewCurrentPrice(itemID, history.price, purchaseTime);
					}

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemPurchase.DName} ({nameof(DbItemPurchase.ItemID)}, {nameof(DbItemPurchase.PurchaseID)}, {nameof(DbItemPurchase.Amount)}, {nameof(DbItemPurchase.ValuePerItem)})" +
															 $"VALUES ({itemID}, {history.purchaseID}, {history.amount}, {history.price})"));
				}
			}
		}


		/// <summary>
		/// Updates <see cref="Item"/>'s <see cref="MeassurementUnit"/>
		/// </summary>
		/// <param name="itemID"><see cref="Item"/>'s GUID</param>
		/// <param name="unit">The units to change to</param>
		public void UpdateUnitOfMeassure(int itemID, MeassurementUnit unit) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Execute(new CommandDefinition($"UPDATE {DbItemMetadata.DName} SET {DbItemMetadata.DName}.{nameof(DbItemMetadata.UnitOfMeassure)} = {unit.ToString()}"));
			}
		}

		/// <summary>
		/// Writes new purchase information
		/// </summary>
		public int WriteNewPurchaseInstance(Purchase purchaseInstance) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				using (IDbTransaction transaction = connection.BeginTransaction()) {

					connection.Execute(new CommandDefinition($"INSERT INTO {DbPurchase.DName} ({nameof(DbPurchase.ShopID)}, {nameof(DbPurchase.Date)}, {nameof(DbPurchase.TotalValue)}) " +
															 $"VALUES ({purchaseInstance.shop}, {purchaseInstance.date}, {purchaseInstance.totalCost})"));

					int latest = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
					return latest;
				}
			}
		}
	}
}