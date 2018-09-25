using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for MatchedProduct.xaml
	/// </summary>
	public partial class UIItem : UserControl {

		internal Item asociatedItem { get; }
		internal int index { get; }
		internal MatchRating quality { get; private set; }

		internal UIItem(Item item, int index, MatchRating quality) {
			InitializeComponent();
			UITEM_OriginalName_Text.Text = item.tirggerForMatch + " | Price: " + item.currentPrice.ToString();
			asociatedItem = item;
			this.quality = quality;
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
