using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Igor.BillScanner.Core.Rules;
using Igor.CPC;
using Igor.Models;

namespace Igor.BillScanner.Core {
	public class MainWindowViewModel : BaseViewModel {

		public static MainWindowViewModel Instance { get; private set; }

		public MainWindowViewModel() {
			Services.Instance.AddMainWindowModel(this);
			Services.Instance.AddManualUserInput(ManualResolveViewModel);

			if (SelectedShopRuleset == null)
				SelectedShopRuleset = BaseRuleset.GetRuleset(Shop.Lidl);

			ImgProcessing = new ImageProcessor(SelectedShopRuleset);
			Services.Instance.ServerHandler.StartServer();

			Services.Instance.ServerHandler.OnImageReceived += (s, e) => { ImageSource = e; };
			ImgProcessing.OnImageParsed += OnImageParsed;

			Finalize = new ParametrizedCommand((obj) => {
				Purchase purchase = new Purchase(SelectedShopRuleset.Shop, ImgProcessing.CurrentParsingResult.Meta.PurchasedAt,
					_matchedItems.Select(s => s.ItemPurchase).ToArray());
				purchase.FinalizePurchase();
				ClearButtonVisible = true;
				FinalizeButtonVisible = false;
			});

			Clear = new ParametrizedCommand((obj) => {
				ImageSource = WPFHelper.resourcesPath + "Transparent.png";
				FinalizeButtonVisible = false;
				SendToMTDBButtonVisible = false;
				ClearButtonVisible = false;
				ManualPurchaseButtonVisible = true;
				AnalyzeButtonVisible = true;
				UnknownItems.Clear();
				MatchedItems.Clear();
			});

			Analyze = new ParametrizedCommand((obj) => {
				if (string.IsNullOrEmpty(ImageSource) || ImageSource == WPFHelper.resourcesPath + "Transparent.png") {
					return;
				}
				_ = ImgProcessing.Analyze(ImageSource);
			});

			SendToMTDB = new ParametrizedCommand(async (obj) => {
				using (CPCServer server = new CrossProcessCommunication().StartServer(5689)) {
					server.StartListening();
					await server.ListenForClient();
					server.SendString(SenderHelper.GetString(ImgProcessing.CurrentParsingResult));
					server.StopListening();
				}
			});

			OnMouseRightClickImage += (s, e) => {
				if (string.IsNullOrEmpty(ImageSource) || ImageSource == "/Igor.BillScanner.WPF.UI;component/Resources/Transparent.png") {
					return;
				}
				new Process { StartInfo = new ProcessStartInfo(ImageSource) }.Start();
			};

			Instance = this;

			StatusBarViewModel.CurrentShop = SelectedShopRuleset.Shop;
		}


		public MainWindowViewModel(Shop shop) : this() {
			SelectedShopRuleset = BaseRuleset.GetRuleset(shop);
		}

		#region BackingFields

		private ICommand _finalize;
		private ICommand _analyze;
		private ICommand _clear;
		private ICommand _manualPurchase;

		private bool _finalizeButtonVisible = false;
		private bool _analyzeButtonVisible = true;
		private bool _clearButtonVisible = false;
		private bool _manualPurchaseButtonVisible = true;

		private ObservableCollection<UIItemViewModel> _matchedItems = new ObservableCollection<UIItemViewModel>();
		private ObservableCollection<UIItemViewModel> _unknownItems = new ObservableCollection<UIItemViewModel>();

		private StatusBarViewModel _statusBarViewModel = new StatusBarViewModel();
		private ManualResolutionViewModel _manualResolveViewModel = new ManualResolutionViewModel();
		private ItemOverlayViewModel _itemInfoOverlayViewModel = new ItemOverlayViewModel();

		private string _imageSource = "/Igor.BillScanner.WPF.UI;component/Resources/Transparent.png";

		private ICommand _sendToMTDB;
		private bool _sendToMTDBButtonVisible;
		private Action<object, EventArgs> _onMouseLeftClickImage;
		private Action<object, EventArgs> _onMouseRightClickImage;

		#endregion


		#region Properties

		public Action<object, EventArgs> OnMouseRightClickImage { get => _onMouseRightClickImage; set { _onMouseRightClickImage = value; Notify(nameof(OnMouseRightClickImage)); } }
		public Action<object, EventArgs> OnMouseLeftClickImage { get => _onMouseLeftClickImage; set { _onMouseLeftClickImage = value; Notify(nameof(OnMouseLeftClickImage)); } }
		public ICommand SendToMTDB { get => _sendToMTDB; set { _sendToMTDB = value; Notify(nameof(SendToMTDB)); } }
		public ICommand Finalize { get => _finalize; set { _finalize = value; Notify(nameof(Finalize)); } }
		public ICommand Analyze { get => _analyze; set { _analyze = value; Notify(nameof(Analyze)); } }
		public ICommand Clear { get => _clear; set { _clear = value; Notify(nameof(Clear)); } }
		public ICommand ManualPurchase { get => _manualPurchase; set { _manualPurchase = value; Notify(nameof(ManualPurchase)); } }


		public bool FinalizeButtonVisible { get => _finalizeButtonVisible; set { _finalizeButtonVisible = value; Notify(nameof(FinalizeButtonVisible)); } }
		public bool AnalyzeButtonVisible { get => _analyzeButtonVisible; set { _analyzeButtonVisible = value; Notify(nameof(AnalyzeButtonVisible)); } }
		public bool ClearButtonVisible { get => _clearButtonVisible; set { _clearButtonVisible = value; Notify(nameof(ClearButtonVisible)); } }
		public bool ManualPurchaseButtonVisible { get => _manualPurchaseButtonVisible; set { _manualPurchaseButtonVisible = value; Notify(nameof(ManualPurchaseButtonVisible)); } }
		public bool SendToMTDBButtonVisible { get => _sendToMTDBButtonVisible; set { _sendToMTDBButtonVisible = value; Notify(nameof(SendToMTDBButtonVisible)); } }

		public string ImageSource { get => _imageSource; set { _imageSource = value; Notify(nameof(ImageSource)); } }


		public ObservableCollection<UIItemViewModel> MatchedItems { get => _matchedItems; set { _matchedItems = value; Notify(nameof(MatchedItems)); } }
		public ObservableCollection<UIItemViewModel> UnknownItems { get => _unknownItems; set { _unknownItems = value; Notify(nameof(UnknownItems)); } }


		public ItemOverlayViewModel ItemInfoOverlayViewModel { get => _itemInfoOverlayViewModel; set { _itemInfoOverlayViewModel = value; Notify(nameof(ItemInfoOverlayViewModel)); } }
		public ManualResolutionViewModel ManualResolveViewModel { get => _manualResolveViewModel; set { _manualResolveViewModel = value; Notify(nameof(ManualResolveViewModel)); } }
		public StatusBarViewModel StatusBarViewModel { get => _statusBarViewModel; set { _statusBarViewModel = value; Notify(nameof(StatusBarViewModel)); } }

		#endregion

		/// <summary>
		/// The rule-set selected at the launch of the application
		/// </summary>
		public IRuleset SelectedShopRuleset { get; }

		/// <summary>
		/// Image processing class that does the scanning and subsequent parsing of the image
		/// </summary>
		public ImageProcessor ImgProcessing { get; private set; }

		#region Actions

		private void OnImageParsed(object sender, ParsingCompleteEventArgs e) {
			MatchedItems = e.Result.MachedItems.ToObservable();
			UnknownItems = e.Result.UnknownItems.ToObservable();
			AnalyzeButtonVisible = false;
			FinalizeButtonVisible = true;
			SendToMTDBButtonVisible = true;
			ClearButtonVisible = true;
		}

		#endregion
	}
}
