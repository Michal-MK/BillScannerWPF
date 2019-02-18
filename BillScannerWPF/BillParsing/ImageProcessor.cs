using BillScannerCore;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tesseract;

namespace BillScannerWPF {

	/// <summary>
	/// Contains functionality to scan bills and return their content as a string array.
	/// </summary>
	internal class ImageProcessor : IDisposable {

		private readonly TesseractEngine engine;
		private readonly Rules.IRuleset ruleset;

		/// <summary>
		/// Static reference to the <see cref="ImageProcessor"/> and its content
		/// </summary>
		public static ImageProcessor instance { get; private set; }

		/// <summary>
		/// Bound container that holds all matched <see cref="UIItem"/>s
		/// </summary>
		internal ObservableCollection<UIItem> uiItemsMatched = new ObservableCollection<UIItem>();

		/// <summary>
		/// Bound container that holds all unknown <see cref="UIItem"/>s
		/// </summary>
		internal ObservableCollection<UIItem> uiItemsUnknown = new ObservableCollection<UIItem>();

		/// <summary>
		/// Parse results and other information from current scan
		/// </summary>
		internal ParsingResult currentParsingResult { get; private set; }

		/// <summary>
		/// Create new <see cref="ImageProcessor"/> with needed references
		/// </summary>
		/// <param name="access">Reference to database IO</param>
		/// <param name="ruleset">Reference to selected <see cref="Shop"/>'s rules</param>
		/// <param name="main">Reference to main application window</param>
		internal ImageProcessor(Rules.IRuleset ruleset, MainWindow main) {
			instance = this;
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
			this.ruleset = ruleset;

			main.MAIN_MatchedItems_Stack.ItemsSource = uiItemsMatched;
			main.MAIN_UnknownProducts_Stack.ItemsSource = uiItemsUnknown;
		}

		/// <summary>
		/// Event for receiving image data from a mobile device
		/// </summary>
		internal void OnImageDataReceived(byte sender, byte[] imageData) {
			Application.Current.Dispatcher.Invoke(() => {
				((MainWindow)App.Current.MainWindow).SetPrevImage(imageData);
			});
		}

		/// <summary>
		/// Main function that starts image analysis
		/// </summary>
		internal async void Analyze(object sender, RoutedEventArgs e) {
			Button bSender = (Button)sender;
			if (((MainWindow)App.Current.MainWindow).currentImageSource == null) {
				return;
			}

			bSender.IsEnabled = false;
			uiItemsMatched.Clear();
			uiItemsUnknown.Clear();

			string[] lines = GetOCRLines();
			ParsingResult result = null;
			StringParser parser = new StringParser(ruleset);

			try {
				result = await parser.ParseAsync(lines);
			}
			catch (ParsingEntryNotFoundException) {
				parser.tryMatchFromBeginning = true;
				result = await parser.ParseAsync(lines);
			}
			ConstructUI(result.matchedItems, uiItemsMatched);
			ConstructUI(result.unknownItems, uiItemsUnknown);
			currentParsingResult = result;

			bSender.IsEnabled = true;
		}

		private string[] GetOCRLines() {
			using (Tesseract.Page p = engine.Process((Bitmap)Bitmap.FromFile(((MainWindow)App.Current.MainWindow).currentImageSource), PageSegMode.Auto)) {
				string[] split = p.GetText().Split('\n');
				string[] ready = split.Where((s) => { return !string.IsNullOrWhiteSpace(s); }).ToArray();
				return ready;
			}
		}

		private void ConstructUI(ObservableCollection<UIItemCreationInfo> from, ObservableCollection<UIItem> destination) {
			foreach (UIItemCreationInfo info in from) {
				UIItem item = new UIItem(info, info.quantity, info.quality);
				item.isRegistered = info.isRegistered;
				destination.Add(item);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				engine.Dispose();
				disposedValue = true;
			}
		}

		~ImageProcessor() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
