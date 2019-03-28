namespace Igor.BillScanner.Core {
	public class ItemList_ItemViewModel : BaseViewModel {
		public string ItemName => Item.ItemName;

		public string ItemPrice => string.Format("{0:f2}Kč", Item.CurrentPriceDecimal);

		public Item Item { get; }

		public ItemList_ItemViewModel() {
			Item = new Item("Undefined Item", int.MinValue);
		}

		public ItemList_ItemViewModel(Item asociated) {
			Item = asociated;
		}
	}
}