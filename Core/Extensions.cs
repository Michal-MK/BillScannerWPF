using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillScannerCore {
	public static class Extensions {

		public static string ReplaceAt(this string s, int index, char toPlace) {
			StringBuilder sb = new StringBuilder(s);
			sb[index] = toPlace;
			return sb.ToString();
		}
	}
}
