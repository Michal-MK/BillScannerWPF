namespace Igor.BillScanner.Core {
	public class ItemPurchaseData {

		/// <summary>
		/// Default constructor
		/// </summary>
		public ItemPurchaseData(Item item, int amount) {
			Item = item;
			Amount = amount;
		}

		/// <summary>
		/// The item purchased
		/// </summary>
		public Item Item { get; }

		/// <summary>
		/// How many units were bought
		/// </summary>
		public int Amount { get; set; }
	}
}
