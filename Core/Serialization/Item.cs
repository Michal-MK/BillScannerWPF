﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BillScannerCore {
	[Serializable]
	public class Item {

		[JsonConstructor]
		private Item(string userFriendlyName, string identifier, List<string> ocrNames, MeassurementUnit unitOfMeassure, decimal currentPrice, List<decimal> pricesInThePast, long totalPurchased, List<ItemPurchaseHistory> purchaseHistory) {
			this.userFriendlyName = userFriendlyName;
			this.identifier = identifier;
			this.ocrNames = ocrNames;
			this.unitOfMeassure = unitOfMeassure;
			this.currentPrice = currentPrice;
			this.pricesInThePast = pricesInThePast;
			this.totalPurchased = totalPurchased;
			this.purchaseHistory = purchaseHistory;
			this.isRegistered = true;
		}
		
		/// <summary>
		/// Create a new instance of an Item with a friendly name and value
		/// </summary>
		/// <param name="userFriendlyName">The name to be displayed to the user</param>
		/// <param name="currentPrice">The value of the item</param>
		public Item(string userFriendlyName, decimal currentPrice) {
			ocrNames = new List<string>();
			pricesInThePast = new List<decimal>();
			purchaseHistory = new List<ItemPurchaseHistory>();
			this.userFriendlyName = userFriendlyName;
			this.currentPrice = currentPrice;
			identifier = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// The name to be displayed to the user
		/// </summary>
		public string userFriendlyName { get; private set; }

		/// <summary>
		/// The GUID string that is unique to this item (Primary key)
		/// </summary>
		public string identifier { get; private set; }

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
		public decimal currentPrice { get; private set; }

		/// <summary>
		/// List of prices this item was valued in the past
		/// </summary>
		public List<decimal> pricesInThePast { get; }

		/// <summary>
		/// Number of purchases of this item
		/// </summary>
		public long totalPurchased { get; private set; }

		/// <summary>
		/// History of purchases of this item
		/// </summary>
		public List<ItemPurchaseHistory> purchaseHistory { get; }

		#region Internal fields and properties

		public bool isRegistered { get; set; }
		[JsonIgnore]
		public string tirggerForMatch { get; set; }

		public void AddOCRName(string v) {
			ocrNames.Add(v);
		}

		public void AddAmount(long amount) {
			totalPurchased += amount;
		}

		public void SetNewCurrentPrice(decimal price) {
			currentPrice = price;
			DatabaseAccess.access.WriteNewCurrentPriceToDatabase(identifier, price);
		}
		public void SetUnitOfMeassure(MeassurementUnit unit) {
			unitOfMeassure = unit;
			DatabaseAccess.access.WriteUnitOfMeassureForItemToDatabase(identifier, unit);
		}

		public void PurchaseModifications(string modifiedName, decimal finalPrice) {
			userFriendlyName = modifiedName;
			currentPrice = finalPrice;
		}

		#endregion
	}
}