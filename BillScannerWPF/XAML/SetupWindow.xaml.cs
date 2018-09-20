using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for SetupWindow.xaml
	/// </summary>
	public partial class SetupWindow : Window {

		public static Shop selectedShop {
			get;
#if !DEBUG
			private
#endif
			set;
		}

		private DatabaseAccess access;


		public SetupWindow() {
			InitializeComponent();
			Shop[] shops = (Shop[])Enum.GetValues(typeof(Shop));
			for (int i = 0; i < shops.Length; i++) {
				s_shopSelect.Items.Add(shops[i]);
			}
			s_shopSelect.SelectedItem = Shop.NotSelected;
		}
		//Setup
		private void Button_Click(object sender, RoutedEventArgs e) {
			access = DatabaseAccess.LoadDatabase((Shop)s_shopSelect.SelectedItem);
		}

		// Update json view
		private void Button_Click_2(object sender, RoutedEventArgs e) {
			json.Text = File.ReadAllText(WPFHelper.dataPath + "itemsdb.json");
		}

		//Alt name
		private void Button_Click_3(object sender, RoutedEventArgs e) {
			string orig = s_origName.Text;
			string alt = s_altName.Text;
			access.AddAlternativeOCRNameForItemToDatabase(orig, alt);
		}

		private void Button_Click_4(object sender, RoutedEventArgs e) {
			int bamount = int.Parse(s_amount.Text);
			string priceee = s_price.Text.Replace(',', '.');
			decimal pricee = decimal.Parse(priceee, CultureInfo.InvariantCulture);
			access.AddNewPurchaseForItemToDatabase(s_origName.Text, new PurchaseHistory(DateTime.Now, bamount, pricee));
		}

		private void Button_Click_5(object sender, RoutedEventArgs e) {
			List<ItemSlim> items = new List<ItemSlim>();
			items.Add(new ItemSlim("Whatever", 20, 59.86m));
			items.Add(new ItemSlim("AAA", 2, 111.99m));
			access.WriteNewShoppingInstance(new Shopping(DateTime.Now, items));
		}

		private void Button_Click_6(object sender, RoutedEventArgs e) {
			selectedShop = (Shop)s_shopSelect.SelectedItem;
			try {
				MainWindow main = new MainWindow();
				Application.Current.MainWindow = main;
				main.Show();
			}
			catch (WindowInitExpection initE) {
				switch (initE.type) {
					case InitExpectionType.SHOP_NOT_SELECTED: {
						MessageBox.Show("No Shop selected, select one from the dropdown menu.", "No Shop Selected!");
						break;
					}
				}
			}
			Close();
		}
	}
}
