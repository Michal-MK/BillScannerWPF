using System;
using System.Windows;
using BillScannerCore;

namespace BillScannerWPF
{
	class ManualPurchaseHandler {

		private readonly Shop _shop;
		private readonly MainWindow _mainWindow;

		public ManualPurchaseHandler(Shop selectedShop, MainWindow mainWindow) {
			_shop = selectedShop;
			_mainWindow = mainWindow;
		}

		internal void Begin(object sender, RoutedEventArgs e) {
			throw new NotImplementedException();
		}
	}
}
