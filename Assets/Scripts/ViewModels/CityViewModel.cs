using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

public class CityViewModel : Model
{
	private GameVars GameVars;
	private Settlement City;

	public readonly ObservableCollection<CrewManagementMemberViewModel> Crew;
	public readonly ObservableCollection<CargoInventoryViewModel> Buy;
	public readonly ObservableCollection<CargoInventoryViewModel> Sell;

	public string PortName => City.name;

	private Action<CityViewModel> _OnClick;
	public Action<CityViewModel> OnClick { get => _OnClick; set { _OnClick = value; Notify(); } }

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

	public CityViewModel(Settlement city, Action<CityViewModel> onClick, bool includeCrew) {
		GameVars = Globals.GameVars;
		City = city;
		OnClick = onClick;

		if(includeCrew) {
			Crew = new ObservableCollection<CrewManagementMemberViewModel>(
				GameVars.Network.CrewMembersWithNetwork(city)
					.OrderBy(c => GameVars.Network.GetCrewMemberNetwork(c).Count())
					.Take(5)
					.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, null))
			);
		}

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
