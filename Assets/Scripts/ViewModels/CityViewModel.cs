using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CityDetailsViewModel : CityViewModel
{
	public readonly ObservableCollection<CrewManagementMemberViewModel> Crew;
	public readonly ObservableCollection<CargoInventoryViewModel> Buy;
	public readonly ObservableCollection<CargoInventoryViewModel> Sell;

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

		Crew = new ObservableCollection<CrewManagementMemberViewModel>(
			GameVars.Network.CrewMembersWithNetwork(city)
				.OrderBy(c => GameVars.Network.GetCrewMemberNetwork(c).Count())
				.Take(5)
				.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, null))
		);

		Buy = new ObservableCollection<CargoInventoryViewModel>(
			PriceInfos
				.OrderBy(o => o.Price - o.AvgPrice)
				.Take(5)
				.Select(o => new CargoInventoryViewModel(o.Resource))
		);

		Sell = new ObservableCollection<CargoInventoryViewModel>(
			PriceInfos
				.OrderByDescending(o => o.Price - o.AvgPrice)
				.Take(5)
				.Select(o => new CargoInventoryViewModel(o.Resource))
		);

	}

	void OnCrewClicked(CrewManagementMemberViewModel crew) {

		// hide a previous details view if one was already showing so they don't stack on top of eachother and confuse the user
		Globals.UI.Hide<CrewDetailsScreen>();
		Globals.UI.Show<CrewDetailsScreen, CrewManagementMemberViewModel>(crew);

	}
}

public class CityViewModel : Model
{
	protected GameVars GameVars { get; private set; }
	public Settlement City { get; private set; }

	public string PortName => City.name;
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
}
