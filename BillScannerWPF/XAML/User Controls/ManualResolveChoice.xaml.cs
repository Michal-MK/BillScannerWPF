using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Igor.BillScanner.Core;
using Igor.Models;

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
			_ = Task.Run(() => SetFocus(e));
		}

		private async Task SetFocus(string elementName) {
			await Task.Delay(1);
			BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
			FrameworkElement toFocus = (FrameworkElement)GetType().GetField(elementName, flag).GetValue(this);
			if (toFocus == null)
				Debugger.Break();
			Dispatcher.Invoke(() => {
				if(toFocus is IFocusable focusable) {
					focusable.SetFocus();
				}
				else {
					toFocus.Focus();
				}
			});
		}
	}
}
