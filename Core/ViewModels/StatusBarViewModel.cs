using BillScannerCore;
using System.ComponentModel;

namespace BillScannerWPF {
	public class StatusBarViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		private bool serverOnline = false;
		private string serverOnlineString;
		private bool clientConnected = false;
		private string clientConnectedString;

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

		/// <summary>
		/// String representing server status
		/// </summary>
		public string ServerOnlineString {
			get { return serverOnlineString; }
			set {
				serverOnlineString = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerOnlineString)));
			}
		}


		/// <summary>
		/// Is a client currently connected the server
		/// </summary>
		public bool ClientConnected {
			get { return clientConnected; }
			set {
				clientConnected = value;
				if (value) { ClientConnectedString = "Client Connected"; }
				else { ClientConnectedString = "No Connection"; }
			}
		}

		/// <summary>
		/// String representing server status
		/// </summary>
		public string ClientConnectedString {
			get { return clientConnectedString; }
			set {
				clientConnectedString = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClientConnectedString)));
			}
		}


		private Shop shop;
		/// <summary>
		/// Current shop + database that is loaded
		/// </summary>
		public Shop CurrentShop { get { return shop; } set { shop = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentShop))); } }
	}
}
