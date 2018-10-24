using System;
using System.Windows;
using System.Windows.Controls;

namespace BillScannerWPF {

	/// <summary>
	/// Code for SetupWindow.xaml
	/// </summary>
	public partial class SetupWindow : Window {

		/// <summary>
		/// Create a new <see cref="SetupWindow"/>
		/// </summary>
		public SetupWindow() {
			InitializeComponent();
			Shop[] shops = (Shop[])Enum.GetValues(typeof(Shop));
			for (int i = 0; i < shops.Length; i++) {
				SETUP_SelectShop_Dropdown.Items.Add(shops[i]);
			}
			SETUP_SelectShop_Dropdown.SelectedItem = Shop.NotSelected;
			SETUP_SwitchScene_Button.Click += SETUP_SwitchScene_Click;
		}
		

		private void SETUP_SwitchScene_Click(object sender, RoutedEventArgs e) {
			((Button)sender).IsEnabled = false;
			MainWindow m = new MainWindow((Shop)SETUP_SelectShop_Dropdown.SelectedItem);
			Application.Current.MainWindow.Close();
			Application.Current.MainWindow = m;
			Application.Current.MainWindow.Show();
		}
	}
}
