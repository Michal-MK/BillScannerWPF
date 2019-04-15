using Igor.TCP;
using System;

namespace Igor.BillScanner.Core {

	public static class ServerStateManager {
		private static TCPServer serverHolder;

		/// <summary>
		/// Store running server instance for later use, for example when switching shops
		/// </summary>
		public static void StoreServerInstance(TCPServer server) {
			serverHolder = server;
		}

		/// <summary>
		/// On consecutive MainWindow openings reuse the server that was stored in this class
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public static TCPServer RestoreServerInstance() {
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
		public static bool isHoldingInstance => serverHolder != null;
	}
}
