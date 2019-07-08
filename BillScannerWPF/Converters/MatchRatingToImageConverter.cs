using System;
using System.Globalization;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class MatchRatingToImageConverter : BaseValueConverter {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is MatchRating rating) {
				return WPFHelper.ImageRatingResourcesPath + rating.ToString() + ".png";
			}
			throw new NotImplementedException($"Only conversions from {nameof(MatchRating)} are allowed!");
		}
	}
}
