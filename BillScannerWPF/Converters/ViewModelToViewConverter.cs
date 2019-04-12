using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public class ViewModelToViewConverter : BaseValueConverter {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is ObservableCollection<ItemList_ItemViewModel> models) {
				ObservableCollection<ItemList_Item> ret = new ObservableCollection<ItemList_Item>();
				foreach (ItemList_ItemViewModel item in models) {
					ret.Add(new ItemList_Item() { DataContext = item });
				}
				return ret;
			}
			throw new NotImplementedException();
		}
	}
}
