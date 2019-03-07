using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Igor.BillScanner.Core.Rules;
using Tesseract;

namespace Igor.BillScanner.Core {

	/// <summary>
	/// Contains functionality to scan bills and return their content as a string array.
	/// </summary>
	public class ImageProcessor : IDisposable {

		private readonly TesseractEngine _engine;
		private readonly IRuleset _ruleset;

		/// <summary>
		/// Static reference to the <see cref="ImageProcessor"/> and its content
		/// </summary>
		public static ImageProcessor Instance { get; private set; }

		/// <summary>
		/// Bound container that holds all matched <see cref="UIItem"/>s
		/// </summary>
		internal ObservableCollection<UIItemCreationInfo> UIItemsMatched = new ObservableCollection<UIItemCreationInfo>();

		/// <summary>
		/// Bound container that holds all unknown <see cref="UIItem"/>s
		/// </summary>
		internal ObservableCollection<UIItemCreationInfo> UIItemsUnknown = new ObservableCollection<UIItemCreationInfo>();

		/// <summary>
		/// Parse results and other information from current scan
		/// </summary>
		public ParsingResult CurrentParsingResult { get; private set; }

		internal IItemPreview preview { get; }

		/// <summary>
		/// Create new <see cref="ImageProcessor"/> with needed references
		/// </summary>
		/// <param name="access">Reference to database IO</param>
		/// <param name="ruleset">Reference to selected <see cref="Shop"/>'s rules</param>
		/// <param name="main">Reference to main application window</param>
		public ImageProcessor(IRuleset ruleset, IItemPreview itemPrev) {
			Instance = this;
			_engine = new TesseractEngine("Resources" + System.IO.Path.DirectorySeparatorChar + "tessdata", "ces");
			_ruleset = ruleset;
			preview = itemPrev;
		}

		/// <summary>
		/// Event for receiving image data from a mobile device
		/// </summary>
		public void OnImageDataReceived(byte sender, byte[] imageData) {
			Services.Instance.UIThread.Run(() => {
				preview.SetPreviewImage(imageData);
			});
		}

		/// <summary>
		/// Main function that starts image analysis
		/// </summary>
		public async void Analyze(string imagePath) {
			UIItemsMatched.Clear();
			UIItemsUnknown.Clear();

			string[] lines = GetOCRLines(imagePath);
			ParsingResult result = null;
			StringParser parser = new StringParser(_ruleset);

			result = await parser.ParseAsync(lines);


			preview.ConstructUI(true, result.MachedItems);
			preview.ConstructUI(false, result.UnknownItems);
			CurrentParsingResult = result;
		}

		private string[] GetOCRLines(string imagePath) {
			using (Page p = _engine.Process((Bitmap)Bitmap.FromFile(imagePath), PageSegMode.Auto)) {
				string[] split = p.GetText().Split('\n');
				string[] ready = split.Where((s) => { return !string.IsNullOrWhiteSpace(s); }).ToArray();
				return ready;
			}
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				_engine.Dispose();
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
