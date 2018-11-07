using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	class McDonaldsRuleset : BaseRuleset, IRuleset {
		public string[] startMarkers { get { return new string[] { "qty", "item", "total", "bezny", "rezim" }; } }

		public string[] endMarkers { get { return new string[] { "vat", "number", "tax", "bkp", "****" }; } }

		public char costPlusQuantitySeparator { get { return '\0'; } }

		public Regex correctItemLine { get; } = new Regex(@"(\d+) (.+) (\d+\.\d+) B");

		public Regex dateTimeFormat { get { return genericDateTimeFormat; } }

		public long GetQuantity(string[] ocrText, int index) {
			string[] split = ocrText[index].Split(null);
			if (long.TryParse(split[0], out long result)) {
				return result;
			}
			string modified = ReplaceAmbiguousToNumber(split[0]);
			if (modified == split[0]) {
				throw new QuantityParsingException("Unable to get quantity from string " + ocrText[index], ocrText[index], index);
			}
			else {
				if (long.TryParse(modified, out long resultModified)) {
					return result;
				}
			}
			throw new QuantityParsingException("Unable to get quantity from string " + ocrText[index] + ", subsequently modified " + modified, ocrText[index], index);
		}

		public decimal GetPriceOfOne(string[] ocrText, ref int index) {
			string line = ocrText[index].Replace(',', '.');
			Match m = correctItemLine.Match(line);
			if (decimal.TryParse(m.Groups[3].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
				return result;
			}
			string modified = ReplaceAmbiguousToNumber(line);
			if (modified == line) {
				throw new PriceParsingException(ocrText[index], index, false);
			}
			else {
				Match mm = correctItemLine.Match(modified);
				if (decimal.TryParse(mm.Groups[3].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal resultModified)) {
					return result;
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
