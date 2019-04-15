using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ItemList.xaml
	/// </summary>
	public partial class ItemList : UserControl {

		/// <summary>
		/// Create a list of items for the UI from a normal <see cref="Item"/> class
		/// </summary>
		public ItemList() {
			InitializeComponent();
		}

		private void ItemsSelectionChanged(object sender, SelectionChangedEventArgs e) {
			(DataContext as ItemListViewModel).Selected(((ListBox)sender).SelectedItem);
		}
	}
}
