using Igor.BillScanner.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Igor.BillScanner.WPF.UI {
	public partial class MainWindow {


		private async void DebugDelay() {
			await Task.Run(() => { Thread.Sleep(1000); });
			//await DebugListAsync();
			///await DebugManualResolveAsync();
			//while (true) {
			//	await DebugDateBoxAsync();
			//}
		}

		public async Task DebugListAsync() {
			ItemList list = new ItemList();
			list.AddItems(DatabaseAccess.access.GetItems());
			Item i = await list.SelectItemAsync();
			System.Diagnostics.Debug.Print(i.ID.ToString());
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
