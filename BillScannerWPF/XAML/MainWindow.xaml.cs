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

		internal ImageProcessor imgProcessing;

		public string currentImageSource;



		public MainWindow() {
			InitializeComponent();

			access = DatabaseAccess.LoadDatabase(Shop.Albert); //TODO
			//client = new TCPClient("192.168.0.133", PORT);
			//client.Connect();
			//client.getConnection.dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
			//imgProcessing = new ImageProcessor(client, access);

			server = new TCPServer();
			server.Start(PORT);
			imgProcessing = new ImageProcessor(server, access);

			IH_freeBtn.Click += imgProcessing.Analyze;
		}


		internal void PreviewImgMouse(object sender, MouseButtonEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "png";
			dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			if (dialog.ShowDialog() == true) {
				SetPrevImage(new Uri(dialog.FileName));
			}
		}

		internal void SetPrevImage(Uri imgUri) {
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
		internal void SetPrevImage(byte[] imgData) {
			File.WriteAllBytes(WPFHelper.dataPath + "current" + ++attempt + ".jpg", imgData);
			currentImageSource = WPFHelper.dataPath + "current" + attempt + ".jpg";
			SetPrevImage(new Uri(currentImageSource));
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			itemInfoOverlay.Visibility = Visibility.Hidden;
		}
	}
}
