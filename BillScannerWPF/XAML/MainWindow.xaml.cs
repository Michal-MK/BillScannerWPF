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
using System.Globalization;
using System.Collections.ObjectModel;
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

		internal IRuleset mainShopParseRuleset { get; }

		public MainWindow() {
			InitializeComponent();

			if (SetupWindow.selectedShop == Shop.NotSelected) {
#if !DEBUG
				throw new WindowInitException(InitExpectionType.SHOP_NOT_SELECTED);
#endif
				Shop[] implemented = new Shop[] { Shop.Albert, Shop.McDonalds, Shop.Lidl };

				Shop selected = implemented[new Random().Next(0, implemented.Length)];
				MessageBox.Show("No Shop selected, automatically selecting " + selected.ToString() + ".", "No Shop Selected!");
				SetupWindow.selectedShop = selected;
			}

			access = DatabaseAccess.LoadDatabase(SetupWindow.selectedShop);
			mainShopParseRuleset = BaseRuleset.GetRuleset(SetupWindow.selectedShop);

			server = new TCPServer();
			try {
				server.Start(PORT);
			}
			catch {
				Debug.WriteLine("Starting in offline mode!");
			}
			imgProcessing = new ImageProcessor(server, access, mainShopParseRuleset, this);

			MAIN_Analyze_Button.Click += imgProcessing.Analyze;
			MAIN_OpenDatabaseFile_Button.Click += MAIN_OpenDatabaseFile_Click;
			MAIN_Finalize_Button.Click += MAIN_FinalizePurchase_Click;
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
