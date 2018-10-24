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
		internal long quantityPurchased { get; }
		internal MatchRating quality { get; private set; }

		internal UIItem(Item item, long quantityPurchased, MatchRating quality) {
			InitializeComponent();
			if (quality == MatchRating.Success) {
				UITEM_OriginalName_Text.Text = item.userFriendlyName + " | Price: " + string.Format("{0:f2}",item.currentPrice) + "Kč";
			}
			else {
				UITEM_OriginalName_Text.Text = item.tirggerForMatch + " | Price: " + string.Format("{0:f2}", item.currentPrice) + "Kč";
			}
			asociatedItem = item;
			this.quality = quality;
			this.quantityPurchased = quantityPurchased;
			SetMatchRatingImage();
		}

		internal void SetMatchRatingImage() {
			this.UITEM_MatchQuality_Iamge.Source = new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + quality.ToString() + ".png", UriKind.Absolute));
		}

		internal void ProductMatchedSuccess() {
			quality = MatchRating.Success;
			this.UITEM_MatchQuality_Iamge.Source = new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + MatchRating.Success.ToString() + ".png", UriKind.Absolute));
			this.UITEM_OriginalName_Text.Text = asociatedItem.userFriendlyName + " | Price: " + asociatedItem.currentPrice.ToString();
		}

		private void UITEM_ShowDetails_Click(object sender, RoutedEventArgs e) {
			ItemInfoOverlay overlay = new ItemInfoOverlay(this);
		}
	}
}
