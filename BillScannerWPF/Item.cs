using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BillScannerWPF {
	[Serializable]
	public class Item {

		[JsonConstructor]
		private Item(string userFriendlyName, string identifier, List<string> ocrNames, MeassurementUnit unitOfMeassure, decimal currentPrice, List<decimal> pricesInThePast, long totalPurchased, List<PurchaseHistory> purchaseHistory) {
			this.userFriendlyName = userFriendlyName;
			this.identifier = identifier;
			this.ocrNames = ocrNames;
			this.unitOfMeassure = unitOfMeassure;
			this.currentPrice = currentPrice;
			this.pricesInThePast = pricesInThePast;
			this.totalPurchased = totalPurchased;
			this.purchaseHistory = purchaseHistory;
		}

		public Item(string userFriendlyName, decimal currentPrice) {
			ocrNames = new List<string>();
			pricesInThePast = new List<decimal>();
			purchaseHistory = new List<PurchaseHistory>();
			this.userFriendlyName = userFriendlyName;
			this.currentPrice = currentPrice;
			identifier = Guid.NewGuid().ToString();
		}

		public string userFriendlyName { get; private set; }
		public string identifier { get; private set; }
		public List<string> ocrNames { get; }
		public MeassurementUnit unitOfMeassure { get; private set; }
		public decimal currentPrice { get; private set; }
		public List<decimal> pricesInThePast { get; }
		public long totalPurchased { get; private set; }
		public List<PurchaseHistory> purchaseHistory { get; }

		internal bool isSingleLine { get; set; }
		internal bool isRegistered { get; set; }
		internal string tirggerForMatch { get; set; }

		internal void AddAmount(long amount) {
			totalPurchased += amount;
		}

		internal void SetNewCurrentPrice(decimal price) {
			currentPrice = price;
			MainWindow.access.WriteNewCurrentPriceToDatabase(identifier, price);
		}
		internal void SetUnitOfMeassure(MeassurementUnit unit) {
			unitOfMeassure = unit;
			MainWindow.access.WriteUnitOfMeassureForItemToDatabase(identifier, unit);
		}

		internal void PurchaseModifications(string modifiedName, decimal finalPrice) {
			userFriendlyName = modifiedName;
			currentPrice = finalPrice;
		}
	}
}
