using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace BillScannerWPF {
	/// <summary>
	/// Core for DateBox.xaml
	/// </summary>
	public partial class DateBox : UserControl {
		public DateBox() {
			InitializeComponent();
		}

		ManualResetEventSlim evnt = new ManualResetEventSlim();

		private void DATEBOX_Input_TextChanged(object sender, TextChangedEventArgs e) {
			TextBox senderBox = (TextBox)sender;
			string currentValue = senderBox.Text;
			if (senderBox.CaretIndex != senderBox.Text.Length) {
				return;
			}
			if (e.Changes.ToArray()[0].AddedLength == 1) {
				switch (currentValue.Length) {
					case 2:
					case 5:
					case 13:
					case 16: {
						senderBox.Text = currentValue + ":";
						break;
					}
					case 10: {
						senderBox.Text = currentValue + " ";
						break;
					}
				}
				ValidateDate(senderBox.Text);
				senderBox.CaretIndex = senderBox.Text.Length;
			}
			if (e.Changes.ToArray()[0].RemovedLength == 1) {
				switch (currentValue.Length) {
					case 2:
					case 5:
					case 10:
					case 13:
					case 16: {
						senderBox.Text = currentValue.Remove(currentValue.Length - 1, 1);
						break;
					}
				}
				senderBox.CaretIndex = senderBox.Text.Length;
				ValidateDate(senderBox.Text);
			}
			if (currentValue.Length == 19) {
				evnt.Set();
			}
		}

		private void ValidateDate(string currentValue) {
			foreach (char c in currentValue) { //Check digits
				if (!char.IsNumber(c)) {
					if (c == ':' || c == ' ')
						continue;

					DATEBOX_Input_Box.Foreground = Brushes.Red;
					return;
				}
			}
			switch (currentValue.Length) {
				case 3: { //Validate day
					int number = int.Parse(currentValue.Substring(0, 2));
					if (number < 1 || number > 31) {
						DATEBOX_Input_Box.Foreground = Brushes.Red;
					}
					else {
						DATEBOX_Input_Box.Foreground = Brushes.Orange;
					}
					return;
				}
				case 6: { //Validate month
					int number = int.Parse(currentValue.Substring(3, 2));
					if (number < 1 || number > 12) {
						DATEBOX_Input_Box.Foreground = Brushes.Red;
					}
					else {
						DATEBOX_Input_Box.Foreground = Brushes.Orange;
					}
					return;
				}
				case 14: { //Validate hour
					int number = int.Parse(currentValue.Substring(11, 2));
					if (number < 0 || number > 23) {
						DATEBOX_Input_Box.Foreground = Brushes.Red;
					}
					else {
						DATEBOX_Input_Box.Foreground = Brushes.Orange;
					}
					return;
				}
				case 17: { //Validate minute
					int number = int.Parse(currentValue.Substring(14, 2));
					if (number < 0 || number > 60) {
						DATEBOX_Input_Box.Foreground = Brushes.Red;
					}
					else {
						DATEBOX_Input_Box.Foreground = Brushes.Orange;
					}
					return;
				}
				case 20: { //Validate second
					int number = int.Parse(currentValue.Substring(17, 2));
					if (number < 0 || number > 60) {
						DATEBOX_Input_Box.Foreground = Brushes.Red;
					}
					else {
						DATEBOX_Input_Box.Foreground = Brushes.Orange;
					}
					return;
				}
			}
		}

		internal async Task<DateTime> FinalizeDateAsync() {
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Add(this);
			await Task.Run(() => {
				evnt.Wait();
			});
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Remove(this);
			return DateTime.ParseExact(DATEBOX_Input_Box.Text, "dd:MM:yyyy HH:mm:ss", CultureInfo.InvariantCulture);
		}
	}
}
