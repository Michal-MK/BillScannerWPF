using System.Windows;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		public App() {
			Services.Initialize();
			//TODO store server instance somewhere and add more functionality to it
			ServerHandler.Initialize();
		}
	}
}
