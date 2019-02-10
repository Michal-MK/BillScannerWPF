namespace BillScannerCore {
	public class ItemPurchaseData {


		public ItemPurchaseData(Item item, int amount) {
			this.item = item;
			this.amount = amount;
		}

		public Item item { get; private set; }
		public int amount { get; private set; }
	}
}
