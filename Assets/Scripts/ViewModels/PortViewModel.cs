using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PortViewModel : CityViewModel
{
	public readonly CrewManagementViewModel CrewManagement;

	public PortViewModel() : base(Globals.GameVars.currentSettlement, null){
		CrewManagement = new CrewManagementViewModel(City);
	}

	public void GoToTown() {
		Globals.UI.Hide<PortScreen>();
		Globals.UI.Show<TownScreen, TradeViewModel>(new TradeViewModel());
	}

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_Button_TryToLeavePort() {
		if (GameVars.Trade.CheckIfPlayerCanAffordToPayPortTaxes()) {
			//MGV.controlsLocked = false;
			//Start Our time passage
			GameVars.playerShipVariables.PassTime(.25f, true);
			GameVars.justLeftPort = true;
			GameVars.playerShipVariables.ship.currency -= GameVars.currentPortTax;

			//Add a new route to the player journey log as a port exit
			GameVars.playerShipVariables.journey.AddRoute(new PlayerRoute(new Vector3(GameVars.playerShip.transform.position.x, GameVars.playerShip.transform.position.y, GameVars.playerShip.transform.position.z), Vector3.zero, GameVars.currentSettlement.settlementID, GameVars.currentSettlement.name, true, GameVars.playerShipVariables.ship.totalNumOfDaysTraveled), GameVars.playerShipVariables, GameVars.currentCaptainsLog);
			//We should also update the ghost trail with this route otherwise itp roduce an empty 0,0,0 position later
			GameVars.playerShipVariables.UpdatePlayerGhostRouteLineRenderer(GameVars.IS_NOT_NEW_GAME);

			//Turn off the coin image texture
			GameVars.menuControlsLock = false;

			GameVars.showSettlementGUI = false;
			GameVars.runningMainGameGUI = true;

			Globals.UI.Hide<PortScreen>();
			Globals.UI.Show<Dashboard, DashboardViewModel>(new DashboardViewModel());

		}
		else {//Debug.Log ("Not Enough Drachma to Leave the Port!");
			GameVars.showNotification = true;
			GameVars.notificationMessage = "Not Enough Drachma to pay the port tax and leave!";
		}
	}
}
