using System;

namespace Igor.BillScanner.Core {
	public interface IDispatcher {
		void Run(Action action);
	}
}