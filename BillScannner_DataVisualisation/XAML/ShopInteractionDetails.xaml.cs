using BillScannerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BillScannner_DataVisualisation {
	/// <summary>
	/// Interaction logic for ShopInteractionDetails.xaml
	/// </summary>
	public partial class ShopInteractionDetails : UserControl {
		public ShopInteractionDetails(DatabaseAccess databaseAccess) {
			InitializeComponent();

			Purchase[] purchases = databaseAccess.GetPurchases();
			if(purchases.Length < 2) {
				return;
			}
			TimeSpan span = purchases[0].date - purchases[purchases.Length - 1].date;
			decimal totalPaid = 0;	
			foreach (Purchase purchase in purchases) {
				totalPaid += purchase.totalCost; 
			}

			decimal dailyPrice = totalPaid / (decimal)span.TotalDays;



			ShopInteractionDetails_DailyAverage_TB.Text = dailyPrice.ToString();
			ShopInteractionDetails_MonthlyAverage_TB.Text = (dailyPrice * 31).ToString();
			ShopInteractionDetails_YearlyAverage_TB.Text =( dailyPrice * 365).ToString();
		}
	}
}
