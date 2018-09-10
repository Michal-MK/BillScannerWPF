using System.Collections.Generic;

namespace BillScannerWPF {
	internal class ParsingResult {
		public UItemCreationInfo[] parsed { get; }
		public UItemCreationInfo[] unknown { get; }
		public string[] originalLines { get; }

		public ParsingResult(string[] lines, List<UItemCreationInfo> mathched, List<UItemCreationInfo> unmatched) {
			parsed = mathched.ToArray();
			unknown = unmatched.ToArray();
			originalLines = lines;
		}
	}
}