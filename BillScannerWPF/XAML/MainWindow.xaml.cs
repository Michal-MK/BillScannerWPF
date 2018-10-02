using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;

using Igor.TCP;
using BillScannerWPF.Rules;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDisposable {

		public const ushort PORT = 6689;

		public static DatabaseAccess access;
		public static TCPClient client;
		public static TCPServer server;

		internal ImageProcessor imgProcessing;

		public string currentImageSource { get; set; }
		public UIItem currentItemBeingInspected { get; set; }

		internal IRuleset selectedShopRuleset { get; }

		public MainWindow(Shop selectedShop) {
			InitializeComponent();

			if (selectedShop == Shop.NotSelected) {
#if !DEBUG
				throw new WindowInitException(InitExpectionType.SHOP_NOT_SELECTED);
#endif
				Shop[] implemented = new Shop[] { Shop.Albert, Shop.McDonalds, Shop.Lidl };

				Shop selected = implemented[new Random().Next(0, implemented.Length)];
				MessageBox.Show("No Shop selected, automatically selecting " + selected.ToString() + ".", "No Shop Selected!");
				selectedShop = selected;
			}

			access = DatabaseAccess.LoadDatabase(selectedShop);
			selectedShopRuleset = BaseRuleset.GetRuleset(selectedShop);

			MAIN_DatabaseStatus_Text.Text = "Loaded | Intact";
			MAIN_DatabaseStatus_Text.Foreground = System.Windows.Media.Brushes.BlueViolet;

			server = new TCPServer();
			try {
				server.Start(PORT);
				server.OnConnectionEstablished += Server_OnConnectionEstablished;
			}
			catch {
				Debug.WriteLine("Starting in offline mode!");
			}

			MAIN_ServerStatus_Text.Text = "Running";
			MAIN_ServerStatus_Text.Foreground = System.Windows.Media.Brushes.LawnGreen;

			imgProcessing = new ImageProcessor(access, selectedShopRuleset, this);

			MAIN_Analyze_Button.Click += imgProcessing.Analyze;
			MAIN_OpenDatabaseFile_Button.Click += MAIN_OpenDatabaseFile_Click;
			MAIN_Finalize_Button.Click += MAIN_FinalizePurchase_Click;

			MAIN_ClientStatusPreImage_Text.Text = "Client not connected!";
			MAIN_ClientStatusPreImage_Text.Foreground = System.Windows.Media.Brushes.Red;

			MAIN_ClientStatusImage_Image.Visibility = Visibility.Collapsed;
			MAIN_ClientStatusPostImage_Text.Visibility = Visibility.Collapsed;
		}

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<byte[]>(1, imgProcessing.OnImageDataReceived);
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
			MAIN_PhotoPreview_Image.HorizontalAlignment = HorizontalAlignment.Center;
			MAIN_PhotoPreview_Image.VerticalAlignment = VerticalAlignment.Center;
			MAIN_PhotoPreview_Image.Stretch = Stretch.Uniform;
			MAIN_PhotoPreview_Image.Source = image;
		}

		private int attempt = -1;
		internal void SetPrevImage(byte[] imgData) {
			File.WriteAllBytes(WPFHelper.dataPath + "current" + ++attempt + ".jpg", imgData);
			currentImageSource = WPFHelper.dataPath + "current" + attempt + ".jpg";
			SetPrevImage(new Uri(currentImageSource));
		}

		//Open Selected image in default image viewer
		private void MAIN_PhotoPreview_RightClick(object sender, MouseButtonEventArgs e) {
			if (string.IsNullOrEmpty(currentImageSource)) {
				return;
			}
			Process p = new Process();
			p.StartInfo = new ProcessStartInfo(currentImageSource);
			p.Start();
		}

		private void MAIN_OpenDatabaseFile_Click(object sender, RoutedEventArgs e) {
			Process p = new Process();
			ProcessStartInfo info = new ProcessStartInfo(access.itemDatabaseFile.FullName);
			p.StartInfo = info;
			p.Start();
		}

		private void MAIN_FinalizePurchase_Click(object sender, RoutedEventArgs e) {
			ImageProcessor pr = WPFHelper.GetMainWindow().imgProcessing;
			Shopping s = new Shopping(pr.currentParsingResult.meta.purchasedAt, pr.uiItemsMatched);
			s.FinalizePurchase();
			MAIN_Finalize_Button.IsEnabled = false;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				imgProcessing.Dispose();
				disposedValue = true;
			}
		}

		~MainWindow() {
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
