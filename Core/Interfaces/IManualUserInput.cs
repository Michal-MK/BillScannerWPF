using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public interface IManualUserInput {

		Task PressOneOf(string displayText, params (string text, ICommand choices)[] data);

		Task<int> GetIntInputAsync(string displayText);

		Task<string> GetStringInput(string displayText);

		Task<DateTime> GetDateTimeInputAsync(string displayText, bool allowNow = true);

		Task<int?> GetDecimalInputAsIntAsync(string displayText, int? knownValue = null);

		Task<Item> SelectOneItemFromListAsync(Item[] items);

		Task<NewItemDefViewModel> DefineNewItemAsync();
	}
}
