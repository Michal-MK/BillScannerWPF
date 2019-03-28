using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Igor.BillScanner.Core;
using Igor.BillScanner.Core.Rules;
using Igor.TCP;
using Microsoft.Win32;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDisposable, IDispatcher, IItemPreview {

		/// <summary>
		/// The constant port the server is listening on for incoming phone connections
		/// </summary>
		public const ushort START_PORT = 6689;

		/// <summary>
		/// Static reference to the server
		/// </summary>
		public static TCPServer server { get; internal set; }

		/// <summary>
		/// Image processing class that does the scanning and subsequent parsing of the image
		/// </summary>
		public ImageProcessor ImgProcessing;

		/// <summary>
		/// Manual bill creation without using OCR technology
		/// </summary>
		internal ManualPurchaseHandler ManualPurchase;

		/// <summary>
		/// Absolute path to the image currently being displayed in the image preview control
		/// </summary>
		public string CurrentImageSource { get; set; }

		/// <summary>
		/// The container that displays currently previewed item
		/// </summary>
		public UIItem CurrentItemBeingInspected { get; set; }

		/// <summary>
		/// The rule-set selected at the launch of the application
		/// </summary>
		internal IRuleset SelectedShopRuleset { get; }

		internal ObservableCollection<UIItem> matchedItems;
		internal ObservableCollection<UIItem> unknownItems;

		/// <summary>
		/// Create a default Albert window (Debug)
		/// </summary>
		public MainWindow() : this(Shop.Lidl) { }

		/// <summary>
		/// Create a new Main window and prepare it for the selected shop type
		/// </summary>
		/// <param name="selectedShop">The shop to load data for</param>
		public MainWindow(Shop selectedShop) {
			InitializeComponent();
			Services.Instance.AddMainThreadDispatcher(this);

			DatabaseAccess.LoadDatabase(selectedShop);
			SelectedShopRuleset = BaseRuleset.GetRuleset(selectedShop);

			(StatusBar.DataContext as StatusBarViewModel).CurrentShop = selectedShop;
			StatusBar.BAR_CurrentLoadedShop_Text.MouseLeftButtonDown += OnShopClicked;

			this.Closed += OnMainWindowClose;


			Task.Run(async () => {
				await StartServer();
				if (server == null) {
					MessageBox.Show("Unable to start server at " + SimpleTCPHelper.GetActiveIPv4Address() + " " + START_PORT + "\n" +
									"Either the port is already taken of you are not connected to the Internet!"
									, "Server Off-line!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}

				Dispatcher.Invoke(() => (StatusBar.DataContext as StatusBarViewModel).ServerOnline = true);
			});

			ImgProcessing = new ImageProcessor(SelectedShopRuleset, this);

			ImgProcessing.OnImageParsed += OnImageParsed;

			ManualPurchase = new ManualPurchaseHandler(selectedShop, this);

			MAIN_Analyze_Button.Click += AnalyzeMethod;
			MAIN_Analyze_Button.Visibility = Visibility.Collapsed;

			MAIN_ManualPurchase_Button.Click += ManualPurchase.Begin;
			MAIN_Finalize_Button.Click += MAIN_FinalizePurchase_Click;
			MAIN_Clear_Button.Click += MAIN_Clear_Click;

			(StatusBar.DataContext as StatusBarViewModel).ClientConnected = false;

			DebugDelay();
		}

		private void OnImageParsed(object sender, ParsingResult e) {
			matchedItems = UIItemConversion(ConstructUI(e.MachedItems));
			unknownItems = UIItemConversion(ConstructUI(e.UnknownItems));
		}

		public ObservableCollection<UIItem> UIItemConversion(ObservableCollection<object> observableCollection) {
			ObservableCollection<UIItem> okay = new ObservableCollection<UIItem>();
			foreach (object item in observableCollection) {
				okay.Add((UIItem)item);
			}
			return okay;
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

		public void Run(Action action) {
			Dispatcher.Invoke(action);
		}

		private void OnMainWindowClose(object sender, EventArgs e) {
			server.OnClientConnected -= Server_OnConnectionEstablished;
			server.OnClientDisconnected -= Server_OnClientDisconnected;

			MAIN_Analyze_Button.Click -= AnalyzeMethod;
			MAIN_Finalize_Button.Click -= MAIN_FinalizePurchase_Click;

			StatusBar.BAR_CurrentLoadedShop_Text.MouseLeftButtonDown -= OnShopClicked;
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
			server.DefineCustomPacket<byte[]>(e.clientInfo.clientID, 55, ImgProcessing.OnImageDataReceived);
			Dispatcher.Invoke(() => {
				(StatusBar.DataContext as StatusBarViewModel).ClientConnected = true;
			});
		}

		private void Server_OnClientDisconnected(object sender, ClientDisconnectedEventArgs e) {
			Dispatcher.Invoke(() => {
				(StatusBar.DataContext as StatusBarViewModel).ClientConnected = false;
			});
		}

		private void AnalyzeMethod(object sender, RoutedEventArgs e) {
			if (CurrentImageSource == null) {
				return;
			}
			ImgProcessing.Analyze(CurrentImageSource);
		}

		#endregion


		#region Image preview container functions: Changing, Opening full view.

		internal void PreviewImgMouse(object sender, MouseButtonEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog {
				DefaultExt = "png",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
			};
			if (dialog.ShowDialog() == true) {
				SetPreviewImage(new Uri(dialog.FileName));
			}
		}

		internal void SetPreviewImage(Uri imgUri) {
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.UriSource = imgUri;
			image.EndInit();
			CurrentImageSource = imgUri.AbsolutePath;

			MAIN_PhotoPreview_Image.Source = image;

			MAIN_ManualPurchase_Button.Visibility = Visibility.Collapsed;
			MAIN_Analyze_Button.Visibility = Visibility.Visible;
		}

		private int attempt = -1;
		public void SetPreviewImage(byte[] imgData) {
			File.WriteAllBytes(WPFHelper.dataPath + "current" + ++attempt + ".jpg", imgData);
			CurrentImageSource = WPFHelper.dataPath + "current" + attempt + ".jpg";
			SetPreviewImage(new Uri(CurrentImageSource));
			MAIN_ManualPurchase_Button.Visibility = Visibility.Collapsed;
			MAIN_Analyze_Button.Visibility = Visibility.Visible;
		}

		//Open Selected image in default image viewer
		private void MAIN_PhotoPreview_RightClick(object sender, MouseButtonEventArgs e) {
			if (string.IsNullOrEmpty(CurrentImageSource)) {
				return;
			}
			new Process {
				StartInfo = new ProcessStartInfo(CurrentImageSource)
			}.Start();
		}

		#endregion


		#region Control button functionality

		private void MAIN_FinalizePurchase_Click(object sender, RoutedEventArgs e) {
			Purchase s = new Purchase(SelectedShopRuleset.shop, ImgProcessing.CurrentParsingResult.Meta.PurchasedAt, matchedItems.Transform());
			s.FinalizePurchase();
			MAIN_Clear_Button.Visibility = Visibility.Visible;
			((Button)sender).Visibility = Visibility.Collapsed;
		}

		private void MAIN_Clear_Click(object sender, RoutedEventArgs e) {
			Clear();

			SetPreviewImage(new Uri(WPFHelper.resourcesPath + "Transparent.png"));

			MAIN_Finalize_Button.Visibility = Visibility.Visible;
			((Button)sender).Visibility = Visibility.Collapsed;
		}

		public ObservableCollection<object> ConstructUI(IEnumerable<UIItemCreationInfo> creation) {
			ObservableCollection<object> receiver = new ObservableCollection<object>();
			foreach (UIItemCreationInfo item in creation) {
				receiver.Add(new UIItem(item));
			}
			return receiver;
		}

		public void Clear() {
			//TODO
		}

		#endregion


		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				ImgProcessing.Dispose();
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
