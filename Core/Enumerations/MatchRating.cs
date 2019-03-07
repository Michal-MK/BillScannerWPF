namespace Igor.BillScanner.Core {

	/// <summary>
	/// How successful was this attempt at matching
	/// </summary>
	public enum MatchRating {
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
