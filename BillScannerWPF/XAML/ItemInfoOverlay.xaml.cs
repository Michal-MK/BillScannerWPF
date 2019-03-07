using System.Windows;
using System.Windows.Controls;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ItemInfoOverlay.xaml
	/// </summary>
	public partial class ItemInfoOverlay : UserControl {
	
		/// <summary>
		/// Parent element of this control
		/// </summary>
		private Grid holder;

		/// <summary>
		/// Construct a new Item info from a small preview <see cref="UIItem"/>
		/// </summary>
		/// <param name="source"></param>
		public ItemInfoOverlay(UIItem source) {
			InitializeComponent();
			holder = ((MainWindow)App.Current.MainWindow).MAIN_Grid;

			holder.Children.Add(this);
			INFO_MainName_Text.Text = source.Item.ItemName;
			INFO_ORC_Text.Text = string.Join(",", source.Item.OcrNames);
			INFO_CurrentValue_Text.Text = source.Item.CurrentPriceDecimal.ToString();
			INFO_PricesBefore_Text.Text = string.Join(",", source.Item.PriceHistory.Values);
			INFO_AmountBought_Text.Text = source.Item.TotalPurchased.ToString();
			INFO_PURCHASE_ItemQuantity.Text = source.AmountPurchased.ToString();
		}

		private void INFO_Back_Click(object sender, RoutedEventArgs e) {
			holder.Children.Remove(this);
		}
	}
}
