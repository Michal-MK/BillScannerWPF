using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace BillScannerWPF {
	/// <summary>
	/// Interaction logic for ManualResolveChoice.xaml
	/// </summary>
	public partial class ManualResolveChoice : UserControl {
		ManualResetEventSlim evnt = new ManualResetEventSlim();

		internal enum Choices {
			NOOP,
			MatchAnyway,
			MatchWithoutAddingAmbiguities,
			NotAnItem,
			UseCurrentTime,
			UseLatestValue,
			DefineNewItem,
			FindExistingItemFromList,
			ManuallyEnterQuantity = 20,
			ManuallyEnterDate = 21,
			ManuallyEnterPrice = 22,
		}

		private readonly Dictionary<Choices, string> texts = new Dictionary<Choices, string>() {
			{ Choices.NOOP, "NOOP" },
			{ Choices.MatchAnyway, "[{0}] - Match!" },
			{ Choices.MatchWithoutAddingAmbiguities, "[{0}] - Match without modifying database." },
			{ Choices.NotAnItem, "[{0}] - Treat this text as not being an item (Nothing will be preformed)" },
			{ Choices.ManuallyEnterDate, "[{0}] - Enter date here manually:" },
			{ Choices.UseCurrentTime, "[{0}] - Use 'today' as the purchase date." },
			{ Choices.ManuallyEnterPrice, "[{0}] - Enter its price here manually:" },
			{ Choices.UseLatestValue, "[{0}] - Use last known value for this item from database."},
			{ Choices.FindExistingItemFromList, "[{0}] - Search database and select an item this string is supposed to match."},
			{ Choices.DefineNewItem, "[{0}] - Define new item from what we got..."},
			{ Choices.ManuallyEnterQuantity, "[{0}] - Enter the amount here manually: " }
		};


		#region Constructors

		internal ManualResolveChoice(string error, Choices choice) : this(error, new Choices[4] { choice, 0, 0, 0 }) { }
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2) : this(error, new Choices[4] { choice1, choice2, 0, 0 }) { }
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2, Choices choice3) : this(error, new Choices[4] { choice1, choice2, choice3, 0 }) { }

		#endregion

		internal ManualResolveChoice(string errorText, Choices[] choices) {
			InitializeComponent();
			TextBlock[] solutionTexts = new TextBlock[] {
				MANUAL_RESOLUTION_Solution1_Text,
				MANUAL_RESOLUTION_Solution2_Text,
				MANUAL_RESOLUTION_Solution3_Text,
				MANUAL_RESOLUTION_Solution4_Text
			};
			Button[] solutionButtons = new Button[] {
				MANUAL_RESOLUTION_Resolve1_Button,
				MANUAL_RESOLUTION_Resolve2_Button,
				MANUAL_RESOLUTION_Resolve3_Button,
				MANUAL_RESOLUTION_Resolve4_Button
			};
			TextBox inputBox = MANUAL_RESOLUTION_Solution4_Box;


			MANUAL_RESOLUTION_ErrorType_Text.Text = errorText;

			int choiceNumbering = 0;

			for (int i = 0; i < choices.Length; i++) {
				solutionTexts[i].Visibility = choices[i] == Choices.NOOP ? Visibility.Collapsed : Visibility.Visible;

				if ((int)choices[i] > 20) {
					inputBox.Visibility = Visibility.Visible;
				}

				if (choices[i] == Choices.ManuallyEnterDate) {
					inputBox.Text = DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss");
				}
				if (choices[i] == Choices.ManuallyEnterPrice) {
					inputBox.Text = "29.90";
				}
				if (choices[i] == Choices.ManuallyEnterQuantity) {
					inputBox.Text = "1";
				}

				solutionTexts[i].Text = string.Format(texts[choices[i]], choiceNumbering);
				solutionButtons[i].Click += ManualResolveChoice_Click;
				solutionButtons[i].Name = choices[i].ToString();
				solutionButtons[i].Content = string.Format("Solution {0}", choiceNumbering);
				if (choices[i] != Choices.NOOP)
					choiceNumbering++;

			}
		}

		#region Choice Selection

		private Choices selected;

		private void ManualResolveChoice_Click(object sender, RoutedEventArgs e) {
			Enum.TryParse(((Button)sender).Name, out selected);
			evnt.Set();
		}

		internal async Task<Choices> SelectChoiceAsync() {
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(this);
			await Task.Run(() => {
				evnt.Wait();
				evnt.Reset();
			});
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(this);
			return selected;
		}

		#endregion
	}
}