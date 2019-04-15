using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	public partial class MainWindow : Window {

		private async void DebugDelay() {
			await Task.Run(() => { Thread.Sleep(500); });
			//await DebugListAsync();
			///await DebugManualResolveAsync();
			//	await DebugDateBoxAsync();
		}

		public async Task DebugListAsync() {
			ItemList list = new ItemList();
            MainWindow w = App.Current.MainWindow as MainWindow;

            w.MAIN_Grid.Children.Add(list);
			(list.DataContext as ItemListViewModel).AddItems(DatabaseAccess.access.GetItems());
            List<ItemList_ItemViewModel> i = (await (list.DataContext as ItemListViewModel).SelectItemAsync());
            w.MAIN_Grid.Children.Remove(list);
			if (i != null) {
				System.Diagnostics.Debug.Print(i[0].Item.ID.ToString());
			}
		}

		public async Task DebugManualResolveAsync() {
			ManualResolveChoice choice = new ManualResolveChoice("Some generic error"
				, Choices.FindExistingItemFromList, Choices.ManuallyEnterDate);
			await choice.SelectChoiceAsync();
		}

		public async Task DebugDateBoxAsync() {
			DateBox db = new DateBox();
			await db.FinalizeDateAsync();
		}
	}
}
