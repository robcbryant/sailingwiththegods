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

	public readonly ObservableCollection<CrewManagementMemberViewModel> AvailableCrew;
	public readonly ObservableCollection<CrewManagementMemberViewModel> MyCrew;

	public string Capacity => GameVars.playerShipVariables.ship.crewRoster.Count + " / " + GameVars.playerShipVariables.ship.crewCapacity + " crew";

	public BoundModel<int> Money;

	public CrewManagementViewModel(Settlement settlement) {
		GameVars = Globals.GameVars;

		Money = new BoundModel<int>(GameVars.playerShipVariables.ship, nameof(GameVars.playerShipVariables.ship.currency));

		AvailableCrew = new ObservableCollection<CrewManagementMemberViewModel>(
			settlement.availableCrew
			.Where(crew => !GameVars.playerShipVariables.ship.crewRoster.Contains(crew))
			.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, null))
		);

		MyCrew = new ObservableCollection<CrewManagementMemberViewModel>(
			GameVars.playerShipVariables.ship.crewRoster
			.Select(crew => new CrewManagementMemberViewModel(crew, OnCrewClicked, null))
		);
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

		GameVars.playerShipVariables.ship.crewRoster.Remove(crewman);
		MyCrew.Remove(crew);
		AvailableCrew.Add(crew);

		Notify(nameof(Capacity));
		Notify(nameof(Money));

		GameVars.showNotification = true;
		GameVars.notificationMessage = crewman.name + " looked at you sadly and said before leaving, 'I thought I was doing so well. I'm sorry I let you down. Guess I'll go drink some cheap wine...";
	}


	public void GUI_HireCrewMember(CrewManagementMemberViewModel crew) {
		var crewman = crew.Member;

		//Check to see if player has enough money to hire
		if (GameVars.playerShipVariables.ship.currency >= crew.CostToHire) {
			//Now check to see if there is room to hire a new crew member!
			if (GameVars.playerShipVariables.ship.crewRoster.Count < GameVars.playerShipVariables.ship.crewCapacity) {
				GameVars.playerShipVariables.ship.crewRoster.Add(crewman);

				//Subtract the cost from the ship's money
				GameVars.playerShipVariables.ship.currency -= crew.CostToHire;

				//Remove Row
				AvailableCrew.Remove(crew);
				MyCrew.Add(crew);

				Notify(nameof(Capacity));
				Notify(nameof(Money));

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
