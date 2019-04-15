using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
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

		public event EventHandler<ParsingCompleteEventArgs> OnImageParsed;

		/// <summary>
		/// Static reference to the <see cref="ImageProcessor"/> and its content
		/// </summary>
		public static ImageProcessor Instance { get; private set; }

		/// <summary>
		/// Bound container that holds all matched <see cref="UIItem"/>s
		/// </summary>
		internal List<UIItemCreationInfo> UIItemsMatched = new List<UIItemCreationInfo>();

		/// <summary>
		/// Bound container that holds all unknown <see cref="UIItem"/>s
		/// </summary>
		internal List<UIItemCreationInfo> UIItemsUnknown = new List<UIItemCreationInfo>();

		/// <summary>
		/// Parse results and other information from current scan
		/// </summary>
		public ParsingResult CurrentParsingResult { get; private set; }

		/// <summary>
		/// Create new <see cref="ImageProcessor"/> with needed references
		/// </summary>
		/// <param name="ruleset">Reference to selected <see cref="Shop"/>'s rules</param>
		public ImageProcessor(IRuleset ruleset) {
			Instance = this;
			_engine = new TesseractEngine("Resources" + Path.DirectorySeparatorChar + "tessdata", "ces");
			_ruleset = ruleset;
		}

		/// <summary>
		/// Main function that starts image analysis
		/// </summary>
		public async void Analyze(string imagePath) {
			UIItemsMatched.Clear();
			UIItemsUnknown.Clear();

			string[] lines = GetOCRLines(imagePath);
			StringParser parser = new StringParser(_ruleset);

			CurrentParsingResult = await parser.ParseAsync(lines);
			OnImageParsed?.Invoke(this, new ParsingCompleteEventArgs(CurrentParsingResult));
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
