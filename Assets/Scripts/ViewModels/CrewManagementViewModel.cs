using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CrewManagementViewModel : Model
{
	private GameVars GameVars;

	public readonly ICollectionModel<CrewManagementMemberViewModel> AvailableCrew;
	public readonly ICollectionModel<CrewManagementMemberViewModel> MyCrew;

	public IValueModel<int> CrewCount;
	public IValueModel<int> CrewCapacity;
	public IValueModel<int> Money;

	Settlement Settlement { get; set; }

	public CrewManagementViewModel(Settlement settlement) {
		GameVars = Globals.GameVars;
		Settlement = settlement;

		Money = new BoundModel<int>(GameVars.playerShipVariables.ship, nameof(GameVars.playerShipVariables.ship.currency));
		CrewCapacity = new BoundModel<int>(GameVars.playerShipVariables.ship, nameof(GameVars.playerShipVariables.ship.crewCapacity));

		CrewCount = ValueModel.Wrap(GameVars.playerShipVariables.ship.crewRoster)
			.Select(c => c.Count());

		AvailableCrew = ValueModel.Wrap(settlement.availableCrew)
			.Where(crew => !GameVars.playerShipVariables.ship.crewRoster.Contains(crew))
			.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, null));

		MyCrew = ValueModel.Wrap(GameVars.playerShipVariables.ship.crewRoster)
			.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, null));
	}

	//=================================================================================================================
	// SETUP THE CREW MANAGEMENT PANEL
	//=================================================================================================================	
	
	private void OnCrewClicked(CrewManagementMemberViewModel crew) {
		if (crew.IsInCrew) {
			GUI_FireCrewMember(crew);
		}
		else {
			GUI_HireCrewMember(crew);
		}
	}

	//----------------------------------------------------------------------------
	//----------------------------CREW PANEL HELPER FUNCTIONS		

	public void GUI_FireCrewMember(CrewManagementMemberViewModel crew) {
		var crewman = crew.Member;

		// remove from your crew first so the where filter for not in your crew applies on add to settlement list
		GameVars.playerShipVariables.ship.crewRoster.Remove(crewman);
		Settlement.availableCrew.Add(crewman);

		GameVars.showNotification = true;
		GameVars.notificationMessage = crewman.name + " looked at you sadly and said before leaving, 'I thought I was doing so well. I'm sorry I let you down. Guess I'll go drink some cheap wine...";
	}


	public void GUI_HireCrewMember(CrewManagementMemberViewModel crew) {
		var crewman = crew.Member;

		//Check to see if player has enough money to hire
		if (GameVars.playerShipVariables.ship.currency >= crew.CostToHire) {
			//Now check to see if there is room to hire a new crew member!
			if (GameVars.playerShipVariables.ship.crewRoster.Count < GameVars.playerShipVariables.ship.crewCapacity) {

				// remove from settlement first so the where filter for not in your crew still applies on remove
				Settlement.availableCrew.Remove(crewman);
				GameVars.playerShipVariables.ship.crewRoster.Add(crewman);

				//Subtract the cost from the ship's money
				GameVars.playerShipVariables.ship.currency -= crew.CostToHire;

				//If there isn't room, then let the player know
			}
			else {
				GameVars.showNotification = true;
				GameVars.notificationMessage = "You don't have room on the ship to hire " + crewman.name + ".";
			}
			//If not enough money, then let the player know
		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You can't afford to hire " + crewman.name + ".";
		}
	}

	public void GUI_GetBackgroundInfo(CrewManagementMemberViewModel crew) {
		GameVars.showNotification = true;
		GameVars.notificationMessage = crew.BackgroundInfo;
	}
}
