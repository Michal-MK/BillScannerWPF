using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Diagnostics;

using Igor.TCP;
using BillScannerWPF.Rules;
using BillScannerCore;
using BillScannerStartup;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDisposable {

		/// <summary>
		/// The constant port the server is listening on for incoming phone connections
		/// </summary>
		public const ushort START_PORT = 6689;

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
		/// The status bar at the top, provides general information about the state of the program
		/// </summary>
		internal StatusBar statusBar { get; }

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

			statusBar = new StatusBar(new StatusBarViewModel());
			MAIN_Grid.Children.Add(statusBar);

			statusBar.model.CurrentShop = selectedShop;
			statusBar.BAR_CurrentLoadedShop_Text.MouseLeftButtonDown += OnShopClicked;

			this.Closed += OnMainWindowClose;


			Task.Run(async () => {
				await StartServer();
				if (server == null) {
					MessageBox.Show("Unable to start server at " + SimpleTCPHelper.GetActiveIPv4Address() + " " + START_PORT + "\n" +
									"Either the port is already taken of you are not connected to the Internet!"
									, "Server Off-line!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}

				statusBar.model.ServerOnline = true;
			});

			imgProcessing = new ImageProcessor(selectedShopRuleset, this);

			MAIN_Analyze_Button.Click += imgProcessing.Analyze;
			MAIN_Finalize_Button.Click += MAIN_FinalizePurchase_Click;
			MAIN_Clear_Button.Click += MAIN_Clear_Click;

			statusBar.model.ClientConnected = false;

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
			server.OnClientConnected -= Server_OnConnectionEstablished;
			server.OnClientDisconnected -= Server_OnClientDisconnected;

			MAIN_Analyze_Button.Click -= imgProcessing.Analyze;
			MAIN_Finalize_Button.Click -= MAIN_FinalizePurchase_Click;

			statusBar.BAR_CurrentLoadedShop_Text.MouseLeftButtonDown -= OnShopClicked;
			this.Closed -= OnMainWindowClose;
		}

		#region Server Startup/Events


		private async Task StartServer() {
			if (ServerStateManager.isHoldingInstance) {
				server = ServerStateManager.RestoreServerInstance();
				server.OnClientConnected += Server_OnConnectionEstablished;
				server.OnClientDisconnected += Server_OnClientDisconnected;
			}
			else {
				server = new TCPServer(new ServerConfiguration());
				try {
					await server.Start(
						   SimpleTCPHelper.GetActiveIPv4Address()
						   //"192.168.137.1"
						   , START_PORT);
					server.OnClientConnected += Server_OnConnectionEstablished;
					server.OnClientDisconnected += Server_OnClientDisconnected;
				}
				catch {
					MessageBox.Show("Unable to start server at " + SimpleTCPHelper.GetActiveIPv4Address() + " and port: " + START_PORT + "!");
				}
			}
		}

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.DefineCustomPacket<byte[]>(e.clientInfo.clientID, 55, imgProcessing.OnImageDataReceived);
			Dispatcher.Invoke(() => {
				statusBar.model.ClientConnected = true;

			});
		}

		private void Server_OnClientDisconnected(object sender, ClientDisconnectedEventArgs e) {
			Dispatcher.Invoke(() => {
				statusBar.model.ClientConnected = false;
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
