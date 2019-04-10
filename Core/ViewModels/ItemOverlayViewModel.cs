namespace Igor.BillScanner.Core {
	public class ItemOverlayViewModel : BaseViewModel {

		public string ItemName => _dataSource.ItemName;

		public string OCRNames => string.Join(",", _dataSource.OcrNames);

		public string CurrentPrice => _dataSource.CurrentPriceDecimal.ToString();

		public string PriceHistory => string.Join(",", _dataSource.PriceHistory.Values);

		public string BoughtTotal => _dataSource.TotalPurchased.ToString();

		private Item _dataSource;

		public ItemOverlayViewModel() {
			_dataSource = new Item("Undefined Item", int.MinValue);
		}

		public ItemOverlayViewModel(Item item) {
			_dataSource = item;
		}
	}
}
