using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for ItemList_Item.xaml
	/// </summary>
	public partial class ItemList_Item : UserControl {
		internal bool isSelected { get; private set; }
		private ItemList parent;
		internal Item asociatedItem { get; }

		public ItemList_Item(ItemList parent, Item asociatedItem) {
			InitializeComponent();

			ITEMLISTCONTENT_ItemName_Text.Text = asociatedItem.userFriendlyName;
			ITEMLISTCONTENT_ItemValue_Text.Text = asociatedItem.currentPrice.ToString();
			ITEMLISTCONTENT_ItemMeassurement_Text.Text = asociatedItem.unitOfMeassure.ToString();
			this.parent = parent;
			this.asociatedItem = asociatedItem;
		}
	}
}
