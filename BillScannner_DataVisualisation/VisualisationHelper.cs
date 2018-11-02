using BillScannerCore;
using System.IO;

namespace BillScannner_DataVisualisation {
	/// <summary>
	/// Helper functions for BillScanner_DataVisualisation
	/// </summary>
	internal static class VisualisationHelper {

		/// <summary>
		/// Get this application's internal 'Resources' folder
		/// </summary>
		public static string resourcesPath { get { return @"pack://application:,,,/BillScannerWPF_DataVisualisation;component/Resources/"; } }

		public static string shopLogosPath { get { return resourcesPath + "Shop Logos/"; } }

		public static bool DatabaseExists(Shop shop) {
			if(File.Exists( WPFHelper.dataPath +  shop.ToString() + "_purchasedb.json")) {
				return true;
			}
			else {
				return false;
			}
		}
	}
}
