using System;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	class LidlRuleset : BaseRuleset, IRuleset {
		public string[] startMarkers { get { return new string[0]; } }

		public string[] endMarkers { get { return new string[] { "číslo", "účtenky", "cislo", " uctenky", "označení", "oznaceni", }; } }

		public char costPlusQuantitySeparator { get { return 'x'; } }

		public Regex correctItemLine { get; } = new Regex(""); //TODO

		public long GetQuantity(string[] ocrText, int index) {
			if (index > ocrText.Length) { throw new IndexOutOfRangeException(nameof(index)); }
			string quantity = ocrText[index + 1];
			string[] split = quantity.Split(costPlusQuantitySeparator);
			if (split.Length != 2) {
				throw new Exception("Unable to split quantity and cost");
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
			throw new NotImplementedException();
		}

		public decimal PriceOfOne(string[] ocrText, ref int index) {
			if (index > ocrText.Length) { throw new IndexOutOfRangeException(nameof(index)); }
			string quantity = ocrText[index + 1];
			string[] split = quantity.Split(costPlusQuantitySeparator);
			if (split.Length != 2) {
				throw new Exception("Unable to split quantity and cost");
			}
			split[1] = split[1].Replace(',', '.');

			if (decimal.TryParse(split[1], System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out decimal result)) {
				index++;
				return result;
			}
			else {
				quantity = ReplaceAmbiguous(quantity);
				if (decimal.TryParse(split[1], System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out decimal resultReplaced)) {
					index++;
					return resultReplaced;
				}
			}
			throw new NotImplementedException("Unable to get quantity from string " + ocrText[index + 1] + ", subsequently modified " + quantity);
		}
	}
}
