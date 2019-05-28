using System.Text.RegularExpressions;

namespace Igor.BillScanner.Core.Rules {
	/// <summary>
	/// Interface for abstracting all shops
	/// </summary>
	public interface IRuleset {

		/// <summary>
		/// Returns the quantity of items purchased
		/// </summary>
		/// <exception cref="QuantityParsingException"></exception>
		/// <param name="ocrText">The OCR'd text array</param>
		/// <param name="index">The index into the array</param>
		int GetQuantity(string[] ocrText, int index);

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
		int GetPriceOfOne(string[] ocrText, ref int index);

		/// <summary>
		/// Base Regular expression that can match a valid item from this shop
		/// </summary>
		Regex correctItemLine { get; }

		/// <summary>
		/// Base Regular expression that can parse a valid date/purchase time from the bill
		/// </summary>
		Regex dateTimeFormat { get; }

		/// <summary>
		/// Regular expression to recognize item price and quantity line
		/// </summary>
		Regex correctCostAndQuantityLine { get; }

		/// <summary>
		/// The strings that mark the start of a items listing
		/// </summary>
		string[] StartMarkers { get; }

		/// <summary>
		/// The strings that mark the end of a items listing
		/// </summary>
		string[] EndMarkers { get; }

		/// <summary>
		/// A character that is used to separate items purchased and the price of one item
		/// </summary>
		char CostPlusQuantitySeparator { get; }

		/// <summary>
		/// The shop this rule set matches
		/// </summary>
		Shop Shop { get; }
	}
}
