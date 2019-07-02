using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Igor.BillScanner.Core;
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
			//Model.Se
		}

		private void OnShopClicked(object sender, MouseButtonEventArgs e) {
			SetupWindow setupWin = new SetupWindow();
			Close();
			Application.Current.MainWindow = setupWin;
			Application.Current.MainWindow.Show();
		}

		#region Image preview container functions: Changing, Opening full view.

		public void ImageSelectDialog(object sender, MouseButtonEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog {
				DefaultExt = "png",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
				Multiselect = false,
			};
			if (dialog.ShowDialog() == true) {
				Model.ImageSource = dialog.FileName;
			}
		}

		public void OpenImageDefault(object sender, MouseButtonEventArgs e) {
			if (string.IsNullOrEmpty(Model.ImageSource) || Model.ImageSource == "/Igor.BillScanner.WPF.UI;component/Resources/Transparent.png") {
				return;
			}
			new Process { StartInfo = new ProcessStartInfo(Model.ImageSource) }.Start();
		}

		#endregion
	}
}
