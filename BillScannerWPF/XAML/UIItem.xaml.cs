using BillScannerCore;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BillScannerWPF {

	/// <summary>
	/// Code for MatchedProduct.xaml
	/// </summary>
	public partial class UIItem : UserControl {

		/// <summary>
		/// An item connected to this UI representation
		/// </summary>
		internal Item asociatedItem { get; }

		/// <summary>
		/// The amount of items purchased
		/// </summary>
		internal int quantityPurchased { get; }

		/// <summary>
		/// Internal rating of how successful this match was
		/// </summary>
		internal MatchRating quality { get; private set; }

		/// <summary>
		/// Is the item already registered in the database, or did we just create it
		/// </summary>
		internal bool isRegistered;

		/// <summary>
		/// Create new <see cref="UIItem"/>
		/// </summary>
		/// <param name="itemCreation">The associated item</param>
		/// <param name="quantityPurchased">The amount bought</param>
		/// <param name="quality">Match quality</param>
		internal UIItem(UIItemCreationInfo itemCreation, int quantityPurchased, MatchRating quality) {
			InitializeComponent();
			if (quality == MatchRating.Success) {
				UITEM_OriginalName_Text.Text = itemCreation.item.userFriendlyName + " | Price: " + string.Format("{0:f2}", itemCreation.currentPrice) + "Kč";
			}
			else {
				UITEM_OriginalName_Text.Text = itemCreation.tirggerForMatch ?? itemCreation.item.userFriendlyName + " | Price: " + string.Format("{0:f2}", itemCreation.currentPrice) + "Kč";
			}
			asociatedItem = itemCreation.item;
			this.quality = quality;
			this.quantityPurchased = quantityPurchased;
			SetMatchRatingImage();
		}

		/// <summary>
		/// Update the match rating image
		/// </summary>
		internal void SetMatchRatingImage() {
			this.UITEM_MatchQuality_Iamge.Source = new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + quality.ToString() + ".png", UriKind.Absolute));
		}

		/// <summary>
		/// Set visuals for a successful match
		/// </summary>
		internal void ProductMatchedSuccess() {
			quality = MatchRating.Success;
			this.UITEM_MatchQuality_Iamge.Source = new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + MatchRating.Success.ToString() + ".png", UriKind.Absolute));
			this.UITEM_OriginalName_Text.Text = asociatedItem.userFriendlyName + " | Price: " + asociatedItem.currentPrice.ToString();
		}

		private void UITEM_ShowDetails_Click(object sender, RoutedEventArgs e) {
			ItemInfoOverlay overlay = new ItemInfoOverlay(this);
		}

		public static implicit operator ItemPurchaseData(UIItem item) {
			return new ItemPurchaseData(item.asociatedItem, item.quantityPurchased);
		}
	}
}
