using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Igor.TCP;

namespace Igor.BillScanner.Core {
	public class ServerHandler {

		/// <summary>
		/// The constant port the server is listening on for incoming phone connections
		/// </summary>
		public const ushort START_PORT = 6689;


		#region Singleton Instance

		public static ServerHandler Initialize() {
			Instance = new ServerHandler();
			return Instance;
		}

		private ServerHandler() { }

		public static ServerHandler Instance { get; private set; }

		#endregion

		public event EventHandler<string> OnImageReceived;

		private TCPServer server;


		#region Server Startup/Events


		public void StartServer() {
			Task.Run(StartServerTask);
		}

		private async Task StartServerTask() {
			if (ServerStateManager.isHoldingInstance) {
				server = ServerStateManager.RestoreServerInstance();
				server.OnClientConnected += Server_OnConnectionEstablished;
				server.OnClientDisconnected += Server_OnClientDisconnected;
			}
			else {
				server = new TCPServer(new ServerConfiguration());
				try {
					await server.Start(SimpleTCPHelper.GetActiveIPv4Address(), START_PORT);

					Services.Instance.MainWindow.StatusBar.ServerOnline = true;
					server.OnClientConnected += Server_OnConnectionEstablished;
					server.OnClientDisconnected += Server_OnClientDisconnected;
				}
				catch {
					Debugger.Break();
				}
			}
		}

		private void Server_OnConnectionEstablished(object sender, ClientConnectedEventArgs e) {
			server.DefineCustomPacket<byte[]>(e.clientInfo.clientID, 55, OnImageDataReceived);
			Services.Instance.MainWindow.StatusBar.ClientConnected = true;
		}

		private void Server_OnClientDisconnected(object sender, ClientDisconnectedEventArgs e) {
			Services.Instance.MainWindow.StatusBar.ClientConnected = false;
		}

		#endregion

		private void OnImageDataReceived(byte senderID, byte[] imageData) {
			try {
				string path = $"{WPFHelper.dataPath}current{DateTime.Now.ToString("dd.MM_hh.mm.sss")}.jpg";
				File.WriteAllBytes(path, imageData);
				OnImageReceived?.Invoke(this, path);
			}
			catch (Exception) {
				Debugger.Break();
				//TODO
			}
		}

	}
}
