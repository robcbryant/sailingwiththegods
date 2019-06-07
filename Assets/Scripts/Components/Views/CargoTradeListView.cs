using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class CargoTradeListView : ListView<ObservableCollection<CargoItemTradeViewModel>, CargoItemTradeViewModel>
{
	protected override bool Filter(CargoItemTradeViewModel item) {
		return item.AmountKg > 0;
	}
}
