using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for NewItemDefinitionPanel.xaml
	/// </summary>
	public partial class NewItemDefinitionPanel : UserControl {
		ManualResetEventSlim evnt = new ManualResetEventSlim();
		public NewItemDefinitionPanel() {
			InitializeComponent();
			ITEMREG_RegisterItem_Button.Click += ITEMREG_RegisterItem_Click;
			ITEMREG_UnitOfMeassure_DropDown.ItemsSource = Enum.GetNames(typeof(MeassurementUnit));
		}

		private void ITEMREG_RegisterItem_Click(object sender, RoutedEventArgs e) {
			evnt.Set();
		}

		internal async Task<(string itemName, decimal itemValue, MeassurementUnit itemMeassurements)> RegisterItemAsync() {
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(this);
			await Task.Run(() => {
				evnt.Wait();
			});

			decimal itemValue;
			string valueString = ITEMREG_ItemValue_Box.Text.Replace(',', '.');
			while (!decimal.TryParse(valueString, NumberStyles.Currency, CultureInfo.InvariantCulture, out itemValue)) {
				ITEMREG_ErrorInfo_Text.Text = "Unable to parse item value: " + valueString + " Enter correct one and register.";
				await Task.Run(() => {
					evnt.Reset();
					evnt.Wait();
				});
				valueString = ITEMREG_ItemValue_Box.Text.Replace(',', '.');
			}
			while (ITEMREG_UnitOfMeassure_DropDown.SelectedItem == null) {
				ITEMREG_ErrorInfo_Text.Text = "No Meassurement Unit selected!";
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

			if (decimal.TryParse(valueString, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal update)){
				if(update != itemValue) {
					itemValue = update;
				}
			}
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(this);
			return (ITEMREG_ItemName_Box.Text, itemValue, (MeassurementUnit)Enum.Parse(typeof(MeassurementUnit),ITEMREG_UnitOfMeassure_DropDown.SelectedItem.ToString()));
		}
	}
}
