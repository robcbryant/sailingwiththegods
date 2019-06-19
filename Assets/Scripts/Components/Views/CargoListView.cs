using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class CargoListView : ListView<ObservableCollection<CargoInventoryViewModel>, CargoInventoryViewModel>
{
	[SerializeField] ButtonView CloseButton = null;

	public override void Bind(ObservableCollection<CargoInventoryViewModel> model) {
		base.Bind(model);

		CloseButton?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => Globals.UI.Hide(this)
		}));
	}
}
