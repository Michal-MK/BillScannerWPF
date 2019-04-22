using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Igor.BillScanner.Core;

namespace Igor.BillScanner.WPF.UI {

	/// <summary>
	/// Code for ManualResolveChoice.xaml
	/// </summary>
	public partial class ManualResolveChoice : UserControl {

		private readonly ManualResetEventSlim evnt = new ManualResetEventSlim();

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

		public ManualResolveChoice(ManualResolutionViewModel model) {
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Add(this);
			InitializeComponent();
			DataContext = model;
		}

		#region Choice Selection

		internal async Task SelectChoiceAsync() {
			await Task.Run(() => {
				evnt.Wait();
				evnt.Reset();
			});
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Remove(this);
		}

		#endregion
	}
}