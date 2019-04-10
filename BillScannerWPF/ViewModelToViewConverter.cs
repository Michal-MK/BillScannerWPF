using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class ViewModelToViewConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is ObservableCollection<ItemList_ItemViewModel> models) {
				ObservableCollection<ItemList_Item> ret = new ObservableCollection<ItemList_Item>();
				foreach (ItemList_ItemViewModel item in models) {
					ret.Add(new ItemList_Item() { DataContext = item });
				}
				return ret;
			}
			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
