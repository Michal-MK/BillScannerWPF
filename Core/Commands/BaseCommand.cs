using System;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public abstract class BaseCommand : ICommand {

#pragma warning disable CS0067
		public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

		public virtual bool CanExecute(object parameter) {
			return true;
		}

		public abstract void Execute(object parameter);
	}
}
