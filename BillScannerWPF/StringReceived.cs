using Igor.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillScannerWPF {
	class StringReceived {
		private void GetConnection_OnStringReceived(object sender, PacketReceivedEventArgs<string> e) {
			List<Database.Item> items = new List<Database.Item>();
			System.Console.WriteLine("Parsed text here!");
			string[] split = e.data.Split('|');
			foreach (string s in split) {
				int lowestD = int.MaxValue;
				Database.Item lowestItem;
				foreach (Database.Item product in MainWindow.access.GetItems()) {
					int distance = WordSimilarity.Compute(s, product.mainName);
					if (distance < lowestD) {
						lowestD = distance;
						lowestItem = product;
					}
					if (distance == 0) {
						//Found exact match
						items.Add(product);
						break;
					}
				}
				if (lowestD <= 1) {
					//Found a almost match
				}
				else {
					foreach (Database.Item product in MainWindow.access.GetItems()) {
						bool matched = false;
						foreach (string ss in product.ocrNames) {
							int dist = WordSimilarity.Compute(s, ss);
							if (dist < lowestD) {
								lowestD = dist;
								lowestItem = product;
							}
							if (dist == 0) {
								//Found match with f*** up word
								items.Add(product);
								matched = true;
								break;
							}
						}
						if (matched) {
							break;
						}
					}
					if (lowestD <= 1) {
						//Found a almost match
					}
				}
			}
			//List all matched, List ~4 closest to unmatched, offer new picture, define new item
		}

	}
}
