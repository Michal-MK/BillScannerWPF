using BillScannerCore;

namespace BillScannerWPF {

	/// <summary>
	/// Structure holding necessary data to construct a full <see cref="UIItem"/> visual element
	/// </summary>
	public struct UIItemCreationInfo {

		/// <summary>
		/// Create new <see cref="UIItemCreationInfo"/>
		/// </summary>
		internal UIItemCreationInfo(Item item, bool isRegistered, int quantity, int currentPrice, MatchRating quality, string triggerForMatch) {
			this.item = item;
			this.quality = quality;
			this.isRegistered = isRegistered;
			this.quantity = quantity;
			_currentPrice = currentPrice;
			tirggerForMatch = triggerForMatch;
		}

		/// <summary>
		/// Reference to the <see cref="Item"/>
		/// </summary>
		internal Item item { get; }

		/// <summary>
		/// Amount of items to be purchased
		/// </summary>
		internal int quantity { get; }

		/// <summary>
		/// Match accuracy
		/// </summary>
		internal MatchRating quality { get; }

		/// <summary>
		/// Is the <see cref="Item"/> already defined
		/// </summary>
		internal bool isRegistered { get; private set; }

		/// <summary>
		/// Current price of the <see cref="Item"/>
		/// </summary>
		internal decimal currentPrice { get { return _currentPrice / 100; } }
		private int _currentPrice;
		/// <summary>
		/// The text that triggered the match
		/// </summary>
		internal string tirggerForMatch { get; }

	}
}
