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

		private readonly DatabaseAccess access;
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
		internal ImageProcessor(DatabaseAccess access, Rules.IRuleset ruleset, MainWindow main) {
			instance = this;
			this.access = access;
			engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
			this.ruleset = ruleset;

			main.MAIN_MatchedItems_Stack.ItemsSource = uiItemsMatched;
			main.MAIN_UnknownProducts_Stack.ItemsSource = uiItemsUnknown;
		}

		/// <summary>
		/// Event for receiving image data from a mobile device
		/// </summary>
		internal void OnImageDataReceived(byte[] imageData, byte sender) {
			Application.Current.Dispatcher.Invoke(() => {
				MainWindow w = WPFHelper.GetMainWindow();
				w.SetPrevImage(imageData);
			});
		}

		/// <summary>
		/// Main function that start image analysis, triggered a UI element
		/// </summary>
		internal async void Analyze(object sender, RoutedEventArgs e) {
			Button bSender = (Button)sender;
			if (WPFHelper.GetMainWindow().currentImageSource == null) {
				return;
			}

			bSender.IsEnabled = false;
			uiItemsMatched.Clear();
			uiItemsUnknown.Clear();

			using (Tesseract.Page p = engine.Process((Bitmap)Bitmap.FromFile(WPFHelper.GetMainWindow().currentImageSource), PageSegMode.Auto)) {
				StringParser instance = new StringParser(ruleset);
				ParsingResult result = null;
				string[] split = p.GetText().Split('\n');
				string[] ready = split.Where((s) => { return !string.IsNullOrWhiteSpace(s); }).ToArray();
				try {
					result = await instance.ParseAsync(ready);
				}
				catch (ParsingEntryNotFoundException) {
					string[] modified = new string[ready.Length + 1];
					modified[0] = ruleset.startMarkers[0];
					ready.CopyTo(modified, 1);
					result = await instance.ParseAsync(modified);
				}
				ConstructUI(result.parsed, uiItemsMatched);
				ConstructUI(result.unknown, uiItemsUnknown);
				currentParsingResult = result;
			}
			bSender.IsEnabled = true;
		}

		private void ConstructUI(ObservableCollection<UIItemCreationInfo> from, ObservableCollection<UIItem> destination) {
			foreach (UIItemCreationInfo info in from) {
				UIItem item = new UIItem(info.item, info.quantity, info.quality);
				item.asociatedItem.isRegistered = info.isRegistered;
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
