﻿using System;
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

		public static Shop selectedShop { get; private set; }

		public SetupWindow() {
			InitializeComponent();
			Shop[] shops = (Shop[])Enum.GetValues(typeof(Shop));
			for (int i = 0; i < shops.Length; i++) {
				s_shopSelect.Items.Add(shops[i]);
			}
		}
		private DatabaseAccess access;
		//Setup
		private void Button_Click(object sender, RoutedEventArgs e) {
			access = DatabaseAccess.LoadDatabase(Shop.Lidl);
		}

		//Define new item
		private void Button_Click_1(object sender, RoutedEventArgs e) {
			access.WriteItemDefinitionToDatabase(s_itemName.Text, s_final.IsChecked.Value);
		}

		// Update json view
		private void Button_Click_2(object sender, RoutedEventArgs e) {
			json.Text = File.ReadAllText(WPFHelper.dataPath + "itemsdb.json");
		}

		private void itemName_MouseDown(object sender, MouseButtonEventArgs e) {
			s_itemName.Text = "";
		}

		//Alt name
		private void Button_Click_3(object sender, RoutedEventArgs e) {
			string orig = s_origName.Text;
			string alt = s_altName.Text;
			access.WriteAlternativeOCRNameForItemToDatabase(orig, alt);
		}

		private void Button_Click_4(object sender, RoutedEventArgs e) {
			int bamount = int.Parse(s_amount.Text);
			string priceee = s_price.Text.Replace(',', '.');
			decimal pricee = decimal.Parse(priceee, CultureInfo.InvariantCulture);
			access.WriteNewPurchaseForItemToDatabase(s_origName.Text, new PurchaseHistory(DateTime.Now, bamount, pricee));
		}

		private void Button_Click_5(object sender, RoutedEventArgs e) {
			List<SimpleItem> items = new List<SimpleItem>();
			items.Add(new SimpleItem("Whatever", 20, 59.86m));
			items.Add(new SimpleItem("AAA", 2, 111.99m));
			access.WriteNewShoppingInstance(new Shopping(DateTime.Now, items));
		}

		private void Button_Click_6(object sender, RoutedEventArgs e) {
			MainWindow main = new MainWindow();
			Application.Current.MainWindow = main;
			selectedShop = (Shop)s_shopSelect.SelectedItem;
			main.Show();
			Close();
		}
	}
}
