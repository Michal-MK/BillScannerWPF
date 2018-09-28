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
			NotAnItem
		}

		internal readonly Dictionary<Choices, string> texts = new Dictionary<Choices, string>() {
			{ Choices.MatchAnyway, "[{0}] - Match!" },
			{ Choices.MatchWithoutAddingAmbiguities, "[{0}] - Match without modifying database." },
			{ Choices.NotAnItem, "[{0}] - Treat this text as not being an item (Nothing will be preformed)" },
		};

		internal readonly Dictionary<string, (Choices choice, string key)> buttons = new Dictionary<string, (Choices choice, string key)>() {
			{ Choices.MatchAnyway.ToString(), (Choices.MatchAnyway, Choices.MatchAnyway.ToString()) },
			{ Choices.MatchWithoutAddingAmbiguities.ToString(), (Choices.MatchWithoutAddingAmbiguities, Choices.MatchWithoutAddingAmbiguities.ToString()) },
			{ Choices.NotAnItem.ToString(), (Choices.NotAnItem, Choices.NotAnItem.ToString()) },
		};

		internal ManualResolveChoice(string error, Choices choice) : this(error, new Choices[] { choice }) { }
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2) : this(error, new Choices[] { choice1, choice2 }) { }
		internal ManualResolveChoice(string error, Choices choice1, Choices choice2, Choices choice3) : this(error, new Choices[] { choice1, choice2, choice3 }) { }

		internal ManualResolveChoice(string error, Choices[] choices) {
			InitializeComponent();
			TextBlock[] solutionTexts = new TextBlock[] { MANUAL_RESOLUTION_Solution1_Text, MANUAL_RESOLUTION_Solution2_Text, MANUAL_RESOLUTION_Solution3_Text, MANUAL_RESOLUTION_Solution4_Text };
			Button[] solutionButtons = new Button[] { MANUAL_RESOLUTION_Resolve1_Button, MANUAL_RESOLUTION_Resolve2_Button, MANUAL_RESOLUTION_Resolve3_Button, MANUAL_RESOLUTION_Resolve4_Button };
			MANUAL_RESOLUTION_ErrorType_Text.Text = error;
			for (int i = 0; i < 4; i++) {
				if (i >= choices.Length || choices[i] == Choices.NOOP) {
					solutionButtons[i].Visibility = Visibility.Collapsed;
					solutionTexts[i].Visibility = Visibility.Collapsed;
				}
				else {
					solutionTexts[i].Text = string.Format(texts[choices[i]],i);
					solutionButtons[i].Click += ManualResolveChoice_Click;
					solutionButtons[i].Name = choices[i].ToString();
					solutionButtons[i].Content = string.Format("Solution {0}", i);
				}
			}
		}

		private Choices selected;

		private void ManualResolveChoice_Click(object sender, RoutedEventArgs e) {
			Enum.TryParse(((Button)sender).Name, out selected);
			evnt.Set();
		}

		internal async Task<Choices> SelectChoice() {
			return await Task.Run(() => {
				evnt.Wait();
				evnt.Reset();
				return selected;
			});
		}
	}
}
