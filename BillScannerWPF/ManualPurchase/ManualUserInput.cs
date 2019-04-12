using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class ManualUserInput : IManualUserInput {

		private readonly MainWindow _mainWindow;

		public ManualUserInput(MainWindow window) {
			_mainWindow = window;
		}

		public async Task<(Choices choice, int value)> GetIntInputAsync(string displayText, params Choices[] choices) {
			ManualResolveChoice choice = new ManualResolveChoice(displayText, choices);
			Choices selected = await choice.SelectChoiceAsync();
			int result;
			while (!int.TryParse(choice.MANUAL_RESOLUTION_Solution5_Box.Text, out result)) {
				await choice.SelectChoiceAsync();
			}
			return (selected, result);
		}

		public async Task<(Choices choice, string value)> GetStringInput(string displayText, params Choices[] choices) {
			ManualResolveChoice choice = new ManualResolveChoice(displayText, choices);
			Choices selected = await choice.SelectChoiceAsync();
			return (selected, choice.MANUAL_RESOLUTION_Solution5_Box.Text);
		}

		public async Task<Choices> SelectChoiceAsync(string displayText, params Choices[] choices) {
			ManualResolveChoice choice = new ManualResolveChoice(displayText, choices);
			Choices selected = await choice.SelectChoiceAsync();
			return selected;
		}

		public async Task<(Choices choice, DateTime value)> GetDateTimeInputAsync(string displayText, params Choices[] choices) {
			ManualResolveChoice choice = new ManualResolveChoice(displayText, choices);
			AGAIN:
			Choices selected = await choice.SelectChoiceAsync();
			if (selected == Choices.ManuallyEnterDate) {
				if (DateTime.TryParseExact(choice.MANUAL_RESOLUTION_Solution5_DateBox.DATEBOX_Input_Box.Text, "dd:MM:yyyy HH:mm:ss", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time)) {
					return (selected, time);
				}
				else if (DateTime.TryParseExact(choice.MANUAL_RESOLUTION_Solution5_DateBox.DATEBOX_Input_Box.Text, "dd:MM:yyyy", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time1)) {
					return (selected, time1);
				}
				else if (DateTime.TryParseExact(choice.MANUAL_RESOLUTION_Solution5_Box.Text, "HH:mm:ss", CultureInfo.GetCultureInfo("cs"), DateTimeStyles.AllowWhiteSpaces, out DateTime time2)) {
					return (selected, new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, time2.Hour, time2.Minute, time2.Second));
				}
				goto AGAIN;
			}
			else {
				return (Choices.UseCurrentTime, DateTime.Now);
			}
		}

		public async Task<(Choices choice, int value)> GetDecimalInputAsIntAsync(string displayText, params Choices[] choices) {
			ManualResolveChoice choice = new ManualResolveChoice(displayText, choices);
			Choices selected = await choice.SelectChoiceAsync();
			if (decimal.TryParse(choice.MANUAL_RESOLUTION_Solution5_Box.Text, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result)) {
				return (selected, (int)result * 100);
			}
			else {
				return await GetDecimalInputAsIntAsync(displayText, choices);
			}
		}

		public async Task<Item> SelectOneItemFromListAsync(Item[] items) {
			ItemList list = new ItemList();
			(list.DataContext as ItemListViewModel).AddItems(items);
			_mainWindow.MAIN_Grid.Children.Add(list);
			List<ItemList_ItemViewModel> selected = await (list.DataContext as ItemListViewModel).SelectItemAsync();
			_mainWindow.MAIN_Grid.Children.Remove(list);
			if (selected == null) {
				return null;
			}
			return selected[0].Item;
		}

		public async Task<(string itemName, int itemPrice, MeassurementUnit itemUnitOfMeassure)> DefineNewItemAsync() {
			NewItemDefinitionPanel panel = new NewItemDefinitionPanel();
			return await panel.RegisterItemAsync();
		}
	}
}