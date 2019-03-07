using System;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Computes the amount of changes needed to get the same string
	/// </summary>
	class WordSimilarity {

		/// <summary>
		/// Takes two <see cref="string"/>s and computes the number of differences
		/// </summary>
		/// <param name="toMatch">The <see cref="string"/> to compare</param>
		/// <param name="template">The <see cref="string"/> to compare against</param>
		/// <returns></returns>
		public static int Compute(string toMatch, string template) {
			int toMatchLength = toMatch.Length;
			int templateLength = template.Length;
			int[,] distanceMatrix = new int[toMatchLength + 1, templateLength + 1];

			for (int i = 0; i <= toMatchLength; distanceMatrix[i, 0] = i++) { };

			for (int j = 1; j <= templateLength; distanceMatrix[0, j] = j++) { };

			for (int i = 1; i <= toMatchLength; i++) {
				for (int j = 1; j <= templateLength; j++) {
					int cost = (template[j - 1] == toMatch[i - 1]) ? 0 : 1;
					int min1 = distanceMatrix[i - 1, j] + 1;
					int min2 = distanceMatrix[i, j - 1] + 1;
					int min3 = distanceMatrix[i - 1, j - 1] + cost;
					distanceMatrix[i, j] = Math.Min(Math.Min(min1, min2), min3);
				}
			}
			return distanceMatrix[toMatchLength, templateLength];
		}
	}
}