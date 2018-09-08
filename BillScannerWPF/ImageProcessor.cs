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


		public ImageProcessor(TCPClient client, DatabaseAccess access) {
			instance = this;
			this.client = client;
			this.access = access;

			client.Connect();
			client.getConnection.dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");

		}
		public ImageProcessor(TCPServer server, DatabaseAccess access) {
			instance = this;
			this.server = server;
			this.access = access;
			server.OnConnectionEstablished += Server_OnConnectionEstablished;
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
		}

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
		}

		private void OnImageDataReceived(byte[] imageData, byte sender) {
			WPFHelper.GetMainWindow().Dispatcher.Invoke(delegate () {
				WPFHelper.GetMainWindow().SetPrevImage(imageData);
			});
		}

		internal void Analyze(object sender, RoutedEventArgs e) {
			MainWindow main = WPFHelper.GetMainWindow();
			main.IH_matchedProducts.Children.Clear();
			main.IH_matchedProducts.Children.Add(new TextBlock() { Text = "Processing image..." });
			Thread t = new Thread(new ThreadStart(delegate () {
				using (Tesseract.Page p = engine.Process((Bitmap)Bitmap.FromFile(main.currentImageSource), PageSegMode.Auto)) {
					string[] text = p.GetText().Split('\n');
					foreach (string s in text) {
						if (string.IsNullOrWhiteSpace(s)) {
							continue;
						}
						try {
							Item i = access.GetItem(s);
							main.Dispatcher.Invoke(delegate () {
								MatchedProduct matchedP = new MatchedProduct(s);
								main.IH_matchedProducts.Children.Add(matchedP);
							});
						}
						catch (ItemNotDefinedException ex) {
							Console.WriteLine(ex.Message);
							main.Dispatcher.Invoke(delegate () {
								MatchedProduct matchedP = new MatchedProduct(s);
								main.IH_unknownProducts.Children.Add(matchedP);
							});
						}
					}
				}
			}));
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
		}
	}
}
