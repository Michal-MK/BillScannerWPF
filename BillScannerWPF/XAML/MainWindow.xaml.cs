using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Igor.BillScanner.Core;
using Microsoft.Win32;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		/// <summary>
		/// Manual bill creation without using OCR technology
		/// </summary>
		internal ManualPurchaseHandler ManualPurchase;

		/// <summary>
		/// The container that displays currently previewed item
		/// </summary>
		public UIItem CurrentItemBeingInspected { get; set; }


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
			ServerHandler.Initialize();

			MainWindowViewModel model = DataContext as MainWindowViewModel;
			model.StatusBar = StatusBar.DataContext as StatusBarViewModel;
			model.StatusBar.CurrentShop = selectedShop;

			Services.Instance.AddManualUserInput(new ManualUserInput(this));

			ServerHandler.Instance.StartServer();
			DatabaseAccess.LoadDatabase(selectedShop);


			ManualPurchase = new ManualPurchaseHandler(selectedShop, this);

			MAIN_ManualPurchase_Button.Click += ManualPurchase.Begin;
			StatusBar.BAR_CurrentLoadedShop_Text.MouseLeftButtonDown += OnShopClicked;
			this.Closed += OnMainWindowClose;

			model.CoreLoaded();

			DebugDelay();
		}

		private void OnShopClicked(object sender, MouseButtonEventArgs e) {
			SetupWindow w = new SetupWindow();
			MAIN_PhotoPreview_Image.Source = null;
			App.Current.MainWindow.Close();
			App.Current.MainWindow = w;
			App.Current.MainWindow.Show();
			//ServerStateManager.StoreServerInstance(server);
		}

		private void OnMainWindowClose(object sender, EventArgs e) {
			//server.OnClientConnected -= Server_OnConnectionEstablished;
			//server.OnClientDisconnected -= Server_OnClientDisconnected;

			StatusBar.BAR_CurrentLoadedShop_Text.MouseLeftButtonDown -= OnShopClicked;
			this.Closed -= OnMainWindowClose;
		}

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
			(DataContext as MainWindowViewModel).ImageSource = imgUri.AbsolutePath;

			MAIN_PhotoPreview_Image.Source = image;

			MAIN_ManualPurchase_Button.Visibility = Visibility.Collapsed;
			//MAIN_Analyze_Button.Visibility = Visibility.Visible;
		}

		//Open Selected image in default image viewer
		private void MAIN_PhotoPreview_RightClick(object sender, MouseButtonEventArgs e) {
			if (string.IsNullOrEmpty((DataContext as MainWindowViewModel).ImageSource)) {
				return;
			}
			new Process {
				StartInfo = new ProcessStartInfo((DataContext as MainWindowViewModel).ImageSource)
			}.Start();
		}

		#endregion
	}
}
