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
using System.Diagnostics;

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

		public string currentImageSource { get; set; }
		public UIItem currentItemBeingInspected { get; set; }

		public MainWindow() {
			InitializeComponent();

			access = DatabaseAccess.LoadDatabase(SetupWindow.selectedShop); //TODO
			//client = new TCPClient("192.168.0.133", PORT);
			//client.Connect();
			//client.getConnection.dataIDs.DefineCustomDataTypeForID<byte[]>(1, OnImageDataReceived);
			//imgProcessing = new ImageProcessor(client, access);

			server = new TCPServer();
			server.Start(PORT);
			imgProcessing = new ImageProcessor(server, access, GetRuleset(SetupWindow.selectedShop));

			IH_freeBtn.Click += imgProcessing.Analyze;
		}

		private Rules.IRuleset GetRuleset(Shop selectedShop) {
			switch (selectedShop) {
				case Shop.Lidl: {
					return new Rules.LidlRuleset();
				}

				case Shop.McDonalds: {
					return new Rules.McDonaldsRuleset();
				}
				default:
				throw new NotImplementedException();
			}
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



		//Open Selected image in default image viewer
		private void IH_previewImg_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			if (string.IsNullOrEmpty(currentImageSource)) {
				return;
			}
			Process p = new Process();
			p.StartInfo = new ProcessStartInfo(currentImageSource);
			p.Start();
		}

		private void info_RegisterItem_Button(object sender, RoutedEventArgs e) {
			access.RegsiterItem(currentItemBeingInspected);
			IH_unknownProducts.Children.RemoveAt(currentItemBeingInspected.index);
			IH_matchedProducts.Children.Add(currentItemBeingInspected);
		}

		private void info_Back_Button(object sender, RoutedEventArgs e) {
			itemInfoOverlay.Visibility = Visibility.Hidden;
			currentItemBeingInspected = null;
		}

		private void newDefinition_Back_Button(object sender, RoutedEventArgs e) {
			itemInfoOverlay.Visibility = Visibility.Hidden;
			currentItemBeingInspected = null;
		}
		private void newDefinition_RegisterItem_Button(object sender, RoutedEventArgs e) {

			itemInfoOverlay.Visibility = Visibility.Hidden;
			currentItemBeingInspected = null;
		}
	}
}
