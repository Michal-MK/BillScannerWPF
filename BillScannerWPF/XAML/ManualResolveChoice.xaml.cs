using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
			ManuallyEnterDate,
			UseCurrentTime,
			UseLatestValue,
			ManuallyEnterPrice,
			DefineNewItem,
			FindExistingItemFromList,
		}

		private readonly Dictionary<Choices, string> texts = new Dictionary<Choices, string>() {
			{ Choices.MatchAnyway, "[{0}] - Match!" },
			{ Choices.MatchWithoutAddingAmbiguities, "[{0}] - Match without modifying database." },
			{ Choices.NotAnItem, "[{0}] - Treat this text as not being an item (Nothing will be preformed)" },
			{ Choices.ManuallyEnterDate, "[{0}] - Enter date here manually:" },
			{ Choices.UseCurrentTime, "[{0}] - Use 'today' as the purchase date." },
			{ Choices.ManuallyEnterPrice, "[{0}] - Enter its price here manually:" },
			{ Choices.UseLatestValue, "[{0}] - Use last known value for this item from database."},
			{ Choices.FindExistingItemFromList, "[{0}] - Serch database and select an item this string is supposed to match."},
			{ Choices.DefineNewItem, "[{0}] - Define new item from what we got..."}
		};

		private readonly Dictionary<string, (Choices choice, string key)> buttons = new Dictionary<string, (Choices choice, string key)>() {
			{ Choices.MatchAnyway.ToString(), (Choices.MatchAnyway, Choices.MatchAnyway.ToString()) },
			{ Choices.MatchWithoutAddingAmbiguities.ToString(), (Choices.MatchWithoutAddingAmbiguities, Choices.MatchWithoutAddingAmbiguities.ToString()) },
			{ Choices.NotAnItem.ToString(), (Choices.NotAnItem, Choices.NotAnItem.ToString()) },
			{ Choices.ManuallyEnterDate.ToString(), (Choices.ManuallyEnterDate, Choices.ManuallyEnterDate.ToString()) },
			{ Choices.UseCurrentTime.ToString(), (Choices.UseCurrentTime, Choices.UseCurrentTime.ToString()) },
			{ Choices.UseLatestValue.ToString(), (Choices.UseLatestValue, Choices.UseLatestValue.ToString()) },
			{ Choices.ManuallyEnterPrice.ToString(), (Choices.ManuallyEnterPrice, Choices.ManuallyEnterPrice.ToString()) },
			{ Choices.DefineNewItem.ToString(), (Choices.DefineNewItem, Choices.DefineNewItem.ToString()) },
			{ Choices.FindExistingItemFromList.ToString(), (Choices.FindExistingItemFromList, Choices.FindExistingItemFromList.ToString()) },
		};

		internal ManualResolveChoice(string error, Choices choice) : this(error, new Choices[] { choice }) { }
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2) : this(error, new Choices[] { choice1, choice2 }) { }
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2, Choices choice3) : this(error, new Choices[] { choice1, choice2, choice3 }) { }

		internal ManualResolveChoice(string error, Choices[] choices) {
			InitializeComponent();
			TextBlock[] solutionTexts = new TextBlock[] { MANUAL_RESOLUTION_Solution1_Text, MANUAL_RESOLUTION_Solution2_Text, MANUAL_RESOLUTION_Solution3_Text, MANUAL_RESOLUTION_Solution4_Text };
			Button[] solutionButtons = new Button[] { MANUAL_RESOLUTION_Resolve1_Button, MANUAL_RESOLUTION_Resolve2_Button, MANUAL_RESOLUTION_Resolve3_Button, MANUAL_RESOLUTION_Resolve4_Button };
			MANUAL_RESOLUTION_ErrorType_Text.Text = error;
			choices = ModifyChoices(choices);
			int choiceNumbering = 0;
			for (int i = 0; i < 4; i++) {
				if (i == 3) {
					MANUAL_RESOLUTION_Solution4_Text.Text = string.Format(texts[choices[3]], choiceNumbering);
					if (choices[i] == Choices.ManuallyEnterDate) {
						MANUAL_RESOLUTION_Solution4_Box.Text = DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss");
					}
					if(choices[i] == Choices.ManuallyEnterPrice) {
						MANUAL_RESOLUTION_Solution4_Box.Text = "29.90";
					}
					solutionButtons[3].Click += ManualResolveChoice_Click;
					solutionButtons[3].Name = choices[3].ToString();
					solutionButtons[3].Content = string.Format("Solution {0}", choiceNumbering);
					solutionButtons[3].Visibility = Visibility.Visible;
				}
				if (i >= choices.Length || choices[i] == Choices.NOOP) {
					solutionButtons[i].Visibility = Visibility.Collapsed;
					solutionTexts[i].Visibility = Visibility.Collapsed;
				}
				else {
					solutionTexts[i].Text = string.Format(texts[choices[i]], choiceNumbering);
					solutionButtons[i].Click += ManualResolveChoice_Click;
					solutionButtons[i].Name = choices[i].ToString();
					solutionButtons[i].Content = string.Format("Solution {0}", choiceNumbering);
					choiceNumbering++;
				}
			}
		}

		private Choices[] ModifyChoices(Choices[] choices) {
			Choices[] modified = new Choices[4];
			choices.CopyTo(modified, 0);
			for (int i = choices.Length; i < modified.Length; i++) {
				modified[i] = Choices.NOOP;
			}
			return modified;
		}

		private Choices selected;

		private void ManualResolveChoice_Click(object sender, RoutedEventArgs e) {
			Enum.TryParse(((Button)sender).Name, out selected);
			evnt.Set();
		}

		internal async Task<Choices> SelectChoice() {
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Add(this);
			await Task.Run(() => {
				evnt.Wait();
				evnt.Reset();
			});
			WPFHelper.GetMainWindow().MAIN_Grid.Children.Remove(this);
			return selected;
		}
	}
}
