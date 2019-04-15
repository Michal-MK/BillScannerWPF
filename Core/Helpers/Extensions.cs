using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Igor.BillScanner.Core {
	public static partial class Extensions {
		public static string ReplaceAt(this string s, int index, char toPlace) {
			StringBuilder sb = new StringBuilder(s);
			sb[index] = toPlace;
			return sb.ToString();
		}
	}
}


namespace System.Linq {
	public static partial class Extensions {
		public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source) {
			ObservableCollection<T> ret = new ObservableCollection<T>();
			foreach (T item in source) {
				ret.Add(item);
			}
			return ret;
		}
	}
}

