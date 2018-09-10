using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for MatchedProduct.xaml
	/// </summary>
	public partial class UIItem : UserControl {

		internal Item asociatedItem { get; }
		internal int index { get; }
		internal MatchRating quality { get; }

		internal UIItem(Item item, int index, MatchRating quality) {
			InitializeComponent();
			IH_originalName.Text = item.tirggerForMatch;
			asociatedItem = item;
			this.quality = quality;
		}

		internal void SetMatchRatingImage() {
			this.IH_matchQuality.Source = new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + quality.ToString() + ".png", UriKind.Absolute));
		}

		private void IH_ShowItemDetails_Button(object sender, RoutedEventArgs e) {
			MainWindow w = WPFHelper.GetCurrentMainWindow();
			w.info_mainName.Text = "Main name: " + asociatedItem.mainName;
			w.info_ocrNames.Text = "OCR registered names: " + asociatedItem.ocrNames.Merge(',');
			w.info_currentValue.Text = "Current item value: " + asociatedItem.currentPrice.ToString();
			w.info_pastPrices.Text = "Previous values: " + asociatedItem.pricesInThePast.Merge(',');
			w.info_bought.Text = "Total items bought: " + asociatedItem.totalPurchased.ToString();
			//w.itemInfoOverlay.Visibility = Visibility.Visible;
			w.currentItemBeingInspected = this;
		}
	}
}
