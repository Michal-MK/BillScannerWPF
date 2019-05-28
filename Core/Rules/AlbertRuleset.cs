using System.Globalization;
using System.Text.RegularExpressions;

namespace Igor.BillScanner.Core.Rules {
	class AlbertRuleset : BaseRuleset, IRuleset {

		private Regex multiLineItems = new Regex(@"(\d+) ?. ?(\d+[., ]?\d+) .+ A");

		public Regex correctItemLine { get; } = new Regex(@"(.+)( (\d+)[g0Ll1])? (\d+[,.]\d+) A");

		public string[] StartMarkers {
			get { return new string[] { "kc", "-----", "oddil", "oddll", "oddíl", "vlozka", "vložka", "19737" }; }
		}

		public string[] EndMarkers {
			get { return new string[] { "vratit", "vrátit", "kod", "sazba", "DPH", "ID uctu" }; }
		}

		public char CostPlusQuantitySeparator { get { return 'x'; } }

		public Regex dateTimeFormat { get { return genericDateTimeFormat; } }

		public Regex correctCostAndQuantityLine => genericItemPriceFormat;

		public Shop Shop => Shop.Albert;

		public int GetQuantity(string[] ocrText, int index) {
			if (IsSingleItem(ocrText, index)) {
				return 1;
			}
			else {
				string[] split = ocrText[index + 1].Split(CostPlusQuantitySeparator);
				if (split.Length == 2) {
					if (int.TryParse(split[0], out int result)) {
						return result;
					}
				}
				throw new QuantityParsingException("Unable to get quantity from string " + ocrText[index + 2], ocrText[index + 2], index + 2);
			}
		}

		public string GetName(string line) {
			Match m = correctItemLine.Match(line);
			if (m.Success) {
				return m.Groups[1].Value;
			}
			else {
				string[] lines = ReplaceAllAmbiguous(correctItemLine, line);
				foreach (string lineModified in lines) {
					m = correctItemLine.Match(lineModified);
					if (m.Success) {
						return m.Groups[1].Value;
					}
				}
			}
			throw new NameParsingException("Unable to get name from string: " + line, line);
		}

		public int GetPriceOfOne(string[] ocrText, ref int index) {
			if (IsSingleItem(ocrText, index)) {
				Match single = correctItemLine.Match(ocrText[index]);
				string final = single.Groups[4].Value;
				if (decimal.TryParse(final, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result)) {
					return (int)(result * 100);
				}
				else {
					throw new PriceParsingException(ocrText[index], index, true);
				}
			}
			else {
				Match multiL = multiLineItems.Match(ocrText[index + 1]);
				string final = multiL.Groups[2].Value.Replace(" ", ".");
				if (decimal.TryParse(final, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result)) {
					index++;
					return (int)(result * 100);
				}
				else {
					throw new PriceParsingException(ocrText[index + 1], index + 1, false);
				}
			}
		}

		private bool IsSingleItem(string[] ocrText, int index) {

			if (correctItemLine.Match(ocrText[index]).Success) {
				return true;
			}
			else if (index + 1 < ocrText.Length && multiLineItems.Match(ocrText[index + 1]).Success) {
				return false;
			}
			else {
				string[] linesModifiedSingle = ReplaceAllAmbiguous(correctItemLine, ocrText[index]);
				foreach (string lineModified in linesModifiedSingle) {
					if (correctItemLine.IsMatch(lineModified)) {
						ocrText[index] = lineModified;
						return true;
					}
				}
				string[] linesModifiedDouble = ReplaceAllAmbiguous(correctItemLine, ocrText[index + 1]);
				foreach (string lineModified in linesModifiedDouble) {
					if (multiLineItems.IsMatch(lineModified)) {
						ocrText[index] = lineModified;
						return false;
					}
				}
			}
			throw new GenericParsingException(ocrText, index);
		}
	}
}
