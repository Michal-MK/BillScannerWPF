namespace Igor.BillScanner.Core {
	public class StatusBarViewModel : BaseViewModel {
		/// <summary>
		/// Default constructor
		/// </summary>
		public StatusBarViewModel() {
			ServerOnlineString = "Offline";
			ClientConnectedString = "No Connection";
		}

		/// <summary>
		/// Boolean for indicating whether the server launched successfully
		/// </summary>
		public bool ServerOnline {
			get { return serverOnline; }
			set {
				serverOnline = value;
				if (value) { ServerOnlineString = "Online"; }
				else { ServerOnlineString = "Offline"; }
			}
		}
		private bool serverOnline = false;

		/// <summary>
		/// String representing server status
		/// </summary>
		public string ServerOnlineString {
			get { return serverOnlineString; }
			set {
				serverOnlineString = value;
				Notify(nameof(ServerOnlineString));
			}
		}
		private string serverOnlineString;


		/// <summary>
		/// Is a client currently connected the server
		/// </summary>
		public bool ClientConnected {
			get { return clientConnected; }
			set {
				clientConnected = value;
				if (value) { ClientConnectedString = "Client Connected"; }
				else { ClientConnectedString = "No Connection"; }
				Notify(nameof(ClientConnected));
			}
		}
		private bool clientConnected = false;

		/// <summary>
		/// String representing server status
		/// </summary>
		public string ClientConnectedString {
			get { return clientConnectedString; }
			set {
				clientConnectedString = value;
				Notify(nameof(ClientConnectedString));
			}
		}
		private string clientConnectedString;


		/// <summary>
		/// Current shop + database that is loaded
		/// </summary>
		public Shop CurrentShop {
			get { return shop; }
			set {
				shop = value;
				Notify(nameof(CurrentShop));
			}
		}
		private Shop shop;
	}
}
