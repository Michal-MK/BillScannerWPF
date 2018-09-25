using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for ItemInfoOverlay.xaml
	/// </summary>
	public partial class ItemInfoOverlay : UserControl {
		public static UIItem currentItemBeingInspected { get; private set; }
		private MainWindow holder;

		public ItemInfoOverlay(UIItem source) {
			currentItemBeingInspected = source;
			holder = WPFHelper.GetCurrentMainWindow();

			InitializeComponent();

			holder.MAIN_Grid.Children.Add(this);

			INFO_MainName_Text.Text = source.asociatedItem.userFriendlyName;
			INFO_ORC_Text.Text = source.asociatedItem.ocrNames.Merge(',');
			INFO_CurrentValue_Text.Text = source.asociatedItem.currentPrice.ToString();
			INFO_PricesBefore_Text.Text = source.asociatedItem.pricesInThePast.Merge(',');
			INFO_AmountBought_Text.Text = source.asociatedItem.totalPurchased.ToString();
			MAIN_ItemInfoOverlay_Grid.Visibility = Visibility.Visible;
			INFO_RegisterItem_Button.IsEnabled = !source.asociatedItem.isRegistered;
			INFO_MainName_Text.IsEnabled = !source.asociatedItem.isRegistered;
		}

		private void INFO_RegisterItem_Click(object sender, RoutedEventArgs e) {
			//Get stuff from input fields
			string modifiedName = INFO_MainName_Text.Text;
			try {
				MainWindow.access.RegisterItemFromUI(currentItemBeingInspected, modifiedName);
				((Button)sender).IsEnabled = false;
				ImageProcessor.instance.uiItemsUnknown.Remove(currentItemBeingInspected);
				ImageProcessor.instance.uiItemsMatched.Add(currentItemBeingInspected);
				currentItemBeingInspected.ProductMatchedSuccess();
				Console.WriteLine("Item Parsed successfully");
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
