using System;
using System.Globalization;
using System.Windows;

namespace Igor.BillScanner.WPF.UI {
	public class BoolToVisibilityCollapseConverter : BaseValueConverter {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value is bool state) {
				return state ? Visibility.Visible : Visibility.Collapsed;
			}
			throw new NotImplementedException();
		}
	}
}
