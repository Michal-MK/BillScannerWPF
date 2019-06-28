using System;
using System.Windows;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ItemList_Item.xaml
	/// </summary>
	public partial class ItemList_Item : UserControl {
		public ItemList_Item() {
			InitializeComponent();
		}

		private void OnMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if (e.ClickCount == 2) {
				(DataContext as ItemList_ItemViewModel).DoubleClickAction?.Invoke();
			}
		}
	}
}
