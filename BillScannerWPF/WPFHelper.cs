using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BillScannerWPF {
	static class WPFHelper {

		public static MainWindow GetCurrentMainWindow() {
			return (MainWindow)Application.Current.MainWindow;
		}

		public static string dataPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "BillScanner" + Path.DirectorySeparatorChar; } }


		public static string resourcesPath { get { return @"pack://application:,,,/BillScannerWPF;component/Resources/"; } }

		public static string imageRatingResourcesPath { get { return resourcesPath + "MatchRating/"; } }


		public const string itemsFile = "itemsdb.json";

		public static string itemsFullPath { get { return dataPath + itemsFile; } }

		public static string Merge<T>(this List<T> list, char connector) {
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < list.Count; i++) {
				builder.Append(list[i].ToString());
				builder.Append(connector + " ");
			}
			builder.Remove(builder.Length - 2, 2);
			return builder.ToString();
		}
	}
}
