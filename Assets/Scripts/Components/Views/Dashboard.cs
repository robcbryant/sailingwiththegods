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
	[SerializeField] ButtonView CaptainsLogButton;
	[SerializeField] ButtonView MainMenuButton;
	[SerializeField] ButtonView CargoButton;
	[SerializeField] ButtonView CrewButton;
	[SerializeField] ButtonView CloutButton;
	[SerializeField] CargoInventoryView FoodInventory;
	[SerializeField] CargoInventoryView WaterInventory;

	// subscreens
	[SerializeField] MessageBoxView CaptainsLogScreen;
	[SerializeField] CargoListView CargoList;

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

		MainMenuButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Debug.Log("Main Menu Clicked") }));
		CrewButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Debug.Log("Crew Clicked") }));
		CloutButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Debug.Log("Clout Clicked") }));

		FoodInventory.Bind(Model.FoodInventory);
		WaterInventory.Bind(Model.WaterInventory);
	}
}
