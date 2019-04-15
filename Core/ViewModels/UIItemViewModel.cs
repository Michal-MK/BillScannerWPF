namespace Igor.BillScanner.Core {
	public class UIItemViewModel : BaseViewModel {

		#region Backing Fields

		private MatchRating _matchQuality = MatchRating.Success;
		private Item _item;
		private UIItemCreationInfo _creationInfo;
		private int _amountPurchased;



		#endregion

		public UIItemViewModel() { }

		public UIItemViewModel(UIItemCreationInfo item) {
			_creationInfo = item;
			_matchQuality = item.MatchQuality;
			_item = item.Item;
			_amountPurchased = item.Amount;
		}

		public string ItemName => "Helo";/*string.IsNullOrEmpty(ItemCreation.MatchTriggerLine) ? Item.ItemName : ItemCreation.MatchTriggerLine +
								  " | Price: " + string.Format("{0:f2}", ItemCreation.CurrentPrice) + "Kč";*/

		public int AmountPurchased {
			get { return _amountPurchased; }
			set { _amountPurchased = value; Notify(nameof(AmountPurchased)); Notify(ItemName); }
		}
		public MatchRating MatchQuality {
			get { return _matchQuality; }
			set { _matchQuality = value; Notify(nameof(MatchQuality)); }
		}
		public Item Item {
			get { return _item; }
			set { _item = value; Notify(nameof(Item)); Notify(ItemName); }
		}
		public UIItemCreationInfo ItemCreation {
			get { return _creationInfo; }
			set { _creationInfo = value; Notify(nameof(ItemCreation)); Notify(ItemName); }
		}
	}
}
