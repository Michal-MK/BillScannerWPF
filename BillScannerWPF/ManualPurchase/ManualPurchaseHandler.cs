using System;
using System.Globalization;
using System.Windows;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	class ManualPurchaseHandler {

		private readonly Shop _shop;
		private readonly MainWindow _mainWindow;

		public ManualPurchaseHandler(Shop selectedShop, MainWindow mainWindow) {
			_shop = selectedShop;
			_mainWindow = mainWindow;
		}

		internal async void Begin(object sender, RoutedEventArgs e) {
			ManualPurchaseView view = new ManualPurchaseView();
			ManualResolveChoice dateChoice = new ManualResolveChoice("Enter purchase date:", Choices.ManuallyEnterDate);
			await dateChoice.SelectChoiceAsync();
			DateTime purchaseDate;
			while (!DateTime.TryParseExact(dateChoice.MANUAL_RESOLUTION_Solution5_DateBox.DATEBOX_Input_Box.Text, "dd:MM:yyyy hh:mm:ss", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out purchaseDate)) {
				await dateChoice.SelectChoiceAsync();
			}
			_mainWindow.MAIN_Grid.Children.Add(view);
		}
	}
}
