using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	interface IRuleset {
		long GetQuantity(string[] ocrText, int index);
		string Name(string line);
		decimal PriceOfOne(string[] ocrText, ref int index);

		Regex correctItemLine { get; }
		Regex dateTimeFormat { get; }
		string[] startMarkers { get; }
		string[] endMarkers { get; }
		char costPlusQuantitySeparator { get; }
		bool skipInitiatingString { get; }
		int itemLineSpan { get; }
	}
}
