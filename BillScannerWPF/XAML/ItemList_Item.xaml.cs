using System.Windows.Controls;

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
			ITEMLISTCONTENT_ItemValue_Text.Text = string.Format("{0:f2}Kč", asociatedItem.currentPrice);
			ITEMLISTCONTENT_ItemMeassurement_Text.Text = asociatedItem.unitOfMeassure.ToString();
			this.parent = parent;
			this.asociatedItem = asociatedItem;
		}
	}
}
