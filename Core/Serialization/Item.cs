using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BillScannerCore {

	public class Item {
		
		/// <summary>
		/// Create a new instance of an Item with a friendly name and value
		/// </summary>
		/// <param name="userFriendlyName">The name to be displayed to the user</param>
		/// <param name="currentPrice">The value of the item</param>
		public Item(string userFriendlyName, int currentPrice) {
			ocrNames = new List<string>();
			pricesInThePast = new Dictionary<DateTime, int>();
			this.userFriendlyName = userFriendlyName;
			this.currentPrice = currentPrice;
			identifier = -1;
		}

		internal Item(Core.Database.DbItem current, IEnumerator<Core.Database.DbItemOcrNames> ocrNames, IEnumerator<Core.Database.DbItemValueHistory> pastValues) {
			this.identifier = current.ID;
			this.userFriendlyName = current.Name;
			this.ocrNames = new List<string>();
			this.pricesInThePast = new Dictionary<DateTime, int>();
			while (ocrNames.MoveNext()) {
				this.ocrNames.Add(ocrNames.Current.OcrName);
			}
			this.currentPrice = current.Value;
			while (pastValues.MoveNext()) {
				this.pricesInThePast.Add(DateTime.Parse(pastValues.Current.Date),pastValues.Current.Value);
			}
		}

		/// <summary>
		/// The ID int that is unique to this item (Primary key)
		/// </summary>
		public int identifier { get; private set; }

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
		public Dictionary<DateTime,int> pricesInThePast { get; }

		/// <summary>
		/// Number of purchases of this item
		/// </summary>
		public long totalPurchased { get; private set; }

		/// <summary>
		/// History of purchases of this item
		/// </summary>
		public List<ItemPurchaseHistory> purchaseHistory => DatabaseAccess.access.GetItemPuchaseHistory(identifier);


		internal bool isRegistered => DatabaseAccess.access.GetItem(identifier) == null;

		#region Internal fields and properties

		public void AddOCRName(string newOcrName) {
			ocrNames.Add(newOcrName);
			DatabaseAccess.access.AddOcrName(identifier, newOcrName);
		}

		public void AddAmount(long amount) {
			totalPurchased += amount;
		}

		public void SetNewCurrentPrice(int price, DateTime purchaseTime) {
			currentPrice = price;
			DatabaseAccess.access.UpdateNewCurrentPrice(identifier, price, purchaseTime);
		}
		public void SetUnitOfMeassure(MeassurementUnit unit) {
			unitOfMeassure = unit;
			DatabaseAccess.access.UpdateUnitOfMeassure(identifier, unit);
		}

		#endregion
	}
}
