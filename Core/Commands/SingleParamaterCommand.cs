using System;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class SingleParamaterCommand : ICommand {

		private readonly Action<object> _action;

		public SingleParamaterCommand(Action<object> action) {
			_action = action;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) {
			return true;
		}

		public void Execute(object parameter) {
			_action(parameter);
		}
	}
}
