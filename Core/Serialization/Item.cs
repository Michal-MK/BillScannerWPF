using System;
using System.Collections.Generic;

namespace BillScannerCore {

	public class Item {

		/// <summary>
		/// Create a new instance of an <see cref="Item"/> with a user-friendly name and its price
		/// </summary>
		public Item(string userFriendlyName, int currentPrice) {
			ocrNames = new List<string>();
			pricesInThePast = new Dictionary<DateTime, int>();

			this.userFriendlyName = userFriendlyName;
			this.currentPrice = currentPrice;
			ID = -1;
		}

		/// <summary>
		/// Create a new <see cref="Item"/> by querying the database and connecting all the needed entries
		/// </summary>
		public Item(Core.Database.DbItem current, IEnumerator<string> ocrNames, IEnumerator<Core.Database.DbItemValueHistory> pastValues) {
			this.ocrNames = new List<string>();
			pricesInThePast = new Dictionary<DateTime, int>();

			ID = current.ID;
			userFriendlyName = current.Name;
			currentPrice = current.Value;

			while (ocrNames.MoveNext()) {
				this.ocrNames.Add(ocrNames.Current);
			}
			while (pastValues.MoveNext()) {
				pricesInThePast.Add(DateTime.Parse(pastValues.Current.Date), pastValues.Current.Value);
			}
		}

		/// <summary>
		/// The ID int that is unique to this item (Primary key)
		/// </summary>
		public int ID { get; private set; }

		/// <summary>
		/// The name to be displayed to the user
		/// </summary>
		public string userFriendlyName { get; private set; }

		/// <summary>
		/// List of names the parser uses to match OCR'd text to this item
		/// </summary>
		public List<string> ocrNames { get; }

		/// <summary>
		/// Units this item is measured in
		/// </summary>
		public MeassurementUnit unitOfMeassure { get; private set; }

		/// <summary>
		/// The last price this item was bought for
		/// </summary>
		public int currentPrice { get; private set; }

		/// <summary>
		/// List of prices this item was valued in the past
		/// </summary>
		public Dictionary<DateTime, int> pricesInThePast { get; }

		/// <summary>
		/// Number of purchases of this item
		/// </summary>
		public int totalPurchased { get; private set; }

		/// <summary>
		/// History of purchases of this item
		/// </summary>
		public List<ItemPurchaseHistory> purchaseHistory => DatabaseAccess.access.GetItemPuchaseHistory(ID);

		/// <summary>
		/// Is this item already present in the database
		/// </summary>
		public bool isRegistered => DatabaseAccess.access.GetItem(ID) == null;

		/// <summary>
		/// Modify this item internally before it is added for the first time to the database
		/// </summary>
		internal void Modify(string newName = null, int? newValue = null) {
			if (isRegistered) {
				return;
			}
			userFriendlyName = newName ?? userFriendlyName;
			currentPrice = newValue ?? newValue.Value;
		}

		/// <summary>
		/// Add new OCR name to the OCR names list
		/// </summary>
		public void AddOCRName(string newOcrName) {
			ocrNames.Add(newOcrName);
			DatabaseAccess.access.AddOcrName(ID, newOcrName);
		}

		/// <summary>
		/// Adds the amount into the counter of how may items of this type were bought
		/// </summary>
		public void AddTotalAmountPurchased(int amount) {
			totalPurchased += amount;
		}

		/// <summary>
		/// Sets new active price for the item, can update the database
		/// </summary>
		public void SetNewCurrentPrice(int price, DateTime purchaseTime, bool updateDatabase = false) {
			currentPrice = price;
			pricesInThePast.Add(purchaseTime, price);

			if (updateDatabase) {
				DatabaseAccess.access.UpdateNewCurrentPrice(ID, price, purchaseTime);
			}
		}

		/// <summary>
		/// Sets the unit of measure for the item, can update the database
		/// </summary>
		public void SetUnitOfMeassure(MeassurementUnit unit, bool updateDatabase = false) {
			unitOfMeassure = unit;
			if (updateDatabase) {
				DatabaseAccess.access.UpdateUnitOfMeassure(ID, unit);
			}
		}
	}
}
