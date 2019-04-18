namespace Igor.BillScanner.Core {
	public class UIItemViewModel : BaseViewModel {

		#region Backing Fields

		private UIItemCreationInfo _creationInfo;

		#endregion

		public UIItemViewModel() { }

		public UIItemViewModel(UIItemCreationInfo item) {
			_creationInfo = item;
		}

		public string ItemName {
			get {
				string ret = (int)MatchQuality <= 1 ? Item.ItemName : ItemCreation.MatchTriggerLine;
				ret += " | Price: " + string.Format("{0:f2}", ItemCreation.CurrentPrice) + "Kč";
				return ret;
			}
		}

		public int AmountPurchased => ItemCreation.Amount;

		public MatchRating MatchQuality => ItemCreation.MatchQuality;

		public Item Item => ItemCreation.Item;

		public UIItemCreationInfo ItemCreation {
			get { return _creationInfo; }
			set { _creationInfo = value; Notify(nameof(ItemCreation)); Notify(nameof(ItemName)); }
		}
	}
}
