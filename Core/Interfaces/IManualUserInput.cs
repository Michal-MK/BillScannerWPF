using System;
using System.Threading.Tasks;

namespace Igor.BillScanner.Core {
	public interface IManualUserInput {
		Task<Choices> SelectChoiceAsync(string displayText, params Choices[] choices);

		Task<(Choices choice, int value)> GetIntInputAsync(string displayText, params Choices[] choices);

		Task<(Choices choice, string value)> GetStringInput(string displayText, params Choices[] choices);

		Task<(Choices choice, DateTime value)> GetDateTimeInputAsync(string displayText, params Choices[] choices);

		Task<(Choices choice, int value)> GetDecimalInputAsIntAsync(string displayText, params Choices[] choices);

		Task<Item> SelectOneItemFromListAsync(Item[] items);

		Task<(string itemName, int itemPrice, MeassurementUnit itemUnitOfMeassure)> DefineNewItemAsync();
	}
}
