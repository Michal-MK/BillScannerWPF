using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class ItemListViewModel : BaseViewModel {

		#region BackingFields

		private ICommand _selectedCommand;
		private ICommand _abortCommand;
		private string _errorOutput;
		private string _searchString;
		private ItemList_ItemViewModel _selectedItem;

		private ObservableCollection<ItemList_ItemViewModel> _items = new ObservableCollection<ItemList_ItemViewModel>();

		#endregion


		public ObservableCollection<ItemList_ItemViewModel> AllItems { get; set; } = new ObservableCollection<ItemList_ItemViewModel>();

		public ObservableCollection<ItemList_ItemViewModel> Items { get => _items; set { _items = value; Notify(nameof(Items)); } }

		public ItemList_ItemViewModel SelectedItem { get => _selectedItem; set { _selectedItem = value; Notify(nameof(SelectedItem)); } }

		public bool Aborted { get; set; } = false;

		public ICommand SelectedCommand { get => _selectedCommand; set { _selectedCommand = value; Notify(nameof(SelectedCommand)); } }

		public ICommand AbortedCommand { get => _abortCommand; set { _abortCommand = value; Notify(nameof(AbortedCommand)); } }

		public string ErrorOutput { get => _errorOutput; set { _errorOutput = value; Notify(nameof(ErrorOutput)); } }

		public string SearchString{ get => _searchString; set { _searchString = value; Notify(nameof(SearchString)); OnSearchStringChanged(value); } }

		public readonly ManualResetEventSlim _evnt = new ManualResetEventSlim();


		public ItemListViewModel() {
			AbortedCommand = new Command(AbortButton);
			SelectedCommand = new Command(SelectButton);
		}

		public void SelectButton() {
			if (SelectedItem == null) {
				return;
			}
			_evnt.Set();
		}

		public void AbortButton() {
			Aborted = true;
			_evnt.Set();
		}

		private void OnSearchStringChanged(string newSearch) {
			Items = AllItems.Where(w => w.Item.ItemName.ToLower().Contains(newSearch.Trim().ToLower())).ToObservable();
		}

		/// <summary>
		/// Add items to display in this ItemList
		/// </summary>
		public void AddItems(IEnumerable<Item> items) {
			Items.Clear();
			AllItems.Clear();
			foreach (Item item in items) {
				ItemList_ItemViewModel i = new ItemList_ItemViewModel(item);
				Items.Add(i);
				AllItems.Add(i);
			}
			Notify(nameof(Items));
		}


		public void AddItems(ObservableCollection<ItemList_ItemViewModel> items) {
			Items.Clear();
			AllItems.Clear();

			foreach (ItemList_ItemViewModel item in items) {
				Items.Add(item);
				AllItems.Add(item);
			}
			Notify(nameof(Items));
		}

		/// <summary>
		/// Handle user selection of an item and clicking of "Select" button, repeat on unsuccessful select
		/// </summary>
		public async Task<ItemList_ItemViewModel> SelectItemAsync() {
			while (SelectedItem == null) {
				await Task.Run(() => {
					_evnt.Wait();
				});
				if (Aborted) {
					return null;
				}
			}
			_evnt.Dispose();
			return SelectedItem;
		}
	}
}
