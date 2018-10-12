using System;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	internal class BaseRuleset {

		protected Regex strayNumber = new Regex(@"[a-zA-Z]+(\d)[a-zA-Z]+");
		protected Regex strayLetter = new Regex(@"\d+[.,]?([a-zA-Z])[.,]?\d+");
		protected Regex genericItemFormat = new Regex(@"(.+) (\d+[.,]\d+) [A-Z]");
		protected Regex genericDateTimeFormat = new Regex(@"\d+:\d+:\d+");

		protected static (char conflicting, char resolving)[] ambiguousLettersArray = new (char conflicting, char resolving)[] {
			('9','g'),
			('0','o'),
			('0','O'),
			('0','Q'),
			('1','i'),
			('1','l'),
			('1','I')
		};


		internal string ReplaceAmbiguous(string original) {
			Match baseMatch = genericItemFormat.Match(original);
			if (baseMatch.Success) {
				Match number = strayNumber.Match(original);
				Match letter = strayLetter.Match(original);
				if (number.Success) {
					for (int i = 0; i < ambiguousLettersArray.Length; i++) {
						if (ambiguousLettersArray[i].conflicting.ToString() == number.Groups[1].Value) {
							original = baseMatch.Groups[1].Value.Replace(ambiguousLettersArray[i].conflicting, ambiguousLettersArray[i].resolving)
									+ baseMatch.Groups[2].Value;
						}
					}
				}
				if (letter.Success) {
					for (int i = 0; i < ambiguousLettersArray.Length; i++) {
						if (ambiguousLettersArray[i].conflicting.ToString() == letter.Groups[1].Value) {
							original = baseMatch.Groups[1]
								+ baseMatch.Groups[2].Value.Replace(ambiguousLettersArray[i].resolving, ambiguousLettersArray[i].conflicting);
						}
					}
				}
			}
			return original;
		}

		internal string ReplaceAmbiguousToNumber(string original) {
			for (int i = 0; i < ambiguousLettersArray.Length; i++) {
				original = original.Replace(ambiguousLettersArray[i].resolving, ambiguousLettersArray[i].conflicting);
			}
			return original;
		}

		internal static IRuleset GetRuleset(Shop selectedShop) {
			switch (selectedShop) {
				case Shop.Lidl: {
					return new LidlRuleset();
				}
				case Shop.McDonalds: {
					return new McDonaldsRuleset();
				}
				case Shop.Albert: {
					return new AlbertRuleset();
				}
				default: {
					throw new NotImplementedException();
				}
			}
		}

		internal string RemoveLetterCharacters(string original, char splitter) {
			Regex r = new Regex(@"(\d+([,.] ?\d+)?) " + splitter + @" (\d+([,.] ?\d+)?)");

			Match m = r.Match(original.ToLower());
			if (m.Success) {
				return m.Value.Replace(" ", "");
			}
			throw new NotImplementedException();
		}	
	}
}