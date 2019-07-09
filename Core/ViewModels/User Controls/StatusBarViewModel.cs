using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Igor.Models;

namespace Igor.BillScanner.Core {
	public class StatusBarViewModel : BaseViewModel {

		public StatusBarViewModel() {
			ServerOnlineString = "Offline";
			ClientConnectedString = "No Connection";
			AllShops = new ObservableCollection<Shop>((Shop[])Enum.GetValues(typeof(Shop)));
			LoadShopCommand = new Command(() => {
				MainWindowViewModel.Instance.ShopLoaded = false;
				MainWindowViewModel.Instance.LoadShop(SelectedShop);
				CurrentShop = SelectedShop;
				LoadNewShopControlsVisible = false;
				LoadNewShopSimpleVisible = true;
				MainWindowViewModel.Instance.ShopLoaded = true;
			});
			ShowShopLoadingControlsCommand = new Command(() => {
				LoadNewShopSimpleVisible = false;
				LoadNewShopControlsVisible = true;
			});
		}

		private bool _serverOnline = false;
		private string _serverOnlineString = "";
		private bool _clientConnected = false;
		private string _clientConnectedString = "";
		private Shop _shop;
		private ICommand _onShopClickCommand;
		private ObservableCollection<Shop> _allShops;
		private bool _loadNewShopSimpleVisible = false;

		private ICommand _loadShopCommand;
		private Shop _selectedShop;
		private bool _loadNewShopControlsVisible = true;
		private ICommand _showShopLoadingControlsCommand;

		public ICommand ShowShopLoadingControlsCommand { get => _showShopLoadingControlsCommand; set { _showShopLoadingControlsCommand = value; Notify(nameof(ShowShopLoadingControlsCommand)); } }
		public bool LoadNewShopSimpleVisible { get => _loadNewShopSimpleVisible; set { _loadNewShopSimpleVisible = value; Notify(nameof(LoadNewShopSimpleVisible)); } }
		public bool IsShopLoaded => CurrentShop != Shop.None;

		public bool LoadNewShopControlsVisible { get => _loadNewShopControlsVisible; set { _loadNewShopControlsVisible = value; Notify(nameof(LoadNewShopControlsVisible)); } }
		public Shop SelectedShop { get => _selectedShop; set { _selectedShop = value; Notify(nameof(SelectedShop)); } }
		public ICommand LoadShopCommand { get => _loadShopCommand; set { _loadShopCommand = value; Notify(nameof(LoadShopCommand)); } }

		public ObservableCollection<Shop> AllShops { get => _allShops; set { _allShops = value; Notify(nameof(AllShops)); } }
		public ICommand OnShopClickCommand { get => _onShopClickCommand; set { _onShopClickCommand = value; Notify(nameof(OnShopClickCommand)); } }
		public bool ServerOnline { get => _serverOnline; set { _serverOnline = value; ServerOnlineString = value ? "Online" : "Offline"; } }
		public string ServerOnlineString { get => _serverOnlineString; set { _serverOnlineString = value; Notify(nameof(ServerOnlineString)); } }
		public bool ClientConnected { get => _clientConnected; set { _clientConnected = value; Notify(nameof(ClientConnected)); ClientConnectedString = value ? "Client Connected" : "No Connection"; } }
		public string ClientConnectedString { get => _clientConnectedString; set { _clientConnectedString = value; Notify(nameof(ClientConnectedString)); } }
		public Shop CurrentShop { get => _shop; set { _shop = value; Notify(nameof(CurrentShop)); } }
	}
}
