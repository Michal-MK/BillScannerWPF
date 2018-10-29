using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BillScannerWPF {

	/// <summary>
	/// Code for ManualResolveChoice.xaml
	/// </summary>
	public partial class ManualResolveChoice : UserControl {

		ManualResetEventSlim evnt = new ManualResetEventSlim();

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

		private Button focusableElement;

		#region Constructors

		/// <summary>
		/// Create new <see cref="ManualResolveChoice"/> with an error message and one solution
		/// </summary>
		internal ManualResolveChoice(string error, Choices choice) : this(error, new Choices[4] { choice, 0, 0, 0 }) { }

		/// <summary>
		/// Create new <see cref="ManualResolveChoice"/> with an error message and two solutions
		/// </summary>
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2) : this(error, new Choices[4] { choice1, choice2, 0, 0 }) { }

		/// <summary>
		/// Create new <see cref="ManualResolveChoice"/> with an error message and three solutions
		/// </summary>
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2, Choices choice3) : this(error, new Choices[4] { choice1, choice2, choice3, 0 }) { }

		#endregion


		/// <summary>
		/// Create new <see cref="ManualResolveChoice"/> with an error message and defined solutions
		/// </summary>
		internal ManualResolveChoice(string errorText, Choices[] choices) {
			InitializeComponent();
			Button[] solutionTexts1to4 = new Button[] {
				MANUAL_RESOLUTION_Solution1_Button,
				MANUAL_RESOLUTION_Solution2_Button,
				MANUAL_RESOLUTION_Solution3_Button,
				MANUAL_RESOLUTION_Solution4_Button
			};

			Button solution5Button = MANUAL_RESOLUTION_Solution5_Button;
			TextBox solution5Box = MANUAL_RESOLUTION_Solution5_Box;

			MANUAL_RESOLUTION_ErrorType_Text.Text = errorText;

			int choiceNumbering = 0;

			for (int i = 0; i < choices.Length; i++) {
				solutionTexts1to4[i].Visibility = choices[i] == Choices.NOOP || (int)choices[i] >= 20 ? Visibility.Collapsed : Visibility.Visible;

				if ((int)choices[i] >= 20) {
					solution5Button.Visibility = Visibility.Visible;
					solution5Button.Content = string.Format(texts[choices[i]], choiceNumbering);
					solution5Button.Click += ManualResolveChoice_Click;
					solution5Button.Name = choices[i].ToString();
				}

				if (choices[i] == Choices.ManuallyEnterDate) {
					solution5Box.Text = DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss");
				}
				if (choices[i] == Choices.ManuallyEnterPrice) {
					solution5Box.Text = "29.90";
				}
				if (choices[i] == Choices.ManuallyEnterQuantity) {
					solution5Box.Text = "1";
				}

				solutionTexts1to4[i].Content = string.Format(texts[choices[i]], choiceNumbering);
				solutionTexts1to4[i].Click += ManualResolveChoice_Click;
				solutionTexts1to4[i].Name = choices[i].ToString();
				if (focusableElement == null) {
					focusableElement = solutionTexts1to4[i];
				}
				if (choices[i] != Choices.NOOP) {
					choiceNumbering++;
				}
			}
		}

		#region Choice Selection

		private Choices selected;

		private void ManualResolveChoice_Click(object sender, RoutedEventArgs e) {
			Enum.TryParse(((Button)sender).Name, out selected);
			evnt.Set();
		}

		/// <summary>
		/// Handles user selection of one resolution for the problem
		/// </summary>
		internal async Task<Choices> SelectChoiceAsync() {
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(this);
			await Task.Run(() => {
				Thread.Sleep(10);
				Dispatcher.Invoke(() => {
					Keyboard.Focus(focusableElement);
				});
			});
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