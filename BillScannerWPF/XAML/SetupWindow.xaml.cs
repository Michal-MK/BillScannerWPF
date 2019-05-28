using System;
using System.Windows;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

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

			SETUP_SelectShop_Dropdown.SelectedItem = Shop.Lidl;
			SETUP_SwitchScene_Button.Click += SETUP_SwitchScene_Click;

			SETUP_SelectShop_Dropdown.Focus();
		}


		private void SETUP_SwitchScene_Click(object sender, RoutedEventArgs e) {
			MainWindow m = new MainWindow((Shop)SETUP_SelectShop_Dropdown.SelectedItem);
			Close();
			Application.Current.MainWindow = m;
			Application.Current.MainWindow.Show();
		}
	}
}
