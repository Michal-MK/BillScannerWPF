using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Igor.BillScanner.Core.Rules;

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

			Finalize = new ButtonCommand((obj) => {
				Purchase purchase = new Purchase(SelectedShopRuleset.Shop, ImgProcessing.CurrentParsingResult.Meta.PurchasedAt,
					_matchedItems.Select(s => new ItemPurchaseData(s.Item, s.AmountPurchased)).ToArray());
				purchase.FinalizePurchase();
				ClearButtonVisible = true;
				FinalizeButtonVisible = false;
			});

			Clear = new ButtonCommand((obj) => {
				ImageSource = WPFHelper.resourcesPath + "Transparent.png";
				FinalizeButtonVisible = false;
				ClearButtonVisible = false;
				ManualPurchaseButtonVisible = true;
				AnalyzeButtonVisible = true;
				UnknownItems.Clear();
				MatchedItems.Clear();
			});

			Analyze = new ButtonCommand((obj) => {
				if (string.IsNullOrEmpty(ImageSource) || ImageSource == WPFHelper.resourcesPath + "Transparent.png") {
					return;
				}
				ImgProcessing.Analyze(ImageSource);
			});
			Instance = this;

			StatusBarViewModel.CurrentShop = SelectedShopRuleset.Shop;
		}

		public MainWindowViewModel(Shop shop) : this() {
			SelectedShopRuleset = BaseRuleset.GetRuleset(shop);
		}

		public object Test { get; set; }

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

		#endregion


		#region Properties

		public ICommand Finalize { get => _finalize; set { _finalize = value; Notify(nameof(Finalize)); } }
		public ICommand Analyze { get => _analyze; set { _analyze = value; Notify(nameof(Analyze)); } }
		public ICommand Clear { get => _clear; set { _clear = value; Notify(nameof(Clear)); } }
		public ICommand ManualPurchase { get => _manualPurchase; set { _manualPurchase = value; Notify(nameof(ManualPurchase)); } }


		public bool FinalizeButtonVisible { get => _finalizeButtonVisible; set { _finalizeButtonVisible = value; Notify(nameof(FinalizeButtonVisible)); } }
		public bool AnalyzeButtonVisible { get => _analyzeButtonVisible; set { _analyzeButtonVisible = value; Notify(nameof(AnalyzeButtonVisible)); } }
		public bool ClearButtonVisible { get => _clearButtonVisible; set { _clearButtonVisible = value; Notify(nameof(ClearButtonVisible)); } }
		public bool ManualPurchaseButtonVisible { get => _manualPurchaseButtonVisible; set { _manualPurchaseButtonVisible = value; Notify(nameof(ManualPurchaseButtonVisible)); } }


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
			MatchedItems = e.Result.MachedItems.Select(s => new UIItemViewModel(s)).ToObservable();
			UnknownItems = e.Result.UnknownItems.Select(s => new UIItemViewModel(s)).ToObservable();
			AnalyzeButtonVisible = false;
			FinalizeButtonVisible = true;
			ClearButtonVisible = true;
		}

		#endregion
	}
}
