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
		}

		public string userFriendlyName { get; }
		public List<string> ocrNames { get; }
		public MeassurementUnit unitOfMeassure { get; private set; }
		public decimal currentPrice { get; private set; }
		public List<decimal> pricesInThePast { get; }
		public long totalPurchased { get; private set; }
		public List<PurchaseHistory> purchaseHistory { get; }


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
			return new ItemSlim(item.userFriendlyName, 0, item.currentPrice);
		}
	}

	public class ItemSlim {
		public ItemSlim(string name, long amountPurchased, decimal price) {
			this.name = name;
			this.amountPurchased = amountPurchased;
			this.price = price;
		}

		public string name { get; }
		public long amountPurchased { get; }
		public decimal price { get; }


		public static implicit operator Item(ItemSlim item) {
			return new Item(item.name, item.price);
		}
	}

}
