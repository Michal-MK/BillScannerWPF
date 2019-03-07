using System.Text;

namespace Igor.BillScanner.Core {
	public static class Extensions {

		public static string ReplaceAt(this string s, int index, char toPlace) {
			StringBuilder sb = new StringBuilder(s);
			sb[index] = toPlace;
			return sb.ToString();
		}
	}
}
