using System;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	class LidlRuleset : BaseRuleset, IRuleset {
		public string[] startMarkers { get { return new string[0]; } }

		public string[] endMarkers { get { return new string[] { "k platbe", "k platbě", "k p1atbe", "karta", "celkova", "castka", "zaplacena"}; } }
		
		public char costPlusQuantitySeparator { get { return 'x'; } }

		public bool skipInitiatingString { get { return false; } }

		public Regex correctItemLine { get; } = new Regex(@"(.+)( \d+[gl%])? (\d+[,.]\d+) B");

		public Regex dateTimeFormat { get { return genericDateTimeFormat; } }

		public int itemLineSpan { get; } = 2;

		public long GetQuantity(string[] ocrText, int index) {
			if (index > ocrText.Length) { throw new IndexOutOfRangeException(nameof(index)); }
			string quantity = ocrText[index + 1];
			string[] split = quantity.Split(costPlusQuantitySeparator);
			if (split.Length != 2) {
				throw new NotImplementedException("Unable to split quantity and cost");
			}

			if (long.TryParse(split[0], out long result)) {
				return result;
			}
			else {
				quantity = ReplaceAmbiguous(quantity);
				if (long.TryParse(quantity.Split(costPlusQuantitySeparator)[0], out long resultReplaced)) {
					return resultReplaced;
				}
			}
			throw new NotImplementedException("Unable to get quantity from string " + ocrText[index + 1] + ", subsequently modified " + quantity);
		}

		public string Name(string line) {
			Match m = correctItemLine.Match(line);
			if (m.Success) {
				string name = m.Groups[1].Value + m.Groups[2];
				return name;
			}
			else {
				string lineModified = ReplaceAmbiguous(line);
				m = correctItemLine.Match(lineModified);
				if (m.Success) {
					string name = m.Groups[1].Value + m.Groups[2];
					return name;
				}
				else {
					throw new NotImplementedException("Unable to get name from line " + line);
				}
			}
		}

		public decimal PriceOfOne(string[] ocrText, ref int index) {
			string quantity = ocrText[index + 1];
			string modified = RemoveLetterCharacters(quantity, costPlusQuantitySeparator).ToLower().Trim();
			string[] split = modified.Split(costPlusQuantitySeparator);
			if (split.Length != 2) {
				throw new Exception("Unable to split quantity and cost");
			}
			split[1] = split[1].Replace(',', '.');

			if (decimal.TryParse(split[1], System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out decimal result)) {
				return result;
			}
			else {
				modified = RemoveLetterCharacters(ReplaceAmbiguous(modified), costPlusQuantitySeparator);
				if (decimal.TryParse(split[1], System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out decimal resultReplaced)) {
					return resultReplaced;
				}
			}
			throw new NotImplementedException("Unable to get PriceOfOne from string " + ocrText[index + 1] + ", subsequently modified " + modified);
		}
	}
}
