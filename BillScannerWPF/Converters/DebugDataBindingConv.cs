using System;
using System.Diagnostics;
using System.Globalization;

namespace Igor.BillScanner.WPF.UI {
	public class DebugDataBindingConv : BaseValueConverter {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Debugger.Break();
			return value;
		}
	}
}