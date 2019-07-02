using System;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ManualResolveChoice.xaml
	/// </summary>
	public partial class ManualResolveChoice : UserControl {

		public ManualResolveChoice() {
			InitializeComponent();

			MainWindowViewModel.Instance.ManualResolveViewModel.GiveFocus += FocusElement;
		}

		private void FocusElement(object sender, string e) {
			throw new NotImplementedException();
		}
	}
}