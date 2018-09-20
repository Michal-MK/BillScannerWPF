using Igor.TCP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tesseract;

namespace BillScannerWPF {
	internal class ImageProcessor : IDisposable {

		private readonly TCPServer server;
		private readonly DatabaseAccess access;

		public static ImageProcessor instance { get; private set; }

		private readonly TesseractEngine engine;
		private readonly Rules.IRuleset ruleset;

		internal ObservableCollection<UIItem> uiItemsMatched = new ObservableCollection<UIItem>();
		internal ObservableCollection<UIItem> uiItemsUnknown = new ObservableCollection<UIItem>();

		internal ImageProcessor(TCPServer server, DatabaseAccess access, Rules.IRuleset ruleset, MainWindow main) {
			instance = this;
			this.server = server;
			this.access = access;
			server.OnConnectionEstablished += Server_OnConnectionEstablished;
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
			this.ruleset = ruleset;

			main.MAIN_MatchedItems_Stack.ItemsSource = uiItemsMatched;
			main.MAIN_UnknownProducts_Stack.ItemsSource = uiItemsUnknown;
		}

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
		}

		private void OnImageDataReceived(byte[] imageData, byte sender) {
			Application.Current.Dispatcher.Invoke(() => {
				MainWindow w = WPFHelper.GetCurrentMainWindow();
				w.SetPrevImage(imageData);
			});
		}

		internal async void Analyze(object sender, RoutedEventArgs e) {
			if (WPFHelper.GetCurrentMainWindow().currentImageSource == null) {
				return;
			}
			((Button)sender).IsEnabled = false;
			uiItemsMatched.Clear();
			uiItemsUnknown.Clear();

			using (Tesseract.Page p = engine.Process((Bitmap)Bitmap.FromFile(WPFHelper.GetCurrentMainWindow().currentImageSource), PageSegMode.Auto)) {
				StringParser instance = new StringParser(ruleset);
				ParsingResult result = null;
				string[] split = p.GetText().Split('\n');
				string[] ready = split.Where((s) => { return !string.IsNullOrWhiteSpace(s); }).ToArray();
				try {
					result = await Task.Run(() => { return instance.Parse(ready); });
				}
				catch (ParsingEntryNotFoundException) {
					string[] modified = new string[ready.Length + 1];
					modified[0] = ruleset.startMarkers[0];
					ready.CopyTo(modified, 1);
					result = await Task.Run(() => { return instance.Parse(modified); });
				}
				ConstructUI(result.parsed, uiItemsMatched);
				ConstructUI(result.unknown, uiItemsUnknown);
			}
			((Button)sender).IsEnabled = true;
		}

		private void ConstructUI(ObservableCollection<UItemCreationInfo> from, ObservableCollection<UIItem> destination) {
			foreach (UItemCreationInfo info in from) {
				UIItem item = new UIItem(info.item, info.index, info.quality);
				item.asociatedItem.isRegistered = info.isRegistered;
				destination.Add(item);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				engine.Dispose();
				disposedValue = true;
			}
		}

		~ImageProcessor() {
		   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		   Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
