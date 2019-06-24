using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Igor.Models;

namespace Igor.BillScanner.Core {
	public class ManualResolutionViewModel : BaseViewModel, IManualUserInput {

		#region Backing Fields

		private string _header;
		private string _errorText;

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

		private DateBoxViewModel _dateBoxControlModel = new DateBoxViewModel();
		private bool _controlVisibility;

		private bool _itemListVisible;
		private ItemListViewModel _itemlistViewModel = new ItemListViewModel();
		private bool _itemDefinitionVisible;
		private NewItemDefViewModel _itemDefinitionModel = new NewItemDefViewModel();
		private Action<string> _returnPress;

		#endregion

		public Action<string> ReturnPress { get => _returnPress; set { _returnPress = value; Notify(nameof(ReturnPress)); } }
		public NewItemDefViewModel ItemDefinitionModel { get => _itemDefinitionModel; set { _itemDefinitionModel = value; Notify(nameof(ItemDefinitionModel)); } }
		public bool ItemDefinitionVisible { get => _itemDefinitionVisible; set { _itemDefinitionVisible = value; Notify(nameof(ItemDefinitionVisible)); } }
		public ItemListViewModel ItemlistViewModel { get => _itemlistViewModel; set { _itemlistViewModel = value; Notify(nameof(ItemlistViewModel)); } }
		public bool ItemListVisible { get => _itemListVisible; set { _itemListVisible = value; Notify(nameof(ItemListVisible)); } }
		public bool ControlVisibility { get => _controlVisibility; set { _controlVisibility = value; Notify(nameof(ControlVisibility)); } }
		public string Header { get => _errorText; set { _errorText = value; Notify(nameof(Header)); } }
		public string ErrorText { get => _header; set { _header = value; Notify(nameof(ErrorText)); } }

		public string ButtonOneText { get => _button1Text; set { _button1Text = value; Notify(nameof(ButtonOneText)); } }
		public string ButtonTwoText { get => _button2Text; set { _button2Text = value; Notify(nameof(ButtonTwoText)); } }
		public string ButtonThreeText { get => _button3Text; set { _button3Text = value; Notify(nameof(ButtonThreeText)); } }

		public bool ButtonOneVisibility { get => _button1Visibility; set { _button1Visibility = value; Notify(nameof(ButtonOneVisibility)); } }
		public bool ButtonTwoVisibility { get => _button2Visibility; set { _button2Visibility = value; Notify(nameof(ButtonTwoVisibility)); } }
		public bool ButtonThreeVisibility { get => _button3Visibility; set { _button3Visibility = value; Notify(nameof(ButtonThreeVisibility)); } }

		public ICommand ButtonOneCommand { get => _button1Command; set { _button1Command = value; Notify(nameof(ButtonOneCommand)); } }
		public ICommand ButtonTwoCommand { get => _button2Command; set { _button2Command = value; Notify(nameof(ButtonTwoCommand)); } }
		public ICommand ButtonThreeCommand { get => _button3Command; set { _button3Command = value; Notify(nameof(ButtonThreeCommand)); } }

		public bool DateBoxControlVisible { get => _dateBoxControlVisible; set { _dateBoxControlVisible = value; Notify(nameof(DateBoxControlVisible)); } }
		public bool SimpleInputControlVisible { get => _simpleInputControlVisible; set { _simpleInputControlVisible = value; Notify(nameof(SimpleInputControlVisible)); } }


		public string ButtonStdInput { get => _buttonStdInput; set { _buttonStdInput = value; Notify(nameof(ButtonStdInput)); } }
		public string ButtonDateTimeInput { get => _buttonDateTimeInput; set { _buttonDateTimeInput = value; Notify(nameof(ButtonDateTimeInput)); } }


		public string CustomInputText { get => _customInputText; set { _customInputText = value; Notify(nameof(CustomInputText)); } }
		public string DateTimeInputText { get => _dateTimeInputText; set { _dateTimeInputText = value; Notify(nameof(DateTimeInputText)); } }


		public ICommand ButtonStdInputCommand { get => _buttonStdInputCommand; set { _buttonStdInputCommand = value; Notify(nameof(ButtonStdInputCommand)); } }
		public ICommand ButtonDateTimeCommand { get => _buttonDateTime; set { _buttonDateTime = value; Notify(nameof(ButtonDateTimeCommand)); } }

		public DateBoxViewModel DateBoxControlModel { get => _dateBoxControlModel; set { _dateBoxControlModel = value; Notify(nameof(DateBoxControlModel)); } }

		public void SetCommand(string text, ICommand command) {
			if (ButtonOneCommand == null) {
				ButtonOneCommand = command;
				ButtonOneText = text;
				ButtonOneVisibility = true;
			}
			else if (ButtonTwoCommand == null) {
				ButtonTwoCommand = command;
				ButtonTwoText = text;
				ButtonTwoVisibility = true;
			}
			else if (ButtonThreeCommand == null) {
				ButtonThreeCommand = command;
				ButtonThreeText = text;
				ButtonThreeVisibility = true;
			}
		}

		public void ClearCommands() {
			ButtonOneCommand = null;
			ButtonOneText = "";
			ButtonOneVisibility = false;
			ButtonTwoCommand = null;
			ButtonTwoText = "";
			ButtonTwoVisibility = false;
			ButtonThreeCommand = null;
			ButtonThreeText = "";
			ButtonThreeVisibility = false;
		}


		public async Task<int> GetIntInputAsync(string displayText) {
			using (ManualResetEventSlim evnt = new ManualResetEventSlim()) {
				ControlVisibility = true;
				Header = displayText;
				SimpleInputControlVisible = true;
				ButtonStdInput = "Use my value provided here:";
				int retVal = -1;

				ButtonStdInputCommand = new Command(() => {
					if (int.TryParse(CustomInputText, out retVal)) {
						evnt.Set();
					}
				});
				ReturnPress = (s) => { ButtonStdInputCommand.Execute(null); };

				await Task.Run(evnt.Wait);

				ClearCommands();
				SimpleInputControlVisible = false;
				ControlVisibility = false;
				CustomInputText = "";
				ReturnPress = null;
				return retVal;
			}
		}

		public async Task<string> GetStringInput(string displayText) {
			using (ManualResetEventSlim evnt = new ManualResetEventSlim()) {
				ControlVisibility = true;
				Header = displayText;
				SimpleInputControlVisible = true;
				ButtonStdInput = "GetStringInput: ";
				ButtonStdInputCommand = new Command(() => { evnt.Set(); });
				ReturnPress = (s) => { evnt.Set(); };

				await Task.Run(evnt.Wait);
				ClearCommands();
				SimpleInputControlVisible = false;
				ControlVisibility = false;
				string retVal = CustomInputText;
				CustomInputText = "";
				ReturnPress = null;
				return retVal;
			}
		}

		public async Task PressOneOf(string displayText, params (string, ICommand)[] choices) {
			using (ManualResetEventSlim evnt = new ManualResetEventSlim()) {
				ControlVisibility = true;
				if (choices.Length > 3) {
					throw new Exception("Too many Choices");
				}

				Header = displayText;

				foreach ((string text, ICommand command) in choices) {
					SetCommand(text, new Command(() => {
						command.Execute(null);
						evnt.Set();
					}));
				}
				await Task.Run(evnt.Wait);
				ClearCommands();
				ControlVisibility = false;
				return;
			}
		}

		public async Task<DateTime> GetDateTimeInputAsync(string displayText, bool allowNow = true) {
			using (ManualResetEventSlim evnt = new ManualResetEventSlim()) {
				ControlVisibility = true;
				DateTime? retVal = null;

				DateBoxControlVisible = true;
				Header = displayText;
				ButtonDateTimeCommand = new Command(() => {
					if (DateBoxControlModel.TryGetDate(out DateTime value)) {
						retVal = value;
						evnt.Set();
					}
				});
				ButtonDateTimeInput = "Use my date provided here: ";
				ReturnPress = (s) => { ButtonDateTimeCommand.Execute(null); }; 

				if (allowNow) {
					SetCommand("Use 'Today' as the date of purchase", new Command(() => {
						retVal = DateTime.Now;
						evnt.Set();
					}));
				}
				Notify("DataContext");
				await Task.Run(evnt.Wait);
				ClearCommands();
				DateBoxControlVisible = false;
				DateBoxControlModel.CurrentText = "";
				ControlVisibility = false;
				ReturnPress = null;
				return retVal.Value;
			}
		}

		public async Task<int?> GetDecimalInputAsIntAsync(string displayText, int? knownValue = null) {
			using (ManualResetEventSlim evnt = new ManualResetEventSlim()) {
				ControlVisibility = true;
				int? retVal = null;
				SimpleInputControlVisible = true;
				Header = displayText;
				ButtonStdInput = "Use my value provided here: ";
				NumberStyles style = NumberStyles.Currency | NumberStyles.AllowDecimalPoint;
				ButtonStdInputCommand = new Command(() => {
					if (decimal.TryParse(CustomInputText, style, CultureInfo.InvariantCulture, out decimal dec)) {
						retVal = (int)dec * 100;
						evnt.Set();
					}
				});
				ReturnPress = (s) => { ButtonStdInputCommand.Execute(null); };

				if (knownValue.HasValue) {
					SetCommand($"Use latest value from the database => '{(knownValue.Value / 100m).ToString("0.00")} Kč'", new Command(() => { evnt.Set(); }));
				}

				await Task.Run(evnt.Wait);
				ClearCommands();
				SimpleInputControlVisible = false;
				ControlVisibility = false;
				CustomInputText = "";
				ReturnPress = null;
				return retVal;
			}
		}

		public async Task<Item> SelectOneItemFromListAsync(Item[] items) {
			ControlVisibility = true;
			ItemListVisible = true;

			ItemlistViewModel.ClearItems();
			ItemlistViewModel.AddItems(items);

			ItemList_ItemViewModel selected = await ItemlistViewModel.SelectItemAsync();
			ItemListVisible = false;
			ControlVisibility = false;
			return selected?.Item;
		}

		public async Task<NewItemDefViewModel> DefineNewItemAsync() {
			using (ManualResetEventSlim evnt = new ManualResetEventSlim()) {
				ControlVisibility = true;
				ItemDefinitionVisible = true;
				ItemDefinitionModel.RegisterItemCommand = new Command(() => {
					if (ItemDefinitionModel.DefinitionSuccess) {
						evnt.Set();
					}
				});

				ItemDefinitionModel.AbortRegistrationCommand = new Command(() => {
					evnt.Set();
				});

				await Task.Run(evnt.Wait);

				ItemDefinitionVisible = false;
				ControlVisibility = false;
				return ItemDefinitionModel;
			}
		}
	}
}
