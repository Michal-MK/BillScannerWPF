using BillScannerCore;
using System.Windows.Controls;


namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for StatusBar.xaml
	/// </summary>
	public partial class StatusBar : UserControl {

		public StatusBarViewModel model { get; }

		public StatusBar(StatusBarViewModel model) {
			InitializeComponent();
			this.model = model;
			this.DataContext = model;
		}
	}
}
