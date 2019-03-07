using System.ComponentModel;

namespace Igor.BillScanner.Core {
	public abstract class BaseViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public void Notify(string property) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
	}
}
