using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	/// <summary>
	/// Interface for abstracting all shops
	/// </summary>
	interface IRuleset {

		/// <summary>
		/// Returns the quantity of items purchased
		/// </summary>
		/// <exception cref="QuantityParsingException"></exception>
		/// <param name="ocrText">The OCR'd text array</param>
		/// <param name="index">The index into the array</param>
		long GetQuantity(string[] ocrText, int index);

		/// <summary>
		/// Returns the name of an Item from a line
		/// </summary>
		/// <exception cref="NameParsingException"></exception>
		/// <param name="line">The line to parse the name from</param>
		string GetName(string line);

		/// <summary>
		/// Returns the price of one item (in case of multiple purchases of one)
		/// </summary>
		/// <param name="ocrText">The OCR'd text array</param>
		/// <param name="index">The index into the array, INDEX CAN BY MODIFIED BY THIS!</param>
		decimal GetPriceOfOne(string[] ocrText, ref int index);

		/// <summary>
		/// Base Regular expression that can match a valid item from this shop
		/// </summary>
		Regex correctItemLine { get; }

		/// <summary>
		/// Base Regular expression that can parse a valid date/purchase time from the bill
		/// </summary>
		Regex dateTimeFormat { get; }

		/// <summary>
		/// The strings that mark the start of a items listing
		/// </summary>
		string[] startMarkers { get; }

		/// <summary>
		/// The strings that mark the end of a items listing
		/// </summary>
		string[] endMarkers { get; }

		/// <summary>
		/// A character that is used to separate items purchased and the price of one item
		/// </summary>
		char costPlusQuantitySeparator { get; }

		/// <summary>
		/// Indicates whether the start marker should be skipped or parsed as the first item
		/// </summary>
		bool skipInitiatingString { get; }

		/// <summary>
		/// NUmber of lines an item takes up
		/// </summary>
		int itemLineSpan { get; }
	}
}
