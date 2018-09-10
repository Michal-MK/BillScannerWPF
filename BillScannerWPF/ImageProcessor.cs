using Igor.TCP;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tesseract;

namespace BillScannerWPF {
	internal class ImageProcessor {

		private TCPServer server;
		private TCPClient client;

		private DatabaseAccess access;

		public static ImageProcessor instance { get; private set; }

		private TesseractEngine engine;

		private Rules.IRuleset ruleset;


		public ImageProcessor(TCPClient client, DatabaseAccess access, Rules.IRuleset ruleset) {
			instance = this;
			this.client = client;
			this.access = access;

			client.Connect();
			client.getConnection.dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
			this.ruleset = ruleset;
		}

		public ImageProcessor(TCPServer server, DatabaseAccess access, Rules.IRuleset ruleset) {
			instance = this;
			this.server = server;
			this.access = access;
			server.OnConnectionEstablished += Server_OnConnectionEstablished;
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
			this.ruleset = ruleset;
		}

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
		}

		private void OnImageDataReceived(byte[] imageData, byte sender) {
			WPFHelper.GetCurrentMainWindow().Dispatcher.Invoke(delegate () {
				WPFHelper.GetCurrentMainWindow().SetPrevImage(imageData);
			});
		}

		internal async void Analyze(object sender, RoutedEventArgs e) {
			MainWindow main = WPFHelper.GetCurrentMainWindow();
			main.IH_matchedProducts.Children.Clear();
			main.IH_matchedProducts.Children.Add(new TextBlock() { Text = "Processing image..." });
			using (Tesseract.Page p = engine.Process((Bitmap)Bitmap.FromFile(main.currentImageSource), PageSegMode.Auto)) {
				StringParser instance = new StringParser(ruleset);
				ParsingResult result = await Task.Run(() => { return instance.Parse(p.GetText()); });
				for (int i = 0; i < result.unknown.Length; i++) {
					main.Dispatcher.Invoke(delegate () {
						UIItem uiItem = new UIItem(result.unknown[i].item, result.unknown[i].index, result.unknown[i].quality);

						main.IH_unknownProducts.Children.Add(uiItem);
						uiItem.SetMatchRatingImage();
					});
				}

				for (int i = 0; i < result.parsed.Length; i++) {
					main.Dispatcher.Invoke(delegate () {
						UIItem uiItem = new UIItem(result.parsed[i].item, result.parsed[i].index, result.parsed[i].quality);
						main.IH_matchedProducts.Children.Add(uiItem);
						uiItem.SetMatchRatingImage();
					});
				}
			}
		}
	}
}
