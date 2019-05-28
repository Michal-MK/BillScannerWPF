using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Igor.BillScanner.Core {
	public class DateBoxViewModel : BaseViewModel {

		#region Backing Fields

		private string _borderColor = "Red";
		private bool _isValidInput;
		private string _currentText = "";
		private readonly Regex _validDate = new Regex(@"(\d{2}):(\d{2}):(\d{4})\s(\d{2}):(\d{2}):(\d{2})");
		private string _previousText = "";
		private int _currentCarretIndex;

		#endregion

		public int CurrentCarretIndex { get => _currentCarretIndex; set { _currentCarretIndex = value; Notify(nameof(CurrentCarretIndex)); } }
		public string CurrentText { get => _currentText; set { _previousText = _currentText; _currentText = value; Notify(nameof(CurrentText)); _ = ValidateText(); } }
		public bool IsValidInput { get => _isValidInput; set { _isValidInput = value; Notify(nameof(IsValidInput)); } }
		public string BorderColor { get => _borderColor; set { _borderColor = value; Notify(nameof(BorderColor)); } }

		public bool TryGetDate(out DateTime time) {
			if (!_validDate.IsMatch(CurrentText)) {
				time = DateTime.MinValue;
				return false;
			}
			time = DateTime.ParseExact(CurrentText, "dd:MM:yyyy dd:mm:ss", CultureInfo.GetCultureInfo("cs"));
			return true;
		}

		private async Task ValidateText() {
			await Task.Delay(1);
			if (_previousText.Length + 1 == _currentText.Length) {
				switch (_currentText.Length) {
					case 2:
					case 5:
					case 13:
					case 16: {
						CurrentText += ":";
						CurrentCarretIndex = CurrentText.Length;
						BorderColor = "Orange";
						break;
					}
					case 10: {
						CurrentText += " ";
						CurrentCarretIndex = CurrentText.Length;
						BorderColor = "Orange";
						break;
					}
					case 19: {
						ValidateDate(CurrentText);
						return;
					}
					default: {
						BorderColor = "Orange";
						break;
					}
				}
			}
			if (_previousText.Length - 1 == _currentText.Length) {
				switch (_currentText.Length) {
					case 2:
					case 5:
					case 10:
					case 13:
					case 16: {
						CurrentText = CurrentText.Remove(_currentText.Length - 1, 1);
						BorderColor = "Orange";
						break;
					}
				}
				CurrentCarretIndex = CurrentText.Length;
			}
			_previousText = _currentText;
			ValidateDate(CurrentText);
		}


		private void ValidateDate(string text) {
			int[] validColumnIndexes = new int[4] { 2, 5, 13, 16 };
			int validSpaceIndex = 10;

			if (text.Length == 19) {
				if (!_validDate.IsMatch(text)) {
					BorderColor = "Red";
				}
				else {
					BorderColor = "Green";
				}
				return;
			}

			for (int i = 0; i < text.Length; i++) {
				if (!char.IsNumber(text[i])) {
					if (text[i] == ':' && validColumnIndexes.Contains(i)) {
						continue;
					}
					if (text[i] == ' ' && i == validSpaceIndex) {
						continue;
					}
					BorderColor = "Red";
					return;
				}
			}

			switch (text.Length) {
				case 3: { //Validate day
					Validate(0, 2, 1, 31);
					return;
				}
				case 6: { //Validate month
					Validate(3, 2, 1, 12);
					return;
				}
				case 11: { //Validate year
					Validate(7, 4, 1000, 9999);
					return;
				}
				case 14: { //Validate hour
					Validate(11, 2, 0, 23);
					return;
				}
				case 17: { //Validate minute
					Validate(14, 2, 0, 59);
					return;
				}
				case 19: { //Validate second
					Validate(17, 2, 0, 59);
					return;
				}
				default: {
					BorderColor = "Red";
					return;
				}
			}

			void Validate(int from ,int length, int min, int max) {
				if (!int.TryParse(text.Substring(from, length), out int number)) {
					BorderColor = "Red";
					return;
				}
				if (number < min || number > max) {
					BorderColor = "Red";
				}
				else {
					BorderColor = "Orange";
				}
			}
		}
	}
}
