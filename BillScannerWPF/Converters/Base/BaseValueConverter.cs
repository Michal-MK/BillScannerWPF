using System;
using System.Globalization;
using System.Windows.Data;

namespace Igor.BillScanner.WPF.UI {
	public abstract class BaseValueConverter : IValueConverter {
		public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
