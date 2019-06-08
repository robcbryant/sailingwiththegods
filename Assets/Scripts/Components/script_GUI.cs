//=======================================================================================================================================
//
//  script_GUI.cs   -- Main GUI code interface
//
//    --This script handles all of the GUI interactions with the player. There is a main update loop that
//      acts as a listener for any changes in the player controls, or through click listeners on any GUI
//      buttons that are enabled here.
//
//    --The first block handles setting variables for all of the required GUI attached GameObjects in the scene
//
//    --The second block handles the main loop that looks for the changes
//
//    --The last block contains all relevant functions called on by the main loop
//
//
//      Note: Anything GUI related should pass through this script. This script can access both the player GameObject variabels
//          and the global variables GameObject (MGV) that are being updated outside this script, e.g. a trade menu here may need
//          access to a set of variables stored in the player ship's object in the player GameObject, as well as global functions
//          stored in the global variables GameObject to process that information. Eventually I would like to keep most of the
//          processing to the global variables script, and keep this script as ONLY a delivery system of pre-processed information 
//
//======================================================================================================================================









using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine.UI;

public class script_GUI : MonoBehaviour
{

	//======================================================================================================================================================================
	//======================================================================================================================================================================
	//  SETUP ALL VARIABLES FOR THE GUI
	//======================================================================================================================================================================
	//======================================================================================================================================================================
	

	//-----------------------------------------------------------
	// Game Over Notification Variables
	//-----------------------------------------------------------
	public GameObject gameover_main;
	public GameObject gameover_message;
	public GameObject gameover_restart;


	//-----------------------------------------------------------
	// Player Non-Port Notification Variables
	//-----------------------------------------------------------
	public GameObject nonport_info_main;
	public GameObject nonport_info_name;
	public GameObject nonport_info_notification;
	public GameObject nonport_info_okay;


	//-----------------------------------------------------------
	// Player Port Notification Variables
	//-----------------------------------------------------------
	public GameObject port_info_main;
	public GameObject port_info_name;
	public GameObject port_info_notification;
	public GameObject port_info_enter;
	public GameObject port_info_leave;

	public GameObject port_info_cityName;
	public GameObject port_info_taxes;
	public GameObject port_info_cloutMeter;
	public GameObject port_info_playerCities;
	public GameObject port_info_playerCities_count;
	public GameObject port_info_monumentsList;
	public GameObject port_info_crewCities;
	public GameObject port_info_crewCities_count;
	public GameObject port_info_crewMakeup;
	public GameObject port_info_description;
	public GameObject port_info_coinImage;
	public GameObject port_info_population;
	public GameObject port_info_portImage;


	//-----------------------------------------------------------
	// Player Notification Variables
	//-----------------------------------------------------------
	public GameObject notice_notificationParent;
	public GameObject notice_notificationSystem;

	//-----------------------------------------------------------
	// Player HUD Variables
	//-----------------------------------------------------------
	public GameObject player_hud_parent;

	public GameObject hud_waterStores;
	public GameObject hud_provisions;
	public GameObject hud_crewmember_count;
	public GameObject hud_daysThirsty;
	public GameObject hud_daysStarving;
	public GameObject hud_daysTraveled;
	public GameObject hud_shipHealth;
	public GameObject hud_currentSpeed;
	public GameObject hud_playerClout;

	public GameObject hud_button_dock;
	public GameObject hud_button_furlSails;
	public GameObject hud_button_helpwindow;

	//-----------------------------------------------------------
	// Port Menu TAB Content Panel Variables
	//-----------------------------------------------------------

	//---------------------------
	// Ship Repairs
	public GameObject tab_shiprepair_shiphealth;
	public GameObject tab_shiprepair_cost_onehp;
	public GameObject tab_shiprepair_cost_allhp;
	public GameObject tab_shiprepair_repairOneButton;
	public GameObject tab_shiprepair_repairAllButton;

	//---------------------------
	// Tavern 
	public GameObject tab_tavern_scrollWindow;

	//---------------------------
	// Crew
	public GameObject tab_crew_hireScrollWindow;
	public GameObject tab_crew_fireScrollWindow;

	//---------------------------
	// Loan Maintenance
	public GameObject tab_loan_new_parent;
	public GameObject tab_loan_old_parent;
	public GameObject tab_loan_elsewhere_parent;

	public GameObject tab_loan_new_loanAmount;
	public GameObject tab_loan_new_dueDate;
	public GameObject tab_loan_new_totalOwed;
	public GameObject tab_loan_new_interestRate;
	public GameObject tab_loan_new_takeLoanButton;

	public GameObject tab_loan_old_loanAmount;
	public GameObject tab_loan_old_dueDate;
	public GameObject tab_loan_old_payBackButton;

	public GameObject tab_loan_elsewhere_loanAmount;
	public GameObject tab_loan_elsewhere_dueDate;
	public GameObject tab_loan_elsewhere_loanOrigin;

	//---------------------------
	// Build a Shrine
	public GameObject tab_monument_buildStatueButton;
	public GameObject tab_monument_buildShrineButton;
	public GameObject tab_monument_buildTempleButton;



	//****************************************************************
	//GUI INFORMATION PANEL VARIABLES
	//****************************************************************

	//---------------------
	//REPAIR SHIP PANEL VARIABLES
	int costToRepair;


	//===================================
	// OTHER VARS
	GameVars GameVars;
	public GameObject all_trade_rows;
	public GameObject player_currency;
	public GameObject player_current_cargo;
	public GameObject player_max_cargo;







	//======================================================================================================================================================================
	//  INITIALIZE ANY NECESSARY VARIABLES
	//======================================================================================================================================================================
	void Start() {
		GameVars = Globals.GameVars;

	}








	//======================================================================================================================================================================
	//======================================================================================================================================================================
	//  START OF THE MAIN UPDATE LOOP
	//======================================================================================================================================================================
	//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv

	void OnGUI() {



		//This is NEW--This is a quick/easy way to make sure the necessary labels are constantly updated without too much
		//	--overhead to worry about.
		updateLabelsForPlayerVariables();

		if (GameVars.updatePlayerCloutMeter) {
			GameVars.updatePlayerCloutMeter = false;
			GUI_UpdatePlayerCloutMeter();
		}

		//=====================================================================================================================================	
		//  IF WE ARE AT THE TITLE SCREEN OR START SCREEN
		//=====================================================================================================================================	

		if (!GameVars.runningMainGameGUI) {

			//=====================================================================================================================================
			// IF WE ARE AT THE TITLE SCREEN
			if (GameVars.isTitleScreen) {

				Globals.UI.Show<TitleScreen, GameViewModel>(new GameViewModel());
				GameVars.isTitleScreen = false;			// TODO: Make this based on an event rather than this hacky one-time execution style.
			}

			//=====================================================================================================================================	
			// IF WE ARE AT THE START SCREEN	-- SHOW START SCREEN GUI


			if (GameVars.isStartScreen) {




			}

			//`````````````````````````````````````````````````````````````````
			//Check to see if we need to show any generic notifications ?
			if (GameVars.showNotification) {
				ShowNotification(GameVars.notificationMessage);
				GameVars.menuControlsLock = true;
				GameVars.showNotification = false;
			}

			//=====================================================================================================================================	
			//  IF WE AREN'T AT THE TITLE SCREEN OR START SCREEN
			//=====================================================================================================================================	
		}
		else if (GameVars.runningMainGameGUI) {

			//----------------------------------------------------------------------------------------------------------
			//      ALL static GUI elements go here for normail gameplay, e.g. ship stats, etc.
			//----------------------------------------------------------------------------------------------------------

			//`````````````````````````````````````````````````````````````````
			// 	SHIP STATS GUI
			//Debug.Log ("Updating stats?");
			hud_waterStores.GetComponent<Text>().text = ((int)GameVars.playerShipVariables.ship.cargo[0].amount_kg).ToString();
			hud_provisions.GetComponent<Text>().text = ((int)GameVars.playerShipVariables.ship.cargo[1].amount_kg).ToString();
			hud_shipHealth.GetComponent<Text>().text = ((int)GameVars.playerShipVariables.ship.health).ToString();
			hud_daysTraveled.GetComponent<Text>().text = (Mathf.Round(GameVars.playerShipVariables.ship.totalNumOfDaysTraveled * 1000.0f) / 1000.0f).ToString();
			hud_daysThirsty.GetComponent<Text>().text = (GameVars.playerShipVariables.dayCounterThirsty).ToString();
			hud_daysStarving.GetComponent<Text>().text = (GameVars.playerShipVariables.dayCounterStarving).ToString();
			hud_currentSpeed.GetComponent<Text>().text = (Mathf.Round(GameVars.playerShipVariables.current_shipSpeed_Magnitude * 1000.0f) / 1000.0f).ToString();
			hud_crewmember_count.GetComponent<Text>().text = (GameVars.playerShipVariables.ship.crew).ToString();
			hud_playerClout.GetComponent<Text>().text = GameVars.GetCloutTitleEquivalency((int)(Mathf.Round(GameVars.playerShipVariables.ship.playerClout * 1000.0f) / 1000.0f));
			//`````````````````````````````````````````````````````````````````
			// DOCKING BUTTON -- other GUI button click handlers are done in the editor--These are done here because the button's behavior changes based on other variables. The others do not
			if (GameVars.showSettlementTradeButton) { hud_button_dock.transform.GetChild(0).GetComponent<Text>().text = "CLICK TO \n  DOCK WITH \n" + GameVars.currentSettlement.name; hud_button_dock.GetComponent<Button>().onClick.RemoveAllListeners(); hud_button_dock.GetComponent<Button>().onClick.AddListener(() => GUI_checkOutOrDockWithPort(true)); }
			else if (GameVars.showNonPortDockButton) { hud_button_dock.transform.GetChild(0).GetComponent<Text>().text = "CHECK OUT \n" + GameVars.currentSettlement.name; hud_button_dock.GetComponent<Button>().onClick.RemoveAllListeners(); hud_button_dock.GetComponent<Button>().onClick.AddListener(() => GUI_checkOutOrDockWithPort(true)); }
			else { hud_button_dock.transform.GetChild(0).GetComponent<Text>().text = "DOCKING \n CURRENTLY \n UNAVAILABLE"; hud_button_dock.GetComponent<Button>().onClick.RemoveAllListeners(); }


			//----------------------------------------------------------------------------------------------------------
			//      The remaining part of this block is for listeners that change the GUI based on variable flags
			//----------------------------------------------------------------------------------------------------------        

			//`````````````````````````````````````````````````````````````````
			//WE ARE SHOWING A YES / NO  PORT TAX NOTIFICATION POP UP	?
			if (GameVars.showPortDockingNotification) {
				GameVars.showPortDockingNotification = false;
				GameVars.menuControlsLock = true;
				GUI_ShowPortDockingNotification();
			}
			else if (GameVars.showNonPortDockingNotification) {
				GameVars.menuControlsLock = true;
				GameVars.showNonPortDockingNotification = false;
				GUI_ShowNonPortDockingNotification();
			}

			//`````````````````````````````````````````````````````````````````
			//Check to see if we need to show any generic notifications ?
			if (GameVars.showNotification) {
				ShowNotification(GameVars.notificationMessage);
				GameVars.menuControlsLock = true;
				GameVars.showNotification = false;
			}

			//`````````````````````````````````````````````````````````````````
			// GAME OVER GUI
			if (GameVars.isGameOver) {
				GameVars.menuControlsLock = true;
				GUI_ShowGameOverNotification();
				GameVars.isGameOver = false;
			}




			//`````````````````````````````````````````````````````````````````
			// WIN THE GAME GUI
			if (GameVars.gameIsFinished) {
				GameVars.menuControlsLock = true;
				GUI_ShowGameIsFinishedNotification();
				GameVars.gameIsFinished = false;
			}

		}


	}//End of On.GUI()
	 //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^	
	 //======================================================================================================================================================================
	 //  END OF MAIN UPDATE LOOP
	 //======================================================================================================================================================================
	 //======================================================================================================================================================================









	//======================================================================================================================================================================
	//======================================================================================================================================================================
	//  ALL FUNCTIONS
	//======================================================================================================================================================================
	//======================================================================================================================================================================

	//=====================================================================================================================================	
	//  GUI Interaction Functions are the remaining code below. All of these functions control some aspect of the GUI based on state changes
	//=====================================================================================================================================	
	
	//-------------------------------------------------------------------------------------------------------------------------
	//   GAME OVER NOTIFICATIONS AND COMPONENTS

	public void GUI_ShowGameOverNotification() {
		GameVars.controlsLocked = true;
		//Set the notification window as active
		gameover_main.SetActive(true);
		//Setup the GameOver Message
		gameover_message.GetComponent<Text>().text = "You have lost! Your crew has died! Your adventure ends here!";
		//GUI_TAB_SetupAShrinePanel the GUI_restartGame button
		gameover_restart.GetComponent<Button>().onClick.RemoveAllListeners();
		gameover_restart.GetComponent<Button>().onClick.AddListener(() => GUI_RestartGame());

	}

	public void GUI_ShowGameIsFinishedNotification() {
		GameVars.controlsLocked = true;
		//Set the notification window as active
		gameover_main.SetActive(true);
		//Setup the GameOver Message
		gameover_message.GetComponent<Text>().text = "You have Won! Congratulations! Your adventure ends here!";
		//GUI_TAB_SetupAShrinePanel the GUI_restartGame button
		gameover_restart.GetComponent<Button>().onClick.RemoveAllListeners();
		gameover_restart.GetComponent<Button>().onClick.AddListener(() => GUI_RestartGame());

	}


	public void GUI_RestartGame() {
		gameover_main.SetActive(false);
		GameVars.menuControlsLock = false;
		//Restart from Beginning
		GameVars.RestartGame();
	}


	//-------------------------------------------------------------------------------------------------------------------------
	//   DOCKING INFO PANEL AND COMPONENTS    


	public void GUI_ShowNonPortDockingNotification() {
		//Show the non port notification window
		nonport_info_main.SetActive(true);
		//Set the title
		nonport_info_name.GetComponent<Text>().text = GameVars.currentSettlement.name;
		//Set the description
		nonport_info_notification.GetComponent<Text>().text = GameVars.currentSettlement.description;
		//Setup the okay button
		nonport_info_okay.GetComponent<Button>().onClick.RemoveAllListeners();
		nonport_info_okay.GetComponent<Button>().onClick.AddListener(() => GUI_ExitPortNotification());
	}


	public void GUI_ShowPortDockingNotification() {
		GameVars.controlsLocked = true;

		//Show the port notification pop up
		port_info_main.SetActive(true);
		//Set the title
		port_info_name.GetComponent<Text>().text = GameVars.currentSettlement.name;
		//Setup the message for the scroll view
		string portMessage = "";
		portMessage += GameVars.currentSettlement.description;
		portMessage += "\n\n";
		if (GameVars.isInNetwork) {
			var crewMemberWithNetwork = GameVars.Network.CrewMemberWithNetwork(GameVars.currentSettlement);
			portMessage += "This Port is part of your network!\n";
			if (crewMemberWithNetwork != null) portMessage += "Your crewman, " + crewMemberWithNetwork.name + " assures you their connections here are strong! They should welcome you openly and waive your port taxes on entering!";
			else portMessage += "You know this port as captain very well! You expect that your social connections here will soften the port taxes in your favor!";
		}
		else {
			portMessage += "This port is outside your social network!\n";
		}

		if (GameVars.currentPortTax != 0) {
			portMessage += "If you want to dock here, your tax for entering will be " + GameVars.currentPortTax + " drachma. \n";
			//If the port tax will make the player go negative--alert them as they enter
			if (GameVars.playerShipVariables.ship.currency - GameVars.currentPortTax < 0) portMessage += "Docking here will put you in debt for " + (GameVars.playerShipVariables.ship.currency - GameVars.currentPortTax) + "drachma, and you may lose your ship!\n";
		}
		else {
			portMessage += "You only have food and water stores on board, with no taxable goods. Thankfully you will dock for free!";
		}

		port_info_notification.GetComponent<Text>().text = portMessage;
		port_info_enter.GetComponent<Button>().onClick.RemoveAllListeners();
		port_info_leave.GetComponent<Button>().onClick.RemoveAllListeners();
		port_info_enter.GetComponent<Button>().onClick.AddListener(() => GUI_EnterPort());
		port_info_leave.GetComponent<Button>().onClick.AddListener(() => GUI_ExitPortNotification());
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//   DOCKING INFO PANEL AND COMPONENTS    HELPER FUNCTIONS	

	public void GUI_ExitPortNotification() {
		//Turn off both nonport AND port notification windows
		port_info_main.SetActive(false);
		GameVars.showPortDockingNotification = false;
		nonport_info_main.SetActive(false);
		GameVars.showNonPortDockingNotification = false;
		GameVars.controlsLocked = false;
		GameVars.menuControlsLock = false;
	}

	public void GUI_EnterPort() {
		//Turn off port welcome screen
		GameVars.showPortDockingNotification = false;
		port_info_main.SetActive(false);
		port_info_taxes.GetComponent<Text>().text = GameVars.currentPortTax.ToString();
		//Check if current Settlement is part of the main quest line
		GameVars.CheckIfCurrentSettlementIsPartOfMainQuest(GameVars.currentSettlement.settlementID);
		//Add this settlement to the player's knowledge base
		GameVars.playerShipVariables.ship.playerJournal.AddNewSettlementToLog(GameVars.currentSettlement.settlementID);
		//Determine what settlements are available to the player in the tavern
		GameVars.showSettlementGUI = true;
		GameVars.showSettlementTradeButton = false;
		GameVars.controlsLocked = true;

		//-------------------------------------------------
		//NEW GUI FUNCTIONS FOR SETTING UP TAB CONTENT
		//Show Port Menu
		Globals.UI.Hide<Dashboard>();
		Globals.UI.Show<PortScreen, PortViewModel>(new PortViewModel());

		//Setup port panels
		GUI_TAB_SetupLoanManagementPanel();
		GUI_TAB_SetupAShrinePanel();
		GUI_TAB_SetupShipRepairInformation();
		GUI_TAB_SetupTavernInformation();

		//Add a new route to the player journey log as a port entry
		GameVars.playerShipVariables.journey.AddRoute(new PlayerRoute(GameVars.playerShip.transform.position, Vector3.zero, GameVars.currentSettlement.settlementID, GameVars.currentSettlement.name, false, GameVars.playerShipVariables.ship.totalNumOfDaysTraveled), GameVars.playerShipVariables, GameVars.currentCaptainsLog);
		//We should also update the ghost trail with this route otherwise itp roduce an empty 0,0,0 position later
		GameVars.playerShipVariables.UpdatePlayerGhostRouteLineRenderer(GameVars.IS_NOT_NEW_GAME);

		//-------------------------------------------------
		// UPDATE PLAYER CLOUT METER
		GUI_UpdatePlayerCloutMeter();


		//-------------------------------------------------
		// OTHER PORT GUI SETUP FUNCTIONS
		GetCrewHometowns();
		GUI_GetListOfBuiltMonuments();
		GUI_SetPortCoinImage();
		GUI_SetPortBGImage();
		GUI_SetPortPopulation();
		GUI_GetBuiltMonuments();
		port_info_cityName.GetComponent<Text>().text = GameVars.currentSettlement.name;
		port_info_description.GetComponent<Text>().text = GameVars.currentSettlement.description;

	}

	public void GUI_UpdatePlayerCloutMeter() {
		//-------------------------------------------------
		// UPDATE PLAYER CLOUT METER
		// *This assumes the child gameobject elements of the clout meter are in order from lowest to highest. If not--then this will produce undesirable results
		bool foundMatch = false;
		hud_playerClout.GetComponent<Text>().text = GameVars.GetCloutTitleEquivalency((int)(Mathf.Round(GameVars.playerShipVariables.ship.playerClout * 1000.0f) / 1000.0f));
		for (int i = 0; i < port_info_cloutMeter.transform.childCount; i++) {
			Transform currentCloutMeter = port_info_cloutMeter.transform.GetChild(i);
			Debug.Log(currentCloutMeter.name + "  =?  " + hud_playerClout.GetComponent<Text>().text);
			if (currentCloutMeter.name == hud_playerClout.GetComponent<Text>().text) {
				currentCloutMeter.gameObject.SetActive(true);
				foundMatch = true;
			}
			else {
				if (!foundMatch) currentCloutMeter.gameObject.SetActive(true);
				else currentCloutMeter.gameObject.SetActive(false);
			}
		}
	}

	public IEnumerable<string> GUI_GetListOfPlayerNetworkCities() {
		//Looks through the player's known settlements and adds it to a list
		var result = new List<string>();
		foreach (int knownSettlementID in GameVars.playerShipVariables.ship.playerJournal.knownSettlements) {
			result.Add( GameVars.GetSettlementFromID(knownSettlementID).name );
		}
		return result;
	}

	public IEnumerable<string> GetCrewHometowns() {
		//Looks through the hometowns of all crew and adds them to a list
		var result = new List<string>();
		foreach (CrewMember crewman in GameVars.playerShipVariables.ship.crewRoster) {
			result.Add(GameVars.GetSettlementFromID(crewman.originCity).name);
		}
		return result;
	}

	public void GUI_GetListOfBuiltMonuments() {

	}

	public string GUI_GetCrewMakeupList() {
		//Loops through all crewmembers and counts their jobs to put into a list
		int sailors = 0;
		int warriors = 0;
		int slaves = 0;
		int seers = 0;
		int other = 0;
		string list = "";
		foreach (CrewMember crewman in GameVars.playerShipVariables.ship.crewRoster) {
			switch (crewman.typeOfCrew) {
				case CrewType.Sailor:
					sailors++;
					break;
				case CrewType.Warrior:
					warriors++;
					break;
				case CrewType.Slave:
					slaves++;
					break;
				case CrewType.Seer:
					seers++;
					break;
				default:
					other++;
					break;
			}
		}
		list += "Sailors  - " + sailors.ToString() + "\n";
		list += "Warriors - " + warriors.ToString() + "\n";
		list += "Seers    - " + seers.ToString() + "\n";
		list += "Slaves   - " + slaves.ToString() + "\n";
		list += "Other   - " + other.ToString() + "\n";

		return list;
	}

	public void GUI_GetBuiltMonuments() {
		port_info_monumentsList.GetComponent<Text>().text = GameVars.playerShipVariables.ship.builtMonuments;
	}

	public void GUI_SetPortPopulation() {
		int pop = GameVars.currentSettlement.population;
		if (pop >= 0 && pop < 25) port_info_population.GetComponent<Text>().text = "Village";
		else if (pop >= 25 && pop < 50) port_info_population.GetComponent<Text>().text = "Town";
		else if (pop >= 50 && pop < 75) port_info_population.GetComponent<Text>().text = "City";
		else if (pop >= 75 && pop <= 100) port_info_population.GetComponent<Text>().text = "Metropolis";

	}

	public void GUI_SetPortBGImage() {
		Debug.Log("LOOKING FOR BG CITY-------->   " + GameVars.currentSettlement.settlementID.ToString());
		//Get the settlement ID as a string
		string currentID = GameVars.currentSettlement.settlementID.ToString();
		Sprite currentBGTex = Resources.Load<Sprite>("settlement_portraits/" + currentID);
		Debug.Log(currentBGTex);
		//Now test if it exists, if the settlement does not have a matching texture, then default to a basic one
		if (currentBGTex) { port_info_portImage.GetComponent<Image>().sprite = currentBGTex; }
		else { port_info_portImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("settlement_portraits/gui_port_portrait_default"); }

	}

	public void GUI_SetPortCoinImage() {
		Debug.Log("LOOKING FOR COIN CITY-------->   " + GameVars.currentSettlement.settlementID.ToString());
		//Show the coin image associated with this settlement
		//Get the settlement ID as a string
		string currentID = GameVars.currentSettlement.settlementID.ToString();
		Sprite currentCoinTex = Resources.Load<Sprite>("settlement_coins/" + currentID);
		Debug.Log(currentCoinTex);
		//Now test if it exists, if the settlement does not have a matching texture, then default to a basic one
		if (currentCoinTex) { port_info_coinImage.GetComponent<Image>().sprite = currentCoinTex; }
		else { port_info_coinImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("settlement_coins/default_coin_texture"); }

	}



	//=================================================================================================================
	// NOTIFICATION POP-UP SYSTEM
	//=================================================================================================================	

	void ShowNotification(string message) {
		//Declare our message as a string to ensure we have a unique instance passed
		string notificationMessage = message;

		//Set the Notification System to Active
		notice_notificationSystem.SetActive(true);

		//Clone the first child of the "Current Notifications" parent(This will be the template used to instantiate more notifications as needed
		GameObject newNotification = Instantiate((GameObject)notice_notificationParent.transform.GetChild(0).gameObject) as GameObject;
		newNotification.transform.SetParent((Transform)notice_notificationParent.transform);
		newNotification.SetActive(true);

		//Reset its transform values
		newNotification.GetComponent<RectTransform>().localScale = Vector3.one;
		newNotification.GetComponent<RectTransform>().localPosition = Vector3.zero;

		//If there are more than 2 notifications present(there are more than 2 children--1 template + 1 notification) then let's
		//	--give the second notification a random location within a specified range of  -600 < x < 600  and -180 < y < 180
		if (notice_notificationParent.transform.childCount > 2) {
			newNotification.GetComponent<RectTransform>().localPosition = new Vector3(Random.Range(-600, 600), Random.Range(-180, 180), 0);
		}
		//Setup the Message Text
		newNotification.transform.Find("ScrollView/Content/Message").GetComponent<Text>().text = notificationMessage;

		//Setup the confirmation button to destroy this specific notification instance
		Button notificationButton = (Button)newNotification.transform.Find("Confirm Button").GetComponent<Button>();
		notificationButton.onClick.RemoveAllListeners();
		notificationButton.onClick.AddListener(() => GUI_RemoveNotification(newNotification));
		//That's it
	}

	public void GUI_RemoveNotification(GameObject notification) {
		//If there are only 2 children (the hidden template notification and the one we are about to delete), then turn off the notification system window and set it to active = false
		if (notice_notificationParent.transform.childCount == 2) {
			notice_notificationSystem.SetActive(false);
			GameVars.menuControlsLock = false;
		}
		//Remove the current notification that flagged this event
		GameObject.Destroy(notification);
	}



	//=================================================================================================================
	// HELPER FUNCTIONS FOR IN-PORT TRADE WINDOW
	//=================================================================================================================	
	

	//This function updates the player cargo labels after any exchange between money and resources has been made
	public void updateLabelsForPlayerVariables() {
		player_currency.GetComponent<Text>().text = GameVars.playerShipVariables.ship.currency.ToString();
		player_current_cargo.GetComponent<Text>().text = Mathf.CeilToInt(GameVars.playerShipVariables.ship.GetTotalCargoAmount()).ToString();
		player_max_cargo.GetComponent<Text>().text = Mathf.CeilToInt(GameVars.playerShipVariables.ship.cargo_capicity_kg).ToString();

	}

	//This function activates the docking element when the dock button is clicked. A bool is passed to determine whether or not the button is responsive
	public void GUI_checkOutOrDockWithPort(bool isAvailable) {
		if (isAvailable) {
			//Figure out the tax on the cargo hold
			GameVars.currentPortTax = GameVars.Trade.GetTaxRateOnCurrentShipManifest();
			GameVars.showPortDockingNotification = true;
		}
		//Else do nothing
	}



	//=================================================================================================================
	// SETUP THE CREW SELECTION START SCREEN
	//=================================================================================================================	

	// TODO: Crew selection disabled for now
	/*
	public void GUI_SetupStartScreenCrewSelection() {
		//We need to be sure to EMPTY the crew list before we start a new one--this is superfluous in a fresh game start--the list is already empty in the GUI, but on an in-game restart
		//we have to empty the list or else we will add a duplicate list and cause all sorts of fun errors and behavior
		for (int i = title_crew_select_crew_list.transform.Find("Content").childCount - 1; i > 0; i--) {
			Destroy(title_crew_select_crew_list.transform.Find("Content").GetChild(i).gameObject);
		}
		//Now add the available crew to our freshly emptied list
		Button startGame = (Button)title_crew_select_start_game.GetComponent<Button>();
		startGame.onClick.RemoveAllListeners();//We have to remove this listener before we add it in case of an in-game restart, otherwise we have to simulataneous duplicate listeners when the button is pressed
		startGame.onClick.AddListener(() => GUI_startMainGame());
		for (int i = 0; i < GameVars.newGameAvailableCrew.Count; i++) {
			//Debug.Log ("CREW COUNT   " +i);
			//We have to re-declare the CrewMember argument here or else when we apply the variable to the onClick() handler
			//	--all onClick()'s in this loop will reference the last CrewMember instance in the loop rather than their
			//	--respective iterated instances
			CrewMember currentMember = GameVars.newGameAvailableCrew[i];

			//First let's get a clone of our hidden row in the tavern scroll view
			GameObject currentMemberRow = Instantiate((GameObject)title_crew_select_entry_template.transform.gameObject) as GameObject;
			currentMemberRow.transform.SetParent((Transform)title_crew_select_crew_list.transform.Find("Content"));
			currentMemberRow.SetActive(true);

			//Set the current clone to active
			currentMemberRow.SetActive(true);
			//We have to reset the new row UI object's transform to 1,1,1 because new ones are instantiated with 0,0,0 for some ass reason
			currentMemberRow.GetComponent<RectTransform>().localScale = Vector3.one;
			Text memberName = (Text)currentMemberRow.transform.Find("Crew Name").GetComponent<Text>();
			Text memberJob = (Text)currentMemberRow.transform.Find("Sailor Job/Job Title").GetComponent<Text>();
			Text memberHome = (Text)currentMemberRow.transform.Find("Home Town/Home Town Name").GetComponent<Text>();
			Text memberClout = (Text)currentMemberRow.transform.Find("Clout/Clout Title").GetComponent<Text>();
			Button hireMember = (Button)currentMemberRow.transform.Find("Hire Button").GetComponent<Button>();
			Button moreMemberInfo = (Button)currentMemberRow.transform.Find("Backstory/Backstory Button").GetComponent<Button>();
			Image crewPortrait = (Image)currentMemberRow.transform.Find("Hire Button").GetComponent<Image>();
			//Get the crewman ID as a string
			string currentID = currentMember.ID.ToString();
			Sprite currentICONTex = Resources.Load<Sprite>("crew_portraits/" + currentID);
			//Now test if it exists, if the crew does not have a matching texture, then default to a basic one
			if (currentICONTex) { crewPortrait.sprite = currentICONTex; }
			else { crewPortrait.sprite = Resources.Load<Sprite>("crew_portraits/phoenician_sailor"); }



			memberName.text = currentMember.name;
			memberJob.text = GameVars.GetJobClassEquivalency(currentMember.typeOfCrew);
			memberHome.text = GameVars.GetSettlementFromID(currentMember.originCity).name;
			memberClout.text = GameVars.GetCloutTitleEquivalency(currentMember.clout);


			moreMemberInfo.onClick.RemoveAllListeners();
			moreMemberInfo.onClick.AddListener(() => GUI_GetBackgroundInfo(currentMember.backgroundInfo));
			//startGame.onClick.AddListener(() => GUI_GetBackgroundInfo(currentMember.backgroundInfo));

			int numOfCrew = 0;
			int currentIndex = i;
			//If the crewmember is necessary for the quest--lock the selection in as true
			if (!GameVars.newGameAvailableCrew[i].isKillable) {
				GameVars.newGameCrewSelectList[i] = true;
				hireMember.transform.GetChild(0).GetComponent<Text>().text = "X";
				numOfCrew++;
			}
			else {
				hireMember.onClick.RemoveAllListeners();
				hireMember.onClick.AddListener(() => GUI_CrewSelectToggle(currentIndex));
			}
			title_crew_select_crew_count.GetComponent<Text>().text = numOfCrew.ToString();
		}

	}
	public void GUI_CrewSelectToggle(int crewIndex) {
		Transform currentCrewman = title_crew_select_crew_list.transform.Find("Content").GetChild(crewIndex + 1).Find("Hire Button");
		if (GameVars.newGameCrewSelectList[crewIndex] != true) {
			currentCrewman.GetChild(0).GetComponent<Text>().text = "X";
			GameVars.newGameCrewSelectList[crewIndex] = true;
		}
		else {
			currentCrewman.GetChild(0).GetComponent<Text>().text = "";
			GameVars.newGameCrewSelectList[crewIndex] = false;
		}
		//Update our crew total!
		int crewTotal = 0;
		foreach (bool crew in GameVars.newGameCrewSelectList) {
			if (crew) crewTotal++;
		}
		title_crew_select_crew_count.GetComponent<Text>().text = crewTotal.ToString();

		//We also need to run a check on whether or not we have 30 members--if we do, then hide the check box if it's 'false'
		//We start at index 1 because the 0 position is the template row
		if (crewTotal >= 30) {
			for (int x = 1; x < title_crew_select_crew_list.transform.Find("Content").childCount; x++) {
				Transform childButton = title_crew_select_crew_list.transform.Find("Content").GetChild(x).Find("Hire Button");
				if (!GameVars.newGameCrewSelectList[x - 1]) childButton.gameObject.SetActive(false);
			}
			//Enable our Start Game Button
			title_crew_select_start_game.SetActive(true);
		}
		else {
			for (int x = 1; x < title_crew_select_crew_list.transform.Find("Content").childCount; x++) {
				Transform childButton = title_crew_select_crew_list.transform.Find("Content").GetChild(x).Find("Hire Button");
				if (!childButton.gameObject.activeSelf) childButton.gameObject.SetActive(true);
			}
			title_crew_select_start_game.SetActive(false);
		}
		//Debug.Log(crewTotal);
	}
	*/




	//============================================================================================================================================================================
	//============================================================================================================================================================================
	//  THESE CONSTRUCT ALL OF THE PANELS WITHIN THE PORT MENU GUI SYSTEM
	//============================================================================================================================================================================

	//=================================================================================================================
	// SETUP THE LOAN MANAGEMENT PANEL
	//=================================================================================================================	

	public void GUI_TAB_SetupLoanManagementPanel() {


		//-------NEW LOAN-----------------------
		if (GameVars.playerShipVariables.ship.currentLoan == null) {
			//Turn on the panel we need and the others off
			tab_loan_new_parent.SetActive(true);
			tab_loan_old_parent.SetActive(false);
			tab_loan_elsewhere_parent.SetActive(false);

			//Setup the initial term to repay the loan
			float numOfDaysToPayOffLoan = 10;
			//Determine the base loan amount off the city's population
			float baseLoanAmount = 500 * (GameVars.currentSettlement.population / 1000);
			//If base loan amount is less than 200 then make it 200 as the smallest amount available
			if (baseLoanAmount < 200f) baseLoanAmount = 200f;
			//Determine the actual loan amount off the player's clout
			int loanAmount = (int)(baseLoanAmount + (baseLoanAmount * GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID)));
			//Determmine the base interest rate of the loan off the city's population
			float baseInterestRate = 10 + (GameVars.currentSettlement.population / 1000);
			//Determine finalized interest rate after determining player's clout
			float finalInterestRate = (float)System.Math.Round(baseInterestRate - (baseInterestRate * GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID)), 3);
			//Determine the precompiled interest if returned on time
			int totalAmountDueAtTerm = Mathf.CeilToInt(loanAmount + (loanAmount * (finalInterestRate / 100)));

			tab_loan_new_dueDate.GetComponent<Text>().text = numOfDaysToPayOffLoan.ToString();
			tab_loan_new_interestRate.GetComponent<Text>().text = finalInterestRate.ToString();
			tab_loan_new_loanAmount.GetComponent<Text>().text = loanAmount.ToString();
			tab_loan_new_totalOwed.GetComponent<Text>().text = totalAmountDueAtTerm.ToString();
			//Create the Loan object for our button to process		
			Loan newLoan = new Loan(loanAmount, finalInterestRate, numOfDaysToPayOffLoan, GameVars.currentSettlement.settlementID);

			tab_loan_new_takeLoanButton.GetComponent<Button>().onClick.RemoveAllListeners();
			tab_loan_new_takeLoanButton.GetComponent<Button>().onClick.AddListener(() => GUI_TakeOutLoan(loanAmount, newLoan));

			//-------OLD LOAN-----------------------	
			//If the player does have a loan already, then we need to make them aware of that
			//--If the player is back at the origin settlement of the loan, then the player should be allowed to pay the loan back
		}
		else if (GameVars.CheckIfShipBackAtLoanOriginPort()) {
			//Turn on the panel we need and the others off
			tab_loan_new_parent.SetActive(false);
			tab_loan_old_parent.SetActive(true);
			tab_loan_elsewhere_parent.SetActive(false);

			int loanAmount = GameVars.playerShipVariables.ship.currentLoan.GetTotalAmountDueWithInterest();
			tab_loan_old_dueDate.GetComponent<Text>().text = GameVars.playerShipVariables.ship.currentLoan.numOfDaysUntilDue.ToString();
			tab_loan_old_loanAmount.GetComponent<Text>().text = loanAmount.ToString();

			tab_loan_old_payBackButton.GetComponent<Button>().onClick.RemoveAllListeners();
			tab_loan_old_payBackButton.GetComponent<Button>().onClick.AddListener(() => GUI_PayBackLoan(loanAmount));

			//-------LOAN IS ELSEWHERE-----------------------
			//If at a different settlement, the player needs to be made aware that they can only take a loan out from a single settlement at a time.
		}
		else {
			//Turn on the panel we need and the others off
			tab_loan_new_parent.SetActive(false);
			tab_loan_old_parent.SetActive(false);
			tab_loan_elsewhere_parent.SetActive(true);

			tab_loan_elsewhere_dueDate.GetComponent<Text>().text = GameVars.playerShipVariables.ship.currentLoan.numOfDaysUntilDue.ToString();
			tab_loan_elsewhere_loanAmount.GetComponent<Text>().text = GameVars.playerShipVariables.ship.currentLoan.GetTotalAmountDueWithInterest().ToString();
			tab_loan_elsewhere_loanOrigin.GetComponent<Text>().text = GameVars.GetSettlementFromID(GameVars.playerShipVariables.ship.currentLoan.settlementOfOrigin).name;

		}


	}
	//----------------------------------------------------------------------------
	//----------------------------LOAN PANEL HELPER FUNCTIONS	

	public void GUI_PayBackLoan(int amountDue) {
		//Pay the loan back if the player has the currency to do it
		if (GameVars.playerShipVariables.ship.currency > amountDue) {
			GameVars.playerShipVariables.ship.currency -= amountDue;
			GameVars.playerShipVariables.ship.currentLoan = null;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You paid back your loan and earned a little respect!";
			//give a boost to the players clout for paying back loan
			GameVars.AdjustPlayerClout(3);
			//Reset our loan panel
			GUI_TAB_SetupLoanManagementPanel();
			//Otherwise let player know they can't afford to pay the loan back
		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You currently can't afford to pay your loan back! Better make some more money!";
		}
	}

	public void GUI_TakeOutLoan(int loanAmount, Loan loan) {
		GameVars.playerShipVariables.ship.currentLoan = loan;
		GameVars.playerShipVariables.ship.currency += loanAmount;
		GameVars.showNotification = true;
		GameVars.notificationMessage = "You took out a loan of " + loanAmount + " drachma! Remember to pay it back in due time!";
		//Reset our loan panel
		GUI_TAB_SetupLoanManagementPanel();
	}



	//=================================================================================================================
	// SETUP THE BUILD A MONUMENT PANEL
	//=================================================================================================================	
	public void GUI_TAB_SetupAShrinePanel() {

		int baseCost = 0;
		//We need to do a clout check as well as a network checks
		int baseModifier = Mathf.CeilToInt(1000 - (200 * GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID)));
		if (GameVars.Network.CheckIfCityIDIsPartOfNetwork(GameVars.currentSettlement.settlementID)) {
			baseCost = Mathf.CeilToInt(GameVars.currentSettlement.tax_network * baseModifier * 1);
		}
		else {
			baseCost = Mathf.CeilToInt(GameVars.currentSettlement.tax_neutral * baseModifier * 1);
		}

		int statueCost = baseCost / 2;
		int shrineCost = baseCost;
		int templeCost = baseCost * 2;
		tab_monument_buildStatueButton.transform.GetChild(0).GetComponent<Text>().text = statueCost.ToString() + " DR";
		tab_monument_buildStatueButton.GetComponent<Button>().onClick.RemoveAllListeners();
		tab_monument_buildStatueButton.GetComponent<Button>().onClick.AddListener(() => GUI_BuildAStatue(statueCost));
		tab_monument_buildShrineButton.transform.GetChild(0).GetComponent<Text>().text = shrineCost.ToString() + " DR";
		tab_monument_buildShrineButton.GetComponent<Button>().onClick.RemoveAllListeners();
		tab_monument_buildShrineButton.GetComponent<Button>().onClick.AddListener(() => GUI_BuildAShrine(shrineCost));
		tab_monument_buildTempleButton.transform.GetChild(0).GetComponent<Text>().text = templeCost.ToString() + " DR";
		tab_monument_buildTempleButton.GetComponent<Button>().onClick.RemoveAllListeners();
		tab_monument_buildTempleButton.GetComponent<Button>().onClick.AddListener(() => GUI_BuildATemple(templeCost));

	}
	//----------------------------------------------------------------------------
	//----------------------------MONUMENT PANEL HELPER FUNCTIONS
	public void GUI_BuildAStatue(int cost) {

		if (GameVars.playerShipVariables.ship.currency > cost) {
			GameVars.playerShipVariables.ship.currency -= cost;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You built a statue for " + GameVars.currentSettlement.name + "! You've earned a little clout!";
			GameVars.AdjustPlayerClout(1);
			GameVars.playerShipVariables.ship.builtMonuments += GameVars.currentSettlement.name + " -- " + "Statue\n";
			GUI_GetBuiltMonuments();

		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You don't have enough money to build a statue for " + GameVars.currentSettlement.name;
		}

	}
	public void GUI_BuildAShrine(int cost) {

		if (GameVars.playerShipVariables.ship.currency > cost) {
			GameVars.playerShipVariables.ship.currency -= cost;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You built a shrine for " + GameVars.currentSettlement.name + "! You've earned quite a bit of clout!";
			GameVars.AdjustPlayerClout(4);
			GameVars.playerShipVariables.ship.builtMonuments += GameVars.currentSettlement.name + " -- " + "Shrine\n";
			GUI_GetBuiltMonuments();

		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You don't have enough money to build a Shrine for " + GameVars.currentSettlement.name;
		}

	}
	public void GUI_BuildATemple(int cost) {

		if (GameVars.playerShipVariables.ship.currency > cost) {
			GameVars.playerShipVariables.ship.currency -= cost;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You built a temple for " + GameVars.currentSettlement.name + "! You've earned a tremendous amount of clout!";
			GameVars.AdjustPlayerClout(8);
			GameVars.playerShipVariables.ship.builtMonuments += GameVars.currentSettlement.name + " -- " + "Temple\n";
			GUI_GetBuiltMonuments();

		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You don't have enough money to build a Temple for " + GameVars.currentSettlement.name;
		}
	}



	//=================================================================================================================
	// SETUP THE TAVERN PANEL
	//=================================================================================================================	
	public void GUI_TAB_SetupTavernInformation() {

		//Let's clear our current list of settlements
		for (int i = 1; i < tab_tavern_scrollWindow.transform.childCount; i++) {
			GameObject.Destroy(tab_tavern_scrollWindow.transform.GetChild(i).gameObject);
		}

		//First clear the settlement list
		foreach (int settlementID in GameVars.playerShipVariables.ship.playerJournal.knownSettlements) {
			Settlement settlement = GameVars.GetSettlementFromID(settlementID);
			//make sure not to list the current settlement the player is at
			if (GameVars.currentSettlement.settlementID != settlementID) {
				//First let's get a clone of our hidden row in the tavern scroll view
				GameObject currentPort = Instantiate((GameObject)tab_tavern_scrollWindow.transform.GetChild(0).gameObject) as GameObject;
				currentPort.transform.SetParent((Transform)tab_tavern_scrollWindow.transform);
				//Set the current clone to active
				currentPort.SetActive(true);
				currentPort.GetComponent<RectTransform>().localScale = Vector3.one;
				Text portLabel = (Text)currentPort.transform.Find("Port Name").GetComponent<Text>();
				Button hintCostBut = (Button)currentPort.transform.Find("Hint Button").GetComponent<Button>();
				Button navCostBut = (Button)currentPort.transform.Find("Hire Navigator Button").GetComponent<Button>();
				//Setup the Port Name
				portLabel.text = settlement.name;
				//Let's setup the Navigator hiring information
				//-- Figure out the Cost to Hire a navigator for this city
				float initialCost = CoordinateUtil.GetDistanceBetweenTwoLatLongCoordinates(GameVars.currentSettlement.location_longXlatY, settlement.location_longXlatY) / 1000f;
				int costToHire = Mathf.RoundToInt(initialCost - (initialCost * GameVars.GetOverallCloutModifier(settlement.settlementID)));
				//--Set the button label to show the cost
				navCostBut.transform.Find("Cost For Navigator").GetComponent<Text>().text = costToHire.ToString() + "Dr";
				navCostBut.onClick.RemoveAllListeners();
				navCostBut.onClick.AddListener(() => GUI_HireANavigator(settlement, costToHire));

				//Now let's setup a hint button if it's a city. If it's not a city, then there is no trading and nothign to ask about
				if (settlement.typeOfSettlement == 1) {
					//Set the button to Active is we are using it!
					hintCostBut.interactable = true;
					//Now figure out the costs for questions for the relevant settlements
					initialCost = CoordinateUtil.GetDistanceBetweenTwoLatLongCoordinates(GameVars.currentSettlement.location_longXlatY, settlement.location_longXlatY) / 10000f;
					int costForHint = Mathf.RoundToInt(initialCost - (initialCost * GameVars.GetOverallCloutModifier(settlement.settlementID)));
					//--Set the Button label to show the cost
					hintCostBut.transform.Find("Cost For Hint").GetComponent<Text>().text = costForHint.ToString() + "Dr";
					hintCostBut.onClick.RemoveAllListeners();
					hintCostBut.onClick.AddListener(() => GUI_BuyHint(settlement, costForHint));

				}
				else {
					//turn off the button if it's not a port
					hintCostBut.interactable = false;
				}
			}
		}
	}
	//----------------------------------------------------------------------------
	//----------------------------TAVERN PANEL HELPER FUNCTIONS

	public string GetInfoOnNetworkedSettlementResource(Resource resource) {
		if (resource.amount_kg < 100)
			return "I hear they are running inredibly low on " + resource.name;
		else if (resource.amount_kg < 300)
			return "Someone mentioned that they have modest stores of " + resource.name;
		else
			return "A sailor just came from there and said he just unloaded an enormous quantity of " + resource.name;

	}

	public void GUI_BuyHint(Settlement currentSettlement, int hintCost) {

		if (GameVars.playerShipVariables.ship.currency < hintCost) {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "Not enough money to buy this information!";
		}
		else {
			GameVars.playerShipVariables.ship.currency -= hintCost;
			GameVars.showNotification = true;
			GameVars.notificationMessage = GetInfoOnNetworkedSettlementResource(currentSettlement.cargo[Random.Range(0, currentSettlement.cargo.Length)]);
		}

	}
	public void GUI_HireANavigator(Settlement thisSettlement, int costToHire) {
		//Do this if button pressed
		//Check to see if player has enough money to hire
		if (GameVars.playerShipVariables.ship.currency >= costToHire) {
			//subtract the cost from the players currency
			GameVars.playerShipVariables.ship.currency -= (int)costToHire;
			//change location of beacon
			Vector3 location = Vector3.zero;
			for (int x = 0; x < GameVars.settlement_masterList_parent.transform.childCount; x++)
				if (GameVars.settlement_masterList_parent.transform.GetChild(x).GetComponent<script_settlement_functions>().thisSettlement.settlementID == thisSettlement.settlementID)
					location = GameVars.settlement_masterList_parent.transform.GetChild(x).position;
			GameVars.navigatorBeacon.transform.position = location;
			GameVars.navigatorBeacon.GetComponent<LineRenderer>().SetPosition(0, new Vector3(location.x, 0, location.z));
			GameVars.navigatorBeacon.GetComponent<LineRenderer>().SetPosition(1, location + new Vector3(0, 400, 0));
			GameVars.playerShipVariables.UpdateNavigatorBeaconAppearenceBasedOnDistance();
			GameVars.playerShipVariables.ship.currentNavigatorTarget = thisSettlement.settlementID;
			GameVars.ShowANotificationMessage("You hired a navigator to " + thisSettlement.name + " for " + costToHire + " drachma.");
			//If not enough money, then let the player know
		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You can't afford to hire a navigator to " + thisSettlement.name + ".";
		}
	}


	//=================================================================================================================
	// SETUP THE SHIP REPAIR PANEL
	//=================================================================================================================	
	public void GUI_TAB_SetupShipRepairInformation() {

		//We need to do a clout check as well as a network checks
		//costToRepair is a GLOBAL var to this script
		int baseModifier = Mathf.CeilToInt(2 - GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID));
		if (GameVars.Network.CheckIfCityIDIsPartOfNetwork(GameVars.currentSettlement.settlementID)) {
			costToRepair = Mathf.CeilToInt(GameVars.currentSettlement.tax_network * baseModifier * 1);
		}
		else {
			costToRepair = Mathf.CeilToInt(GameVars.currentSettlement.tax_neutral * baseModifier * 1);
		}

		Text cost_allHP = (Text)tab_shiprepair_cost_allhp.GetComponent<Text>();
		Text cost_1HP = (Text)tab_shiprepair_cost_onehp.GetComponent<Text>();
		Text currentHP = (Text)tab_shiprepair_shiphealth.GetComponent<Text>();

		string oneHPCost = Mathf.CeilToInt(costToRepair).ToString();
		string allHPCost = (Mathf.CeilToInt(100 - Mathf.CeilToInt(GameVars.playerShipVariables.ship.health)) * costToRepair).ToString();
		string shipCurrent = Mathf.CeilToInt(GameVars.playerShipVariables.ship.health).ToString();

		//If the ship is at 100HP already, then let's not worry about giving the player the costs--we'll replace the costs by an X
		//	--and disable the repair buttons
		if (shipCurrent == "100") {
			cost_1HP.text = "X";
			cost_allHP.text = "X";
			currentHP.text = shipCurrent;
			tab_shiprepair_repairOneButton.GetComponent<Button>().interactable = false;
			tab_shiprepair_repairAllButton.GetComponent<Button>().interactable = false;
		}
		else {
			cost_1HP.text = oneHPCost;
			cost_allHP.text = allHPCost;
			currentHP.text = shipCurrent;
			tab_shiprepair_repairOneButton.GetComponent<Button>().interactable = true;
			tab_shiprepair_repairAllButton.GetComponent<Button>().interactable = true;
		}
	}


	//----------------------------------------------------------------------------
	//----------------------------SHIP REPAIR PANEL HELPER FUNCTIONS		
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_RepairShipByOneHP() {
		GameVars.playerShipVariables.ship.health += 1f;
		//make sure the hp can't go above 100
		if (GameVars.playerShipVariables.ship.health > 100) {
			GameVars.playerShipVariables.ship.health = 100;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "Your ship is already fully repaired";
		}
		else {
			GameVars.playerShipVariables.ship.currency -= costToRepair;
		}
		GUI_TAB_SetupShipRepairInformation();
	}

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_RepairShipByAllHP() {
		if (Mathf.CeilToInt(GameVars.playerShipVariables.ship.health) >= 100) {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "Your ship is already fully repaired";
		}
		else {
			GameVars.playerShipVariables.ship.currency -= (int)(costToRepair * Mathf.CeilToInt(100 - GameVars.playerShipVariables.ship.health));
			GameVars.playerShipVariables.ship.health = 100f;
		}
		GUI_TAB_SetupShipRepairInformation();
	}





	//============================================================================================================================================================================
	//============================================================================================================================================================================
	// ADDITIONAL FUNCTIONS FOR GUI BUTTONS (These are linked from the Unity Editor)
	//============================================================================================================================================================================

	//-----------------------------------------------------
	//THIS IS THE UNFURLING / FURLING OF SAILS BUTTON
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_furlOrUnfurlSails() {
		if (GameVars.sailsAreUnfurled) {
			hud_button_furlSails.transform.GetChild(0).GetComponent<Text>().text = "Furl Sails";
			GameVars.sailsAreUnfurled = false;
			foreach (GameObject sail in GameVars.sails)
				sail.SetActive(false);
		}
		else {
			hud_button_furlSails.transform.GetChild(0).GetComponent<Text>().text = "Unfurl Sails";
			GameVars.sailsAreUnfurled = true;
			foreach (GameObject sail in GameVars.sails)
				sail.SetActive(true);

		}
	}


	//-----------------------------------------------------
	//THIS IS THE DROP ANCHOR BUTTON
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_dropAnchor() {
		//If the controls are locked--we are traveling so force it to stop
		if (GameVars.controlsLocked && !GameVars.showSettlementGUI)
			GameVars.playerShipVariables.rayCheck_stopShip = true;
	}

	//-----------------------------------------------------
	//THIS IS THE REST BUTTON

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_restOverNight() {
		//If the controls are locked--we are traveling so force it to stop
		if (GameVars.controlsLocked && !GameVars.showSettlementGUI)
			GameVars.playerShipVariables.rayCheck_stopShip = true;
		//Run a script on the player controls that fast forwards time by a quarter day
		GameVars.controlsLocked = true;
		GameVars.playerShipVariables.PassTime(.25f, false);
	}

	//-----------------------------------------------------
	//THIS IS THE SAVE DATA BUTTON
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_saveGame() {
		GameVars.notificationMessage = "Saved Data File 'player_save_game.txt' To: " + Application.persistentDataPath + "/";
		GameVars.showNotification = true;
		GameVars.SaveUserGameData(false);
	}

	//-----------------------------------------------------
	//THIS IS THE RESTART GAME BUTTON	
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_restartGame() {
		GameVars.RestartGame();
	}

	//-----------------------------------------------------
	//THIS IS THE HELP BUTTON	
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_showHelpMenu() {
		hud_button_helpwindow.SetActive(true);
	}

	//-----------------------------------------------------
	//THIS IS THE CLOSE HELP BUTTON	
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_closeHelpMenu() {
		hud_button_helpwindow.SetActive(false);
	}

}
