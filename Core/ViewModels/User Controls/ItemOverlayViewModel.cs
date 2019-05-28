using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class ItemOverlayViewModel : BaseViewModel {

		private ICommand _closeCommand;
		private bool _controlVisible;

		public string ItemName => _dataSource.ItemName;

		public string OCRNames => string.Join(",", _dataSource.OcrNames);

		public string CurrentPrice => _dataSource.CurrentPriceDecimal.ToString();

		public string PriceHistory => string.Join(",", _dataSource.PriceHistory.Values);

		public string BoughtTotal => _dataSource.TotalPurchased.ToString();

		public ICommand CloseCommand { get => _closeCommand; set { _closeCommand = value; Notify(nameof(CloseCommand)); } }
		public bool ControlVisible { get => _controlVisible; set { _controlVisible = value; Notify(nameof(ControlVisible)); } }

		private Item _dataSource;

		public ItemOverlayViewModel() {
			_dataSource = new Item("Undefined Item", int.MinValue);
			CloseCommand = new Command(() => { ControlVisible = false; });
		}

		public void SetNewSource(Item source, bool show = true) {
			_dataSource = source;
			Notify(nameof(ItemName));
			Notify(nameof(OCRNames));
			Notify(nameof(CurrentPrice));
			Notify(nameof(PriceHistory));
			Notify(nameof(BoughtTotal));
			ControlVisible = show;
		}
	}
}
