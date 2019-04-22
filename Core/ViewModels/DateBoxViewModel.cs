using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Igor.BillScanner.Core {
	public class DateBoxViewModel : BaseViewModel {

		#region Backing Fields

		private string _borderColor = "Red";
		private bool _isValidInput;
		private string _currentText;
		private readonly Regex _validDate = new Regex(@"(\d{2}):(\d{2}):(\d{4})\s(\d{2}):(\d{2}):(\d{2})");
		private int _currentCarretIndex;
		private string _previousText = "";

		#endregion

		public string CurrentText { get => _currentText; set { _currentText = value; ValidateText(); Notify(nameof(CurrentText)); } }
		public bool IsValidInput { get => _isValidInput; set { _isValidInput = value; Notify(nameof(IsValidInput)); } }
		public string BorderColor { get => _borderColor; set { _borderColor = value; Notify(nameof(BorderColor)); } }
		public dynamic Control { get; set; }

		public bool TryGetDate(out DateTime time) {
			if (!_validDate.IsMatch(CurrentText)) {
				time = DateTime.MinValue;
				return false;
			}
			time = DateTime.ParseExact(CurrentText, "dd:MM:yyyy dd:mm:ss", CultureInfo.GetCultureInfo("cs"));
			return true;
		}

		private void ValidateText() {
			if (Control.DateBoxInput.CaretIndex != _currentText.Length) {
				ValidateDate(_currentText);
				return;
			}

			if (_previousText.Length + 1 == _currentText.Length) {
				switch (_currentText.Length) {
					case 2:
					case 5:
					case 13:
					case 16: {
						CurrentText += ":";
						Control.DateBoxInput.CaretIndex = _currentText.Length;
						break;
					}
					case 10: {
						CurrentText += " ";
						Control.DateBoxInput.CaretIndex = _currentText.Length;
						break;
					}
				}
				//ValidateDate(_currentText);
			}
			if (_previousText.Length - 1 == _currentText.Length) {
				switch (_currentText.Length) {
					case 2:
					case 5:
					case 10:
					case 13:
					case 16: {
						CurrentText = CurrentText.Remove(_currentText.Length - 1, 1);
						break;
					}
				}
				Control.DateBoxInput.CaretIndex = _currentText.Length;
				ValidateDate(_currentText);
			}
			_previousText = _currentText;
		}


		private void ValidateDate(string text) {
			int[] validColumnIndexes = new int[4] { 2, 5, 13, 16 };
			int validSpaceIndex = 10;

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
					int number = int.Parse(text.Substring(0, 2));
					if (number < 1 || number > 31) {
						BorderColor = "Red";
					}
					else {
						BorderColor = "Orange";
					}
					return;
				}
				case 6: { //Validate month
					int number = int.Parse(text.Substring(3, 2));
					if (number < 1 || number > 12) {
						BorderColor = "Red";
					}
					else {
						BorderColor = "Orange";
					}
					return;
				}
				case 14: { //Validate hour
					int number = int.Parse(text.Substring(11, 2));
					if (number < 0 || number > 23) {
						BorderColor = "Red";
					}
					else {
						BorderColor = "Orange";
					}
					return;
				}
				case 17: { //Validate minute
					int number = int.Parse(text.Substring(14, 2));
					if (number < 0 || number > 60) {
						BorderColor = "Red";
					}
					else {
						BorderColor = "Orange";
					}
					return;
				}
				case 19: { //Validate second
					int number = int.Parse(text.Substring(17, 2));
					if (number < 0 || number > 60) {
						BorderColor = "Red";
					}
					else {
						BorderColor = "Green";
					}
					return;
				}
				default: {
					BorderColor = "Red";
					return;
				}
			}
		}
	}
}
