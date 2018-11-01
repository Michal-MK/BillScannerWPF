namespace BillScannerCore {
	public class ItemPurchaseData {


		public ItemPurchaseData(Item item, long quantityPurchased) {
			this.item = item;
			this.quantityPurchased = quantityPurchased;
		}

		public Item item { get; private set; }
		public long quantityPurchased { get; private set; }
	}
}
