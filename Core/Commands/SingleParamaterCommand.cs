using System;

namespace Igor.BillScanner.Core {
	public class SingleParamaterCommand : BaseCommand {

		private readonly Action<object> _action;

		public SingleParamaterCommand(Action<object> action) {
			_action = action;
		}

		public override void Execute(object parameter) {
			_action(parameter);
		}
	}
}
