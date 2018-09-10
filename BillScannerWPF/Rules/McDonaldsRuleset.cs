﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BillScannerWPF.Rules {
	class McDonaldsRuleset : BaseRuleset, IRuleset {
		public string[] startMarkers { get { return new string[] { "qty", "item", "total", "bezny", "rezim" }; } }

		public string[] endMarkers { get { return new string[] { "vat", "number", "tax", "bkp", "****" }; } }

		public char costPlusQuantitySeparator { get { return '\0'; } }


		public long GetQuantity(string[] ocrText, int index) {
			string[] split = ocrText[index].Split(null);
			if (long.TryParse(split[0], out long result)) {
				return result;
			}
			string modified = ReplaceAmbiguousToNumber(split[0]);
			if (modified == split[0]) {
				throw new Exception("Unable to get quantity");
			}
			else {
				if (long.TryParse(modified, out long resultModified)) {
					return result;
				}
			}
			throw new NotImplementedException("Unable to get quantity from string " + ocrText[index] + ", subsequently modified " + modified);
		}

		public decimal PriceOfOne(string[] ocrText, int index) {
			Regex r = new Regex(@"(\d) (\w+) (\d+\.\d+) B");
			string line = ocrText[index].Replace(',', '.');
			Match m = r.Match(line);
			if (decimal.TryParse(m.Groups[2].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result)) {
				return result;
			}
			string modified = ReplaceAmbiguousToNumber(line);
			if (modified == line) {
				throw new Exception("Unable to get Text");
			}
			else {
				Match mm = r.Match(line);
				if (decimal.TryParse(mm.Groups[2].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal resultModified)) {
					return result;
				}
			}
			throw new NotImplementedException("Unable to get quantity from string " + ocrText[index] + ", subsequently modified " + modified);
		}
	}
}
