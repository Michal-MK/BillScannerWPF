namespace BillScannerWPF {

	/// <summary>
	/// How successful was this attempt at matching
	/// </summary>
	internal enum MatchRating {
		Success,
		One,
		Two,
		Three,
		Four,
		Five,
		FivePlus,
		Fail
	}
}
