using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BillScannerWPF {
	[Serializable]
	public class Database {

		protected Dictionary<char, Dictionary<string, Dictionary<string, Item>>> items;


		protected DirectoryInfo appData;
		protected FileInfo databaseFile;


		internal Database() {
			appData = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			databaseFile = new FileInfo(appData.FullName + Path.DirectorySeparatorChar + "BillScanner" + Path.DirectorySeparatorChar + "database.datab");
			if (!InitFiles()) {
				using (FileStream fs = File.OpenRead(databaseFile.FullName)) {
					BinaryFormatter bf = new BinaryFormatter();
					items = (Dictionary<char, Dictionary<string, Dictionary<string, Item>>>)bf.Deserialize(fs);
				}
			}
		}

		private bool InitFiles() {
			DirectoryInfo billScanner = new DirectoryInfo(appData.FullName + Path.DirectorySeparatorChar + "BillScanner");
			if (!billScanner.Exists) {
				items = new Dictionary<char, Dictionary<string, Dictionary<string, Item>>>();
				Directory.CreateDirectory(appData.FullName + Path.DirectorySeparatorChar + "BillScanner");
				using (FileStream fs = File.Create(databaseFile.FullName)) {
					BinaryFormatter bf = new BinaryFormatter();
					bf.Serialize(fs, items);
					return true;
				}
			}
			return false;
		}

		[Serializable]
		public class Item {
			public Item(string mainName, decimal currentPrice) {
				this.mainName = mainName;
				this.currentPrice = currentPrice;
				ocrNames = new string[1] {
					mainName
				};

				pricesInThePast = new decimal[1] {
					currentPrice
				};
			}

			public string mainName { get; }
			public string[] ocrNames { get; }
			public decimal currentPrice { get; }
			public decimal[] pricesInThePast { get; }
		}
	}

	public enum Shop {
		Lidl,
		Albert,
		Penny,
		Billa
	}
}