using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class ItemListViewModel : BaseViewModel {

		#region BackingFields

		private ICommand _selectedCommand;
		private ICommand _abortCommand;
		private string _errorOutput;

		#endregion

		public ObservableCollection<ItemList_ItemViewModel> Items { get; set; } = new ObservableCollection<ItemList_ItemViewModel>();

		public List<ItemList_ItemViewModel> SelectedItems { get; set; } = new List<ItemList_ItemViewModel>();

		public ItemList_ItemViewModel SelectedItem => SingleItemSelection ? SelectedItems[0] :
													  throw new System.InvalidOperationException("Calling SelectItem on list with that can select multiple.");

		public bool SingleItemSelection { get; set; } = true;

		public bool Aborted { get; set; } = false;

		public ICommand SelectedCommand { get => _selectedCommand; set { _selectedCommand = value; Notify(nameof(SelectedCommand)); } }

		public ICommand AbortedCommand { get => _abortCommand; set { _abortCommand = value; Notify(nameof(AbortedCommand)); } }

		public string ErrorOutput { get => _errorOutput; set { _errorOutput = value; Notify(nameof(ErrorOutput)); } }

		public readonly ManualResetEventSlim _evnt = new ManualResetEventSlim();


		public ItemListViewModel() {
			AbortedCommand = new Command(AbortButton);
			SelectedCommand = new Command(SelectButton);
		}

		public void SelectButton() {
			if (SelectedItems.Count == 0) {
				return;
			}
			_evnt.Set();
		}

		public void AbortButton() {
			Aborted = true;
			_evnt.Set();
		}

		public void Selected(dynamic changed) {
			ItemList_ItemViewModel model = changed.DataContext as ItemList_ItemViewModel;
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


		/// <summary>
		/// Add items to display in this ItemList
		/// </summary>
		public void AddItems(IEnumerable<Item> items) {
			Items.Clear();
			foreach (Item item in items) {
				ItemList_ItemViewModel i = new ItemList_ItemViewModel(item);
				Items.Add(i);
			}
			Notify(nameof(Items));
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
			_evnt.Dispose();
			return SelectedItems;
		}
	}
}
