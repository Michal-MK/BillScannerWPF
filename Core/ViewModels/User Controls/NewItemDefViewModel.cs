using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace Igor.BillScanner.Core {
	public class NewItemDefViewModel : BaseViewModel {

		public NewItemDefViewModel() {
			MeassurementUnits = ((MeassurementUnit[])Enum.GetValues(typeof(MeassurementUnit))).ToObservable();
		}

		private ICommand _registerItemCommand;
		private MeassurementUnit _selectedMeassureUnit;
		private ICommand _abortRegistrationCommand;
		private string _itemName = "";
		private string _itemValue = "";
		private ObservableCollection<MeassurementUnit> _meassurementUnits;

		public Func<string, bool> IsValid => IsValidDecimal;

		public int? DatabaseItemValue {
			get {
				if (IsValid(ItemValue))
					return 100 * (int)decimal.Parse(ItemValue, NumberStyles.Currency | NumberStyles.AllowDecimalPoint);
				return null;
			}
		}
		public ObservableCollection<MeassurementUnit> MeassurementUnits { get => _meassurementUnits; set { _meassurementUnits = value; Notify(nameof(MeassurementUnits)); } }
		public string ItemName { get => _itemName; set { _itemName = value; Notify(nameof(ItemName)); } }
		public string ItemValue { get => _itemValue;
			set { _itemValue = value; Notify(nameof(ItemValue)); } }
		public ICommand AbortRegistrationCommand { get => _abortRegistrationCommand; set { _abortRegistrationCommand = value; Notify(nameof(AbortRegistrationCommand)); } }
		public MeassurementUnit SelectedMeassureUnit { get => _selectedMeassureUnit; set { _selectedMeassureUnit = value; Notify(nameof(SelectedMeassureUnit)); } }
		public ICommand RegisterItemCommand { get => _registerItemCommand; set { _registerItemCommand = value; Notify(nameof(RegisterItemCommand)); } }
		public int AssignedID { get; set; }

		public bool DefinitionSuccess {
			get {
				if (IsValid(ItemValue)) {
					AssignedID = DatabaseAccess.Access.WriteItemDefinitionToDatabase(new Item(ItemName, 100 * (int)decimal.Parse(ItemValue, NumberStyles.Currency | NumberStyles.AllowDecimalPoint)), DateTime.Now);
					return true;
				}
				return false;
			}
		}

		private bool IsValidDecimal(string text) {
			NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.Currency;
			return decimal.TryParse(text, style, CultureInfo.InvariantCulture, out _);
		}
	}
}
