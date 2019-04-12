namespace Igor.BillScanner.Core {
	public class UIItemViewModel : BaseViewModel {

		#region Backing Fields

		private MatchRating _matchQuality;
		private Item _item;
		private UIItemCreationInfo _creationInfo;
		private int _amountPurchased;

		#endregion

		public string ItemName {
			get {
				return string.IsNullOrEmpty(Item.MatchTriggerLine) ? itemCreation.Item.ItemName : itemCreation.MatchTriggerLine + " | Price: " + string.Format("{0:f2}", itemCreation.CurrentPrice) + "Kč";
				return _item.ItemName + " | Price: " + string.Format("{0:f2}", _item.CurrentPriceDecimal) + " Kč";
			}
		}


		public int AmountPurchased {
			get { return _amountPurchased; }
			set { _amountPurchased = value; Notify(nameof(AmountPurchased)); }
		}
		public MatchRating MatchQuality {
			get { return _matchQuality; }
			set { _matchQuality = value; Notify(nameof(MatchQuality)); }
		}
		public Item Item {
			get { return _item; }
			set { _item = value; Notify(nameof(Item)); }
		}
		public UIItemCreationInfo ItemCreation {
			get { return _creationInfo; }
			set { _creationInfo = value; Notify(nameof(ItemCreation)); }
		}

	}
}
