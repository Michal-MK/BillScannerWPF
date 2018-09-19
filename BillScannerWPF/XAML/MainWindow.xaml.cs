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

		internal Rules.IRuleset mainShopParseRuleset { get; }

		public MainWindow() {
			InitializeComponent();

			if (SetupWindow.selectedShop == Shop.NotSelected) {
				SetupWindow.selectedShop = Shop.McDonalds;
			} //TODO force selection

			access = DatabaseAccess.LoadDatabase(SetupWindow.selectedShop);
			mainShopParseRuleset = GetRuleset(SetupWindow.selectedShop);

			server = new TCPServer();
			try {
				server.Start(PORT);
			}
			catch {
				Console.WriteLine("Starting in offline mode!");
			}
			imgProcessing = new ImageProcessor(server, access, mainShopParseRuleset, this);

			MAIN_Analyze_Button.Click += imgProcessing.Analyze;
		}

		private Rules.IRuleset GetRuleset(Shop selectedShop) {
			switch (selectedShop) {
				case Shop.Lidl: {
					return new Rules.LidlRuleset();
				}
				case Shop.McDonalds: {
					return new Rules.McDonaldsRuleset();
				}
				case Shop.Albert: {
					return new Rules.AlbertRuleset();
				}
				default: {
					throw new NotImplementedException();
				}
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

		private void INFO_RegisterItem_Click(object sender, RoutedEventArgs e) {
			//Get stuff from input fields
			string modifiedName = INFO_MainName_Text.Text;

			try {
				access.RegisterItemFromUI(currentItemBeingInspected, modifiedName);
				((Button)sender).IsEnabled = false;
				ImageProcessor.instance.uiItemsUnknown.Remove(currentItemBeingInspected);
				ImageProcessor.instance.uiItemsMatched.Add(currentItemBeingInspected);
				currentItemBeingInspected.ProductMatchedSuccess();
				Console.WriteLine("Item Parsed successfully");
				currentItemBeingInspected.asociatedItem.isRegistered = true;
				MAIN_ItemInfoOverlay_Grid.Visibility = Visibility.Hidden;
			}
			catch(Exception ex) {
				throw new Exception(ex.Message);
			}
		}

		private void INFO_Back_Click(object sender, RoutedEventArgs e) {
			MAIN_ItemInfoOverlay_Grid.Visibility = Visibility.Hidden;
			currentItemBeingInspected = null;
		}
	}
}
