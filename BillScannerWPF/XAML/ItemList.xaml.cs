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

		private ObservableCollection<ItemList_Item> observedItems;
		private readonly ManualResetEventSlim evnt = new ManualResetEventSlim();
		private bool wasAborted = false;

		/// <summary>
		/// Create a list of items for the UI from a normal <see cref="Item"/> class
		/// </summary>
		/// <param name="items"></param>
		public ItemList(Item[] items) {
			InitializeComponent();

			WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(this);

			observedItems = new ObservableCollection<ItemList_Item>();
			foreach (Item item in items) {
				ItemList_Item i = new ItemList_Item(this, item);
				observedItems.Add(i);
			}

			ITEMLIST_Items_ListBox.ItemsSource = observedItems;
			ITEMLIST_Select_Button.Click += ITEMLIST_Select_Click;
			ITEMLIST_Back_Button.Click += ITEMLIST_Back_Click;
		}


		private void ITEMLIST_Back_Click(object sender, RoutedEventArgs e) {
			wasAborted = true;
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
				if (wasAborted) {
					WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(this);
					return null;
				}
			}
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(this);
			return ((ItemList_Item)ITEMLIST_Items_ListBox.SelectedItem).asociatedItem;
		}
	}
}
