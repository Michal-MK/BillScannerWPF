using BillScannerCore;
using System.Windows;
using System;

namespace BillScannner_DataVisualisation {
	/// <summary>
	/// Code for MainWindow.xaml (Visualisation)
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			for (int shop = 0; shop < Enum.GetNames(typeof(Shop)).Length; shop++) {
				if (VisualisationHelper.DatabaseExists((Shop)shop)) {
					DatabaseAccess dbAccess = DatabaseAccess.LoadDatabase((Shop)shop);
					Purchase[] purchases = dbAccess.GetPurchases();
					if(purchases.Length == 0) {
						continue;
					} 
					Purchase latestPurchase = purchases[purchases.Length - 1];

					MAIN_Shops_StackPanel.Children.Add(new PurchaseEntry(latestPurchase.date, "null", latestPurchase.purchasedItems.Length, latestPurchase.totalCost, purchases.Length,(Shop)shop));
				}
				
			}

			
		}
	}
}
