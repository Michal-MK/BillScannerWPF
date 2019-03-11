namespace Igor.BillScanner.Core {
	public class ItemList_ItemViewModel : BaseViewModel {
		public string ItemName => _item.ItemName;

		public string ItemPrice => string.Format("{0:f2}Kč", _item.CurrentPriceDecimal);

		public Item Item => _item;

		private Item _item;

		public ItemList_ItemViewModel() {
			_item = new Item("UNDEFINED", -1);
		}

		public ItemList_ItemViewModel(Item asociated) {
			_item = asociated;
		}
	}
}