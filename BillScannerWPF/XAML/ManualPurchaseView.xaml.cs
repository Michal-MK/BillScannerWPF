using System.Windows.Controls;
using System.Windows.Input;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for ManualPurchaseView.xaml
	/// </summary>
	public partial class ManualPurchaseView : UserControl {

		public ManualPurchaseViewModel Model { get; set; }

		public ManualPurchaseView() {
			InitializeComponent();
			DataContext = Model;
		}

		public void OnTextChanged(object sender, TextChangedEventArgs e) {
			Model.PopulateList();
		}
	}
}
