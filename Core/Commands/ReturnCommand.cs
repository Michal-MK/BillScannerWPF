using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Igor.BillScanner.Core {
	public class ReturnCommand<T> : BaseCommand {

		private Func<T> function;

		public T Result { get; private set; }

		public ReturnCommand(Func<T> toExecute) {
			function = toExecute;
		}

		public override void Execute(object parameter) {
			Result = function();
		}
	}
}
