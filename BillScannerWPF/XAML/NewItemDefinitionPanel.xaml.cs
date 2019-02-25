using BillScannerCore;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BillScannerWPF {

	/// <summary>
	/// Code for NewItemDefinitionPanel.xaml
	/// </summary>
	public partial class NewItemDefinitionPanel : UserControl {

		private ManualResetEventSlim evnt = new ManualResetEventSlim();

		/// <summary>
		/// Create a new <see cref="NewItemDefinitionPanel"/>
		/// </summary>
		public NewItemDefinitionPanel() {
			InitializeComponent();
			ITEMREG_RegisterItem_Button.Click += ITEMREG_RegisterItem_Click;
			ITEMREG_UnitOfMeassure_DropDown.ItemsSource = Enum.GetNames(typeof(MeassurementUnit));
		}

		private void ITEMREG_RegisterItem_Click(object sender, RoutedEventArgs e) {
			evnt.Set();
		}


		/// <summary>
		/// Handle new Item registration from a user, returned values are sanity checked
		/// </summary>
		internal async Task<(string itemName, int itemValue, MeassurementUnit itemMeassurements)> RegisterItemAsync() {
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Add(this);
			await Task.Run(() => {
				evnt.Wait();
			});

			decimal itemValue;
			string valueString = ITEMREG_ItemValue_Box.Text;
			while (!decimal.TryParse(valueString, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out itemValue)) {
				ITEMREG_ErrorInfo_Text.Text = "Unable to parse item value: " + valueString + " Enter correct one and register.";
				await Task.Run(() => {
					evnt.Reset();
					evnt.Wait();
				});
				valueString = ITEMREG_ItemValue_Box.Text;
			}
			while (ITEMREG_UnitOfMeassure_DropDown.SelectedItem == null) {
				ITEMREG_ErrorInfo_Text.Text = "No Measurement Unit selected!";
				await Task.Run(() => {
					evnt.Reset();
					evnt.Wait();
				});
			}
			while (string.IsNullOrEmpty(ITEMREG_ItemName_Box.Text)) {
				ITEMREG_ErrorInfo_Text.Text = "Item name is empty!";
				await Task.Run(() => {
					evnt.Reset();
					evnt.Wait();
				});
			}

			if (decimal.TryParse(valueString, NumberStyles.Currency | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal update)){
				if(update != itemValue) {
					itemValue = update;
				}
			}
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Remove(this);
			return (ITEMREG_ItemName_Box.Text, (int)(itemValue * 100), (MeassurementUnit)Enum.Parse(typeof(MeassurementUnit),ITEMREG_UnitOfMeassure_DropDown.SelectedItem.ToString()));
		}
	}
}
