using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Core for DateBox.xaml
	/// </summary>
	public partial class DateBox : UserControl {
		public DateBox() {
			InitializeComponent();
			DateBoxInput = DateBoxInputTextBox;
			(DataContext as DateBoxViewModel).Control = this;
		}

		public TextBox DateBoxInput { get; set; }

		public string DateBoxText {
			get => (DateBoxInput.DataContext as DateBoxViewModel).CurrentText;
			set { (DateBoxInput.DataContext as DateBoxViewModel).CurrentText = value; }
		}

		public static readonly DependencyProperty DateBoxTextProperty =
			DependencyProperty.Register(nameof(DateBoxText), typeof(string), typeof(DateBox));

		private void OnTextChanged(object sender, TextChangedEventArgs e) {
			DateBoxText = (sender as TextBox).Text;
		}

		public ManualResolutionViewModel ManualResolveDataContext {
			get { return (ManualResolutionViewModel)GetValue(ManualResolveDataContextProperty); }
			set { SetValue(ManualResolveDataContextProperty, value); }
		}

		public static readonly DependencyProperty ManualResolveDataContextProperty =
			DependencyProperty.Register(nameof(ManualResolveDataContext), typeof(ManualResolutionViewModel), typeof(DateBox), new PropertyMetadata(OnChange));

		private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			(d as DateBox).ManualResolveDataContext = e.NewValue as ManualResolutionViewModel;
			Debugger.Break();
		}
	}
}
