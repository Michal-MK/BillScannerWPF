using System.Collections.Generic;
using System.Windows.Controls;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ManualResolveChoice.xaml
	/// </summary>
	public partial class ManualResolveChoice : UserControl {

		private static Dictionary<Choices, string> TextsPerOperation { get; } = new Dictionary<Choices, string>() {
			{ Choices.NOOP, "NOOP" },
			{ Choices.MatchAnyway, "Match!" },
			{ Choices.MatchWithoutAddingAmbiguities, "Match without modifying database." },
			{ Choices.NotAnItem, "Treat this text as not being an item (Nothing will be preformed)" },
			{ Choices.ManuallyEnterDate, "Enter date here manually:" },
			{ Choices.UseCurrentTime, "Use 'today' as the purchase date." },
			{ Choices.ManuallyEnterPrice, "Enter its price here manually:" },
			{ Choices.UseLatestValue, "Use last known value for this item from database."},
			{ Choices.FindExistingItemFromList, "Search database and select an item this string is supposed to match."},
			{ Choices.DefineNewItem, "Define new item from what we got..."},
			{ Choices.ManuallyEnterQuantity, "Enter the amount here manually: " }
		};

		public ManualResolveChoice() {
			InitializeComponent();
		}
	}
}