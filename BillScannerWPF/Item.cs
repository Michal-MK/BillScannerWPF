using System;
using System.Collections.Generic;

namespace BillScannerWPF {
	[Serializable]
	public class Item {
		public Item(string userFriendlyName, decimal currentPrice, bool isSingleLine) {
			this.userFriendlyName = userFriendlyName;
			this.currentPrice = currentPrice;
			this.isSingleLine = isSingleLine;
			ocrNames = new List<string>();
			pricesInThePast = new List<decimal>();
			purchaseHistory = new List<PurchaseHistory>();
		}

		public string userFriendlyName { get; private set; }
		public List<string> ocrNames { get; }
		public MeassurementUnit unitOfMeassure { get; private set; }
		public decimal currentPrice { get; private set; }
		public List<decimal> pricesInThePast { get; }
		public long totalPurchased { get; private set; }
		public List<PurchaseHistory> purchaseHistory { get; }
		public bool isSingleLine { get; private set; }

		internal bool isRegistered { get; set; }
		internal string tirggerForMatch { get; set; }

		internal void AddAmount(int amount) {
			totalPurchased += amount;
		}

		internal void SetNewCurrentPrice(decimal price) {
			currentPrice = price;
		}
		internal void SetUnitOfMeassure(MeassurementUnit unit) {
			unitOfMeassure = unit;
		}

		public static implicit operator ItemSlim(Item item) {
			return new ItemSlim(item.userFriendlyName, 0, item.currentPrice, item.isSingleLine);
		}

		internal void PurchaseModifications(string modifiedName, decimal finalPrice) {
			userFriendlyName = modifiedName;
			currentPrice = finalPrice;
		}
	}

	public class ItemSlim {
		public ItemSlim(string name, long amountPurchased, decimal price, bool isSingleLine) {
			this.name = name;
			this.amountPurchased = amountPurchased;
			this.price = price;
			this.isSingleLine = isSingleLine;
		}

		public string name { get; }
		public long amountPurchased { get; }
		public decimal price { get; }
		public bool isSingleLine { get; }

		public static implicit operator Item(ItemSlim item) {
			return new Item(item.name, item.price, item.isSingleLine);
		}
	}

}
