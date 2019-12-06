using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Network
{
	const int INDEPENDENT = 0;

	GameVars GameVars;
	Ship Ship => GameVars.playerShipVariables.ship;

	public Network(GameVars gameVars) {
		GameVars = gameVars;
	}

	public bool CheckForNetworkMatchBetweenTwoSettlements(int cityA, int cityB) {
		Settlement cityAObj = GameVars.GetSettlementFromID(cityA);
		Settlement cityBObj = GameVars.GetSettlementFromID(cityB);
		foreach (int cityA_ID in cityAObj.networks) {
			foreach (int cityB_ID in cityBObj.networks) {
				if (cityA_ID == cityB_ID && cityA_ID != INDEPENDENT) {
					return true;
				}
			}
		}
		return false;
	}

	public IEnumerable<Settlement> GetCitiesFromNetwork(int netId) => Globals.GameVars.settlement_masterList.Where(s => s.networks.Contains(netId));

	public IEnumerable<Settlement> MyImmediateNetwork => Ship.networks
		.Where(netId => netId != INDEPENDENT)
		.SelectMany(netId => GetCitiesFromNetwork(netId))
		.Concat(new[] { GameVars.GetSettlementFromID(Ship.originSettlement) });

	public IEnumerable<Settlement> GetCrewMemberNetwork(CrewMember crew) =>
		Globals.GameVars.GetSettlementFromID(crew.originCity).networks
			.Where(netId => netId != INDEPENDENT)
			.SelectMany(netId => GetCitiesFromNetwork(netId))
			.Concat(new[] { GameVars.GetSettlementFromID(crew.originCity) });

	public IEnumerable<Settlement> MyCompleteNetwork => Ship.crewRoster
		.SelectMany(crew => GetCrewMemberNetwork(crew))
		.Concat(MyImmediateNetwork);

	public IEnumerable<CrewMember> CrewMembersWithNetwork(Settlement settlement, bool includeJason = false) {
		var list = Ship.crewRoster.Where(crew => GetCrewMemberNetwork(crew).Contains(settlement));
		if (includeJason && GetCrewMemberNetwork(GameVars.Jason).Contains(settlement)) {
			list = list.Concat(new[] { GameVars.Jason });
		}
		return list;
	}

	public CrewMember CrewMemberWithNetwork(Settlement settlement) => CrewMembersWithNetwork(settlement).FirstOrDefault();

	public bool CheckIfCityIDIsPartOfNetwork(int cityID) {
		var settlement = GameVars.GetSettlementFromID(cityID);
		return settlement != null && MyCompleteNetwork.Contains(settlement);
	}
}
