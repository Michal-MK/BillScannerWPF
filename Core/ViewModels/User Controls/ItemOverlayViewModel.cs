using System;
using System.Globalization;
using System.Windows.Input;
using Igor.Models;

namespace Igor.BillScanner.Core {
	public class ItemOverlayViewModel : BaseViewModel {

		#region Backing Fields

		private ICommand _closeCommand;
		private bool _controlVisible;
		private UIItemViewModel _modelPreview;
		private Func<string, bool> _priceValidationFunc;
		private Func<string, bool> _quantityValidationFunc;
		private string _currentPrice;
		private string _currentQuantity;

		#endregion

		public string CurrentQuantity { get => _currentQuantity; set { _currentQuantity = value; Notify(nameof(CurrentQuantity)); } }
		public string CurrentPrice { get => _currentPrice; set { _currentPrice = value; Notify(nameof(CurrentPrice)); } }

		public Func<string, bool> QuantityValidationFunc { get => _quantityValidationFunc; set { _quantityValidationFunc = value; Notify(nameof(QuantityValidationFunc)); } }
		public Func<string, bool> PriceValidationFunc { get => _priceValidationFunc; set { _priceValidationFunc = value; Notify(nameof(PriceValidationFunc)); } }
		public string OCRNames => string.Join(",", Model.ItemPurchase.Item.OcrNames);
		public string PriceHistory => string.Join(",", Model.ItemPurchase.Item.PriceHistory.Values);


		public int AmountPurchased { get => _modelPreview.ItemPurchase.Amount; set { _modelPreview.ItemPurchase.Amount = value; Notify(nameof(AmountPurchased)); } }

		public ICommand CloseCommand { get => _closeCommand; set { _closeCommand = value; Notify(nameof(CloseCommand)); } }
		public bool ControlVisible { get => _controlVisible; set { _controlVisible = value; Notify(nameof(ControlVisible)); } }
		public UIItemViewModel Model { get => _modelPreview; set { _modelPreview = value; Notify(nameof(Model)); } }

		public ItemOverlayViewModel() {
			SetNewSource(new UIItemViewModel(new ItemPurchaseData(new Item("NONE", -1), 0), -1, MatchRating.Fail, "NONE"), false);
			CloseCommand = new Command(() => ControlVisible = false);
			QuantityValidationFunc = ValidQuantity;
			PriceValidationFunc = ValidCurrency;
		}

		public void SetNewSource(UIItemViewModel source, bool show = true) {
			_modelPreview = source;
			CurrentPrice = (Model.CurrentPrice / 100m).ToString("0.00");
			Notify(nameof(OCRNames));
			Notify(nameof(CurrentPrice));
			Notify(nameof(PriceHistory));
			ControlVisible = show;
		}

		public bool ValidQuantity(string input) {
			if (int.TryParse(input, out int value)) {
				Model.ItemPurchase.Amount = value;
				return true;
			}
			return false;
		}

		public bool ValidCurrency(string input) {
			NumberStyles style = NumberStyles.Currency | NumberStyles.AllowDecimalPoint;
			if(decimal.TryParse(input, style, CultureInfo.InvariantCulture, out decimal value)) {
				Model.CurrentPrice = (int)(value * 100);
				return true;
			}
			return false;
		}
	}
}
