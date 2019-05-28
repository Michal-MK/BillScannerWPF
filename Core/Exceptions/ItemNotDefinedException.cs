using System;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Exception that is thrown when an Item can not be found inside the internal representation of database file.
	/// </summary>
	[Serializable]
	class ItemNotDefinedException : Exception {
		public ItemNotDefinedException(string message): base(message) { }
	}
}