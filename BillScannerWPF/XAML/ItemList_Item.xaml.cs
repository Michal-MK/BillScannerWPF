using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ItemList_Item.xaml
	/// </summary>
	public partial class ItemList_Item : UserControl {

		/// <summary>
		/// The view model
		/// </summary>
		public ItemList_ItemViewModel Model { get; set; }

		/// <summary>
		/// Create a new visual representation of an item in a list
		/// </summary>
		public ItemList_Item() {
			InitializeComponent();
			DataContext = Model = new ItemList_ItemViewModel();
		}
	}
}
