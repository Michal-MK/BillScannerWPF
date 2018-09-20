using System;

namespace BillScannerWPF {

	[Serializable]
	class ItemNotDefinedException : Exception {
		public ItemNotDefinedException(string message) {
			
		}
	}
}