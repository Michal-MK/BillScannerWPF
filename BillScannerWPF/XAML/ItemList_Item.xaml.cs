﻿using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ItemList_Item.xaml
	/// </summary>
	public partial class ItemList_Item : UserControl {

		/// <summary>
		/// The parent element this item is a child of
		/// </summary>
		private ItemList parent;

		/// <summary>
		/// The item connected to this UI representation
		/// </summary>
		internal Item asociatedItem { get; }

		/// <summary>
		/// Create a new visual representation of an item in a list
		/// </summary>
		public ItemList_Item(ItemList parent, Item asociatedItem) {
			InitializeComponent();

			ITEMLISTCONTENT_ItemName_Text.Text = asociatedItem.ItemName;
			ITEMLISTCONTENT_ItemValue_Text.Text = string.Format("{0:f2}Kč", asociatedItem.CurrentPriceDecimal);
			//ITEMLISTCONTENT_ItemMeassurement_Text.Text = asociatedItem.UnitOfMeassure.ToString();
			this.parent = parent;
			this.asociatedItem = asociatedItem;
		}
	}
}
