using BillScannerCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	class AlbertRuleset : BaseRuleset, IRuleset {

		private Regex multiLineItems = new Regex(@"(\d+) x (\d+[., ]?\d+) .+ A");

		public Regex correctItemLine { get; } = new Regex(@"(.+)( (\d+)[g0Ll1])? (\d+[,.]\d+) A");

		public string[] startMarkers {
			get { return new string[] { "kc", "-----", "oddil", "oddll", "oddíl", "vlozka", "vložka", "19737" }; }
		}

		public string[] endMarkers {
			get { return new string[] { "vratit", "vrátit", "kod", "sazba", "DPH", "ID uctu" }; }
		}

		public char costPlusQuantitySeparator { get { return 'x'; } }

		public Regex dateTimeFormat { get { return genericDateTimeFormat; } }

		public Shop shop => Shop.Albert;

		public int GetQuantity(string[] ocrText, int index) {
			if (IsSingleItem(ocrText, index)) {
				return 1;
			}
			else {
				string[] split = ocrText[index + 1].Split(costPlusQuantitySeparator);
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
				m = correctItemLine.Match(ReplaceAmbiguous(line));
				if (m.Success) {
					return m.Groups[1].Value;
				}
			}
			throw new NameParsingException("Unable to get name from string: " + line, line);
		}

		public int GetPriceOfOne(string[] ocrText, ref int index) {
			if (IsSingleItem(ocrText, index)) {
				Match single = correctItemLine.Match(ocrText[index]);
				string final = single.Groups[4].Value.Replace(',', '.');
				if (decimal.TryParse(final, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
					return (int)result * 100;
				}
				else {
					throw new PriceParsingException(ocrText[index], index, true);
				}
			}
			else {
				Match multiL = multiLineItems.Match(ocrText[index + 1]);
				string final = multiL.Groups[2].Value.Replace(',', '.').Replace(" ", ".");
				if (decimal.TryParse(final, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
					index++;
					return (int)result * 100;
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
			else if (correctItemLine.Match(ReplaceAmbiguous(ocrText[index])).Success) {
				ocrText[index] = ReplaceAmbiguous(ocrText[index]);
				return true;
			}
			else if (index + 1 < ocrText.Length && multiLineItems.Match(ocrText[index + 1]).Success) {
				return false;
			}
			else if (index + 1 < ocrText.Length && multiLineItems.Match(ReplaceAmbiguous(ocrText[index + 1])).Success) {
				ocrText[index + 1] = ReplaceAmbiguous(ocrText[index + 1]);
				return false;
			}
			return true;
			throw new GenericParsingException(ocrText, index);
		}
	}
}
