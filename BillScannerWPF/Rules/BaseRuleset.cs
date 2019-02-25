using BillScannerCore;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BillScannerWPF.Rules {

	/// <summary>
	/// Base class for all shop parsing rules.
	/// </summary>
	internal class BaseRuleset {

		/// <summary>
		/// Regex to find one number sequences surrounded by letters, most likely an OCR inaccuracy
		/// </summary>
		protected readonly Regex strayNumber = new Regex(@"[a-zA-Z]+(\d)[a-zA-Z]+");

		/// <summary>
		/// Regex to find one letter sequences surrounded by numbers, most likely an OCR inaccuracy
		/// </summary>
		protected readonly Regex strayLetter = new Regex(@"\d+[.,]?([a-zA-Z])[.,]?\d+");

		/// <summary>
		/// Base date time format as observed on multiple bills, shops should not use it and create a new better one
		/// </summary>
		protected Regex genericDateTimeFormat = new Regex(@"\d+:\d+:\d+");


		protected Regex genericItemPriceFormat = new Regex(@"(\d+)([,.])(\d+)");

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
		/// Attempts a match on the original string with the generic item Regex:
		/// <para>On success replaces stray numbers and letters</para>
		/// <para>On failure returns the original</para>
		/// </summary>
		[Obsolete("This method is deprecated, use the one that returns an array since it contains all possible ambiguities")]
		internal string ReplaceAmbiguous(Regex correctItemRegex, string original) {
			Match number = strayNumber.Match(original);
			Match letter = strayLetter.Match(original);
			if (number.Success) {
				for (int i = 0; i < ambiguousLettersArray.Length; i++) {
					if (ambiguousLettersArray[i].conflicting.ToString() == number.Groups[1].Value) {
						original = original.ReplaceAt(1, ambiguousLettersArray[i].resolving);
					}
				}
			}
			if (letter.Success) {
				for (int i = 0; i < ambiguousLettersArray.Length; i++) {
					if (ambiguousLettersArray[i].conflicting.ToString() == letter.Groups[1].Value) {
						original = original.ReplaceAt(letter.Groups[1].Index, ambiguousLettersArray[i].conflicting);
					}
				}
			}
			return original;
		}

		/// <summary>
		/// Returns almost all possible single character mismatch corrections from the original 
		/// </summary>
		internal string[] ReplaceAllAmbiguous(Regex correctItemRegex, string original) {
			List<string> strings = new List<string>();
			Match number = strayNumber.Match(original);
			Match letter = strayLetter.Match(original);

			if (number.Success) {
				for (int i = 0; i < ambiguousLettersArray.Length; i++) {
					if (ambiguousLettersArray[i].conflicting.ToString() == number.Groups[1].Value) {
						strings.Add(original.ReplaceAt(1, ambiguousLettersArray[i].resolving));
					}
				}
			}
			if (letter.Success) {
				for (int i = 0; i < ambiguousLettersArray.Length; i++) {
					if (ambiguousLettersArray[i].conflicting.ToString() == letter.Groups[1].Value) {
						strings.Add(original.ReplaceAt(letter.Groups[1].Index, ambiguousLettersArray[i].conflicting));
					}
				}
			}
			if(!number.Success && !letter.Success) {
				strings.Add(original);
			}
			return strings.ToArray();
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
	}
}