using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DashboardViewModel : Model
{
	GameVars GameVars => Globals.GameVars;

	public string CaptainsLog => Globals.GameVars.currentCaptainsLog;
	public readonly CargoInventoryViewModel WaterInventory;
	public readonly CargoInventoryViewModel FoodInventory;
	public readonly ObservableCollection<CargoInventoryViewModel> CargoList;
	public readonly ObservableCollection<CrewManagementMemberViewModel> CrewList;

	public BoundModel<float> Clout;
	public CrewMember Jason => Globals.GameVars.masterCrewList.FirstOrDefault(c => c.isJason);

	public BoundModel<bool> SailsAreUnfurled { get; private set; }

	public DashboardViewModel() {

		Clout = new BoundModel<float>(Globals.GameVars.playerShipVariables.ship, nameof(Globals.GameVars.playerShipVariables.ship.playerClout));

		var water = Globals.GameVars.playerShipVariables.ship.cargo.FirstOrDefault(r => r.name == Resource.Water);
		WaterInventory = new CargoInventoryViewModel(water);

		var food = Globals.GameVars.playerShipVariables.ship.cargo.FirstOrDefault(r => r.name == Resource.Provisions);
		FoodInventory = new CargoInventoryViewModel(food);

		CargoList = new ObservableCollection<CargoInventoryViewModel>(Globals.GameVars.playerShipVariables.ship.cargo.Select(c => new CargoInventoryViewModel(c)));
		
		CrewList = new ObservableCollection<CrewManagementMemberViewModel>(Globals.GameVars.playerShipVariables.ship.crewRoster
			.OrderBy(c => Globals.GameVars.Network.GetCrewMemberNetwork(c).Count())
			.Select(c => new CrewManagementMemberViewModel(c, OnCrewClicked, OnCrewCityClicked))
		);

		SailsAreUnfurled = new BoundModel<bool>(Globals.GameVars.playerShipVariables.ship, nameof(Globals.GameVars.playerShipVariables.ship.sailsAreUnfurled));

	}

	public void OnCrewCityClicked(CityViewModel city) {
		Debug.Log("City clicked: " + city.PortName);

		if(Globals.UI.IsShown<CityView>()) {
			Globals.UI.Hide<CityView>();
		}

		Globals.GameVars.MoveNavigatorBeacon(Globals.GameVars.crewBeacon, city.City.theGameObject.transform.position);
		Globals.GameVars.RotateCameraTowards(city.City.theGameObject.transform.position);
		Globals.UI.Show<CityView, CityViewModel>(new CityDetailsViewModel(city.City, null));
	}

	public void OnCrewClicked(CrewManagementMemberViewModel crew) {

		if (Globals.UI.IsShown<CityView>()) {
			Globals.UI.Hide<CityView>();
		}

		Globals.UI.Show<CrewDetailsScreen, CrewManagementMemberViewModel>(crew);
	}

	public void GUI_furlOrUnfurlSails() {
		if (GameVars.playerShipVariables.ship.sailsAreUnfurled) {
			GameVars.playerShipVariables.ship.sailsAreUnfurled = false;
			foreach (GameObject sail in GameVars.sails)
				sail.SetActive(false);
		}
		else {
			GameVars.playerShipVariables.ship.sailsAreUnfurled = true;
			foreach (GameObject sail in GameVars.sails)
				sail.SetActive(true);

		}
	}

	public void GUI_dropAnchor() {
		//If the controls are locked--we are traveling so force it to stop
		if (GameVars.controlsLocked && !GameVars.showSettlementGUI)
			GameVars.playerShipVariables.rayCheck_stopShip = true;
	}
}
