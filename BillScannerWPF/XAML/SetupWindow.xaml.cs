using System;
using System.Windows;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for SetupWindow.xaml
	/// </summary>
	public partial class SetupWindow : Window {

		public SetupWindow() {
			InitializeComponent();
			Shop[] shops = (Shop[])Enum.GetValues(typeof(Shop));
			for (int i = 0; i < shops.Length; i++) {
				SETUP_SelectShop_Dropdown.Items.Add(shops[i]);
			}
			SETUP_SelectShop_Dropdown.SelectedItem = Shop.NotSelected;
			SETUP_SwitchScene_Button.Click += SETUP_SwitchScene_Click;
		}
		//Setup
		private void SETUP_SwitchScene_Click(object sender, RoutedEventArgs e) {
			Application.Current.MainWindow = new MainWindow((Shop)SETUP_SelectShop_Dropdown.SelectedItem);

		}
	}
}
