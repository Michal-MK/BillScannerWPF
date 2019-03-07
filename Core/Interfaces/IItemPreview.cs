using System.Collections.Generic;

namespace Igor.BillScanner.Core {

	public interface IItemPreview {
		void ConstructUI(bool match, IEnumerable<UIItemCreationInfo> creation);

		void Clear();

		void SetPreviewImage(byte[] data);
	}
}