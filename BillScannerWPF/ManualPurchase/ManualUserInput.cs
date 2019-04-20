using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class ManualUserInput : IManualUserInput {

		private readonly MainWindow _mainWindow;

		public ManualUserInput(MainWindow window) {
			_mainWindow = window;
		}

		public async Task<ManualResolutionViewModel> GetCustom(ManualResolutionViewModel input) {
			ManualResolveChoice choice = new ManualResolveChoice(input);
			await choice.SelectChoiceAsync();
			return choice.DataContext as ManualResolutionViewModel;
		}

		public async Task<int> GetIntInputAsync(string displayText) {

			ManualResolutionViewModel model = new ManualResolutionViewModel() {
				Header = displayText,
				SimpleInputControlVisible = true,
				ButtonStdInput = "Use my value provided here:",
				ButtonStdInputCommand = new Command(() => { /*TODO*/ })
			};

			ManualResolveChoice choice = new ManualResolveChoice(model);

			await choice.SelectChoiceAsync();
			int result;
			while (!int.TryParse(model.CustomInputText, out result)) {
				await choice.SelectChoiceAsync();
			}
			return result;
		}

		public async Task<string> GetStringInput(string displayText) {
			ManualResolutionViewModel model = new ManualResolutionViewModel() {
				Header = displayText,
				SimpleInputControlVisible = true,
				ButtonStdInput = "GetStringInput: ",
				ButtonStdInputCommand = new Command(() => { /*TODO*/ })
			};
			ManualResolveChoice choice = new ManualResolveChoice(model);
			await choice.SelectChoiceAsync();
			return model.CustomInputText;
		}

		public async Task PressOneOf(string displayText, params (string, ICommand)[] choices) {
			ManualResolutionViewModel model = new ManualResolutionViewModel();
			if (choices.Length > 3) {
				throw new Exception("Too many Choices");
			}
			model.Header = displayText;

			int current = 1;
			foreach ((string text, ICommand command) in choices) {
				model.SetCommand(current, text, command);
			}

			ManualResolveChoice choice = new ManualResolveChoice(model);
			await choice.SelectChoiceAsync();
		}

		public async Task<DateTime> GetDateTimeInputAsync(string displayText, bool allowNow = true) {
			ManualResolutionViewModel model = new ManualResolutionViewModel {
				DateBoxControlVisible = true,
				Header = displayText,
				ButtonDateTimeCommand = new ReturnCommand<Choices>(() => Choices.ManuallyEnterDate),
				ButtonDateTimeInput = "Use my date provided here: ",
			};

			if (allowNow) {
				model.SetCommand(1, "Use 'Today' as the date of purchase", new ReturnCommand<DateTime>(() => DateTime.Now));
			}

			ManualResolveChoice choice = new ManualResolveChoice(model);
			RedoTime:
			await choice.SelectChoiceAsync();

			if ((model.ButtonDateTimeCommand as ReturnCommand<DateTime>).Result != DateTime.MinValue) {
				if (model.DateBoxControlModel.TryGetDate(out DateTime time)) {
					return time;
				}
				goto RedoTime;
			}
			else {
				return DateTime.Now;
			}
		}

		public async Task<int?> GetDecimalInputAsIntAsync(string displayText, bool allowKnown = false) {
			Choices buttonChoice = Choices.NOOP;
			ManualResolutionViewModel model = new ManualResolutionViewModel {
				SimpleInputControlVisible = true,
				Header = displayText,
				ButtonStdInput = "Use my value provided here: ",
				ButtonStdInputCommand = new Command(() => buttonChoice = Choices.ManuallyEnterPrice)
			};
			if (allowKnown) {
				model.SetCommand(1, "Use latest value from the database.", new Command(() => { buttonChoice = Choices.UseLatestValue; }));
			}

			ManualResolveChoice choice = new ManualResolveChoice(model);

			await choice.SelectChoiceAsync();

			if (buttonChoice == Choices.UseLatestValue) {
				return null;
			}

			if (decimal.TryParse(model.CustomInputText, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result)) {
				return (int)result * 100;
			}
			else {
				return await GetDecimalInputAsIntAsync(displayText);
			}
		}

		public async Task<Item> SelectOneItemFromListAsync(Item[] items) {
			ItemList list = new ItemList();
			(list.DataContext as ItemListViewModel).AddItems(items);
			_mainWindow.MAIN_Grid.Children.Add(list);
			ItemList_ItemViewModel selected = await (list.DataContext as ItemListViewModel).SelectItemAsync();
			_mainWindow.MAIN_Grid.Children.Remove(list);
			if (selected == null) {
				return null;
			}
			return selected.Item;
		}

		public async Task<(string Name, int Price, MeassurementUnit UnitOfMeassure)> DefineNewItemAsync() {
			NewItemDefinitionPanel panel = new NewItemDefinitionPanel();
			return await panel.RegisterItemAsync();
		}
	}
}