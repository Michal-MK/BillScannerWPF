using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {
	internal class BaseRuleset {

		protected Regex strayNumber = new Regex(@"[a-zA-Z]+(\d)[a-zA-Z]+");
		protected Regex strayLetter = new Regex(@"\d+[.,]?([a-zA-Z])[.,]?\d+");
		protected Regex genericItemFormat = new Regex(@"(.+) (\d+[.,]\d+) [A-Z]");

		protected static (char conflicting, char resolving)[] ambiguousLettersArray = new (char conflicting, char resolving)[] {
			('9','g'),
			('0','o'),
			('0','O'),
			('0','Q'),
			('1','i'),
			('1','l'),
			('1','I')
		};


		protected string ReplaceAmbiguous(string original) {
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

		protected string ReplaceAmbiguousToNumber(string original) {
			for (int i = 0; i < ambiguousLettersArray.Length; i++) {
				original = original.Replace(ambiguousLettersArray[i].resolving, ambiguousLettersArray[i].conflicting);
			}
			return original;
		}
	}
}