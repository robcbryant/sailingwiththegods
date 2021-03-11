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
	[SerializeField] ButtonView AnchorButton = null;
	[SerializeField] ButtonView SailsButton = null;
	[SerializeField] CargoInventoryView FoodInventory = null;
	[SerializeField] CargoInventoryView WaterInventory = null;

	[SerializeField] ButtonView CloutButton = null;
	[SerializeField] SliderView CloutSlider = null;
	[SerializeField] StringView CloutTitle = null;

	[SerializeField] StringView Objective = null;

	// subscreens
	[SerializeField] MessageBoxView CaptainsLogScreen = null;
	[SerializeField] CargoListView CargoList = null;
	[SerializeField] CrewListScreen CrewList = null;
	[SerializeField] Scrollbar CrewListScroll = null;

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
		CrewButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = CrewButtonClick }));

		MainMenuButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Globals.UI.Show<MainMenuScreen, GameViewModel>(new GameViewModel()) }));
		CloutButton.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Globals.UI.Show<CrewDetailsScreen, CrewManagementMemberViewModel>(
			new CrewManagementMemberViewModel(model.Jason, model.OnCrewClicked, model.OnCrewCityClicked)
		)}));

		// TODO: make 5000 max clout a const somewhere
		CloutSlider.Bind(Model.Clout.Select(c => c / 5000f));
		CloutTitle.Bind(ValueModel.Wrap(Model.Clout)
			.Select(c => Globals.GameVars.GetCloutTitleEquivalency((int)c)));

		FoodInventory.Bind(Model.FoodInventory);
		WaterInventory.Bind(Model.WaterInventory);

		AnchorButton?.Bind(ValueModel.New(new ButtonViewModel { OnClick = Model.GUI_dropAnchor }));
		SailsButton?.Bind(model.SailsAreUnfurled.Select(b => new ButtonViewModel {
			Label = b ? "Furl Sails" : "Unfurl Sails",
			OnClick = model.GUI_furlOrUnfurlSails
		}));

		Objective.Bind(Model.Objective);
	}

	private void CrewButtonClick() {
		Globals.UI.Show(CrewList, Model.CrewList);
		CrewListScroll.value = 0;
	}
}
