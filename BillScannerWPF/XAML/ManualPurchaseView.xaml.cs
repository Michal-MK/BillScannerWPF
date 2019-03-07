using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for ManualPurchaseView.xaml
	/// </summary>
	public partial class ManualPurchaseView : UserControl, INotifyPropertyChanged {

		private Item[] itemsForSelectedShop;

		public event PropertyChangedEventHandler PropertyChanged;

		public string SearchQuery { get; set; }

		public ObservableCollection<ItemList_Item> Items { get; set; }

		private readonly ManualPurchaseHandler _manualPurchaseHandler;

		public ManualPurchaseView(ManualPurchaseHandler handler) {
			_manualPurchaseHandler = handler;
			itemsForSelectedShop = DatabaseAccess.access.GetItems(_manualPurchaseHandler.Shop);
			InitializeComponent();
			DataContext = this;
			MANUALPURCHASE_Items_List.DataContext = this;
			PopulateList();
		}

		private void MANUALPURCHASAE_RegisterNewItem_Button(object sender, RoutedEventArgs e) {
			//TODO
		}

		private void OnNewSearchQuery(object sender, TextChangedEventArgs e) {
			e.Handled = true;
			if (itemsForSelectedShop == null) {
				return;
			}
			PopulateList();
		}

		private void PopulateList() {
			Items = new ObservableCollection<ItemList_Item>();

			if (string.IsNullOrWhiteSpace(SearchQuery)) {
				foreach (Item i in itemsForSelectedShop) {
					Items.Add(new ItemList_Item(MANUALPURCHASE_Items_List, i));
				}
			}
			else {
				foreach (Item i in itemsForSelectedShop.Where((i) => { return i.ItemName.Contains(SearchQuery); })) {
					Items.Add(new ItemList_Item(MANUALPURCHASE_Items_List, i));
				}
			}
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
		}
	}
}
