using BillScannerCore;

namespace BillScannerWPF {

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
		/// Reference to the <see cref="BillScannerCore.Item"/>
		/// </summary>
		internal Item Item { get; }

		/// <summary>
		/// Amount of items to be purchased
		/// </summary>
		internal int Amount{ get; }

		/// <summary>
		/// Match accuracy
		/// </summary>
		internal MatchRating MatchQuality { get; }

		/// <summary>
		/// Current price of the <see cref="BillScannerCore.Item"/>
		/// </summary>
		internal decimal CurrentPrice { get { return _currentPrice / 100m; } }
		private int _currentPrice;


		/// <summary>
		/// The text that triggered the match
		/// </summary>
		internal string MatchTriggerLine { get; }
	}
}
