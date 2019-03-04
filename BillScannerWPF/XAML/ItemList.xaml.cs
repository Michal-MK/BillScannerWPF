using BillScannerCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BillScannerWPF {

	/// <summary>
	/// Code for ItemList.xaml
	/// </summary>
	public partial class ItemList : UserControl {

		private ObservableCollection<ItemList_Item> Items { get; set; }

		private readonly ManualResetEventSlim evnt = new ManualResetEventSlim();
		private bool _wasAborted = false;

		/// <summary>
		/// Create a list of items for the UI from a normal <see cref="Item"/> class
		/// </summary>
		public ItemList() {
			InitializeComponent();
			DataContext = this;
			ITEMLIST_Select_Button.Click += ITEMLIST_Select_Click;
			ITEMLIST_Back_Button.Click += ITEMLIST_Back_Click;
		}

		/// <summary>
		/// Add items to display in this ItemList
		/// </summary>
		public void AddItems(IEnumerable<Item> items) {
			Items = new ObservableCollection<ItemList_Item>();
			foreach (Item item in items) {
				ItemList_Item i = new ItemList_Item(this, item);
				Items.Add(i);
			}
		}


		private void ITEMLIST_Back_Click(object sender, RoutedEventArgs e) {
			_wasAborted = true;
			evnt.Set();
		}

		private void ITEMLIST_Select_Click(object sender, RoutedEventArgs e) {
			if ((ItemList_Item)ITEMLIST_Items_ListBox.SelectedItem == null) {
				ITEMLIST_ErrorInfo_Text.Text = "No Item selected!";
				return;
			}
			evnt.Set();
		}

		/// <summary>
		/// Handle user selection of an item and clicking of "Select" button, repeat on unsuccessful select
		/// </summary>
		internal async Task<Item> SelectItemAsync() {
			while (((ItemList_Item)ITEMLIST_Items_ListBox.SelectedItem) == null) {
				await Task.Run(() => {
					evnt.Wait();
				});
				if (_wasAborted) {
					((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Remove(this);
					return null;
				}
			}
			return ((ItemList_Item)ITEMLIST_Items_ListBox.SelectedItem).asociatedItem;
		}
	}
}
