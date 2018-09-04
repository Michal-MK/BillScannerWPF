using Igor.TCP;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Tesseract;
using System.Drawing;
using System.Threading;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public const ushort PORT = 6689;

		public static DatabaseAccess access;
		public static TCPClient client;
		public static TCPServer server;

		public string currentImageSource;

		public MainWindow() {
			InitializeComponent();
			IH_freeBtn.Click += Analyze;
			access = DatabaseAccess.LoadDatabase(Shop.Albert);
			//client = new TCPClient("192.168.0.133", PORT);
			server = new TCPServer();
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
			server.Start(PORT);
			server.OnConnectionEstablished += Server_OnConnectionEstablished;
			//client.Connect();
			//client.getConnection.dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
		}

		private TesseractEngine engine;

		private void Analyze(object sender, RoutedEventArgs e) {
			IH_matchedProducts.Children.Add(new TextBlock() { Text = "Processing image..." });
			Thread t = new Thread(new ThreadStart(delegate () {
				using (Tesseract.Page p = engine.Process((Bitmap)Bitmap.FromFile(currentImageSource), PageSegMode.Auto)) {
					string[] text = p.GetText().Split('\n');
					foreach (string s in text) {
						if (string.IsNullOrWhiteSpace(s)) {
							continue;
						}
						Dispatcher.Invoke(delegate () {
							MatchedProduct matchedP = new MatchedProduct(s);
							IH_matchedProducts.Children.Add(matchedP);
						});
					}
				}
			}));
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
		}

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
		}

		private void OnImageDataReceived(byte[] imageData, byte sender) {
			this.Dispatcher.Invoke(delegate () {
				SetPrevImage(imageData);
			});
		}

		private void PreviewImgMouse(object sender, MouseButtonEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "png";
			dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			if (dialog.ShowDialog() == true) {
				SetPrevImage(new Uri(dialog.FileName));
			}
		}

		private void SetPrevImage(Uri imgUri) {
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.UriSource = imgUri;
			image.EndInit();
			currentImageSource = imgUri.AbsolutePath;
			IH_previewImg.HorizontalAlignment = HorizontalAlignment.Center;
			IH_previewImg.VerticalAlignment = VerticalAlignment.Center;
			IH_previewImg.Stretch = Stretch.Uniform;
			IH_previewImg.Source = image;
		}
		private int attempt = -1;
		private void SetPrevImage(byte[] imgData) {
			File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + "BillScanner" + System.IO.Path.DirectorySeparatorChar + "current" + ++attempt + ".jpg", imgData);
			currentImageSource = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + "BillScanner" + System.IO.Path.DirectorySeparatorChar + "current" + attempt + ".jpg";
			SetPrevImage(new Uri(currentImageSource));
		}
	}
}
