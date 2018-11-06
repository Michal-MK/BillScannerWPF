﻿using System;
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

			MANUAL_RESOLUTION_ErrorType_Text.Text = errorText;

			SelectivelyEnableInput(choices, solutionTexts1to4);

			for (int i = 0; i < choices.Length; i++) {
				if ((int)choices[i] >= 20) {
					solution5Button.Visibility = Visibility.Visible;
					solution5Button.Content = texts[choices[i]];
					solution5Button.Click += ManualResolveChoice_Click;
					solution5Button.Name = choices[i].ToString();
					if (choices[i] == Choices.ManuallyEnterDate) {
						MANUAL_RESOLUTION_Solution5_DateBox.Visibility = Visibility;
					}
					if (choices[i] == Choices.ManuallyEnterPrice) {
						MANUAL_RESOLUTION_Solution5_Box.Text = "0.0";
						MANUAL_RESOLUTION_Solution5_Box.Visibility = Visibility.Visible;
					}
					if (choices[i] == Choices.ManuallyEnterQuantity) {
						MANUAL_RESOLUTION_Solution5_Box.Text = "1";
						MANUAL_RESOLUTION_Solution5_Box.Visibility = Visibility.Visible;
					}
				}
				if (focusableElement == null) {
					focusableElement = solutionTexts1to4[i];
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
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Add(this);
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
			((MainWindow)App.Current.MainWindow).MAIN_Grid.Children.Remove(this);
			return selected;
		}

		#endregion

		private void SelectivelyEnableInput(Choices[] choices, Button[] buttons) {
			for (int i = 0; i < choices.Length; i++) {
				switch (choices[i]) {
					case Choices.NOOP: {
						buttons[i].Visibility = Visibility.Collapsed;
						break;
					}
					case Choices.MatchAnyway:
					case Choices.MatchWithoutAddingAmbiguities:
					case Choices.NotAnItem:
					case Choices.UseCurrentTime:
					case Choices.UseLatestValue:
					case Choices.DefineNewItem:
					case Choices.FindExistingItemFromList: {
						buttons[i].Visibility = Visibility.Visible;
						buttons[i].Content = texts[choices[i]];
						buttons[i].Click += ManualResolveChoice_Click;
						buttons[i].Name = choices[i].ToString();
						break;
					}
					default: {
						break;
					}
				}
			}
		}
	}
}