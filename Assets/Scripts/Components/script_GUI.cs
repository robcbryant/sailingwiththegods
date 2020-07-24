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


	public bool useDialog = true;
	//-----------------------------------------------------------
	// Game Over Notification Variables
	//-----------------------------------------------------------
	[Header("Game Over Notification")]
	public GameObject gameover_main;
	public GameObject gameover_message;
	public GameObject gameover_restart;


	//-----------------------------------------------------------
	// Player Non-Port Notification Variables
	//-----------------------------------------------------------
	[Header("Not Port Notifications")]
	public GameObject nonport_info_main;
	public GameObject nonport_info_name;
	public GameObject nonport_info_notification;
	public GameObject nonport_info_okay;


	//-----------------------------------------------------------
	// Player Port Notification Variables
	//-----------------------------------------------------------
	[Header("Port Notifications")]
	public GameObject port_dialog;
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
	[Header("Player Notification")]
	public GameObject notice_notificationParent;
	public GameObject notice_notificationSystem;

	//-----------------------------------------------------------
	// Player HUD Variables
	//-----------------------------------------------------------
	[Header("Player HUD")]
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

	//-----------------------------------------------------------
	// Port Menu TAB Content Panel Variables
	//-----------------------------------------------------------
	

	//****************************************************************
	//GUI INFORMATION PANEL VARIABLES
	//****************************************************************
	[Header("Misc")]
	//---------------------
	//REPAIR SHIP PANEL VARIABLES
	int costToRepair;


	//===================================
	// OTHER VARS
	GameVars GameVars;
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
			if (GameVars.NotificationQueued) {
				ShowNotification(GameVars.QueuedNotificationMessage);
				GameVars.menuControlsLock = true;
				GameVars.ConsumeNotification();
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
			if (GameVars.NotificationQueued) {
				ShowNotification(GameVars.QueuedNotificationMessage);
				GameVars.menuControlsLock = true;
				GameVars.ConsumeNotification();
			}

			//`````````````````````````````````````````````````````````````````
			// GAME OVER GUI (prevent blocking a more important UI if one is up using menuControlIsLock to check)
			if (GameVars.isGameOver && !GameVars.menuControlsLock) {
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

		if (useDialog) {
			port_dialog.SetActive(true);
			port_dialog.GetComponent<DialogScreen>().StartDialog(GameVars.currentSettlement);
		}
		else {
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
				if (crewMemberWithNetwork != null)
					portMessage += "Your crewman, " + crewMemberWithNetwork.name + " assures you their connections here are strong! They should welcome you openly and waive your port taxes on entering!";
				else
					portMessage += "You know this port as captain very well! You expect that your social connections here will soften the port taxes in your favor!";
			}
			else {
				portMessage += "This port is outside your social network!\n";
			}

			if (GameVars.currentPortTax != 0) {
				portMessage += "If you want to dock here, your tax for entering will be " + GameVars.currentPortTax + " drachma. \n";
				//If the port tax will make the player go negative--alert them as they enter
				if (GameVars.playerShipVariables.ship.currency - GameVars.currentPortTax < 0)
					portMessage += "Docking here will put you in debt for " + (GameVars.playerShipVariables.ship.currency - GameVars.currentPortTax) + "drachma, and you may lose your ship!\n";
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
		Globals.Quests.CheckCityTriggers(GameVars.currentSettlement.settlementID);
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

		//Add a new route to the player journey log as a port entry
		GameVars.playerShipVariables.journey.AddRoute(new PlayerRoute(GameVars.playerShip.transform.position, Vector3.zero, GameVars.currentSettlement.settlementID, GameVars.currentSettlement.name, false, GameVars.playerShipVariables.ship.totalNumOfDaysTraveled), GameVars.playerShipVariables, GameVars.CaptainsLog);
		//We should also update the ghost trail with this route otherwise itp roduce an empty 0,0,0 position later
		GameVars.playerShipVariables.UpdatePlayerGhostRouteLineRenderer(GameVars.IS_NOT_NEW_GAME);

		//-------------------------------------------------
		// UPDATE PLAYER CLOUT METER
		GUI_UpdatePlayerCloutMeter();


		//-------------------------------------------------
		// OTHER PORT GUI SETUP FUNCTIONS
		GetCrewHometowns();
		GUI_GetListOfBuiltMonuments();
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



	//=================================================================================================================
	// NOTIFICATION POP-UP SYSTEM
	//=================================================================================================================	

	void ShowNotification(string message) {

		// KD: I don't think the complexity of stacked notifications is needed. pretty sure it'd be okay to queue them up (stack them so you close one by one)
		// also changed to the new popup which i think looks better
		Globals.UI.Show<InfoScreen, InfoScreenModel>(new InfoScreenModel {
			Title = "Attention!",
			Message = message,
			OnClose = () => GameVars.menuControlsLock = false
	});
		
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
	// ADDITIONAL FUNCTIONS FOR GUI BUTTONS (These are linked from the Unity Editor)
	//============================================================================================================================================================================

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

}
