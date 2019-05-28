using System.Collections.ObjectModel;
using System.Linq;

namespace Igor.BillScanner.Core {
	public class ManualPurchaseViewModel : BaseViewModel {

		private ObservableCollection<ItemList_ItemViewModel> _items = new ObservableCollection<ItemList_ItemViewModel>();

		public ObservableCollection<ItemList_ItemViewModel> ManualItems { get => _items; set { _items = value; Notify(nameof(ManualItems)); } }

		public ManualPurchaseViewModel() {
			ManualItems = DatabaseAccess.Access.GetItems().Select(s => new ItemList_ItemViewModel(s)).ToObservable();
		}
	}
}
