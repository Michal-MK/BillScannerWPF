using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ItemList.xaml
	/// </summary>
	public partial class ItemList : UserControl {

		public ItemListViewModel Model { get; set; }

		/// <summary>
		/// Create a list of items for the UI from a normal <see cref="Item"/> class
		/// </summary>
		public ItemList() {
			InitializeComponent();
			DataContext = Model = new ItemListViewModel();			
		}

		private void ITEMLIST_Items_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			Model.Selected(((ListBox)sender).SelectedItem);
		}
	}
}
