using System;
using System.Windows;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for MatchedProduct.xaml
	/// </summary>
	public partial class UIItem : UserControl {

		public UIItemViewModel Model => DataContext as UIItemViewModel;

		public UIItem() {
			InitializeComponent();
		}

		public UIItem(UIItemViewModel model) {
			InitializeComponent();
			DataContext = model;
		}

		private void UITEM_ShowDetails_Click(object sender, RoutedEventArgs e) {
			new ItemInfoOverlay(this);
		}
	}
}
