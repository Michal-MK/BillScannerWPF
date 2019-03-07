using System;
using System.Text.RegularExpressions;

namespace Igor.BillScanner.Core.Rules {
	class LidlRuleset : BaseRuleset, IRuleset {
		public string[] startMarkers { get { return new string[0]; } }

		public string[] endMarkers { get { return new string[] { "k platbe", "k platbě", "k p1atbe", "karta", "celkova", "castka", "zaplacena" }; } }

		public char costPlusQuantitySeparator { get { return 'x'; } }

		public Regex correctItemLine { get; } = new Regex(@"(.+)( \d+[gl%])? (\d+[,.]\d+) B");

		public Regex dateTimeFormat { get { return genericDateTimeFormat; } }

		public Shop shop => Shop.Lidl;

		public Regex correctCostAndQuantityLine => genericItemPriceFormat;

		public int GetQuantity(string[] ocrText, int index) {
			if (index > ocrText.Length) { throw new IndexOutOfRangeException(nameof(index)); }
			string quantity = ocrText[index + 1];
			string[] split = quantity.Replace(" ", "").ToLower().Split(costPlusQuantitySeparator);
			if (split.Length != 2) {
				throw new QuantityParsingException($"Unable to get quantity from string {ocrText[index + 1]}", ocrText[index + 2], index + 1);
			}

			if (int.TryParse(split[0], out int result)) {
				return result;
			}
			else {
				quantity = ReplaceAmbiguousToNumber(quantity);
				if (int.TryParse(quantity.Split(costPlusQuantitySeparator)[0], out int resultReplaced)) {
					return resultReplaced;
				}
			}
			throw new QuantityParsingException($"Unable to get quantity from string {ocrText[index + 1]}, subsequently modified {quantity}", ocrText[index + 1], index + 1);
		}

		public string GetName(string line) {
			Match m = correctItemLine.Match(line);
			if (m.Success) {
				string name = m.Groups[1].Value + m.Groups[2];
				return name;
			}
			else {
				string[] linesModified = ReplaceAllAmbiguous(correctItemLine, line);
				foreach (string lineModified in linesModified) {
					m = correctItemLine.Match(lineModified);
					if (m.Success) {
						string name = m.Groups[1].Value + m.Groups[2];
						return name;
					}
				}
				throw new NameParsingException("Unable to get name from line " + line, line);
			}
		}

		public int GetPriceOfOne(string[] ocrText, ref int index) {
			string quantity = ocrText[index + 1];
			string modified = quantity.Replace(" ", "").ToLower().Trim();
			string[] split = modified.Split(costPlusQuantitySeparator);
			if (split.Length != 2) {
				throw new PriceParsingException(ocrText[index + 1], index + 1, false);
			}
			if (decimal.TryParse(split[1], System.Globalization.NumberStyles.Currency | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out decimal result)) {
				return (int)(result * 100);
			}
			else {
				string[] linesModified = ReplaceAllAmbiguous(correctItemLine, split[1]);
				foreach (string lineModified in linesModified) {
					Match m = genericItemPriceFormat.Match(lineModified);
					if (m.Success) {
						if (decimal.TryParse(m.Groups[1].Value + "." + m.Groups[3].Value, System.Globalization.NumberStyles.Currency | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out decimal resultReplaced)) {
							return (int)(resultReplaced * 100);
						}
					}
				}
			}
			throw new PriceParsingException(ocrText[index + 1], index + 1, false);
		}
	}
}
