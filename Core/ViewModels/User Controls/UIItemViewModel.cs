using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class UIItemViewModel : BaseViewModel {

		#region Backing Fields

		private ICommand _showDetailsCommand;

		#endregion

		public UIItemViewModel() {
			ShowDetailsCommand = new Command(() => {
				MainWindowViewModel.Instance.ItemInfoOverlayViewModel.SetNewSource(this);
			});
		}

		public UIItemViewModel(ItemPurchaseData itemPurchase, int currentPrice, MatchRating matchQuality, string triggerLine) : this() {
			ItemPurchase = itemPurchase;
			MatchQuality = matchQuality;
			MatchTriggerLine = triggerLine;
			CurrentPrice = currentPrice;
		}

		public ICommand ShowDetailsCommand { get => _showDetailsCommand; set { _showDetailsCommand = value; Notify(nameof(ShowDetailsCommand)); } }

		public ItemPurchaseData ItemPurchase { get; set; }
		public int CurrentPrice { get; set; }
		public MatchRating MatchQuality { get; }
		public string MatchTriggerLine { get; }

		public string ItemName {
			get {
				string ret = MatchQuality <= MatchRating.One ? ItemPurchase.Item.ItemName : MatchTriggerLine;
				ret += " | Price: " + ItemPurchase.Item.CurrentPriceDecimal.ToString("0.00") + "Kč";
				ret += " | Qty: " + ItemPurchase.Amount;
				return ret;
			}
		}
	}
}
