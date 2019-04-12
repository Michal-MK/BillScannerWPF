﻿using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class MatchRatingToImageConverter : BaseValueConverter {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is MatchRating rating) {
				return new BitmapImage(new Uri(WPFHelper.imageRatingResourcesPath + rating.ToString() + ".png", UriKind.Absolute));
			}
			throw new NotImplementedException($"Only conversions from {nameof(MatchRating)} are allowed!");
		}
	}
}
