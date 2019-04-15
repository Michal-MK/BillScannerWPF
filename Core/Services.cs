namespace Igor.BillScanner.Core {
	public class Services {

		static Services() {
			Instance = new Services();
		}

		private Services() { /* No instances can be created of this class*/ }

		public static Services Instance { get; set; }

		public IManualUserInput UserInput { get; private set; }

		public MainWindowViewModel MainWindow { get; private set; }

		public void AddManualUserInput(IManualUserInput input) {
			UserInput = input;
		}

		public void AddMainWindowModel(MainWindowViewModel model) {
			MainWindow = model;
		}
	}
}
