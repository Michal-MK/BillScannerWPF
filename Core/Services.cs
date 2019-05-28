namespace Igor.BillScanner.Core {
	public class Services {

		#region Singleton instance

		public static void Initialize() {
			if (Instance == null)
				Instance = new Services();
		}

		private Services() { /* No instances can be created of this class*/ }

		public static Services Instance { get; set; }

		#endregion

		public IManualUserInput UserInput { get; private set; }
		public MainWindowViewModel MainWindow { get; private set; }
		public ServerHandler ServerHandler { get; private set; }

		#region Registrations

		public void AddManualUserInput(IManualUserInput input) {
			UserInput = input;
		}

		internal void AddServerHandler(ServerHandler serverHandler) {
			ServerHandler = serverHandler;
		}

		public void AddMainWindowModel(MainWindowViewModel model) {
			MainWindow = model;
		}

		#endregion
	}
}
