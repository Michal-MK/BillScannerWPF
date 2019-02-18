namespace BillScannerCore {
	public class ItemPurchaseData {

		/// <summary>
		/// Default constructor
		/// </summary>
		public ItemPurchaseData(Item item, int amount) {
			this.item = item;
			this.amount = amount;
		}

		/// <summary>
		/// The item purchased
		/// </summary>
		public Item item { get; }

		/// <summary>
		/// How many units were bought
		/// </summary>
		public int amount { get; }
	}
}
