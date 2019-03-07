using System;
using System.Collections.Generic;
using Igor.BillScanner.Core.Database;

namespace Igor.BillScanner.Core {

	public class Item {

		/// <summary>
		/// Create a new instance of an <see cref="Item"/> with a user-friendly name and its price
		/// </summary>
		public Item(string userFriendlyName, int currentPrice) {
			OcrNames = new List<string>();
			PriceHistory = new Dictionary<DateTime, int>();

			ItemName = userFriendlyName;
			CurrentPriceInt = currentPrice;
			ID = -1;
		}

		/// <summary>
		/// Create a new <see cref="Item"/> by querying the database and connecting all the needed entries
		/// </summary>
		public Item(DbItem current, IEnumerator<string> ocrNames, IEnumerator<Database.DbItemValueHistory> pastValues) {
			OcrNames = new List<string>();
			PriceHistory = new Dictionary<DateTime, int>();

			ID = current.ID;
			ItemName = current.Name;
			CurrentPriceInt = current.Value;

			while (ocrNames.MoveNext()) {
				OcrNames.Add(ocrNames.Current);
			}
			while (pastValues.MoveNext()) {
				PriceHistory.Add(DateTime.Parse(pastValues.Current.Date), pastValues.Current.Value);
			}
		}

		/// <summary>
		/// The ID int that is unique to this item (Primary key)
		/// </summary>
		public int ID { get; private set; }

		/// <summary>
		/// The name to be displayed to the user
		/// </summary>
		public string ItemName { get; private set; }

		/// <summary>
		/// List of names the parser uses to match OCR'd text to this item
		/// </summary>
		public List<string> OcrNames { get; }

		/// <summary>
		/// Units this item is measured in
		/// </summary>
		public MeassurementUnit UnitOfMeassure { get; private set; }

		/// <summary>
		/// The last price this item was bought for
		/// </summary>
		public int CurrentPriceInt { get; private set; }

		/// <summary>
		/// The last price represented in decimal value
		/// </summary>
		public decimal CurrentPriceDecimal => CurrentPriceInt / 100m;

		/// <summary>
		/// List of prices this item was valued in the past
		/// </summary>
		public Dictionary<DateTime, int> PriceHistory { get; }

		/// <summary>
		/// Number of purchases of this item
		/// </summary>
		public int TotalPurchased { get; private set; }

		/// <summary>
		/// History of purchases of this item
		/// </summary>
		public List<ItemPurchaseHistory> PurchaseHistory => DatabaseAccess.access.GetItemPuchaseHistory(ID);

		/// <summary>
		/// Is this item already present in the database
		/// </summary>
		public bool IsRegistered => DatabaseAccess.access.GetItem(ID) == null;

		/// <summary>
		/// Modify this item internally before it is added for the first time to the database
		/// </summary>
		internal void Modify(string newName = null, int? newValue = null) {
			if (IsRegistered) {
				return;
			}
			ItemName = newName ?? ItemName;
			CurrentPriceInt = newValue ?? newValue.Value;
		}

		/// <summary>
		/// Add new OCR name to the OCR names list and updates the database
		/// </summary>
		public void AddOCRName(string newOcrName) {
			OcrNames.Add(newOcrName);
			DatabaseAccess.access.AddOcrName(ID, newOcrName);
		}

		/// <summary>
		/// Add new OCR name to the OCR names list
		/// </summary>
		public void AddOCRNameNew(string newOcrName) {
			OcrNames.Add(newOcrName);
		}

		/// <summary>
		/// Adds the amount into the counter of how may items of this type were bought
		/// </summary>
		public void AddTotalAmountPurchased(int amount) {
			TotalPurchased += amount;
		}

		/// <summary>
		/// Sets new active price for the item, can update the database
		/// </summary>
		public void SetNewCurrentPrice(int price, DateTime purchaseTime, bool updateDatabase = false) {
			CurrentPriceInt = price;
			PriceHistory.Add(purchaseTime, price);

			if (updateDatabase) {
				DatabaseAccess.access.UpdateNewCurrentPrice(ID, price, purchaseTime);
			}
		}

		/// <summary>
		/// Sets the unit of measure for the item, can update the database
		/// </summary>
		public void SetUnitOfMeassure(MeassurementUnit unit, bool updateDatabase = false) {
			UnitOfMeassure = unit;
			if (updateDatabase) {
				DatabaseAccess.access.UpdateUnitOfMeassure(ID, unit);
			}
		}
	}
}
