using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CityDetailsViewModel : CityViewModel
{
	public readonly ICollectionModel<CrewManagementMemberViewModel> Crew;
	public readonly ICollectionModel<CargoInventoryViewModel> Buy;
	public readonly ICollectionModel<CargoInventoryViewModel> Sell;

	class PriceInfo
	{
		public Resource Resource;
		public int Price;
		public int AvgPrice;
	}

	IEnumerable<PriceInfo> PriceInfos => City.cargo
				.Select(resource => new PriceInfo {
					Resource = resource,
					Price = GameVars.Trade.GetPriceOfResource(resource.name, City),
					AvgPrice = GameVars.Trade.GetAvgPriceOfResource(resource.name)
				})
				.ToArray();

	public CityDetailsViewModel(Settlement city, Action<CityViewModel> onClick) : base(city, onClick) {

		Crew = ValueModel.Wrap(new ObservableCollection<CrewManagementMemberViewModel>(
			GameVars.Network.CrewMembersWithNetwork(city, true)
				.OrderBy(c => GameVars.Network.GetCrewMemberNetwork(c).Count())
				.Take(5)
				.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, OnCrewCityClicked))
		));

		Buy = ValueModel.Wrap(new ObservableCollection<CargoInventoryViewModel>(
			PriceInfos
				.OrderBy(o => o.Price - o.AvgPrice)
				.Take(5)
				.Select(o => new CargoInventoryViewModel(o.Resource))
		));

		Sell = ValueModel.Wrap(new ObservableCollection<CargoInventoryViewModel>(
			PriceInfos
				.OrderByDescending(o => o.Price - o.AvgPrice)
				.Take(5)
				.Select(o => new CargoInventoryViewModel(o.Resource))
		));

	}

	void OnCrewClicked(CrewManagementMemberViewModel crew) {

		// hide a previous details view if one was already showing so they don't stack on top of eachother and confuse the user
		Globals.UI.Hide<CrewDetailsScreen>();
		Globals.UI.Show<CrewDetailsScreen, CrewManagementMemberViewModel>(crew);

	}

	// TODO: Yikes. I copied this from DashboardViewModel
	public void OnCrewCityClicked(CityViewModel city) {
		Debug.Log("City clicked: " + city.PortName);

		if (Globals.UI.IsShown<CityView>()) {
			Globals.UI.Hide<CityView>();
		}

		var beacon = Globals.GameVars.crewBeacon;
		if (city.City != beacon.Target) {
			beacon.Target = city.City;
			Globals.GameVars.ActivateNavigatorBeacon(Globals.GameVars.crewBeacon, city.City.theGameObject.transform.position);
			Globals.GameVars.RotateCameraTowards(city.City.theGameObject.transform.position);
			Globals.UI.Show<CityView, CityViewModel>(new CityDetailsViewModel(city.City, null));
		}
		else {
			beacon.IsBeaconActive = false;
		}
	}
}

public class CityViewModel : Model
{
	protected GameVars GameVars { get; private set; }
	public Settlement City { get; private set; }

	public string PortName => City.name;
	public string RegionName => City.Region.Name;
	public string PortDescription => City.description;

	public float Distance => Vector3.Distance(City.theGameObject.transform.position, GameVars.playerShip.transform.position);

	private Action<CityViewModel> _OnClick;
	public Action<CityViewModel> OnClick { get => _OnClick; set { _OnClick = value; Notify(); } }

	public Sprite PortIcon {
		get {
			Sprite currentBGTex = Resources.Load<Sprite>("settlement_portraits/" + City.settlementID);

			//Now test if it exists, if the settlement does not have a matching texture, then default to a basic one
			if (currentBGTex) return currentBGTex;
			else return Resources.Load<Sprite>("settlement_portraits/gui_port_portrait_default");
		}
	}

	public Sprite PortCoin {
		get {
			Sprite currentCoinTex = Resources.Load<Sprite>("settlement_coins/" + City.settlementID);

			//Now test if it exists, if the settlement does not have a matching texture, then default to a basic one
			if (currentCoinTex) return currentCoinTex;
			else return Resources.Load<Sprite>("settlement_coins/default_coin_texture");
		}
	}

	public string PortPopulationRank {
		get {
			int pop = City.population;
			if (pop >= 0 && pop < 25) return "Population: Village";
			else if (pop >= 25 && pop < 50) return "Population: Town";
			else if (pop >= 50 && pop < 75) return "Population: City";
			else if (pop >= 75 && pop <= 100) return "Population: Metropolis";
			else return "";
		}

	}

	public CityViewModel(Settlement city, Action<CityViewModel> onClick) {
		GameVars = Globals.GameVars;
		City = city;
		OnClick = onClick;
	}

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_Button_TryToLeavePort() {
		//if (GameVars.Trade.CheckIfPlayerCanAffordToPayPortTaxes()) {
			//MGV.controlsLocked = false;
			//Start Our time passage
			GameVars.playerShipVariables.PassTime(.25f, true);
			GameVars.justLeftPort = true;
			//GameVars.playerShipVariables.ship.currency -= GameVars.currentPortTax;

			//Add a new route to the player journey log as a port exit
			GameVars.playerShipVariables.journey.AddRoute(new PlayerRoute(new Vector3(GameVars.playerShip.transform.position.x, GameVars.playerShip.transform.position.y, GameVars.playerShip.transform.position.z), Vector3.zero, GameVars.currentSettlement.settlementID, GameVars.currentSettlement.name, true, GameVars.playerShipVariables.ship.totalNumOfDaysTraveled), GameVars.playerShipVariables, GameVars.CaptainsLog);
			//We should also update the ghost trail with this route otherwise itp roduce an empty 0,0,0 position later
			GameVars.playerShipVariables.UpdatePlayerGhostRouteLineRenderer(GameVars.IS_NOT_NEW_GAME);

			//Turn off the coin image texture
			GameVars.menuControlsLock = false;

			GameVars.showSettlementGUI = false;
			GameVars.runningMainGameGUI = true;

			Globals.UI.Hide<PortScreen>();
			Globals.UI.Hide<TownScreen>();
			Globals.UI.Show<Dashboard, DashboardViewModel>(new DashboardViewModel());

		//}
		//else {//Debug.Log ("Not Enough Drachma to Leave the Port!");
		//	GameVars.ShowANotificationMessage("Not Enough Drachma to pay the port tax and leave!");
		//}
	}
}
