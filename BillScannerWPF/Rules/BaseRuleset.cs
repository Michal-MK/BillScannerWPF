using BillScannerCore;
using System;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {

	/// <summary>
	/// Base class for all shop parsing rules.
	/// </summary>
	internal class BaseRuleset {

		/// <summary>
		/// Regex to find one number sequences surrounded by letters, most likely an OCR inaccuracy
		/// </summary>
		protected Regex strayNumber = new Regex(@"[a-zA-Z]+(\d)[a-zA-Z]+");

		/// <summary>
		/// Regex to find one letter sequences surrounded by numbers, most likely an OCR inaccuracy
		/// </summary>
		protected Regex strayLetter = new Regex(@"\d+[.,]?([a-zA-Z])[.,]?\d+");

		/// <summary>
		/// Base item format as observed on multiple bills, shops should not use it and create a new better one
		/// </summary>
		protected Regex genericItemFormat = new Regex(@"(.+) (\d+[.,]\d+) [A-Z]");

		/// <summary>
		/// Base date time format as observed on multiple bills, shops should not use it and create a new better one
		/// </summary>
		protected Regex genericDateTimeFormat = new Regex(@"\d+:\d+:\d+");

		/// <summary>
		/// A collection of commonly mismatched characters in a word
		/// </summary>
		protected static (char conflicting, char resolving)[] ambiguousLettersArray = new (char conflicting, char resolving)[] {
			('9','g'),
			('0','o'),
			('0','O'),
			('0','Q'),
			('1','i'),
			('1','l'),
			('1','I')
		};

		/// <summary>
		/// Attempts a match on with the generic item Regex:
		/// <para>On success replaces stray numbers and letters</para>
		/// <para>On failure returns the original</para>
		/// </summary>
		/// <param name="original">The original string to modify</param>
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

		/// <summary>
		/// Aggressively replaces all instances of commonly conflicting character inside the string for corresponding numbers
		/// </summary>
		/// <param name="original">The original line to modify</param>
		internal string ReplaceAmbiguousToNumber(string original) {
			for (int i = 0; i < ambiguousLettersArray.Length; i++) {
				original = original.Replace(ambiguousLettersArray[i].resolving, ambiguousLettersArray[i].conflicting);
			}
			return original;
		}

		/// <summary>
		/// Helper class to create an instance of <see cref="IRuleset"/> for provided shop
		/// </summary>
		/// <param name="selectedShop">The shop for which to create a <see cref="IRuleset"/></param>
		/// <returns></returns>
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
					throw new NotImplementedException(selectedShop.ToString() + " was not yet implemented!");
				}
			}
		}


		/// <summary>
		/// Attempts to remove any leading or trailing non numeric characters from a string
		/// <para>On failure returns the original!</para>
		/// </summary>
		internal string RemoveLetterCharacters(string original, char splitter) {
			Regex r = new Regex(@"(\d+([,.] ?\d+)?) " + splitter + @" (\d+([,.] ?\d+)?)");

			Match m = r.Match(original.ToLower());
			if (m.Success) {
				return m.Value.Replace(" ", "");
			}
			return original;
		}	
	}
}