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
			IH_newEntryBtn.Click += ShowRegisterWindow;
		}


		private string selectedProduct;
		private TextBlock previousSelected = null;
		private void ShowRegisterWindow(object sender, RoutedEventArgs e) {
			if(IH_unknownProducts.Children.Count == 0){
				return;
			}
			newItemDefinition.Visibility = Visibility.Visible;
			foreach (UIItem item in IH_unknownProducts.Children) {
				TextBlock block = new TextBlock();
				block.Text = item.IH_originalName.Text;
				block.MouseLeftButtonDown += Block_MouseLeftButtonDown;
				newDefinition_selectProduct_Stack.Children.Add(block);
			}
		}

		private void Block_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			((TextBlock)e.Source).Background = System.Windows.Media.Brushes.Green;
			if (previousSelected != null) {
				previousSelected.Background = System.Windows.Media.Brushes.White;
			}
			previousSelected = ((TextBlock)e.Source);
			selectedProduct = ((TextBlock)e.Source).Text;
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
			access.RegisterItem(currentItemBeingInspected);
			IH_unknownProducts.Children.RemoveAt(currentItemBeingInspected.index);
		}

		private void info_Back_Button(object sender, RoutedEventArgs e) {
			itemInfoOverlay.Visibility = Visibility.Hidden;
			currentItemBeingInspected = null;
		}

		private void newDefinition_Back_Button(object sender, RoutedEventArgs e) {
			newItemDefinition.Visibility = Visibility.Hidden;
			currentItemBeingInspected = null;
		}
		private void newDefinition_RegisterItem_Button(object sender, RoutedEventArgs e) {
			string new_mainName = newDefinition_mainName.Text;
			if(decimal.TryParse(newDefinition_currentValue.Text, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal new_result)) {
				newItemDefinition.Visibility = Visibility.Hidden;
				currentItemBeingInspected = null;
				access.RegisterItem(new_mainName, new_result, new string[] { previousSelected.Text });
			}
		}
	}
}
