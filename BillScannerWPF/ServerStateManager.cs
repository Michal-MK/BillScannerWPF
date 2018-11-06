using Igor.TCP;

namespace BillScannerWPF {

	internal static class ServerStateManager {
		private static TCPServer serverHolder;

		/// <summary>
		/// Store running server instance for later use, for example when switching shops
		/// </summary>
		/// <param name="server"></param>
		internal static void StoreServerInstance(TCPServer server) {
			serverHolder = server;
		}

		/// <summary>
		/// On consecutive MainWindow openings reuse the server that was stored in this class
		/// </summary>
		/// <returns></returns>
		internal static TCPServer RestoreServerInstance() {
			return serverHolder;
		}

		/// <summary>
		/// Do we have a server active and running
		/// </summary>
		internal static bool isHoldingInstance { get { return serverHolder != null; } }
	}
}
