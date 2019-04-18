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
		}

		public string Text {
			get => (DataContext as DateBoxViewModel).CurrentText;
			set { (DataContext as DateBoxViewModel).CurrentText = value; }
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(nameof(Text), typeof(string), typeof(DateBox));
	}
}
