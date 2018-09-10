using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillScannerWPF.Rules {
	interface IRuleset {
		long GetQuantity(string[] ocrText, int index);

		decimal PriceOfOne(string[] ocrText, int index);

		string[] endMarkers { get; }
		string[] startMarkers { get; }
		char costPlusQuantitySeparator { get; }

	}
}
