using System;
using System.Globalization;
using System.Windows;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class ManualPurchaseHandler {

		public Shop Shop { get; }
		private readonly MainWindow _mainWindow;

		public ManualPurchaseHandler(Shop selectedShop, MainWindow mainWindow) {
			Shop = selectedShop;
			_mainWindow = mainWindow;
		}

		internal async void Begin(object sender, RoutedEventArgs e) {
			ManualPurchaseView view = new ManualPurchaseView(this);
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
