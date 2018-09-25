using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	class AlbertRuleset : BaseRuleset, IRuleset {

		private Regex singleLineItem = new Regex(@"(.+)( (\d+)[g0Ll1])? (\d+[,.]\d+) A");
		private Regex multiLineItems = new Regex(@"(\d+) x (\d+[., ]?\d+) .+ A");

		public Regex correctItemLine { get { return singleLineItem; } }

		public string[] startMarkers {
			get { return new string[] { "kc", "-----", "oddil", "oddíl", "vlozka", "vložka", "19737" }; }
		}

		public string[] endMarkers {
			get { return new string[] { "vratit", "vrátit", "kod", "sazba", "DPH", "ID uctu" }; }
		}

		public char costPlusQuantitySeparator { get { return 'x'; } }

		public bool skipInitiatingString { get { return true; } }

		public int itemLineSpan { get; } = -1;

		public Regex dateTimeFormat { get { return genericDateTimeFormat; } }

		public long GetQuantity(string[] ocrText, int index) {
			if (IsSingleItem(ocrText, index)) {
				string[] split = ocrText[index].Split(costPlusQuantitySeparator);
				if (split.Length == 2) {
					if (long.TryParse(split[0], out long result)) {
						return result;
					}
				}
				throw new NotImplementedException("Unable to get quantity from string " + ocrText[index]);
			}
			else {
				string[] split = ocrText[index + 1].Split(costPlusQuantitySeparator);
				if (split.Length == 2) {
					if (long.TryParse(split[0], out long result)) {
						return result;
					}
				}
				throw new NotImplementedException("Unable to get quantity from string " + ocrText[index + 2]);
			}
		}

		public string Name(string line) {
			Match m = singleLineItem.Match(line);
			if (m.Success) {
				return m.Groups[1].Value;
			}
			else {
				m = singleLineItem.Match(ReplaceAmbiguous(line));
				if (m.Success) {
					return m.Groups[1].Value;
				}
			}
			throw new NotImplementedException("Unable to get name from string: " + line);
		}

		public decimal PriceOfOne(string[] ocrText, ref int index) {
			if (IsSingleItem(ocrText, index)) {
				Match single = singleLineItem.Match(ocrText[index]);
				string final = single.Groups[4].Value.Replace(',', '.');
				if (decimal.TryParse(final, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
					return result;
				}
				else {
					throw new NotImplementedException();
				}
			}
			else {
				Match multiL = multiLineItems.Match(ocrText[index + 1]);
				string final = multiL.Groups[2].Value.Replace(',', '.').Replace(" ",".");
				if (decimal.TryParse(final, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
					index++;
					return result;
				}
				else {
					throw new NotImplementedException();
				}
			}
		}

		private bool IsSingleItem(string[] ocrText, int index) {
			if (singleLineItem.Match(ocrText[index]).Success) {
				return true;
			}
			else if (singleLineItem.Match(ReplaceAmbiguous(ocrText[index])).Success) {
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
			throw new NotImplementedException();
		}
	}
}
