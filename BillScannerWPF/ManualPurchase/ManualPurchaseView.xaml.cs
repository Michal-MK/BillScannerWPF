using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for ManualPurchaseView.xaml
	/// </summary>
	public partial class ManualPurchaseView : UserControl {

		public ManualPurchaseView() {
			InitializeComponent();
		}

		public void OnTextChanged(object sender, TextChangedEventArgs e) {
			ManualPurchaseViewModel purchaseModel = DataContext as ManualPurchaseViewModel;
			ItemListViewModel listModel = MANUALPURCHASE_Items_List.DataContext as ItemListViewModel;

			purchaseModel.PopulateList();
			listModel.Items = purchaseModel.Items;
			listModel.Notify("Items");
		}
	}
}
