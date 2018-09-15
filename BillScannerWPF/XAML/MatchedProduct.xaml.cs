﻿using System;
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
		internal MatchRating quality { get; private set; }

		internal UIItem(Item item, int index, MatchRating quality) {
			InitializeComponent();
			UITEM_OriginalName_Text.Text = item.tirggerForMatch;
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
		}

		private void UITEM_ShowDetails_Click(object sender, RoutedEventArgs e) {
			MainWindow w = WPFHelper.GetCurrentMainWindow();
			w.INFO_MainName_Text.Text = asociatedItem.mainName;
			w.INFO_ORC_Text.Text = asociatedItem.ocrNames.Merge(',');
			w.INFO_CurrentValue_Text.Text = asociatedItem.currentPrice.ToString();
			w.INFO_PricesBefore_Text.Text = asociatedItem.pricesInThePast.Merge(',');
			w.INFO_AmountBought_Text.Text = asociatedItem.totalPurchased.ToString();
			w.MAIN_ItemInfoOverlay_Grid.Visibility = Visibility.Visible;
			w.INFO_RegisterItem_Button.IsEnabled = !asociatedItem.isRegistered;
			w.INFO_MainName_Text.IsEnabled = !asociatedItem.isRegistered;
			w.currentItemBeingInspected = this;
		}
	}
}
