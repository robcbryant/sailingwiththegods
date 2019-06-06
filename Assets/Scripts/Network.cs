using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Network
{
	Ship Ship;

	public Network(Ship ship) {
		Ship = ship;
	}

	public IEnumerator<Settlement> GetCitiesFromNetwork(int netId) => 

	public IEnumerator<Settlement> GetMyImmediateNetwork() {
		Ship.networks.Select()
	}

	public IEnumerator<Settlement> GetCrewMemberImmediateNetwork() {

	}

	public IEnumerator<Settlement> GetMyCompleteNetwork() {

	}

	public CrewMember crewMemberWithNetwork => playerShipVariables.ship.crewRoster
		.FirstOrDefault(c => )

	public bool CheckIfCityIDIsPartOfNetwork(int cityID) {
		//Debug.Log ("CITY PART OF NETWORK CEHCK: " + cityID);
		Settlement thisSettlement = GetSettlementFromID(cityID);
		int INDEPENDENT = 0;//Settlements who don't belong to a network are independent
							//Debug.Log ("DEBUG ID CHECK 2: " + thisSettlement.name);
							//First check if the player is part of the network
		foreach (int playerNetID in playerShipVariables.ship.networks) {
			//Debug.Log ("DEBUG ID CHECK: " + playerShipVariables.ship.networks);
			foreach (int cityNetID in thisSettlement.networks) {
				if (playerNetID == cityNetID && cityNetID != INDEPENDENT) {
					crewMemberWithNetwork = null;
					return true;
				}
			}
		}
		//Check if this is the player's hometown
		if (cityID == playerShipVariables.ship.originSettlement)
			return true;


		//Then Check are any crewmembers are part of the network
		foreach (CrewMember thisCrewMember in playerShipVariables.ship.crewRoster) {
			//Debug.Log (thisCrewMember.name);
			Settlement crewOriginCity = GetSettlementFromID(thisCrewMember.originCity);
			if (crewOriginCity.name != "ERROR") {

				//First check if the crewman is part of this towns network
				//	--we have to run through each network in the settlement's list against the networks in the crewman's origin city's list
				//TODO it will probably be useful down the road to ahve a crewman's origin city give extra information / bonuses beyond a network bonus
				foreach (int cityNetID in thisSettlement.networks) {
					foreach (int crewCityNetID in crewOriginCity.networks) {
						if (cityNetID == crewCityNetID && cityNetID != INDEPENDENT) {
							crewMemberWithNetwork = thisCrewMember;
							return true;
						}
					}
				}
			}
			//Check if this is the crewman's hometown
			if (cityID == thisCrewMember.originCity)
				return true;
		}

		//If we don't come up with any matches anywhere then return false
		return false;

	}
}
