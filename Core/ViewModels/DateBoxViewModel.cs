using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Igor.BillScanner.Core {
	public class DateBoxViewModel : BaseViewModel {
		#region Backing Fields
		private string _borderColor = "Red";
		private bool _isValidInput;
		private string _currentText;
		private Regex _validDate = new Regex(@"(\d{2}):(\d{2}):(\d{4})\s(\d{2}):(\d{2}):(\d{2})");

		#endregion
		public string CurrentText { get => _currentText; set { _currentText = value; Notify(nameof(CurrentText)); } }
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
	}
}
