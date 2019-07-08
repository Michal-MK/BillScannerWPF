using System;
using System.Globalization;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class LoadedShopConverter : BaseValueConverter {

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) return null;
			if (!(value is Shop)) throw new NotSupportedException("Only strings allowed");

			Shop val = (Shop)value;

			if (val.ToString() == "None") {
				return "Select a Shop";
			}
			else {
				return $"Load {value.ToString()}!";
			}
		}
	}
}
