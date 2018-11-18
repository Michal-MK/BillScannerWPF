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
using BillScannerCore;
using BillScannerStartup;
using System.Windows.Controls;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDisposable {

		/// <summary>
		/// The constant port the server is listening on for incoming phone connections
		/// </summary>
		public const ushort START_PORT = 6689;

		public const ushort PORT_RANGE = 10;

		/// <summary>
		/// Static reference to the server
		/// </summary>
		public static TCPServer server;

		/// <summary>
		/// Image processing class that does the scanning and subsequent parsing of the image
		/// </summary>
		internal ImageProcessor imgProcessing;

		/// <summary>
		/// Absolute path to the image currently being displayed in the image preview control
		/// </summary>
		public string currentImageSource { get; set; }

		/// <summary>
		/// The container that displays currently previewed item
		/// </summary>
		public UIItem currentItemBeingInspected { get; set; }

		/// <summary>
		/// The rule-set selected at the launch of the application
		/// </summary>
		internal IRuleset selectedShopRuleset { get; }

		/// <summary>
		/// Create a default Albert window (Debug)
		/// </summary>
		public MainWindow() : this(Shop.Albert) { }

		/// <summary>
		/// Create a new Main window and prepare it for the selected shop type
		/// </summary>
		/// <param name="selectedShop">The shop to load data for</param>
		public MainWindow(Shop selectedShop) {
			InitializeComponent();

			DatabaseAccess.LoadDatabase(selectedShop);
			selectedShopRuleset = BaseRuleset.GetRuleset(selectedShop);

			MAIN_DatabaseStatus_Text.Text = "Loaded | Intact";
			MAIN_DatabaseStatus_Text.Foreground = Brushes.BlueViolet;

			MAIN_CurrentLoadedShop_Text.Text = selectedShop.ToString();

			MAIN_CurrentLoadedShop_Text.MouseLeftButtonDown += OnShopClicked;

			this.Closed += OnMainWindowClose;
			if (ServerStateManager.isHoldingInstance) {
				server = ServerStateManager.RestoreServerInstance();
				server.OnConnectionEstablished += Server_OnConnectionEstablished;
				server.OnClientDisconnected += Server_OnClientDisconnected;
			}
			else {
				server = new TCPServer(new ServerConfiguration());
	
				for (ushort i = 0; i < PORT_RANGE; i++) {
					try {
						server.Start(
							Helper.GetActiveIPv4Address()
							//"192.168.137.1"
							, (ushort)(START_PORT + i));
						server.OnConnectionEstablished += Server_OnConnectionEstablished;
						server.OnClientDisconnected += Server_OnClientDisconnected;
						break;
					}
					catch { }
				}
			}

			//if (!server) {
			//	MessageBox.Show("Unable to start server at " + Helper.GetActiveIPv4Address() + " " + START_PORT + "\n" +
			//					"Either the port is already taken of you are not connected to the Internet!"
			//					, "Server Off-line!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			//}

			MAIN_ServerStatus_Text.Text = "Running";
			MAIN_ServerStatus_Text.Foreground = Brushes.LawnGreen;

			imgProcessing = new ImageProcessor(selectedShopRuleset, this);

			MAIN_Analyze_Button.Click += imgProcessing.Analyze;
			MAIN_OpenDatabaseFile_Button.Click += MAIN_OpenDatabaseFile_Click;
			MAIN_Finalize_Button.Click += MAIN_FinalizePurchase_Click;
			MAIN_Clear_Button.Click += MAIN_Clear_Click;

			MAIN_ClientStatusPreImage_Text.Text = "Client not connected!";
			MAIN_ClientStatusPreImage_Text.Foreground = Brushes.Red;

			MAIN_ClientStatusImage_Image.Visibility = Visibility.Collapsed;
			MAIN_ClientStatusPostImage_Text.Visibility = Visibility.Collapsed;
			DebugDelay();
		}

		private void OnShopClicked(object sender, MouseButtonEventArgs e) {
			SetupWindow w = new SetupWindow();
			MAIN_PhotoPreview_Image.Source = null;
			App.Current.MainWindow.Close();
			App.Current.MainWindow = w;
			App.Current.MainWindow.Show();
			ServerStateManager.StoreServerInstance(server);
			Dispose();
		}

		private void OnMainWindowClose(object sender, EventArgs e) {
			server.OnConnectionEstablished -= Server_OnConnectionEstablished;
			server.OnClientDisconnected -= Server_OnClientDisconnected;

			MAIN_Analyze_Button.Click -= imgProcessing.Analyze;
			MAIN_OpenDatabaseFile_Button.Click -= MAIN_OpenDatabaseFile_Click;
			MAIN_Finalize_Button.Click -= MAIN_FinalizePurchase_Click;

			MAIN_CurrentLoadedShop_Text.MouseLeftButtonDown -= OnShopClicked;
			this.Closed -= OnMainWindowClose;
		}

		#region Server Connection/Disconnection events

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.GetConnection(e.clientInfo.clientID).dataIDs.DefineCustomDataTypeForID<byte[]>(1, imgProcessing.OnImageDataReceived);
			Dispatcher.Invoke(() => {
				MAIN_ClientStatusPreImage_Text.Text = e.myServer.getConnectedClients.Length.ToString();
				MAIN_ClientStatusPreImage_Text.Foreground = Brushes.LawnGreen;
				MAIN_ClientStatusImage_Image.Visibility = Visibility.Visible;
			});
		}

		private void Server_OnClientDisconnected(object sender, ClientDisconnectedEventArgs e) {
			Dispatcher.Invoke(() => {
				MAIN_ClientStatusPreImage_Text.Text = "Client disconnected successfully!";
				MAIN_ClientStatusPreImage_Text.Foreground = Brushes.Blue;
				MAIN_ClientStatusImage_Image.Visibility = Visibility.Collapsed;
			});
		}

		#endregion


		#region Image preview container functions: Changing, Opening full view.

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
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.UriSource = imgUri;
			image.EndInit();
			currentImageSource = imgUri.AbsolutePath;
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

		#endregion


		#region Control button functionality

		private void MAIN_OpenDatabaseFile_Click(object sender, RoutedEventArgs e) {
			Process p = new Process();
			ProcessStartInfo info = new ProcessStartInfo(DatabaseAccess.access.itemDatabaseFile.FullName);
			p.StartInfo = info;
			p.Start();
		}

		private void MAIN_FinalizePurchase_Click(object sender, RoutedEventArgs e) {
			Purchase s = new Purchase(imgProcessing.currentParsingResult.meta.purchasedAt, imgProcessing.uiItemsMatched.Transform());
			s.FinalizePurchase();
			MAIN_Clear_Button.Visibility = Visibility.Visible;
			((Button)sender).Visibility = Visibility.Collapsed;
		}

		private void MAIN_Clear_Click(object sender, RoutedEventArgs e) {
			imgProcessing.uiItemsMatched.Clear();
			imgProcessing.uiItemsUnknown.Clear();

			SetPrevImage(new Uri(WPFHelper.resourcesPath + "Transparent.png"));

			MAIN_Finalize_Button.Visibility = Visibility.Visible;
			((Button)sender).Visibility = Visibility.Collapsed;
		}

		#endregion


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
