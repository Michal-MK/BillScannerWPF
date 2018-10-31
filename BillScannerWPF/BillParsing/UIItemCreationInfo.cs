using BillScannerCore;

namespace BillScannerWPF {

	/// <summary>
	/// Structure holding necessary data to construct a full <see cref="UIItem"/> visual element
	/// </summary>
	public struct UIItemCreationInfo {

		/// <summary>
		/// Create new <see cref="UIItemCreationInfo"/>
		/// </summary>
		internal UIItemCreationInfo(Item i, bool isRegistered, long quantity, decimal currentPrice, MatchRating quality) {
			item = i;
			this.quality = quality;
			this.isRegistered = isRegistered;
			this.quantity = quantity;
			this.currentPrice = currentPrice;
		}

		/// <summary>
		/// Reference to the <see cref="Item"/>
		/// </summary>
		internal Item item { get; }

		/// <summary>
		/// Amount of items to be purchased
		/// </summary>
		internal long quantity { get; }

		/// <summary>
		/// Match accuracy
		/// </summary>
		internal MatchRating quality { get; }

		/// <summary>
		/// Is the <see cref="Item"/> already defined in <see cref="Database.itemDatabase"/>
		/// </summary>
		internal bool isRegistered { get; private set; }

		/// <summary>
		/// Current price of the <see cref="Item"/>
		/// </summary>
		internal decimal currentPrice { get; }
	}
}
