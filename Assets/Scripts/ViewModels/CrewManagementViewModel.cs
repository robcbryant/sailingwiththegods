using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CrewManagementViewModel : ViewModel
{
	private GameVars GameVars;

	public readonly ObservableCollection<CrewManagementMemberViewModel> AvailableCrew;
	public readonly ObservableCollection<CrewManagementMemberViewModel> MyCrew;

	public CrewManagementViewModel() {
		GameVars = Globals.GameVars;

		AvailableCrew = new ObservableCollection<CrewManagementMemberViewModel>(GameVars.GenerateRandomCrewMembers(5).Select(crew => new CrewManagementMemberViewModel(crew)));
		MyCrew = new ObservableCollection<CrewManagementMemberViewModel>(GameVars.playerShipVariables.ship.crewRoster.Select(crew => new CrewManagementMemberViewModel(crew)));
	}

	//=================================================================================================================
	// SETUP THE CREW MANAGEMENT PANEL
	//=================================================================================================================	
	
	//----------------------------------------------------------------------------
	//----------------------------CREW PANEL HELPER FUNCTIONS		

	public void GUI_FireCrewMember(CrewManagementMemberViewModel crew) {
		var crewman = crew.Member;

		GameVars.playerShipVariables.ship.crewRoster.Remove(crewman);
		MyCrew.Remove(crew);

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
				MyCrew.Remove(crew);


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
