using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class StatusBarViewModel : BaseViewModel {

		public StatusBarViewModel() {
			ServerOnlineString = "Offline";
			ClientConnectedString = "No Connection";
		}

		private bool _serverOnline = false;
		private string _serverOnlineString = "";
		private bool _clientConnected = false;
		private string _clientConnectedString = "";
		private Shop _shop;
		private ICommand _onShopClickCommand;


		public ICommand OnShopClickCommand { get => _onShopClickCommand; set { _onShopClickCommand = value; Notify(nameof(OnShopClickCommand)); } }
		public bool ServerOnline { get => _serverOnline; set { _serverOnline = value; ServerOnlineString = value ? "Online" : "Offline"; } }
		public string ServerOnlineString {get => _serverOnlineString; set { _serverOnlineString = value; Notify(nameof(ServerOnlineString)); } }
		public bool ClientConnected { get => _clientConnected; set { _clientConnected = value; Notify(nameof(ClientConnected)); ClientConnectedString = value ? "Client Connected" : "No Connection"; } }
		public string ClientConnectedString {get => _clientConnectedString; set { _clientConnectedString = value; Notify(nameof(ClientConnectedString)); } }
		public Shop CurrentShop { get => _shop; set { _shop = value; Notify(nameof(CurrentShop)); } }
	}
}
