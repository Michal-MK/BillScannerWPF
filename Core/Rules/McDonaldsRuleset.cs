using System.Globalization;
using System.Text.RegularExpressions;

namespace Igor.BillScanner.Core.Rules {
	class McDonaldsRuleset : BaseRuleset, IRuleset {
		public string[] StartMarkers { get { return new string[] { "qty", "item", "total", "bezny", "rezim" }; } }

		public string[] EndMarkers { get { return new string[] { "vat", "number", "tax", "bkp", "****" }; } }

		public char CostPlusQuantitySeparator { get { return '\0'; } }

		public Regex correctItemLine { get; } = new Regex(@"(\d+) (.+) (\d+\.\d+) B");

		public Regex dateTimeFormat { get { return genericDateTimeFormat; } }

		public Regex correctCostAndQuantityLine => genericItemPriceFormat;

		public Shop Shop => Shop.McDonalds;

		public int GetQuantity(string[] ocrText, int index) {
			string[] split = ocrText[index].Split(null);
			if (int.TryParse(split[0], out int result)) {
				return result;
			}
			string modified = ReplaceAmbiguousToNumber(split[0]);
			if (modified == split[0]) {
				throw new QuantityParsingException("Unable to get quantity from string " + ocrText[index], ocrText[index], index);
			}
			else {
				if (int.TryParse(modified, out int resultModified)) {
					return resultModified;
				}
			}
			throw new QuantityParsingException("Unable to get quantity from string " + ocrText[index] + ", subsequently modified " + modified, ocrText[index], index);
		}

		public int GetPriceOfOne(string[] ocrText, ref int index) {
			string line = ocrText[index].Replace(',', '.');
			Match m = correctItemLine.Match(line);
			if (decimal.TryParse(m.Groups[3].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
				return (int)(result * 100);
			}
			string modified = ReplaceAmbiguousToNumber(line);
			if (modified == line) {
				throw new PriceParsingException(ocrText[index], index, false);
			}
			else {
				Match mm = correctItemLine.Match(modified);
				if (decimal.TryParse(mm.Groups[3].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal resultModified)) {
					return (int)(resultModified * 100);
				}
			}
			throw new PriceParsingException(ocrText[index], index, false);
		}


		public string GetName(string line) {
			Regex nameR = new Regex(@"(\d+) (\w+) (\d+\.\d+) B");
			Match m = nameR.Match(line);
			if (m.Groups.Count < 2) {
				return m.Groups[2].Value;
			}
			else {
				throw new NameParsingException("Unable to get name from line " + line, line);
			}
		}
	}
}
