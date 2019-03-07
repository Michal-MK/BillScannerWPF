using Igor.TCP;
using System;

namespace Igor.BillScanner.WPF.UI {

	internal static class ServerStateManager {
		private static TCPServer serverHolder;

		/// <summary>
		/// Store running server instance for later use, for example when switching shops
		/// </summary>
		internal static void StoreServerInstance(TCPServer server) {
			serverHolder = server;
		}

		/// <summary>
		/// On consecutive MainWindow openings reuse the server that was stored in this class
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		internal static TCPServer RestoreServerInstance() {
			if (isHoldingInstance) {
				return serverHolder;
			}
			else {
				throw new InvalidOperationException("No instance stored");
			}
		}

		/// <summary>
		/// Do we have a server active and running
		/// </summary>
		internal static bool isHoldingInstance => serverHolder != null;
	}
}
