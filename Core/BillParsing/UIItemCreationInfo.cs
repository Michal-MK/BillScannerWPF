namespace Igor.BillScanner.Core {

	/// <summary>
	/// Structure holding necessary data to construct a full <see cref="UIItem"/> visual element
	/// </summary>
	public struct UIItemCreationInfo {

		/// <summary>
		/// Create new <see cref="UIItemCreationInfo"/>
		/// </summary>
		internal UIItemCreationInfo(Item item, int quantity, int currentPrice, MatchRating quality, string triggerForMatch) {
			Item = item;
			MatchQuality = quality;
			Amount = quantity;
			_currentPrice = currentPrice;
			MatchTriggerLine = triggerForMatch;
		}

		/// <summary>
		/// Reference to the <see cref="Item"/>
		/// </summary>
		public Item Item { get; }

		/// <summary>
		/// Amount of items to be purchased
		/// </summary>
		public int Amount { get; }

		/// <summary>
		/// Match accuracy
		/// </summary>
		public MatchRating MatchQuality { get; }

		/// <summary>
		/// Current price of the <see cref="Item"/> human readable
		/// </summary>
		public decimal CurrentPriceStdBase => _currentPrice / 100m;
		
		/// <summary>
		/// Current price of the <see cref="Item"/> represented in the lowest nominal value
		/// </summary>
		public int CurrentPriceDB => _currentPrice;
		private int _currentPrice;


		/// <summary>
		/// The text that triggered the match
		/// </summary>
		public string MatchTriggerLine { get; }
	}
}
