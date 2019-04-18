using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	public partial class ItemList : UserControl {

		public ItemList() {
			InitializeComponent();
		}

		public IEnumerable<object> Items {
			get { return (DataContext as ItemListViewModel).AllItems; }
			set { (DataContext as ItemListViewModel).AddItems(value as ObservableCollection<ItemList_ItemViewModel>); }
		}

		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register(nameof(Items), typeof(IEnumerable<object>), typeof(ItemList)
				, new PropertyMetadata(OnChange));

		private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			(d as ItemList).Items =(ObservableCollection<ItemList_ItemViewModel>)e.NewValue;
		}
	}
}
