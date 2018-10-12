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
			await DebugAsync();
		}

		public async Task DebugAsync() {
			ItemList list = new ItemList(access.GetItems());
			Item i = await list.SelectItemAsync();
			System.Diagnostics.Debug.Print(i.identifier);
		}
	}
}
