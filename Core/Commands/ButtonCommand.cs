using System;

namespace Igor.BillScanner.Core {
	public class ButtonCommand : BaseCommand {

		private Action<object> _buttonAction;

		public ButtonCommand(Action<object> buttonAction) {
			_buttonAction = buttonAction;
		}

		public override void Execute(object parameter) {
			_buttonAction(parameter);
		}
	}
}
