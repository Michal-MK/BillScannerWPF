using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for ItemList.xaml
	/// </summary>
	public partial class ItemList : UserControl {

		private ObservableCollection<ItemList_Item> observedItems;
		ManualResetEventSlim evnt = new ManualResetEventSlim();

		public ItemList( Item[] items) {
			InitializeComponent();

			WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(this);

			observedItems = new ObservableCollection<ItemList_Item>();
			foreach (Item item in items) {
				ItemList_Item i = new ItemList_Item(this, item);
				i.HorizontalAlignment = HorizontalAlignment.Stretch;
				observedItems.Add(i);
			}

			ITEMLIST_Items_ListBox.ItemsSource = observedItems;
			ITEMLIST_Select_Button.Click += ITEMLIST_Select_Click;
		}

		private void ITEMLIST_Select_Click(object sender, RoutedEventArgs e) {
			if((ItemList_Item)ITEMLIST_Items_ListBox.SelectedItem == null) {
				ITEMLIST_ErrorInfo_Text.Text = "No Item selected!";
				return;
			}
			evnt.Set();
		}

		internal async Task<Item> SelectItemAsync() {
			await Task.Run(() => {
				evnt.Wait();
			});
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(this);
			return ((ItemList_Item)ITEMLIST_Items_ListBox.SelectedItem).asociatedItem;
		}
	}
}
