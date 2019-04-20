using System.Threading.Tasks;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class ManualResolutionViewModel : BaseViewModel {

		#region Backing Fields

		private string _header = "The parser encountered a situation where it is unsure what to do!";
		private string _errorText = "Error";

		private string _button1Text;
		private bool _button1Visibility;
		private string _button2Text;
		private bool _button2Visibility;
		private string _button3Text;
		private bool _button3Visibility;

		private ICommand _button1Command;
		private ICommand _button2Command;
		private ICommand _button3Command;
		private ICommand _buttonStdInputCommand;
		private ICommand _buttonDateTime;


		private bool _dateBoxControlVisible;
		private bool _simpleInputControlVisible;

		private string _buttonStdInput;
		private string _buttonDateTimeInput;

		private string _customInputText;
		private string _dateTimeInputText;

		private DateBoxViewModel _dateBoxControlModel;


		#endregion


		public string Header { get => _errorText; set { _errorText = value; Notify(Header); } }
		public string ErrorText { get => _header; set { _header = value; Notify(ErrorText); } }

		public string ButtonOne { get => _button1Text; set { _button1Text = value; Notify(ButtonOne); } }
		public string ButtonTwo { get => _button2Text; set { _button2Text = value; Notify(ButtonTwo); } }
		public string ButtonThree { get => _button3Text; set { _button3Text = value; Notify(ButtonThree); } }


		public bool ButtonOneVisibility { get => _button1Visibility; set { _button1Visibility = value; Notify(nameof(ButtonOneVisibility)); } }
		public bool ButtonTwoVisibility { get => _button2Visibility; set { _button2Visibility = value; Notify(nameof(ButtonTwoVisibility)); } }
		public bool ButtonThreeVisibility { get => _button3Visibility; set { _button3Visibility = value; Notify(nameof(ButtonThreeVisibility)); } }

		public ICommand ButtonOneCommand { get => _button1Command; set { _button1Command = value; Notify(nameof(ButtonOneCommand)); } }
		public ICommand ButtonTwoCommand { get => _button2Command; set { _button2Command = value; Notify(nameof(ButtonTwoCommand)); } }
		public ICommand ButtonThreeCommand { get => _button3Command; set { _button3Command = value; Notify(nameof(ButtonThreeCommand)); } }

		public bool DateBoxControlVisible { get => _dateBoxControlVisible; set { _dateBoxControlVisible = value; Notify(nameof(_dateBoxControlVisible)); } }
		public bool SimpleInputControlVisible { get => _simpleInputControlVisible; set { _simpleInputControlVisible = value; Notify(nameof(SimpleInputControlVisible)); } }


		public string ButtonStdInput { get => _buttonStdInput; set { _buttonStdInput = value; Notify(nameof(ButtonStdInput)); } }
		public string ButtonDateTimeInput { get => _buttonDateTimeInput; set { _buttonDateTimeInput = value; Notify(nameof(ButtonDateTimeInput)); } }


		public string CustomInputText { get => _customInputText; set { _customInputText = value; Notify(nameof(CustomInputText)); } }
		public string DateTimeInputText { get => _dateTimeInputText; set { _dateTimeInputText = value; Notify(nameof(DateTimeInputText)); } }


		public ICommand ButtonStdInputCommand { get => _buttonStdInputCommand; set { _buttonStdInputCommand = value; Notify(nameof(ButtonStdInputCommand)); } }
		public ICommand ButtonDateTimeCommand { get => _buttonDateTime; set { _buttonDateTime = value; Notify(nameof(ButtonDateTimeCommand)); } }

		public DateBoxViewModel DateBoxControlModel { get => _dateBoxControlModel; set { _dateBoxControlModel = value; Notify(nameof(DateBoxControlModel)); } }

		public void MadeSelection() {

		}

		public void SetCommand(int index, string text, ICommand command) {
			if (index == 1) {
				ButtonOneCommand = new Command(() => { MadeSelection(); command.Execute(null); });
				ButtonOne = text;
				ButtonOneVisibility = true;
			}
			else if (index == 2) {
				ButtonTwoCommand = new Command(() => { MadeSelection(); command.Execute(null); });
				ButtonTwo = text;
				ButtonTwoVisibility = true;
			}
			if (index == 3) {
				ButtonThreeCommand = new Command(() => { MadeSelection(); command.Execute(null); });
				ButtonThree = text;
				ButtonThreeVisibility = true;
			}
		}
	}
}
