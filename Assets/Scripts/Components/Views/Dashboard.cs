using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : ViewBehaviour<DashboardViewModel>
{
	[SerializeField] ButtonView CaptainsLogButton = null;
	[SerializeField] ButtonView MainMenuButton = null;
	[SerializeField] ButtonView CargoButton = null;
	[SerializeField] ButtonView CrewButton = null;
	[SerializeField] ButtonView CloutButton = null;
	[SerializeField] CargoInventoryView FoodInventory = null;
	[SerializeField] CargoInventoryView WaterInventory = null;

	// subscreens
	[SerializeField] MessageBoxView CaptainsLogScreen = null;
	[SerializeField] CargoListView CargoList = null;
	[SerializeField] CrewListScreen CrewList = null;

	public override void Bind(DashboardViewModel model) {
		base.Bind(model);

		CaptainsLogButton.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => Globals.UI.Show(CaptainsLogScreen,
			new MessageBoxViewModel {
				Message = Model.CaptainsLog,
				Cancel = new ButtonViewModel { Label = "Close", OnClick = () => Globals.UI.Hide(CaptainsLogScreen) }
			})
		}));

		CargoButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Globals.UI.Show(CargoList, Model.CargoList) }));
		CrewButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Globals.UI.Show(CrewList, Model.CrewList) }));

		MainMenuButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Debug.Log("Main Menu Clicked") }));
		CloutButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Debug.Log("Clout Clicked") }));

		FoodInventory.Bind(Model.FoodInventory);
		WaterInventory.Bind(Model.WaterInventory);
	}
}
