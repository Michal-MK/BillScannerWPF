using System.Threading.Tasks;
using System.Windows.Controls;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for NewItemDefinitionPanel.xaml
	/// </summary>
	public partial class NewItemDefinitionPanel : UserControl {
		public NewItemDefinitionPanel() {
			InitializeComponent();
			IsVisibleChanged += NewItemDefinitionPanel_IsVisibleChanged;
		}

		private async void NewItemDefinitionPanel_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			await Task.Delay(1);
			FocusMe.Focus();
		}
	}
}
