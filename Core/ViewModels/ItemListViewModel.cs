using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class ItemListViewModel : BaseViewModel {

		public ObservableCollection<ItemList_ItemViewModel> Items { get; set; }

		public List<ItemList_ItemViewModel> SelectedItems { get; set; }

		public ItemList_ItemViewModel SelectedItem => SingleItemSelection ? SelectedItems[0] :
													  throw new System.InvalidOperationException("Calling SelectItem on list with that can select multiple.");

		public bool SingleItemSelection { get; set; } = true;

		public bool Aborted { get; set; } = false;

		public ICommand SelectedCommand { get; set; }

		public ICommand AbortCommand { get; set; }

		public ICommand SelectButtonCommand { get; set; }


		public readonly ManualResetEventSlim _evnt = new ManualResetEventSlim();


		public ItemListViewModel() {
			Items = new ObservableCollection<ItemList_ItemViewModel>();

			AbortCommand = new Command(Abort);
			SelectedCommand = new Command(SelectButton);
		}

		public void Abort() {
			Aborted = true;
			_evnt.Set();
		}

		public void Selected(object changed) {
			ItemList_ItemViewModel model = (ItemList_ItemViewModel)changed;
			if (SingleItemSelection) {
				SelectedItems.Clear();
				SelectedItems.Add(model);
			}
			else {
				if (SelectedItems.Contains(model)) {
					SelectedItems.Remove(model);
				}
				else {
					SelectedItems.Add(model);
				}
			}
		}

		public void SelectButton() {
			if (SelectedItems.Count == 0) {
				return;
			}
			_evnt.Set();
		}


		/// <summary>
		/// Add items to display in this ItemList
		/// </summary>
		public void AddItems(IEnumerable<Item> items) {
			Items = new ObservableCollection<ItemList_ItemViewModel>();
			foreach (Item item in items) {
				ItemList_ItemViewModel i = new ItemList_ItemViewModel();
				Items.Add(i);
			}
		}


		/// <summary>
		/// Handle user selection of an item and clicking of "Select" button, repeat on unsuccessful select
		/// </summary>
		public async Task<List<ItemList_ItemViewModel>> SelectItemAsync() {
			while (SelectedItems.Count == 0) {
				await Task.Run(() => {
					_evnt.Wait();
				});
				if (Aborted) {
					return null;
				}
			}
			return SelectedItems;
		}
	}
}
