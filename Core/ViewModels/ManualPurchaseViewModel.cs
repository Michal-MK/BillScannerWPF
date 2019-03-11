using System.Collections.ObjectModel;
using System.Linq;

namespace Igor.BillScanner.Core {
	public class ManualPurchaseViewModel : BaseViewModel {
		private Item[] _items;
		private readonly IManualPurchaseHandler _handler;
		public string SearchQuery { get; set; }

		public ObservableCollection<ItemList_ItemViewModel> Items { get; set; }

		public ManualPurchaseViewModel(IManualPurchaseHandler handler) {
			_handler = handler;
			_items = DatabaseAccess.access.GetItems(_handler.Shop);
		}

		public void PopulateList() {
			Items = new ObservableCollection<ItemList_ItemViewModel>();

			if (string.IsNullOrWhiteSpace(SearchQuery)) {
				foreach (Item i in _items) {
					Items.Add(new ItemList_ItemViewModel(i));
				}
			}
			else {
				foreach (Item i in _items.Where((i) => { return i.ItemName.Contains(SearchQuery); })) {
					Items.Add(new ItemList_ItemViewModel(i));
				}
			}
			Notify(nameof(Items));
		}
	}
}
