using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Igor.BillScanner.Core {

	public interface IItemPreview {
		ObservableCollection<object> ConstructUI(IEnumerable<UIItemCreationInfo> creation);

		void Clear();

		void SetPreviewImage(byte[] data);
	}
}