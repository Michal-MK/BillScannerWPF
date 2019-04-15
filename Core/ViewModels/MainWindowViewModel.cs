﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Igor.BillScanner.Core.Rules;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Igor.BillScanner.Core {
	public class MainWindowViewModel : BaseViewModel {


		public MainWindowViewModel() {
			Services.Instance.AddMainWindowModel(this);

			SelectedShopRuleset = BaseRuleset.GetRuleset(Shop.Lidl);

			ImgProcessing = new ImageProcessor(SelectedShopRuleset);

			Finalize = new ButtonCommand((obj) => {
				Purchase purchase = new Purchase(SelectedShopRuleset.shop, ImgProcessing.CurrentParsingResult.Meta.PurchasedAt,
					_matchedItems.Select(s => new ItemPurchaseData(s.Item, s.AmountPurchased)).ToArray());
				purchase.FinalizePurchase();
				ClearButtonVisible = true;
				FinalizeButtonVisible = false;
			});

			Clear = new ButtonCommand((obj) => {
				ImageSource = WPFHelper.resourcesPath + "Transparent.png";
				FinalizeButtonVisible = true;
				ClearButtonVisible = false;
			});

			Analyze = new ButtonCommand((obj) => {
				if (string.IsNullOrEmpty(ImageSource) || ImageSource == WPFHelper.resourcesPath + "Transparent.png") {
					return;
				}
				ImgProcessing.Analyze(ImageSource);
			});
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

		private ObservableCollection<UIItemViewModel> _matchedItems = new ObservableCollection<UIItemViewModel> { new UIItemViewModel(new UIItemCreationInfo(new Item("Test", 10), 1, 10, MatchRating.One, "test")) };
		private ObservableCollection<UIItemViewModel> _unknownItems = new ObservableCollection<UIItemViewModel> { new UIItemViewModel(new UIItemCreationInfo(new Item("FAIL", 10), 1, 10, MatchRating.Five, "failasd")) };



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



		public event EventHandler OnCoreLoaded;

		#endregion

		/// <summary>
		/// The rule-set selected at the launch of the application
		/// </summary>
		public IRuleset SelectedShopRuleset { get; }

		/// <summary>
		/// Image processing class that does the scanning and subsequent parsing of the image
		/// </summary>
		public ImageProcessor ImgProcessing { get; private set; }



		public StatusBarViewModel StatusBar { get; set; }

		#region Actions

		private void OnImageParsed(object sender, ParsingCompleteEventArgs e) {
			MatchedItems = e.Result.MachedItems.Select(s => new UIItemViewModel(s)).ToObservable();
			UnknownItems = e.Result.UnknownItems.Select(s => new UIItemViewModel(s)).ToObservable();
		}

		public void CoreLoaded() {
			OnCoreLoaded?.Invoke(this, EventArgs.Empty);
			ServerHandler.Instance.OnImageReceived += (s, e) => { ImageSource = e; };
			ImgProcessing.OnImageParsed += OnImageParsed;
		}
		#endregion
	}
}
