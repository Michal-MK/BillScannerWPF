using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for StatusBar.xaml
	/// </summary>
	public partial class StatusBar : UserControl {

		public StatusBarViewModel Model { get; }

		public StatusBar() {
			InitializeComponent();
			DataContext = Model = new StatusBarViewModel();
		}
	}
}
