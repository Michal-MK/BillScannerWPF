using System;
using System.Windows;
using System.Windows.Controls;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Core for DateBox.xaml
	/// </summary>
	public partial class DateBox : UserControl {
		public DateBox() {
			InitializeComponent();
			DateBoxInputTextBox.TextChanged += (s, e) => { BindableCarretIndex = (e.Source as TextBox).CaretIndex; };
		}

		public int BindableCarretIndex {
			get { return DateBoxInputTextBox.CaretIndex; }
			set { DateBoxInputTextBox.CaretIndex = value; }
		}

		public static readonly DependencyProperty BindableCarretIndexProperty =
			DependencyProperty.Register("BindableCarretIndex", typeof(int), typeof(DateBox), new PropertyMetadata(OnChange));

		private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			(d as DateBox).DateBoxInputTextBox.CaretIndex = (int)e.NewValue;
		}
	}
}
