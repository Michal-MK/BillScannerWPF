using System.Collections.Generic;

namespace BillScannerWPF.Rules {
	internal class BaseRuleset {

		protected static (char conflicting, char resolving)[] ambiguousLettersArray = new(char conflicting, char resolving)[] {
			('9','g'),
			('0','o'),
			('0','O'),
			('0','Q'),
			('1','i'),
			('1','l'),
			('1','I')
		};


		protected string ReplaceAmbiguous(string original) {
			for (int i = 0; i < ambiguousLettersArray.Length; i++) {
				original = original.Replace(ambiguousLettersArray[i].conflicting, ambiguousLettersArray[i].resolving);
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