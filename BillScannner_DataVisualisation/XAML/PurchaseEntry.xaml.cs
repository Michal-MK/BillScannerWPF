﻿using BillScannerCore;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BillScannner_DataVisualisation {
	/// <summary>
	/// Code for PurchaseEntry.xaml
	/// </summary>
	public partial class PurchaseEntry : UserControl {
		public PurchaseEntry() {
			InitializeComponent();
		}

		/// <summary>
		/// Initialize all controls in the UserControl with data
		/// </summary>
		/// <param name="purchaseDate">The <see cref="DateTime"/> at which the latest purchase was made</param>
		/// <param name="purchaseLocation">The location where the purchase was made</param>
		/// <param name="itemsPurchased">The amount of items purchased</param>
		/// <param name="purchaseCost">Total cost</param>
		/// <param name="shopPurchaseEntries">How many entries are recorded under this <see cref="Shop"/></param>
		public PurchaseEntry(DateTime purchaseDate, string purchaseLocation,
							 int itemsPurchased, decimal purchaseCost, int shopPurchaseEntries) {
			InitializeComponent();

			PURCHASE_DatabaseEntries_Text.Text = string.Format(PURCHASE_DatabaseEntries_Text.Text, shopPurchaseEntries);
			PURCHASE_LatestPurchase_Text.Text = string.Format(PURCHASE_LatestPurchase_Text.Text, purchaseDate.ToString("hh:mm:ss dd:MM:yyyy"));
			PURCHASE_DETAIL_ItemsBought_Text.Text = string.Format(PURCHASE_DETAIL_ItemsBought_Text.Text, itemsPurchased);
			PURCHASE_DETAIL_Location_Text.Text = string.Format(PURCHASE_DETAIL_Location_Text.Text, purchaseLocation);
			PURCHASE_DETAIL_TotalCost_Text.Text = string.Format(PURCHASE_DETAIL_TotalCost_Text.Text, purchaseCost);
		}

		/// <summary>
		/// Set the shop logo according to passed in <see cref="Shop"/>
		/// </summary>
		public void SetShopLogo(Shop shop) {
			PURCHASE_ShopLogo_Image.Source = new BitmapImage(new Uri(VisualisationHelper.shopLogosPath + shop.ToString()));
		}
	}
}
