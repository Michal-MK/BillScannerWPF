using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillScannerWPF.Rules {
	class LidlRuleset : BaseRuleset, IRuleset {
		public string[] startMarkers { get { return new string[0]; } }

		public string[] endMarkers { get { return new string[] { "číslo", "účtenky", "cislo", " uctenky", "označení", "oznaceni", }; } }

		public char costPlusQuantitySeparator { get { return 'x'; } }

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

		public decimal PriceOfOne(string[] ocrText, int index) {
			if (index > ocrText.Length) { throw new IndexOutOfRangeException(nameof(index)); }
			string quantity = ocrText[index + 1];
			string[] split = quantity.Split(costPlusQuantitySeparator);
			if (split.Length != 2) {
				throw new Exception("Unable to split quantity and cost");
			}
			split[1] = split[1].Replace(',', '.');

			if (decimal.TryParse(split[1], System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out decimal result)) {
				return result;
			}
			else {
				quantity = ReplaceAmbiguous(quantity);
				if (decimal.TryParse(split[1], System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.InvariantCulture, out decimal resultReplaced)) {
					return resultReplaced;
				}
			}
			throw new NotImplementedException("Unable to get quantity from string " + ocrText[index + 1] + ", subsequently modified " + quantity);
		}
	}
}
