using System;
using System.Windows;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class ManualPurchaseHandler : IManualPurchaseHandler {

		public Shop Shop { get; }
		private readonly MainWindow _mainWindow;

		public ManualPurchaseHandler(Shop selectedShop, MainWindow mainWindow) {
			Shop = selectedShop;
			_mainWindow = mainWindow;
		}

		internal async void Begin(object sender, RoutedEventArgs e) {
			ManualPurchaseView view = new ManualPurchaseView();
			DateTime pDate = await Services.Instance.UserInput.GetDateTimeInputAsync("Enter purchase date:", true);
			Console.WriteLine(pDate);
			//_mainWindow.MAIN_Grid.Children.Add(view);
		}
	}
}
