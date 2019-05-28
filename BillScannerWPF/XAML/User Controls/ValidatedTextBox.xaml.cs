using System;
using System.Windows;
using System.Windows.Controls;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for ValidatedTextBox.xaml
	/// </summary>
	public partial class ValidatedTextBox : UserControl {

		public ValidatedTextBox() {
			InitializeComponent();
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
			TextBox box = sender as TextBox;
			SetValue(TextProperty, box.Text); //Propagate changes to bindings

			if (!ValidationFunction(box.Text)) {
				border.Visibility = Visibility.Visible;
			}
			else {
				border.Visibility = Visibility.Hidden;
			}
		}

		#region DependencyProperties

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(nameof(Text), typeof(string), typeof(ValidatedTextBox));

		public string Text {
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public static readonly DependencyProperty ValidationFunctionProperty =
			DependencyProperty.Register(nameof(ValidationFunction), typeof(Func<string, bool>), typeof(ValidatedTextBox));

		public Func<string, bool> ValidationFunction {
			get => (Func<string, bool>)GetValue(ValidationFunctionProperty);
			set => SetValue(ValidationFunctionProperty, value);
		}

		#endregion
	}
}
