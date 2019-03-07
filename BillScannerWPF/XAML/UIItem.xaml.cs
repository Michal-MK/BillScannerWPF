using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for MatchedProduct.xaml
	/// </summary>
	public partial class UIItem : UserControl {

		/// <summary>
		/// An item connected to this UI representation
		/// </summary>
		internal Item Item { get; }

		/// <summary>
		/// The amount of items purchased
		/// </summary>
		internal int AmountPurchased { get; }

		/// <summary>
		/// Internal rating of how successful this match was
		/// </summary>
		internal MatchRating MatchQuality { get; private set; }

		/// <summary>
		/// Create new <see cref="UIItem"/>
		/// </summary>
		/// <param name="itemCreation">The associated item</param>
		/// <param name="quantityPurchased">The amount bought</param>
		/// <param name="quality">Match quality</param>
		internal UIItem(UIItemCreationInfo itemCreation) {
			InitializeComponent();
			if (itemCreation.MatchQuality == MatchRating.Success) {
				UITEM_OriginalName_Text.Text = itemCreation.Item.ItemName + " | Price: " + string.Format("{0:f2}", itemCreation.CurrentPrice) + "Kč";
			}
			else {
				UITEM_OriginalName_Text.Text = string.IsNullOrEmpty(itemCreation.MatchTriggerLine) ? itemCreation.Item.ItemName : itemCreation.MatchTriggerLine + " | Price: " + string.Format("{0:f2}", itemCreation.CurrentPrice) + "Kč";
			}
			Item = itemCreation.Item;
			MatchQuality = itemCreation.MatchQuality;
			AmountPurchased = itemCreation.Amount;
			SetMatchRatingImage();
		}

		/// <summary>
		/// Update the match rating image
		/// </summary>
		internal void SetMatchRatingImage() {
			UITEM_MatchQuality_Image.Source = new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + MatchQuality.ToString() + ".png", UriKind.Absolute));
		}

		/// <summary>
		/// Set visuals for a successful match
		/// </summary>
		internal void ProductMatchedSuccess() {
			MatchQuality = MatchRating.Success;
			UITEM_MatchQuality_Image.Source = new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + MatchRating.Success.ToString() + ".png", UriKind.Absolute));
			UITEM_OriginalName_Text.Text = Item.ItemName + " | Price: " + Item.CurrentPriceDecimal.ToString("{0:f2}");
		}

		private void UITEM_ShowDetails_Click(object sender, RoutedEventArgs e) {
			ItemInfoOverlay overlay = new ItemInfoOverlay(this);
		}

		public static implicit operator ItemPurchaseData(UIItem item) {
			return new ItemPurchaseData(item.Item, item.AmountPurchased);
		}
	}
}
