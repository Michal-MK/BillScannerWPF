using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillScannerWPF {
	[Serializable]
	public class Item {
		public Item(string mainName, decimal currentPrice) {
			this.mainName = mainName;
			this.currentPrice = currentPrice;
			ocrNames = new List<string>() {
				mainName
			};

			pricesInThePast = new List<decimal>() {
				currentPrice
			};
			totalPurchased = 0;
			purchaseHistory = new List<PurchaseHistory>();
		}

		public string mainName { get; }
		public List<string> ocrNames { get; }
		public string unitOfMeassure { get; }
		public decimal currentPrice { get; private set; }
		public List<decimal> pricesInThePast { get; }
		public long totalPurchased { get; private set; }
		public List<PurchaseHistory> purchaseHistory { get; }

		internal string tirggerForMatch { get; set; }

		internal void AddAmount(int amount) {
			totalPurchased += amount;
		}

		internal void SetNewCurrentPrice(decimal price) {
			currentPrice = price;
		}

		public static implicit operator SimpleItem (Item item) {
			return new SimpleItem(item.mainName, 0, item.currentPrice);
		}
	}

	public class SimpleItem {
		public SimpleItem(string name, long amountPurchased, decimal price) {
			this.name = name;
			this.amountPurchased = amountPurchased;
			this.price = price;
		}


		public string name { get; }
		public long amountPurchased { get; }
		public decimal price { get; }


		public static implicit operator Item(SimpleItem item) {
			return new Item(item.name, item.price);
		}
	}

}
