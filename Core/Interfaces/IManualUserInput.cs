using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public interface IManualUserInput {

		Task<ManualResolutionViewModel> GetCustom(ManualResolutionViewModel input);

		Task PressOneOf(string displayText, params (string text, ICommand choices)[] data);

		Task<int> GetIntInputAsync(string displayText);

		Task<string> GetStringInput(string displayText);

		Task<DateTime> GetDateTimeInputAsync(string displayText, bool allowNow = true);

		Task<int?> GetDecimalInputAsIntAsync(string displayText, bool allowKnown = false);

		Task<Item> SelectOneItemFromListAsync(Item[] items);

		Task<(string Name, int Price, MeassurementUnit UnitOfMeassure)> DefineNewItemAsync();
	}
}
