using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class CargoListView : ListView<ObservableCollection<CargoInventoryViewModel>, CargoInventoryViewModel>
{
	[SerializeField] Button CloseButton;

	private void Start() {
		Subscribe(CloseButton.onClick, () => Globals.UI.Hide(this));
	}

	protected override bool Filter(CargoInventoryViewModel item) {
		return item.AmountKg > 0;
	}
}
