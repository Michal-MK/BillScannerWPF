﻿using System.Data.SQLite;
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


		private string DbConnectionString => $@"Data Source={WPFHelper.dataPath}ShoppingDB.db;Version=3;";
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

		/// <summary>
		/// Get the actual item by ID from the database
		/// </summary>
		/// <exception cref="ItemNotDefinedException"></exception>
		/// <param name="ID">ID of the item</param>
		public Item GetItem(int ID) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				DbItem item = connection.QueryFirst<DbItem>($"SELECT * FROM {DbItem.DName}" +
															$"WHERE {nameof(DbItem.ID)} = {ID}");
				if (item == null) {
					throw new ItemNotDefinedException($"Item with ID {ID} is not present in the database");
				}

				IEnumerator<string> ocrNames = connection.Query<string>($"SELECT {nameof(DbItemOcrNames.OcrName)} FROM {DbItemOcrNames.DName} " +
																		$"WHERE {DbItemOcrNames.DName}.{nameof(DbItemOcrNames.ItemID)} = {item.ID}").GetEnumerator();

				IEnumerator<DbItemValueHistory> pastValues = connection.Query<DbItemValueHistory>($"SELECT * FROM {DbItemValueHistory.DName} " +
																								  $"WHERE {DbItemValueHistory.DName}.{nameof(DbItemValueHistory.ItemID)} = {item.ID}").GetEnumerator();

				return new Item(item, ocrNames, pastValues);
			}
		}

		/// <summary>
		/// Get all items that were bought in a session with selected 'purchaseID'
		/// </summary>
		internal Item[] GetItems(int purchaseID) {
			List<Item> ret = new List<Item>();
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				IEnumerator<DbItem> items = connection.Query<DbItem>($"SELECT * FROM {DbItem.DName} WHERE {DbItem.DName}.{nameof(DbItem.ID)} IN " +
																	 $"(SELECT {DbItemPurchase.DName}.{nameof(DbItemPurchase.ItemID)} FROM {DbItemPurchase.DName} " +
																	 $"WHERE {nameof(DbItemPurchase.PurchaseID)} = {purchaseID})").GetEnumerator();
				while (items.MoveNext()) {
					DbItem current = items.Current;

					IEnumerator<string> ocrNames = connection.Query<string>($"SELECT {nameof(DbItemOcrNames.OcrName)} FROM {DbItemOcrNames.DName} " +
																			$"WHERE {DbItemOcrNames.DName}.{nameof(DbItemOcrNames.ItemID)} = {current.ID}").GetEnumerator();


					IEnumerator<DbItemValueHistory> pastValues = connection.Query<DbItemValueHistory>($"SELECT * FROM {DbItemValueHistory.DName} " +
																									  $"WHERE {DbItemValueHistory.DName}.{nameof(DbItemValueHistory.ItemID)} = {current.ID}").GetEnumerator();

					ret.Add(new Item(current, ocrNames, pastValues));
				}
			}
			return ret.ToArray();
		}

		/// <summary>
		/// Return all items of selected <see cref="Shop"/> from the database in form of an array
		/// </summary>
		public Item[] GetItems(Shop selected) {
			List<Item> ret = new List<Item>();
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				IEnumerator<DbItem> items = connection.Query<DbItem>($"SELECT * FROM {DbItem.DName} WHERE {nameof(DbItem.ShopID)} = {(int)selected}").GetEnumerator();
				while (items.MoveNext()) {
					DbItem current = items.Current;

					IEnumerator<string> ocrNames = connection.Query<string>($"SELECT {nameof(DbItemOcrNames.OcrName)} FROM {DbItemOcrNames.DName} " +
																			$"WHERE {DbItemOcrNames.DName}.{nameof(DbItemOcrNames.ItemID)} = {current.ID}").GetEnumerator();


					IEnumerator<DbItemValueHistory> pastValues = connection.Query<DbItemValueHistory>($"SELECT * FROM {DbItemValueHistory.DName} " +
																									  $"WHERE {DbItemValueHistory.DName}.{nameof(DbItemValueHistory.ItemID)} = {current.ID}").GetEnumerator();

					ret.Add(new Item(current, ocrNames, pastValues));
				}
			}
			return ret.ToArray();
		}

		/// <summary>
		/// Return all items from the database in form of an array
		/// </summary>
		public Item[] GetItems() {
			List<Item> ret = new List<Item>();
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				IEnumerator<DbItem> items = connection.Query<DbItem>($"SELECT * FROM {DbItem.DName}").GetEnumerator();
				while (items.MoveNext()) {
					DbItem current = items.Current;

					IEnumerator<string> ocrNames = connection.Query<string>($"SELECT {nameof(DbItemOcrNames.OcrName)} FROM {DbItemOcrNames.DName} " +
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
				connection.Open();
				IEnumerator<DbPurchase> purchases = connection.Query<DbPurchase>($"SELECT * FROM {DbPurchase.DName}").GetEnumerator();
				while (purchases.MoveNext()) {
					ret.Add(new Purchase(purchases.Current));
				}
			}
			return ret.ToArray();

		}

		/// <summary>
		/// Get the actual purchase by ID from the database
		/// </summary>		
		/// <exception cref="PurchaseNotFoundException"></exception>
		public Purchase GetPurchase(int ID) {
			using (IDbConnection connection = new SQLiteConnection($@"Data Source={WPFHelper.dataPath}Database.db;Version=3;")) {
				connection.Open();
				DbPurchase purchase = connection.QueryFirst<DbPurchase>($"SELECT * FROM {DbPurchase.DName}" +
																		$"WHERE {DbPurchase.DName}.{nameof(DbPurchase.ID)} = {ID}");
				if (purchase == null) {
					throw new PurchaseNotFoundException("Purchase with GUID:" + ID + " does not exist!");
				}
				return new Purchase(purchase);
			}
		}

		/// <summary>
		/// Allows user to register unknown item from the UI
		/// </summary>
		public void RegisterItemFromUI(Item item, string modifiedName, int finalPrice, DateTime purchaseTime) {
			item.Modify(modifiedName, finalPrice);
			WriteItemDefinitionToDatabase(item, purchaseTime);
		}

		/// <summary>
		/// Returns all <see cref="Item"/> purchases where an <see cref="Item"/> with 'itemID' appears 
		/// </summary>
		public List<ItemPurchaseHistory> GetItemPuchaseHistory(int itemID) {
			List<ItemPurchaseHistory> ret = new List<ItemPurchaseHistory>();
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();

				IEnumerator<DbItemPurchase> items = connection.Query<DbItemPurchase>($"SELECT * FROM {DbItemPurchase.DName} " +
																					 $"WHERE {DbItemPurchase.DName}.{nameof(DbItemPurchase.ItemID)} = {itemID}").GetEnumerator();
				while (items.MoveNext()) {
					ret.Add(new ItemPurchaseHistory(items.Current.PurchaseID, items.Current.Amount, items.Current.ValuePerItem));
				}
			}
			return ret;
		}

		/// <summary>
		/// Write newly defined <see cref="Item"/> into the database
		/// </summary>
		public void WriteItemDefinitionToDatabase(Item newItemDefinition, DateTime purchaseTime) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				using (IDbTransaction transaction = connection.BeginTransaction()) {

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItem.DName} ({nameof(DbItem.Name)}, {nameof(DbItem.ShopID)}, {nameof(DbItem.Value)}) " +
															 $"VALUES (@{nameof(newItemDefinition.ItemName)}, {(int)current}, @{nameof(newItemDefinition.CurrentPriceInt)})", newItemDefinition));

					int latest = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");

					foreach (string ocrName in newItemDefinition.OcrNames) {
						connection.Execute(new CommandDefinition($"INSERT INTO {DbItemOcrNames.DName} ({nameof(DbItemOcrNames.ItemID)},{nameof(DbItemOcrNames.OcrName)}) " +
																 $"VALUES ({latest}, @OcrName)", new { OcrName = ocrName }));
					}

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemValueHistory.DName} ({nameof(DbItemValueHistory.ItemID)},{nameof(DbItemValueHistory.Value)},{nameof(DbItemValueHistory.Date)}) " +
															 $"VALUES ({latest}, {newItemDefinition.CurrentPriceInt}, @Date)", new { Date = purchaseTime.ToString(DbDateTimeFormat) }));

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemMetadata.DName} ({nameof(DbItemMetadata.ItemID)}, {nameof(DbItemMetadata.UnitOfMeassure)}) " +
															 $"VALUES ({latest}, @Date)", new { Date = newItemDefinition.UnitOfMeassure.ToString() }));
					transaction.Commit();
				}
			}
		}

		/// <summary>
		/// Searches the database and updates <see cref="Item"/>'s current price, on change updates price history as well
		/// </summary>
		public void UpdateNewCurrentPrice(int itemID, int price, DateTime purchaseTime) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				using (IDbTransaction transaction = connection.BeginTransaction()) {

					connection.Execute(new CommandDefinition($"UPDATE {DbItem.DName} SET {DbItem.DName}.{nameof(DbItem.Value)} = {price}"));

					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemValueHistory.DName} ({nameof(DbItemValueHistory.ItemID)},{nameof(DbItemValueHistory.Value)},{nameof(DbItemValueHistory.Date)}) " +
															 $"VALUES ({itemID}, {price}, @Date)", new { Date = purchaseTime.ToString(DbDateTimeFormat) }));
				}
			}
		}

		/// <summary>
		/// Update items list of OCR recognized strings
		/// </summary>
		public void AddOcrName(int itemID, string ocrName) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				try {
					connection.Execute(new CommandDefinition($"INSERT INTO {DbItemOcrNames.DName} ({nameof(DbItemOcrNames.ItemID)},{nameof(DbItemOcrNames.OcrName)}) " +
															 $"VALUES ({itemID}, @OcrName)", new { OcrName = ocrName }));
				}
				catch (SQLiteException e) {
					if (e.ErrorCode == 19) {
						// UNIQUE is not satisfied, that is OK
					}
				}
			}
		}

		/// <summary>
		/// Updates <see cref="Item"/>'s <see cref="MeassurementUnit"/>
		/// </summary>
		public void UpdateUnitOfMeassure(int itemID, MeassurementUnit unit) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				connection.Execute(new CommandDefinition($"UPDATE {DbItemMetadata.DName} SET {DbItemMetadata.DName}.{nameof(DbItemMetadata.UnitOfMeassure)} = {unit.ToString()}"));
			}
		}

		/// <summary>
		/// Records new purchase for an <see cref="Item"/> into the database
		/// </summary>
		public void AddNewPurchaseForItemToDatabase(int itemID, ItemPurchaseHistory history, DateTime purchaseTime, IDbConnection connection) {
			int selectedValue = connection.QueryFirst<int>(new CommandDefinition($"SELECT {DbItem.DName}.{nameof(DbItem.Value)} FROM {DbItem.DName} WHERE {DbItem.DName}.{nameof(DbItem.ID)} = {itemID}"));

			if (selectedValue != history.Price) {
				UpdateNewCurrentPrice(itemID, history.Price, purchaseTime);
			}

			connection.Execute(new CommandDefinition($"INSERT INTO {DbItemPurchase.DName} ({nameof(DbItemPurchase.ItemID)}, {nameof(DbItemPurchase.PurchaseID)}, {nameof(DbItemPurchase.Amount)}, {nameof(DbItemPurchase.ValuePerItem)})" +
													 $"VALUES ({itemID}, {history.PurchaseID}, {history.Amount}, {history.Price})"));

		}

		/// <summary>
		/// Writes new purchase information into the database
		/// </summary>
		public int WriteNewPurchaseInstance(Purchase purchaseInstance, IDbConnection connection) {
			connection.Execute(new CommandDefinition($"INSERT INTO {DbPurchase.DName} ({nameof(DbPurchase.ShopID)}, {nameof(DbPurchase.Date)}, {nameof(DbPurchase.TotalValue)}) " +
													 $"VALUES ({(int)purchaseInstance.Shop}, @Date, {purchaseInstance.TotalCost})", new { Date = purchaseInstance.PurchaseDate.ToString(DbDateTimeFormat) }));

			int latest = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
			return latest;
		}


		public void RecordPurchase(Purchase purchase, ItemPurchaseData[] purchasedItems) {
			using (IDbConnection connection = new SQLiteConnection(DbConnectionString)) {
				connection.Open();
				using (IDbTransaction transaction = connection.BeginTransaction()) {

					int purchaseID = WriteNewPurchaseInstance(purchase, connection);
					for (int i = 0; i < purchasedItems.Length; i++) {
						AddNewPurchaseForItemToDatabase(purchasedItems[i].Item.ID,
														new ItemPurchaseHistory(purchaseID, purchasedItems[i].Amount, purchasedItems[i].Item.CurrentPriceInt),
														purchase.PurchaseDate,
														connection);
					}
					transaction.Commit();
				}
			}
		}
	}
}