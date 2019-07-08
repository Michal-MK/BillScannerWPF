using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Igor.BillScanner.Core;
using Igor.Models;
using Microsoft.Win32;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private MainWindowViewModel Model => DataContext as MainWindowViewModel;

		/// <summary>
		/// Create a default Lidl window (Debug)
		/// </summary>
		public MainWindow() : this(Shop.Lidl) { }

		/// <summary>
		/// Create a new Main window and prepare it for the selected shop type
		/// </summary>
		/// <param name="selectedShop">The shop to load data for</param>
		public MainWindow(Shop selectedShop) {
			InitializeComponent();
			DatabaseAccess.LoadDatabase(selectedShop);
			Model.OnMouseLeftClickImage = (s, e) => {
				OpenFileDialog dialog = new OpenFileDialog {
					DefaultExt = "png",
					InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
					Multiselect = false,
				};
				if (dialog.ShowDialog() == true) {
					Model.ImageSource = dialog.FileName;
				}
			};

			Model.StatusBarViewModel.OnShopClickCommand = new Command(() => {
				Services.Instance.ServerHandler.StoreServer();
				SetupWindow setupWin = new SetupWindow();
				Application.Current.MainWindow.Close();
				Application.Current.MainWindow = setupWin;
				Application.Current.MainWindow.Show();
			});
		}

		#region Image preview container functions: Changing, Opening full view.

		public void ImageSelectDialog(object sender, MouseButtonEventArgs e) {
			Model.OnMouseLeftClickImage.Invoke(sender, e);
		}

		public void OpenImageDefault(object sender, MouseButtonEventArgs e) {
			Model.OnMouseRightClickImage.Invoke(sender, e);
		}

		#endregion
	}
}
