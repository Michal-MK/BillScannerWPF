using BillScannerCore;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BillScannerWPF {

	/// <summary>
	/// Code for ItemInfoOverlay.xaml
	/// </summary>
	public partial class ItemInfoOverlay : UserControl {

		/// <summary>
		/// Static reference to the items currently being inspected in the application
		/// </summary>
		public static UIItem currentItemBeingInspected { get; private set; }

		/// <summary>
		/// Parent element of this control
		/// </summary>
		private MainWindow holder;

		/// <summary>
		/// Construct a new Item info from a small preview <see cref="UIItem"/>
		/// </summary>
		/// <param name="source"></param>
		public ItemInfoOverlay(UIItem source) {
			currentItemBeingInspected = source;
			holder = (MainWindow)App.Current.MainWindow;

			InitializeComponent();

			holder.MAIN_Grid.Children.Add(this);

			INFO_MainName_Text.Text = source.asociatedItem.userFriendlyName;
			INFO_ORC_Text.Text = source.asociatedItem.ocrNames.Merge(',');
			INFO_CurrentValue_Text.Text = source.asociatedItem.currentPrice.ToString();
			INFO_PricesBefore_Text.Text = source.asociatedItem.pricesInThePast.Merge(',');
			INFO_AmountBought_Text.Text = source.asociatedItem.totalPurchased.ToString();
			INFO_PURCHASE_ItemQuantity.Text = source.quantityPurchased.ToString();
			MAIN_ItemInfoOverlay_Grid.Visibility = Visibility.Visible;
			INFO_RegisterItem_Button.IsEnabled = !source.asociatedItem.isRegistered;
			INFO_MainName_Text.IsEnabled = !source.asociatedItem.isRegistered;
			INFO_CurrentValue_Text.IsEnabled = !source.asociatedItem.isRegistered;
			Binding b = new Binding("Text");
			b.Source = INFO_CurrentValue_Text;
			INFO_PricesBefore_Text.SetBinding(TextBlock.TextProperty, b);
		}


		private void INFO_RegisterItem_Click(object sender, RoutedEventArgs e) {
			string modifiedName = INFO_MainName_Text.Text;
			if (!decimal.TryParse(INFO_CurrentValue_Text.Text.Trim().Replace(',','.'), NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal finalPrice)) {
				return;
			}
			try {
				DatabaseAccess.access.RegisterItemFromUI(currentItemBeingInspected, modifiedName, finalPrice);
				((Button)sender).IsEnabled = false;
				ImageProcessor.instance.uiItemsUnknown.Remove(currentItemBeingInspected);
				ImageProcessor.instance.uiItemsMatched.Add(currentItemBeingInspected);
				currentItemBeingInspected.ProductMatchedSuccess();
				Debug.WriteLine("Item Parsed successfully");
				currentItemBeingInspected.asociatedItem.isRegistered = true;
				MAIN_ItemInfoOverlay_Grid.Visibility = Visibility.Hidden;
			}
			catch (Exception ex) {
				throw new Exception(ex.Message);
			}
		}

		private void INFO_Back_Click(object sender, RoutedEventArgs e) {
			holder.MAIN_Grid.Children.Remove(this);
			currentItemBeingInspected = null;
		}
	}
}
