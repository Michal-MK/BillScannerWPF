using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BillScannerWPF {
	static class WPFHelper {

		public static MainWindow GetMainWindow() {
			return (MainWindow)Application.Current.MainWindow;
		}

		public static string dataPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "BillScanner" + Path.DirectorySeparatorChar; } }
	}
}
