using System.Text;

namespace Igor.BillScanner.Core {
	public class SenderHelper {

		public static string GetString(ParsingResult currentParsingResult) {
			StringBuilder builder = new StringBuilder();

			builder.Append(currentParsingResult.Meta.PurchasedAt.ToString("dd/MM/yyyy") + '\n');
			builder.Append("ITEMS" + '\n');
			foreach (var item in currentParsingResult.MachedItems) {
				builder.Append($"{item.ItemPurchase.Item.ItemName},{item.CurrentPrice},{item.ItemPurchase.Amount}" + '\n');
			}
			builder.Append("ITEMS" + '\n');
			return builder.ToString();
		}
	}
}
