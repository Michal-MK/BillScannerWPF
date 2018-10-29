using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BillScannerWPF {
	public partial class MainWindow {


		private async void DebugDelay() {
			await Task.Run(() => { Thread.Sleep(1000); });
			//await DebugListAsync();
			await DebugManualResolveAsync();
		}

		public async Task DebugListAsync() {
			ItemList list = new ItemList(access.GetItems());
			Item i = await list.SelectItemAsync();
			System.Diagnostics.Debug.Print(i.identifier);
		}

		public async Task DebugManualResolveAsync() {
			ManualResolveChoice choice = new ManualResolveChoice("Some generic error"
				, Choices.FindExistingItemFromList, Choices.ManuallyEnterQuantity);
			await choice.SelectChoiceAsync();
		}
	}
}
