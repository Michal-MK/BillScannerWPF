using System;

namespace Igor.BillScanner.Core {
	public class Command : BaseCommand {

		private readonly Action _action;

		public Command(Action action) {
			_action = action;
		}

		public override void Execute(object parameter) {
			_action();
		}
	}
}
