using System;
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

		public MainWindow() {
			InitializeComponent();
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
