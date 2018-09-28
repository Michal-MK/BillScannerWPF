using System;
using System.Collections.Generic;

namespace BillScannerWPF {
	[Serializable]
	public class Item {
		public Item(string userFriendlyName, decimal currentPrice) {
			this.userFriendlyName = userFriendlyName;
			this.currentPrice = currentPrice;
			ocrNames = new List<string>();
			pricesInThePast = new List<decimal>();
			purchaseHistory = new List<PurchaseHistory>();
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

		internal void AddAmount(int amount) {
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

		//public static bool operator !=(Item item, Item other) {
		//	return item.identifier != other.identifier;
		//}
		//public static bool operator ==(Item item, Item other) {
		//	return item.identifier == other.identifier;
		//}
	}
}
