namespace BillScannerWPF {

	public struct UIItemCreationInfo {
		internal UIItemCreationInfo(Item i, bool isRegistered, long quantity, int index, decimal currentPrice, MatchRating quality) {
			item = i;
			this.index = index;
			this.quality = quality;
			this.isRegistered = isRegistered;
			this.quantity = quantity;
			this.currentPrice = currentPrice;
		}

		internal Item item { get; }
		internal int index { get; }
		internal long quantity { get; }
		internal MatchRating quality { get; }
		internal bool isRegistered { get; private set; }
		internal decimal currentPrice { get; }
	}
}
