namespace Igor.BillScanner.Core {

	/// <summary>
	/// Functionality for <see cref="ManualResolveChoice"/> control
	/// </summary>
	public enum Choices {
		NOOP,
		MatchAnyway,
		MatchWithoutAddingAmbiguities,
		NotAnItem,
		UseCurrentTime,
		UseLatestValue,
		DefineNewItem,
		FindExistingItemFromList,
		ManuallyEnterQuantity = 20,
		ManuallyEnterDate = 21,
		ManuallyEnterPrice = 22,
	}
}
