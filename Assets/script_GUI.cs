using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

public class script_GUI : MonoBehaviour {

	globalVariables MGV;
	
	public GUISkin myGuiSkin;
	GUIStyle style_background = new GUIStyle();
	GUIStyle style_button = new GUIStyle();
	GUIStyle style_label = new GUIStyle();
	
	float gui_city_left = 20;
	float gui_city_top = 20;
	float gui_city_width = 430;
	float gui_city_height = 550;
	
	float gui_ship_left = 460;
	float gui_ship_top = 20;
	float gui_ship_width = 430;
	float gui_ship_height = 550;
	
	float gui_row_width = 310;
	float gui_row_height = 20;
	
	public float screenX;
	public float screenY;
	public float xUnit;
	public float yUnit;
	
	float gui_stat_left = 1200;
	float gui_stat_top = 580;
	float gui_stat_w = 400;
	float gui_stat_h = 120;
	
	
	float originalHeight = 720;
	float originalWidth = 1680;
	
	public bool showNotification = false;
	//string notificationMessage = "testing message";
	
	Vector2 scrollPosition = new Vector2(0,0);
	Vector2 captainsLogScrollPosition = Vector2.zero;
	Vector2 hireNavigatorScrollPosition = Vector2.zero;
	
	//****************************************************************
	//GUI INFORMATION PANEL VARIABLES
	//****************************************************************
	
	//This is a general use list of subset Settlements from the master list for any panel to use
	List<Settlement> relevantSettlements = new List<Settlement>();
	
	//---------------------
	//LOAN PANEL VARIABLES
	float numOfDaysToPayOffLoan;
	float baseLoanAmount;
	int loanAmount;
	float baseInterestRate;
	float finalInterestRate;
	float totalAmountDueAtTerm;
	
	//---------------------
	//NAVIGATOR PANEL VARIABLES
	List<int> navPanelCosts = new List<int>();
	//---------------------
	//ASK ABOUT CITY PANEL VARIABLES
	List<int> costForHints = new List<int>();
	
	//---------------------
	//BUILD A SHRINE PANEL VARIABLES
	int costToBuild;
	
	//---------------------
	//HIRE CREW PANEL VARIABLES
	List<int> hireCrewCosts = new List<int>();
	//---------------------
	//FIRE CREW PANEL VARIABLES
	
	//---------------------
	//REPAIR SHIP PANEL VARIABLES
	int costToRepair;
	
	
	// Use this for initialization
	void Start () {
	MGV = GameObject.FindGameObjectWithTag("global_variables").GetComponent<globalVariables>();

	style_background.fontSize = (int) yUnit * 3;
	style_background.normal.textColor = Color.white;
	style_background.normal.background = MakeTex( 2, 2, new Color( 0f, .5f, .5f, 0.8f ) );
	
	style_button.fontSize = (int) yUnit * 3;
	style_button.normal.textColor = Color.white;
	style_button.hover.background = MakeTex( 2, 2, new Color( 0f, .9f, .9f, 0.8f ) );
	style_button.active.background = MakeTex( 2, 2, new Color( 0f, .3f, .3f, 0.8f ) );
	style_button.normal.background = MakeTex( 2, 2, new Color( 0f, .8f, .8f, 0.8f ) );
	
	style_label.fontSize = (int) yUnit * 3;
	style_label.normal.textColor = Color.white;
	
	xUnit = Screen.width / 100f;
	yUnit = Screen.height / 100f;
		

	
	}
	
	void OnGUI(){
	//************************************************************
	// This code controls the screen scaling of the GUI interface!
	//		It finishes at the end of OnGUI() with a Reset on the Matrix
	//************************************************************
	
	Vector2 ratio = new Vector2(Screen.width/originalWidth, Screen.height/originalHeight);
	Matrix4x4 guiMatrix = Matrix4x4.identity;
	guiMatrix.SetTRS(new Vector3(1,1,1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1) );
	GUI.matrix = guiMatrix;
	
	//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
	
	
	GUI.skin = myGuiSkin;
		//############################
		// UPDATE SCREEN RATIO SIZING
		//############################
		//This portions out the screen in percent. Each x or y unit is = to 1% of the screen in that dimension
		screenX = Screen.width;
		screenY = Screen.height;
		xUnit = Screen.width / 100f;
		yUnit = Screen.height / 100f;
		style_background.fontSize = (int) (yUnit * 2f);
		style_label.fontSize = (int) (yUnit * 2f);
		style_button.fontSize = (int) (yUnit * 2f);
	
	
	
	//=====================================================================================================================================
	// IF WE ARE AT THE TITLE SCREEN
	if (MGV.isTitleScreen){	
			//Show background fade out
			if (MGV.showNotification || MGV.showSecondaryNotification) GUI.Window (0,new Rect (0, 0, originalWidth, originalHeight), ShowNotification, "");
			
			if(GUI.Button(new Rect(1060,330,400,40), "-----NEW GAME : Normal-----")){
				MGV.isTitleScreen = false;
				MGV.isStartScreen = true;
				MGV.bg_titleScreen.SetActive(false);
				MGV.bg_startScreen.SetActive(true);
				MGV.FillNewGameCrewRosterAvailability();
				MGV.gameDifficulty_Beginner = false;
			}
			if(GUI.Button(new Rect(1060,380,400,40), "-----NEW GAME : Beginner(Map)-----")){
				MGV.isTitleScreen = false;
				MGV.isStartScreen = true;
				MGV.bg_titleScreen.SetActive(false);
				MGV.bg_startScreen.SetActive(true);
				MGV.FillNewGameCrewRosterAvailability();
				MGV.gameDifficulty_Beginner = true;
			}
			if(GUI.Button(new Rect(1060,430,400,40), "-----LOAD GAME-----")){
				//if a successful load continue--otherwise stay on title screen
				if (MGV.LoadSavedGame()){
					MGV.isLoadedGame = true;
					MGV.isTitleScreen = false;
					MGV.isStartScreen = false;
					MGV.bg_titleScreen.SetActive(false);
					MGV.bg_startScreen.SetActive(false);
					MGV.camera_titleScreen.SetActive(false);
					//Turn on the environment fog
					RenderSettings.fog = true;
					//Now turn on the main player controls camera
					MGV.FPVCamera.SetActive(true);
					//Turn on the player distance fog wall
					MGV.playerShipVariables.fogWall.SetActive(true);
					//Now enable the controls
					MGV.controlsLocked = false;
					//For the argonautica, let's set the crew capacity to 30
					MGV.playerShipVariables.ship.crewCapacity = 30;
					MGV.playerShipVariables.ship.crew = MGV.playerShipVariables.ship.crewRoster.Count;
					//Let's increase the ships cargo capacity
					MGV.playerShipVariables.ship.cargo_capicity_kg = 1200f;
					//Set the player's initial position to the new position
					MGV.playerShipVariables.lastPlayerShipPosition = MGV.playerShip.transform.position;
					//Update Ghost Route
					MGV.LoadSavedGhostRoute();
					//Setup Difficulty Level
					MGV.gameDifficulty_Beginner = false;
					MGV.SetupBeginnerGameDifficulty();
					
					MGV.controlsLocked = false;
				}
				
			}
			if(GUI.Button(new Rect(1060,480,400,40), "-----LOAD GAME : Beginner(Map)-----")){
				//if a successful load continue--otherwise stay on title screen
				if (MGV.LoadSavedGame()){
					MGV.isLoadedGame = true;
					MGV.isTitleScreen = false;
					MGV.isStartScreen = false;
					MGV.bg_titleScreen.SetActive(false);
					MGV.bg_startScreen.SetActive(false);
					MGV.camera_titleScreen.SetActive(false);
					//Turn on the environment fog
					RenderSettings.fog = true;
					//Now turn on the main player controls camera
					MGV.FPVCamera.SetActive(true);
					//Turn on the player distance fog wall
					MGV.playerShipVariables.fogWall.SetActive(true);
					//Now enable the controls
					MGV.controlsLocked = false;
					//For the argonautica, let's set the crew capacity to 30
					MGV.playerShipVariables.ship.crewCapacity = 30;
					MGV.playerShipVariables.ship.crew = MGV.playerShipVariables.ship.crewRoster.Count;
					//Let's increase the ships cargo capacity
					MGV.playerShipVariables.ship.cargo_capicity_kg = 1200f;
					//Set the player's initial position to the new position
					MGV.playerShipVariables.lastPlayerShipPosition = MGV.playerShip.transform.position;
					//Update Ghost Route
					MGV.LoadSavedGhostRoute();
					//Setup Difficulty Level
					MGV.gameDifficulty_Beginner = true;
					MGV.SetupBeginnerGameDifficulty();
					
					MGV.controlsLocked = false;
				}
				
			}
			
			
			
			//=====================================================================================================================================	
	// IF WE ARE AT THE START SCREEN	-- SHOW START SCREEN GUI
	} else if (MGV.isStartScreen){
			//===================================================
			//HERE WE DISPLAY THE GAME INFORMATION AND DIRECTIONS
			GUI.Label (new Rect (260,120,220,500),"Use the arrow keys to rotate the camera view in the game.\n\n" +
												  "Use the mouse wheel to zoom in and out from the player's ship.\n\n" +
												  "Use the left mouse button to select things.\n\n" +
												  "Use the mouse to move the cursor around the world. If the spinning cursor turns blue, then you can select that location to sail to. If the cursor is red, then the player does not have a line of sight " + 
												  "to that location, or the cursor is over land or obstacles in the sea.");
			GUI.Label (new Rect (50,395,440,500), "Your goal, as a new captain, is to sail the dangerous waters of the ancient Mediterranean! Be wary of pirates, storms, and dying at sea! Learn to put together "+
												  "a good crew to reach distant lands and trade along the way. Trading is essential for resupplying your crew with Provisions and water, gaining clout and prestige, repairing "+
												  "one's ship, and replacing crew who were lost asea. Keep an eye on the winds and water currents--a captain should plan their routes accordingly "+
												  "to reach their destinations more easily. Never be afraid to furl sails and ride the currents against the wind! Sticking close to the coastlines will also keep the ship away "+
												  "from the stronger sea currents. If a captain knows the stars, look for them at night to steer by. You'll find north close to Ursa Minor in the ancient era "+
												  "where axial precession has changed the skies! There are no maps and sophisticated navigation here. Learn the landscape and skies to find your way...or perhaps hire a navigator for a hint. "+
												  "Expand your knowledge of the world, and its many places by traveling to new ports and settlements! Knowledge is key and so is experience! Good luck!");
			//===========================================================
			//HERE WE GIVE THE FIRST LEG OF THE ARGONAUTICA QUEST / INTRO
			GUI.Label (new Rect (540,150,340,500),"Welcome to the Argonautica Jason! Find your way through the dangerous seas to complete your quest! You have found yourself at Pagasae, where King Pelias has given you the task of sailing across "+
												  "the Aegean and the Black Sea to retrieve the Golden Fleece.  This is the hide of the flying ram that brought Phrixus from Boetia to Aea. The hide now hangs on a tree on the other side of the Black "+
												  "Sea in the city of Aea. The great lord Aeetes prizes the fleece, and a very large dragon guards it. \n\nThe task seems impossible! But you do not sail alone, Jason. You have assembled a group of the most "+
												  "powerful warriors, sailors, and prophets in Greece to help you in your quest.  Most are the sons of royal families, and each one has a unique skill. Once the heroes have all arrived, your crew stocks "+
												  "the ships and the people of Pagasae greet you all.");
			
			//======================================================================
			//HERE WE DISPLAY A LIST OF CREW FOR THE PLAYER TO CONSTRUCT A CREW FROM
			
			//Update the number of currently selected crewmen
			int numOfCrew = 0;
			foreach(bool i in MGV.newGameCrewSelectList) if(i) numOfCrew++;
			GUI.Label (new Rect (960,150,590,500),"It's up to you, Jason, to select your crew! Certain members, integral to your journey, must be chosen...the others are up to you! Choose wisely! You will start with 20 crewmen provided by the King of Pagasae!");
			GUI.Label (new Rect (960,210,590,500),"You currently have "+ numOfCrew +" / 20 crewmembers!\n___________________________________________________________________________________");
			
			
			//_____________________________________
			//START OF THE CREW ROSTER SCROLL VIEW
			scrollPosition = GUI.BeginScrollView(new Rect( 960,250,620,380), scrollPosition, new Rect(960,250,600,4000),false,true);
				
				for(int i = 0; i < MGV.newGameAvailableCrew.Count; i++){
					//If the crewmember is necessary for the quest--lock the selection in as true
					if(!MGV.newGameAvailableCrew[i].isKillable){
						MGV.newGameCrewSelectList[i] = GUI.Toggle (new Rect (960,250 + (i*100),50,50),true,"");
					//Otherwise let the player toggle on/off the selection
					} else {
						//We also need to run a check on whether or not we have 30 members--if we do, then hide the check box if it's 'false'
						if(numOfCrew != 20 || MGV.newGameCrewSelectList[i]){
							MGV.newGameCrewSelectList[i] = GUI.Toggle (new Rect (960,250 + (i*100),50,50),MGV.newGameCrewSelectList[i],"");
						}
					}
					//Show the Crewmember's information / backstory
					string title = MGV.GetCloutTitleEquivalency(MGV.newGameAvailableCrew[i].clout) + " " + MGV.newGameAvailableCrew[i].name + ", the " + MGV.GetJobClassEquivalency(MGV.newGameAvailableCrew[i].typeOfCrew) + " from " +GetSettlementFromID(MGV.newGameAvailableCrew[i].originCity).name + ".";
					GUI.Label (new Rect (1010,255 + (i*100),520,60),title);
					GUI.Label (new Rect (1010,257 + (i*100),520,60),"___________________________________________________________________");
					GUI.Label (new Rect (1010,270 + (i*100),520,100),MGV.newGameAvailableCrew[i].backgroundInfo);

				
				
				}						
				
			GUI.EndScrollView();
			
			if(numOfCrew == 20 || MGV.DEBUG_MODE_ON){
				if(GUI.Button(new Rect(1060,640,400,40), "-----START GAME-----")){
					MGV.startGameButton_isPressed = true;
				}
			}
	//======================================================================
	//WE ARE SHOWING A YES / NO  PORT TAX NOTIFICATION POP UP	
	} else if(MGV.showPortDockingNotification){
			GUI.Box(new Rect (540,250,590,300), "You have found " + MGV.currentSettlement.name + "!");

			GUI.Label(new Rect (540,257,590,300), "                   _______________________________________________________________");
			GUI.Label(new Rect (540,280,550,300), MGV.currentSettlement.description);
			if (MGV.isInNetwork){
				GUI.Label(new Rect (540,400,550,300), "This port is part of your network!");
				string message = "";
				if (MGV.crewMemberWithNetwork != null)
				message = "Your crewman, " + MGV.crewMemberWithNetwork.name + " assures you their connections here are strong! They should welcome you openly and waive your port taxes on entering!";
				else
				message = "You know this port as captain very well! You expect that your social connections here will soften the port taxes in your favor!";
				GUI.Label(new Rect (540,420,550,300), message);	
			} else {
				GUI.Label(new Rect (540,400,550,300), "This port is outside your social network!");
				string message = "If you want to dock here, your tax for entering will be " + MGV.currentPortTax + " drachma. ";
				//If the port tax will make the player go negative--alert them as they enter
				if (MGV.playerShipVariables.ship.currency - MGV.currentPortTax < 0) message += "Docking here will put you in debt for " + (MGV.playerShipVariables.ship.currency - MGV.currentPortTax) + "drachma, and you may lose your ship";
				GUI.Label(new Rect (540,420,550,300), message);
			}
			if(GUI.Button(new Rect(730,480,100,40), "Enter Port")){
				//Check if current Settlement is part of the main quest line
				MGV.CheckIfCurrentSettlementIsPartOfMainQuest(MGV.currentSettlement.settlementID);
				//Add this settlement to the player's knowledge base
				MGV.playerShipVariables.ship.playerJournal.AddNewSettlementToLog(MGV.currentSettlement.settlementID);
				//Determine what settlements are available to the player in the tavern
				MGV.GenerateProbableInfoListOfSettlementsInCurrentNetwork();
				MGV.GenerateListOfAvailableCrewAtCurrentPort();
				MGV.showSettlementTradeGUI = true;
				MGV.showSettlementTradeButton = false;
				MGV.controlsLocked = true;
				
				//Figure out the tax on the cargo hold
				if (MGV.isInNetwork) MGV.currentPortTax = 0;
				else MGV.currentPortTax = GetTaxRateOnCurrentShipManifest();
				
				foreach (int i in MGV.playerShipVariables.ship.playerJournal.knownSettlements) Debug.Log ("(*) " +i);
				MGV.showPortDockingNotification = false;
				
				//Add a new route to the player journey log as a port entry
				MGV.playerShipVariables.journey.AddRoute(new PlayerRoute( new Vector3(MGV.currentSettlement.location_longXlatY.x, MGV.currentSettlement.location_longXlatY.y, MGV.currentSettlement.elevation), Vector3.zero, MGV.currentSettlement.settlementID, MGV.currentSettlement.name, false, MGV.playerShipVariables.ship.totalNumOfDaysTraveled), MGV.playerShipVariables, MGV.currentCaptainsLog);

				
			}
			if(GUI.Button(new Rect(830,480,100,40), "Leave")){
				MGV.showPortDockingNotification = false;
			}	
			
	} else if(MGV.showNonPortDockingNotification){
		GUI.Box(new Rect (540,250,590,300), "You have found " + MGV.currentSettlement.name + "!");
		GUI.Label(new Rect (540,257,590,300), "                   _______________________________________________________________");
		GUI.Label(new Rect (540,280,550,300), MGV.currentSettlement.description);		
		if(GUI.Button(new Rect(830,480,100,40), "Leave")){
			MGV.showNonPortDockingNotification = false;
		}		
	//=====================================================================================================================================	
	//IF WE AREN'T AT THE TITLE SCREEN OR START SCREEN OR SPECIAL NOTIFICATION WINDOW
	} else {
		//Show background fade out
		if (MGV.showNotification || MGV.showSecondaryNotification) GUI.Window (0,new Rect (0, 0, originalWidth, originalHeight), ShowNotification, "");

		
		//###########################################
		//	CAPTAINS LOG GUI ELEMENT
		//###########################################
		//TODO I need to make a for loop that turns every log entry into another label because there is currently a limit to how many characters are allowed in a string to pass to the Label(it's a lot but not enough)
		//Only show this if the help menu is off--otherwise the GUI layer textures will block the help screen camera sprite b/c the GUI layer is ALWAYS on top
		if(!MGV.showHelpGUI){
			GUI.Box (new Rect(0,580,840,140), "");
			captainsLogScrollPosition = GUI.BeginScrollView(new Rect(0,580,820,130), captainsLogScrollPosition, new Rect(0,580,800,(MGV.currentCaptainsLog.Length/170)*130),false,true);
			GUI.Label (new Rect (10,580,780,(MGV.currentCaptainsLog.Length/170)*130), MGV.currentCaptainsLog);	
			GUI.EndScrollView();
		}
		// ######################
		// 	SHIP STATS GUI
		// ######################

		GUI.Box (new Rect(gui_stat_left,gui_stat_top,gui_stat_w,gui_stat_h), "Ship Stats");
		GUI.Label (new Rect (gui_stat_left+10,gui_stat_top + 20,gui_stat_w,gui_stat_h),  MGV.playerShipVariables.ship.cargo[0].name + " :\t" + (int)MGV.playerShipVariables.ship.cargo[0].amount_kg + "kg");
		GUI.Label (new Rect (gui_stat_left+125,gui_stat_top + 20,gui_stat_w,gui_stat_h), "# of Days Traveled: " + (Mathf.Round(MGV.playerShipVariables.ship.totalNumOfDaysTraveled * 1000.0f) / 1000.0f) + ",  " + GetCurrentMonth());
		GUI.Label (new Rect (gui_stat_left+10,gui_stat_top + 40,gui_stat_w,gui_stat_h), MGV.playerShipVariables.ship.cargo[1].name + " :" + (int)MGV.playerShipVariables.ship.cargo[1].amount_kg + "kg");
		GUI.Label (new Rect (gui_stat_left+10,gui_stat_top + 60,gui_stat_w,gui_stat_h), (int)MGV.playerShipVariables.ship.health + "hp"); 
		GUI.Label (new Rect (gui_stat_left+125,gui_stat_top + 40,gui_stat_w,gui_stat_h), "Speed: " + (Mathf.Round(MGV.playerShipVariables.current_shipSpeed_Magnitude * 1000.0f) / 1000.0f) + "km/h");
		GUI.Label (new Rect (gui_stat_left+240,gui_stat_top + 40,gui_stat_w,gui_stat_h), "Clout: " + MGV.GetCloutTitleEquivalency((int)(Mathf.Round(MGV.playerShipVariables.ship.playerClout * 1000.0f) / 1000.0f)));
		GUI.Label (new Rect (gui_stat_left+10,gui_stat_top + 80,gui_stat_w,gui_stat_h),  "Crew members:   " + MGV.playerShipVariables.ship.crew);
		GUI.Label (new Rect (gui_stat_left+170,gui_stat_top + 60,gui_stat_w,gui_stat_h),  "# Days Thirsty:\t" + MGV.playerShipVariables.dayCounterThirsty);
		GUI.Label (new Rect (gui_stat_left+170,gui_stat_top + 80,gui_stat_w,gui_stat_h),  "# Days Starving:\t" + MGV.playerShipVariables.dayCounterStarving);

		
		//################################################
		//THIS IS THE SETTLEMENT DOCKING BUTTON
		
		if (MGV.showSettlementTradeButton){
			if(GUI.Button(new Rect(850,580,200,130), "CLICK TO \n  DOCK WITH \n" + MGV.currentSettlement.name)){
				MGV.showPortDockingNotification = true;
				//Figure out if this settlement is part of the player's network
				MGV.isInNetwork = MGV.CheckIfCityIDIsPartOfNetwork(MGV.currentSettlement.settlementID);
				//Figure out the tax on the cargo hold
				MGV.currentPortTax = GetTaxRateOnCurrentShipManifest();
				
			}
			//Show a blank working button if not
		} else if (MGV.showNonPortDockButton){
			if(GUI.Button(new Rect(850,580,200,130), "CHECK OUT \n" + MGV.currentSettlement.name)){
				MGV.showNonPortDockingNotification = true;
				//Figure out if this settlement is part of the player's network
				MGV.isInNetwork = MGV.CheckIfCityIDIsPartOfNetwork(MGV.currentSettlement.settlementID);
				//Figure out the tax on the cargo hold
				MGV.currentPortTax = GetTaxRateOnCurrentShipManifest();
				
			}		
		} else {
			if(GUI.Button(new Rect(850,580,200,130), "DOCKING \n CURRENTLY \n UNAVAILABLE")){
			}
			
		}
		
		if (!MGV.showSettlementTradeGUI){
		//################################################
		//THIS IS THE UNFURLING / FURLING OF SAILS BUTTON

			if(MGV.sailsAreUnfurled){
				if(GUI.Button(new Rect(1060,578,120,25), "FURL SAILS")){
					MGV.sailsAreUnfurled = false;
					foreach(GameObject sail in MGV.sails)
						sail.SetActive(false);
				}
			} else {
				if(GUI.Button(new Rect(1060,578,120,25), "UNFURL SAILS")){
					MGV.sailsAreUnfurled = true;
					foreach(GameObject sail in MGV.sails)
						sail.SetActive(true);
				}
			}
			
			//################################################
			//THIS IS THE DROP ANCHOR BUTTON
			
			if(GUI.Button(new Rect(1060,604,120,25), "DROP ANCHOR")){
				//If the controls are locked--we are traveling so force it to stop
				if(MGV.controlsLocked && !MGV.showSettlementTradeGUI)
					MGV.playerShipVariables.rayCheck_stopShip = true;
			}
			
			//################################################
			//THIS IS THE DROP ANCHOR BUTTON
			
			if(GUI.Button(new Rect(1060,630,120,25), "REST")){
				//If the controls are locked--we are traveling so force it to stop
				if(MGV.controlsLocked && !MGV.showSettlementTradeGUI)
					MGV.playerShipVariables.rayCheck_stopShip = true;
				//Run a script on the player controls that fast forwards time by a quarter day
				MGV.isPassingTime = true;
				MGV.controlsLocked = true;
				StartCoroutine(MGV.playerShipVariables.WaitForTimePassing(.25f, false));
			}	
		
			//################################################
			//THIS IS THE SAVE DATA BUTTON
			
		
				if(GUI.Button(new Rect(1060,656,120,25), "SAVE DATA")){
					MGV.notificationMessage = "Saved Data File 'player_data.csv' To: " + Application.dataPath + "/player_data.csv" ;
					MGV.showNotification = true;
					MGV.SaveUserGameData();
				}
			
			
			if(GUI.Button(new Rect(1060,682,120,25), "RESTART GAME")){
				MGV.RestartGame();
			}
		
		}
		

		
		//#############################
		//  SETTLEMENT TRADE MENU GUI
		//#############################
		if (MGV.showSettlementTradeGUI){

			//Draw the main background of the trade window
			GUI.Box (new Rect(gui_city_left,gui_city_top,gui_city_width,gui_city_height), "Settlement Info");
			//Draw the Exit Button
			if(GUI.Button(new Rect(310,85 ,80,25), "Leave Port")){
				if (CheckIfPlayerCanAffordToPayPortTaxes()){
					MGV.showSettlementTradeGUI = false;
					MGV.showSettlementTradeButton = true;
					//MGV.controlsLocked = false;
					//Start Our time passage
					MGV.isPassingTime = true;
					StartCoroutine(MGV.playerShipVariables.WaitForTimePassing(.25f, true));
					MGV.justLeftPort = true;
					MGV.playerShipVariables.ship.currency -= MGV.currentPortTax;
					//Be sure to reset the tavern menu to the original tavern state
					MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_DEFAULT;
					
					//Add a new route to the player journey log as a port exit
						MGV.playerShipVariables.journey.AddRoute(new PlayerRoute(new Vector3(MGV.currentSettlement.location_longXlatY.x, MGV.currentSettlement.location_longXlatY.y, MGV.currentSettlement.elevation), Vector3.zero, MGV.currentSettlement.settlementID, MGV.currentSettlement.name, true, MGV.playerShipVariables.ship.totalNumOfDaysTraveled), MGV.playerShipVariables, MGV.currentCaptainsLog);

					
				} else {//Debug.Log ("Not Enough Drachma to Leave the Port!");
					MGV.showNotification = true;
					MGV.notificationMessage = "Not Enough Drachma to pay the port tax and leave!";
				}
			}
			
			//Show Port Tax
			GUI.Label (new Rect (gui_city_left + 240,50,150, 50), "Cargo Tax to Leave: \n\t" + MGV.currentPortTax + "drachma");
			
			//Settlement Info and Manifest
			GUI.Label (new Rect (gui_city_left,gui_city_top + 25,gui_row_width, gui_row_height), MGV.currentSettlement.name);
			GUI.Label (new Rect (gui_city_left,gui_city_top + 50,gui_row_width, gui_row_height), "Population: "+MGV.currentSettlement.population);
			GUI.Label (new Rect (gui_city_left,gui_city_top + 75,gui_row_width, gui_row_height), "Description:\n" + MGV.currentSettlement.description);
			//Settlement Resources
			ShowSettlementResources();
			//Buy Buttons
			ShowBuyButtons();



			
			//Draw the main background of the trade window
			GUI.Box (new Rect(gui_ship_left,gui_ship_top,gui_ship_width,gui_ship_height), "Ship Manifest");
			//Player Ship Info and Manifest
			GUI.Label (new Rect (gui_ship_left,gui_ship_top + 25,gui_row_width, 20), MGV.playerShipVariables.ship.name);
			GUI.Label (new Rect (gui_ship_left,gui_ship_top + 50,gui_row_width, 20), "Crew Members: "+MGV.playerShipVariables.ship.crewRoster.Count);
			GUI.Label (new Rect (gui_ship_left,gui_ship_top + 75,gui_row_width, 70), "Description: A small trireme.");
			GUI.Label (new Rect (gui_ship_left,gui_ship_top + 100,gui_row_width, 70), "Currency: " + MGV.playerShipVariables.ship.currency);
			GUI.Label (new Rect (gui_ship_left,gui_ship_top + 125,gui_row_width, 20), "Cargo Limit(kg): "+Mathf.CeilToInt(MGV.playerShipVariables.ship.GetTotalCargoAmount())+ " / " +MGV.playerShipVariables.ship.cargo_capicity_kg);
			//Settlement Resources
			ShowShipResources();
			//Sell Buttons
			ShowSellButtons();
	

			

			
			float gui_network_left = 1050;
			float gui_network_top = 20;
			float gui_network_width = 580;
			float gui_network_height = 550;
			
			//###########################################
			//	GOSSIP AND RUMOR GUI ELEMENT
			//###########################################
			
			GUI.Box (new Rect(gui_network_left,gui_network_top,gui_network_width,gui_network_height), "City Activities");
			
			GUI.TextArea(new Rect(gui_network_left,gui_network_top+40,gui_network_width,gui_network_height-370), "");
			
			
			//THESE ARE THE MENU BUTTONS
			if( GUI.Button (new Rect (gui_network_left + 5,gui_network_top + 200+ (25),185, 20),"Ask About a City" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_CITY;SetupCityQuestionPanelVariables();}
			if( GUI.Button (new Rect (gui_network_left + 5 + 185,gui_network_top + 200+ (25),185, 20),"Hire A Navigator" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_NAVIGATOR; SetupNavigatorPanelVariables();}
			if( GUI.Button (new Rect (gui_network_left + 5 + 370,gui_network_top + 200+ (25),185, 20),"Build A Shrine" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_SHRINE;SetupBuildAShrinePanelVariables();}
			if( GUI.Button (new Rect (gui_network_left + 5,gui_network_top + 200+ (50),185, 20),"Look into A Loan" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_LOAN; SetupLoanPanelVariables();}
			if( GUI.Button (new Rect (gui_network_left + 5 + 185,gui_network_top + 200+ (50),185, 20),"Hire Crew Members" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_HIRECREW;SetupHireCrewMemberPanelVariables();}
			if( GUI.Button (new Rect (gui_network_left + 5 + 370,gui_network_top + 200+ (50),185, 20),"Look For Work" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_JOBS;}
			if( GUI.Button (new Rect (gui_network_left + 5,gui_network_top + 200+ (75),185, 20),"Ship Repairs" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_REPAIRS;SetupRepairShipPanelVariables();}
			if( GUI.Button (new Rect (gui_network_left + 5 + 185,gui_network_top + 200+ (75),185, 20),"Fire Crew Members" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_FIRECREW;SetupFireCrewMemberPanelVariables();}
			if( GUI.Button (new Rect (gui_network_left + 5 + 370,gui_network_top + 200+ (75),185, 20),"Tavern" ) ){MGV.currentTavernMenuState = GV_CONST.TAVERNSTATE_DEFAULT;}			
			
			
			//scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,gui_network_height-400), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,1000),false,true);
			//START OF SCROLLBAR MENU WINDOW

				
				switch (MGV.currentTavernMenuState){
				
					case GV_CONST.TAVERNSTATE_DEFAULT:
					// THIS IS THE OPENING DEFAULT VIEW FOR THE TAVERN
					// --HERE WE SHOW A FEW RANDOM TIDBITS OF INFORMATION OVERHEARD IN THE TAVERN--IT MAY BE NEWS OF A NEW CITY
						BuildTavernPanel();
						break;
					case GV_CONST.TAVERNSTATE_CITY:
					//THIS IS THE CITY INFORMATION PANEL
					//--HERE IS WHERE THE PLAYER CAN BUY INFORMATION ABOUT A CITY
						BuildCityQuestionPanel();
						break;
					case GV_CONST.TAVERNSTATE_NAVIGATOR:
					//THIS IS THE PANEL TO HIRE A NAVIGATOR
					//--THE PLAYER CAN CHOOSE TO HIRE A NAVIGATOR BASED ON KNOWN CITIES IN THEIR JOURNAL
						BuildHireNavigatorPanel();
						break;
					case GV_CONST.TAVERNSTATE_SHRINE:
					//THIS IS THE PANEL TO BUILD A SHRINE
					//--HERE THE PLAYER CAN OFFER TO PAY FOR A SHRINE TO BE BUILT FOR THE LOCAL GOD(S) RECIEVING
					//--LARGE AMOUNTS OF CLOUT AND BECOMES PART OF THE CITY'S NETWORK
						BuildAShrinePanel();
						break;
					case GV_CONST.TAVERNSTATE_LOAN:
					//THIS IS THE LOAN PANEL
					//--HERE THE PLAYER CAN TAKE OUT A LOAN--THE AMOUNT DETERMINED BY CLOUT--WITH OPTIONS TO PAY IT OFF 
					//--WITH INTEREST AND DUE DATES FOR REPAYMENT
						BuildLoanPanel();
						break;
					case GV_CONST.TAVERNSTATE_HIRECREW:
					//THIS IS THE PANEL TO HIRE CREW MEMBERS
					//--THE PLAYER CAN HIRE NEW CREW MEMBERS AND VIEW BASIC INFO ABOUT THEM BEFORE HIRING
						BuildHireCrewMemberPanel();
						break;
					case GV_CONST.TAVERNSTATE_JOBS:
					//THIS IS THE 'QUEST' PANEL
					//--HERE THE PLAYER CAN ASSIST IN GAME CHARACTERS THROUGH POSSIBLE QUESTS THAT EARN MONEY / RESOURCES / OR CLOUT
						break;
					case GV_CONST.TAVERNSTATE_REPAIRS:
						//THIS IS THE PANEL TO HIRE CREW MEMBERS
						//--THE PLAYER CAN HIRE NEW CREW MEMBERS AND VIEW BASIC INFO ABOUT THEM BEFORE HIRING
						BuildRepairShipPanel();
						break;
					case GV_CONST.TAVERNSTATE_FIRECREW:
						//THIS IS THE 'QUEST' PANEL
						//--HERE THE PLAYER CAN ASSIST IN GAME CHARACTERS THROUGH POSSIBLE QUESTS THAT EARN MONEY / RESOURCES / OR CLOUT
						BuildFireCrewMemberPanel();
						break;				
				}
				

			//END OF SCROLLBAR WINDOW


		}
		


			if(!MGV.showHelpGUI){
				if(GUI.Button(new Rect(10,10,120,25), "?")){
					MGV.showHelpGUI = true;
					//Adjust the attributes of the GUI splash screen so it only shows the left portion of the background
					MGV.bg_startScreen.transform.localPosition = new Vector3(-6.18f, 0, 1);
					MGV.bg_startScreen.transform.localScale = new Vector3(5.69f, 10.2f, 0);
					MGV.bg_startScreen.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex", new Vector2(0.315f, 1f));
					MGV.bg_startScreen.SetActive(true);
				}
			}
			
			if (MGV.showHelpGUI){
				if(GUI.Button(new Rect(10,10,120,25), "X")){
					MGV.showHelpGUI = false;
					//reset the splash screen to its defaults so that it will be normal again if the player restarts a new game
					MGV.bg_startScreen.SetActive(false);
					MGV.bg_startScreen.transform.localPosition = new Vector3(0, 0, 1);
					MGV.bg_startScreen.transform.localScale = new Vector3(18.09f, 10.2f, 0);
					MGV.bg_startScreen.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex", new Vector2(1f, 1f));
				}
				//===================================================
				//HERE WE DISPLAY THE HELP INFO
				GUI.Label (new Rect (260,120,220,500),"Use the arrow keys to rotate the camera view in the game.\n\n" +
				           "Use the mouse wheel to zoom in and out from the player's ship.\n\n" +
				           "Use the left mouse button to select things.\n\n" +
				           "Use the mouse to move the cursor around the world. If the spinning cursor turns blue, then you can select that location to sail to. If the cursor is red, then the player does not have a line of sight " + 
				           "to that location, or the cursor is over land or obstacles in the sea.");
				GUI.Label (new Rect (50,395,440,500), "Your goal, as a new captain, is to sail the dangerous waters of the ancient Mediterranean! Be wary of pirates, storms, and dying at sea! Learn to put together "+
				           "a good crew to reach distant lands and trade along the way. Trading is essential for resupplying your crew with Provisions and water, gaining clout and prestige, repairing "+
				           "one's ship, and replacing crew who were lost asea. Keep an eye on the winds and water currents--a captain should plan their routes accordingly "+
				           "to reach their destinations more easily. Never be afraid to furl sails and ride the currents against the wind! Sticking close to the coastlines will also keep the ship away "+
				           "from the stronger sea currents. If a captain knows the stars, look for them at night to steer by. You'll find north close to Ursa Minor in the ancient era "+
				           "where axial precession has changed the skies! There are no maps and sophisticated navigation here. Learn the landscape and skies to find your way...or perhaps hire a navigator for a hint. "+
				           "Expand your knowledge of the world, and its many places by traveling to new ports and settlements! Knowledge is key and so is experience! Good luck!");
				
			}






		
						
		//##############
		// GAME OVER GUI
		//##############
		if (MGV.isGameOver){
		
			GUI.Box (new Rect(610, 260, 400, 200), "GAME OVER");
			if(GUI.Button(new Rect(710,360,150,20), "Restart?")){
				//Restart from Beginning
				MGV.RestartGame();
				//Remove Game Over Flag
				MGV.isGameOver = false;
			}
		}
		
		
		
		
		
		
		
		
						
		//##################################################################
		// DEBUG GUI
		//###################################################################
		if (MGV.DEBUG_MODE_ON){
			//map zoom in and out button
			if(GUI.Button(new Rect(50,30,100,20), "ZOOM IN")){
				MGV.camera_Mapview.transform.position += -Vector3.up * 20;
			}
			//map zoom in and out button
			if(GUI.Button(new Rect(170,30,100,20), "ZOOM OUT")){
				MGV.camera_Mapview.transform.position += Vector3.up * 20;
			}
					//This saves the current water zone information
			if(GUI.Button(new Rect(1500,100,150,20), "Save Water Zones")){
				MGV.SaveWaterCurrentZones();
			}
			
			//This teleports player ship to given settlement ID
			GUI.Box (new Rect(1450, 123, 200, 30), "");
			MGV.DEBUG_settlementWarpID = GUI.TextField (new Rect (1570,125,60,20), MGV.DEBUG_settlementWarpID,3);
			if(GUI.Button(new Rect(1460,125,80,20), "Warp")){
				MGV.playerShip.transform.position = GetSettlementFromID(int.Parse(MGV.DEBUG_settlementWarpID)).theGameObject.transform.position;
			}
			if(GUI.Button(new Rect(1460,145,80,20), "Save Loc")){
				MGV.Tool_SaveCurrentSettlementPositionsToFile();
			}
				// ######################
				// 	Date And Time GUI
				// ######################
						GUI.Box (new Rect(gui_stat_left,10,200,gui_stat_h), "Date of World");
						
						GUI.Label (new Rect (gui_stat_left+10,30,60,gui_stat_h),  "Year");
						MGV.TD_year = GUI.TextField (new Rect (gui_stat_left+10,40,60,20), MGV.TD_year, 5);
						GUI.Label (new Rect (gui_stat_left+70,30,60,gui_stat_h),  "Month");
						MGV.TD_month = GUI.TextField (new Rect (gui_stat_left+70,40,60,20), MGV.TD_month, 2);
						GUI.Label (new Rect (gui_stat_left+130,30,60,gui_stat_h),  "Day");
						MGV.TD_day = GUI.TextField (new Rect (gui_stat_left+130,40,60,20), MGV.TD_day, 2);
						
						GUI.Label (new Rect (gui_stat_left+10,60,60,gui_stat_h),  "Hour");
						MGV.TD_hour = GUI.TextField (new Rect (gui_stat_left+10,70,60,20), MGV.TD_hour, 5);
						GUI.Label (new Rect (gui_stat_left+70,60,60,gui_stat_h),  "Minute");
						MGV.TD_minute = GUI.TextField (new Rect (gui_stat_left+70,70,60,20), MGV.TD_minute, 2);
						GUI.Label (new Rect (gui_stat_left+130,60,60,gui_stat_h),  "Second");
						MGV.TD_second = GUI.TextField (new Rect (gui_stat_left+130,70,60,20), MGV.TD_second, 2);
						GUI.Label (new Rect (gui_stat_left+30,90,200,20),  "Toggle Celestial Grids");
						MGV.TD_showCelestialGrids = GUI.Toggle(new Rect (gui_stat_left+10,90,100,20), MGV.TD_showCelestialGrids, "");
						//set the celestial grids gameobjects as active or inactive  state based on toggle
						MGV.skybox_eclipticGrid.SetActive(MGV.TD_showCelestialGrids);
						MGV.skybox_celestialGrid.SetActive(MGV.TD_showCelestialGrids);
				
		}
		
		
		
		
		
		///////////////////////////////////////////////////////////////////////////////////////
		//HERE WE SEND A FLAG TO THE PLAYER CONTROLS TO LET IT KNOW WHETHER OR NOT THE CURSOR IS ON A GUI ELEMENT
		//and not to perform normal movement control functions while the cursor is over a GUI element
		// -- right now this only checks if the showsettlement button is being hovered over. I'm using GUI
		//rather than GUILayout so I can't check for lastRectUsed -- I have to specify a Rectangle of the screen
		//--  I need to add the FPS camera screen to this later
		///////////
		if (new Rect(10,10,150,20).Contains(Event.current.mousePosition)){
			MGV.mouseCursorUsingGUI = true;
			//Debug.Log ("Hitting a GUI element");
		}else MGV.mouseCursorUsingGUI = false;
		
		
		
		
		//************************************************************
		// This code controls the screen scaling of the GUI interface!
		//		Here is Resets the Matrix at the end of OnGUI()
		//************************************************************
		GUI.matrix = Matrix4x4.identity;
		}	
	
	}
	
	string GetDirectionsToSettlement(Vector3 target){
		string directions = "";
		//float angle = Vector3.Angle(Vector3.right,target -MGV.currentSettlementGameObject.transform.position);
		float angle =  Mathf.Atan2(target.z-MGV.currentSettlementGameObject.transform.position.z, target.x-MGV.currentSettlementGameObject.transform.position.x) * Mathf.Rad2Deg;
		if (angle < 0) angle += 360;
		
		//Debug.Log (angle);
		if(angle >=0 && angle < 20)
			directions += "East:  ";
		else if(angle >= 20 && angle < 70)
			directions+= "Northeast:   ";
		else if(angle >= 70 && angle < 110)		
			directions+= "North:   ";		
		else if(angle >= 110 && angle < 160)		
			directions+= "Northwest:   ";
		else if(angle >= 160 && angle < 200)		
			directions+= "West:   ";
		else if(angle >= 200 && angle < 250)		
			directions+= "Southwest:   ";
		else if(angle >= 250 && angle < 290)		
			directions+= "South:   ";
		else if(angle >= 290 && angle < 340)		
			directions+= "Southeast:   ";	
		else if(angle >= 340 && angle < 360)		
			directions+= "East:   ";		
			
		float distance = (Vector3.Distance(MGV.currentSettlementGameObject.transform.position, target) * MGV.unityWorldUnitResolution) / 1000;
		directions += distance + "km";	
		return directions;
	}
	
	string GetInfoOnNetworkedSettlementResource(Resource resource){
		if (resource.amount_kg < 100)
			return "I hear they are running inredibly low on " + resource.name;
		else if (resource.amount_kg < 300)
			return "Someone mentioned that they have modest stores of " +resource.name;
		else 
			return "A sailor just came from there and said he just unloaded an enormous quantity of " + resource.name;
		
	}
	
	int GetPriceOfResource(float amount){
		//Price = 1000 * 1.2^(-.1*x)
		int price = (int) Mathf.Ceil( 1000 * Mathf.Pow(1.2f,(-.1f*amount)) );
		if(price < 1) price = 1;
		return price;
	
	}
	
	bool CheckIfPlayerCanAffordToPayPortTaxes(){
		if (MGV.playerShipVariables.ship.currency >= MGV.currentPortTax) return true; else return false;
	}
	
	int GetTaxRateOnCurrentShipManifest(){
		//We need to get the total price of all cargo on the ship
		float totalPriceOfGoods = 0f;
		
		//Loop through each resource in the settlement's cargo and figure out the price of that resource 
		for (int setIndex = 2; setIndex < MGV.currentSettlement.cargo.Length; setIndex++){
			float currentResourcePrice = GetPriceOfResource(MGV.currentSettlement.cargo[setIndex].amount_kg);
			//with this price, let's check the ships cargo at the same index position and calculate its worth and add it to the total
			totalPriceOfGoods += (currentResourcePrice * MGV.playerShipVariables.ship.cargo[setIndex].amount_kg);
			//Debug.Log (MGV.currentSettlement.cargo[setIndex].name + totalPriceOfGoods);
				
			
		}
		
		float taxRateToApply = 0f;
		//Now we need to figure out the tax on the total price of the cargo--which is based on the settlements in/out of network tax
		// total price / 100 * tax rate = amount player owes to settlement for docking
		if(MGV.isInNetwork)
			taxRateToApply = MGV.currentSettlement.tax_network;
		else
			taxRateToApply = MGV.currentSettlement.tax_neutral;
			
		//clout affects tax rate as well: 50 clout = neutral. 100 clout equals %50 reduction in tax rate. 0 clout = 50% increase in tax
		//	--formula is :  (currentTaxRate/100) * ((clout - 50) / 100)   --   e.g. 0 clout - 50 = -50 --> -50 / 100 = -.5 --> 50% of the current tax rate should be subtracted.
		taxRateToApply += (taxRateToApply/100) * ((MGV.playerShipVariables.ship.playerClout - 50f) / 100);
		
		return (int) ((totalPriceOfGoods / 100) * taxRateToApply);
	}
	
	
	int GetCostToHireCrewMember(){
		//The cost of a crew member is determined by clout and whether or not you're network
		//the base cost is set to 5 drachma to start, and 5 drachma for each day after
		float baseCost = 5;
		float cost = baseCost;
		//apply / subtract the clout modifier
		cost -= (baseCost/100) * ((MGV.playerShipVariables.ship.playerClout - 50f) / 200);//divide by 200 instead of 100 b/c at most only 25% of the cost can be reduced through clout
		//apply /subtract the network modifier
		if(MGV.isInNetwork)
			cost -= (baseCost/100) * (MGV.GetRange(MGV.currentSettlement.population, 0, 10000f, 0, .50f));//at most only %50 of the base cost can be subtracted
		else //if out of network
			cost += (baseCost/100) * .25f;
			
		return Mathf.CeilToInt(cost);	
	}
	
	
	bool ShowSettlementHintIfInfluenceHighEnough(){
	//This checks to see if the current settlement's influence is high enough to warrant showing information on a city within the network
		//specifically--each settlement has an influence probability of 0-100--we match that up on a random num and if it wins then proceed
		return true;
	}
	
	bool CheckSettlementResourceAvailability(int amountToCheck, int cargoIndex){
		//This function checks 3 thing(s)
		//	1 Does the city have the resource for the player to buy?
		//	2 Does the player have the currency to buy the resource?
		//	3 Does the player have the cargo hold space to buy the resource?
		float resourceAmount = MGV.currentSettlement.cargo[cargoIndex].amount_kg;
		float price = GetPriceOfResource(resourceAmount);

		if( resourceAmount >= amountToCheck 
			&& (price * amountToCheck) < MGV.playerShipVariables.ship.currency 
			&& (MGV.playerShipVariables.ship.cargo_capicity_kg - MGV.playerShipVariables.ship.GetTotalCargoAmount()) >= amountToCheck )
			return true;
		else
			return false;
	}
	
	bool CheckShipResourceAvailability(int amountToCheck, int cargoIndex){
		//This function checks 1 thing(s):
		//	1 Does the player have cargo to sell?
		if (MGV.playerShipVariables.ship.cargo[cargoIndex].amount_kg >= amountToCheck)
			return true;
		else 
			return false;
		
	}
	
	void ChangeSettlementCargo(int cargoIndex, float changeAmount){
		MGV.currentSettlement.cargo[cargoIndex].amount_kg += changeAmount;
	}
	
	void ChangeShipCargo(int cargoIndex, float changeAmount){
		float price = GetPriceOfResource(MGV.currentSettlement.cargo[cargoIndex].amount_kg);
		MGV.playerShipVariables.ship.cargo[cargoIndex].amount_kg += changeAmount;
		//we use a (-) change amount here because the changeAmount reflects the direction of the goods
		//e.g. if the player is selling--they are negative in cargo---but their currency is positive and vice versa.
		MGV.playerShipVariables.ship.currency += (int)(price * -changeAmount); 
	}
	
	void ShowSettlementResources(){
		float fromTop = 175;
		for(int i = 0; i < 15; i++){
			string name = MGV.currentSettlement.cargo[i].name;
			if (name.Length > 6) name = name.Substring(0,6);
			GUI.Label (new Rect (gui_city_left+10,fromTop,gui_row_width, gui_row_height), 
			name + ":\t      " + 
			(int)MGV.currentSettlement.cargo[i].amount_kg + "kg @ " + 
			GetPriceOfResource(MGV.currentSettlement.cargo[i].amount_kg) + "d/kg");
			fromTop += 25;
		}
	
	}
	
	Settlement GetSettlementFromID(int ID){
		foreach (Settlement city in MGV.settlement_masterList){
			if (city.settlementID == ID){
				return city;
			}
		
		}
		
		//if no matches(this shouldn't be possible--return a fake settlement rather than a null
		//	--this is more sophisticated than a null--it won't crash but the error is obvious.
		return new Settlement(-1, "ERROR: DIDNT FIND ID MATCH IN GetSettlementFromID Function", Vector2.zero, -1, -1);
	}
	
	
	void ShowShipResources(){
		float fromTop = 175;
		for(int i = 0; i < 15; i++){
			string name = MGV.currentSettlement.cargo[i].name;
			if (name.Length > 6) name = name.Substring(0,6);
			GUI.Label (new Rect (gui_ship_left +10,fromTop,gui_row_width, gui_row_height), 
			name + " :\t      " + 
			(int)MGV.playerShipVariables.ship.cargo[i].amount_kg + "kg @ " + 
			GetPriceOfResource(MGV.currentSettlement.cargo[i].amount_kg) + "d/kg");
			fromTop += 25;
		}
	}
	
	void ShowBuyButtons(){ 
		float buttonWidth = 75;
		float buttonHeight = 25;
		
		float fromTop = 175;
		float fromLeftA = 275;
		float fromLeftB = 350;
		
		for(int i = 0; i < 15; i++){
			if (CheckSettlementResourceAvailability(1,i)) if(GUI.Button(new Rect(fromLeftA,fromTop,buttonWidth,buttonHeight), "Buy 1kg")){ChangeShipCargo(i, 1f); ChangeSettlementCargo(i, -1f); }
			if (CheckSettlementResourceAvailability(10,i)) if(GUI.Button(new Rect(fromLeftB,fromTop,buttonWidth,buttonHeight), "Buy 10kg")){ChangeShipCargo(i, 10f); ChangeSettlementCargo(i, -10f); }
			fromTop += 25;
			//Figure out the tax on the current cargo hold
			if (MGV.isInNetwork) MGV.currentPortTax = 0;
			else MGV.currentPortTax = GetTaxRateOnCurrentShipManifest();
		}
	}
	
	void ShowSellButtons(){ 
		float buttonWidth = 75;
		float buttonHeight = 25;
		
		float fromTop = 175;
		float fromLeftA = 715;
		float fromLeftB = 790;
		
		for(int i = 0; i < 15; i++){
			if (CheckShipResourceAvailability(1,i)) if(GUI.Button(new Rect(fromLeftA,fromTop,buttonWidth,buttonHeight), "Sell 1kg")){ChangeSettlementCargo(i, 1f); ChangeShipCargo(i, -1f); }
			if (CheckShipResourceAvailability(10,i)) if(GUI.Button(new Rect(fromLeftB,fromTop,buttonWidth,buttonHeight), "Sell 10kg")){ChangeSettlementCargo(i, 10f); ChangeShipCargo(i, -10f); }
			fromTop += 25;
			//Figure out the tax on the current cargo hold
			if (MGV.isInNetwork) MGV.currentPortTax = 0;
			else MGV.currentPortTax = GetTaxRateOnCurrentShipManifest();
		}
	}
	
	
	
	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	string GetCurrentMonth(){
		int numOfDaysTraveled = (int) MGV.playerShipVariables.ship.totalNumOfDaysTraveled;
		string monthName = "";
		if(numOfDaysTraveled <= 30){
			monthName = "January";
		} else if (numOfDaysTraveled <=60) {
			monthName = "February";
		} else if (numOfDaysTraveled <=90) {
			monthName = "March";
		} else if (numOfDaysTraveled <=120) {
			monthName = "April";
		} else if (numOfDaysTraveled <=150) {
			monthName = "May";
		} else if (numOfDaysTraveled <=180) {
			monthName = "June";
		} else if (numOfDaysTraveled <=210) {
			monthName = "July";
		} else if (numOfDaysTraveled <=240) {
			monthName = "August";
		} else if (numOfDaysTraveled <=270) {
			monthName = "September";
		} else if (numOfDaysTraveled <=300) {
			monthName = "October";
		} else if (numOfDaysTraveled <=330) {
			monthName = "November";
		} else if (numOfDaysTraveled <=360) {
			monthName = "December";
		}
		
		return monthName;
			
	
	}
	
	void ShowNotification(int windowID){
	// Normal notifications require user action to destroy, through clicking on a button.
	//--This first detects if a primary message is displaying--if it is it shows it and looks for a secondary after
	//--If there is no secondary, it does nothing after the primary
		if (MGV.showNotification){
			GUI.Box (new Rect(610, 100, 400, (MGV.notificationMessage.Length*0.3f) + 70), "");
			GUI.Label (new Rect(630, 110, 360, (MGV.notificationMessage.Length*0.6f)+ 15), MGV.notificationMessage);
			if(GUI.Button(new Rect(740,125 +(MGV.notificationMessage.Length*0.3f) ,150,20), "Okay!")){
				MGV.showNotification = false;
			}
		} else if (MGV.showSecondaryNotification){
			GUI.Box (new Rect(610, 100, 400, (MGV.notificationMessage.Length*0.3f) + 70), "");
			GUI.Label (new Rect(630, 110, 360, (MGV.notificationMessage.Length*0.6f)+ 15), MGV.secondaryNotificationMessage);
			if(GUI.Button(new Rect(740,125 +(MGV.notificationMessage.Length*0.3f) ,150,20), "Okay!")){
				MGV.showSecondaryNotification = false;
			}
		}
		
	}
	
	void ShowSoftNotification(int windowID){
	//TODO DEPRECATED
	// Soft Notification Windows Auto destroy within a few seconds
	//	GUI.Box (new Rect(910, 460, 400, 200), "");
	//	GUI.Label (new Rect(930, 480, 360, 160), MGV.softNotificationMessage);

	}
	
	//#####################################################################
	//	START OF TAVERN MENU PANEL BUILDER FUNCTIONS
	//#####################################################################
		//=========================
		//TAVERN PANEL
		void BuildTavernPanel(){
			float gui_network_left = 1050;
			float gui_network_top = 20;
			float gui_network_width = 580;
			float gui_network_height = 680;
			int newLineCount = 1;
		
			float scrollViewOrganicHeight;
			if(MGV.currentNetworkSettlements.Count * 25 < 200) scrollViewOrganicHeight = 200f; else scrollViewOrganicHeight = MGV.currentNetworkSettlements.Count * 25;
			
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,(200)), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(scrollViewOrganicHeight)),false,true);
			
				for(int i = 0; i < MGV.currentNetworkSettlements.Count; i++){
					//First draw the Settlement name with the directions
					GUI.Label (new Rect (gui_network_left,gui_network_top + 280 + (25*newLineCount),gui_network_width, 20), MGV.currentNetworkSettlements[i].name + "     - Directions: " + GetDirectionsToSettlement(MGV.currentNetworkSettlements[i].theGameObject.transform.position));
					newLineCount++;
					//Second, List the Resources available for hints
					foreach(int resource in MGV.currentNetworkSettlements[i].networkHintResources){
						GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*newLineCount),gui_network_width, 20), GetInfoOnNetworkedSettlementResource(MGV.currentNetworkSettlements[i].cargo[resource]));
						newLineCount++;
					}
				}
			GUI.EndScrollView();
		}
	
	//========================================================================================================
	// TAKE A LOAN PANEL
		void SetupLoanPanelVariables(){
			//Setup the initial term to repay the loan
			numOfDaysToPayOffLoan = 10;
			//Determine the base loan amount off the city's population
			baseLoanAmount = 500 * (MGV.currentSettlement.population / 1000);
			//If base loan amount is less than 200 then make it 200 as the smallest amount available
			if (baseLoanAmount < 200f) baseLoanAmount = 200f;
			//Determine the actual loan amount off the player's clout
			loanAmount = (int) (baseLoanAmount + (baseLoanAmount * MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID)));
			//Determmine the base interest rate of the loan off the city's population
			baseInterestRate = 10 + (MGV.currentSettlement.population / 1000);
			//Determine finalized interest rate after determining player's clout
			finalInterestRate = baseInterestRate - (baseInterestRate * MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID));
			//Determine the precompiled interest if returned on time
			totalAmountDueAtTerm = loanAmount + (loanAmount * (finalInterestRate/100));
		}
	
		void BuildLoanPanel(){
		float gui_network_left = 1050;
			float gui_network_top = 20;
			float gui_network_width = 580;
			float gui_network_height = 680;
			
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,200), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(200)),false,true);
		
			//If the player doesn't already have a loan taken out
			if(MGV.playerShipVariables.ship.currentLoan == null){		
			
					GUI.Label (new Rect (gui_network_left,gui_network_top + 280 + (25),gui_network_width, 20), "Take Out a Loan of " + loanAmount +  " drachma?");
					GUI.Label (new Rect (gui_network_left,gui_network_top + 280 + (50),gui_network_width, 20), "Your Interest Rate is " + finalInterestRate +  " %");
					GUI.Label (new Rect (gui_network_left,gui_network_top + 280 + (75),gui_network_width, 20), "You will owe " + (int) totalAmountDueAtTerm +  " drachma in " + numOfDaysToPayOffLoan + " days at this same port.");
					
					if (GUI.Button (new Rect (gui_network_left + 250,gui_network_top + 280+ (50),80, 20), "YES" ) ){
						MGV.playerShipVariables.ship.currentLoan = new Loan (loanAmount, finalInterestRate, numOfDaysToPayOffLoan, MGV.currentSettlement.settlementID);
						MGV.playerShipVariables.ship.currency += loanAmount;
						MGV.showNotification = true;
						MGV.notificationMessage = "You took out a loan of " + loanAmount +  " drachma! Remember to pay it back in due time!";
					}
				
			//If the player does have a loan already, then we need to make them aware of that
			} else {
			
				//If the player is back at the origin settlement of the loan, then the player should be allowed to pay the loan back
				if (MGV.CheckIfShipBackAtLoanOriginPort()){
					int amountDue = MGV.playerShipVariables.ship.currentLoan.GetTotalAmountDueWithInterest();
					GUI.Label (new Rect (gui_network_left,gui_network_top + 280 + (25),gui_network_width, 20), "Welcome back! You currently owe " + amountDue +  " drachma! Pay it back now?");
					if (GUI.Button (new Rect (gui_network_left + 250,gui_network_top + 280+ (50),80, 20), "YES" ) ){
						//Pay the loan back if the player has the currency to do it
						if (MGV.playerShipVariables.ship.currency > amountDue){
							MGV.playerShipVariables.ship.currency -= amountDue;
							MGV.playerShipVariables.ship.currentLoan = null;
							MGV.showNotification = true;
							MGV.notificationMessage = "You paid back your loan and earned a little respect!";
							//give a boost to the players clout for paying back loan
							MGV.AdjustPlayerClout(3);
						//Otherwise let player know they can't afford to pay the loan back
						} else {
							MGV.showNotification = true;
							MGV.notificationMessage = "You currently can't afford to pay your loan back! Better make some more money!";
						}
					}
				//If at a different settlement, the player needs to be made aware that they can only take a loan out from a single settlement at a time.
				} else {
					GUI.Label (new Rect (gui_network_left,gui_network_top + 280 + (25),gui_network_width, 20),"You already have a loan with " + GetSettlementFromID(MGV.playerShipVariables.ship.currentLoan.settlementOfOrigin).name +  ". You can only have one loan at a time.");
				}
			}
			GUI.EndScrollView();
		}
		
	//================================================================================================================
	// BUILD A SHRINE PANEL
		void SetupBuildAShrinePanelVariables(){
			//base the amount off of clout and network 
			costToBuild = 200; 
			//We need to do a clout check as well as a network checks
			int baseModifier = Mathf.CeilToInt(1000 - (200 * MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID)));
			if(MGV.CheckIfCityIDIsPartOfNetwork(MGV.currentSettlement.settlementID)){
				costToBuild = Mathf.CeilToInt(MGV.currentSettlement.tax_network * baseModifier * 1);
			} else {
				costToBuild = Mathf.CeilToInt(MGV.currentSettlement.tax_neutral * baseModifier * 1);
			}
		}
		
		void BuildAShrinePanel(){
			float gui_network_left = 1050;
			float gui_network_top = 20;
			float gui_network_width = 580;
			float gui_network_height = 680;
			
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,(200)), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(200)),false,true);
			GUI.Label (new Rect (gui_network_left,gui_network_top + 280 + (25),gui_network_width, 20), "Build a Temple in " + MGV.currentSettlement.name + " for " + Mathf.CeilToInt(costToBuild * 1.5f) +  " drachma?");
			if (GUI.Button (new Rect (gui_network_left + 320,gui_network_top + 280+ (25),80, 20), "YES" ) ){
				MGV.playerShipVariables.ship.currency -= Mathf.CeilToInt(costToBuild * 1.5f);
				MGV.showNotification = true;
				MGV.notificationMessage = "You built a temple for " + MGV.currentSettlement.name +  "! You've earned a tremendous amount of clout!";
				MGV.AdjustPlayerClout(15);
			}
			GUI.Label (new Rect (gui_network_left,gui_network_top + 330 + (25),gui_network_width, 20), "Build a shrine in " + MGV.currentSettlement.name + " for " + costToBuild +  " drachma?");
			if (GUI.Button (new Rect (gui_network_left + 320,gui_network_top + 330+ (25),80, 20), "YES" ) ){
				MGV.playerShipVariables.ship.currency -= costToBuild;
				MGV.showNotification = true;
				MGV.notificationMessage = "You built a shrine for " + MGV.currentSettlement.name +  "! You've earned quite a bit of clout!";
				MGV.AdjustPlayerClout(8);
			}
			GUI.Label (new Rect (gui_network_left,gui_network_top + 380 + (25),gui_network_width, 20), "Build a statue in " + MGV.currentSettlement.name + " for " + Mathf.CeilToInt(costToBuild / 2f)+  " drachma?");
			if (GUI.Button (new Rect (gui_network_left + 320,gui_network_top + 380+ (25),80, 20), "YES" ) ){
				MGV.playerShipVariables.ship.currency -= Mathf.CeilToInt(costToBuild / 2f);
				MGV.showNotification = true;
				MGV.notificationMessage = "You built a statue for " + MGV.currentSettlement.name +  "! You've earned a little clout!";
				MGV.AdjustPlayerClout(4);
			}
		GUI.EndScrollView();
		}
		
	//================================================================================================================
	// CITY QUESTION PANEL
		void SetupCityQuestionPanelVariables(){

			//First clear the settlement list
			relevantSettlements.Clear ();
			foreach(int settlementID in MGV.playerShipVariables.ship.playerJournal.knownSettlements){
				Settlement settlement = GetSettlementFromID(settlementID);
				//We only want to offer questions for the city if it's an actual port for trading
				if (settlement.typeOfSettlement == 1)
					//make sure not to list the current settlement the player is at
					if(MGV.currentSettlement.settlementID != settlementID)
						relevantSettlements.Add(settlement);
			}
			//Now figure out the costs for questions for the relevant settlements
			//First clear the costs list
			costForHints.Clear ();
			for (int i = 0; i < relevantSettlements.Count; i++){
				//TODO temporary fix--need an overarching clout-calculator formula for the cost of things
				costForHints.Add (Mathf.RoundToInt(MGV.GetDistanceBetweenTwoLatLongCoordinates(MGV.currentSettlement.location_longXlatY, relevantSettlements[i].location_longXlatY) / 10000f) );
			}
		
	}
	
	void BuildCityQuestionPanel(){
			float gui_network_left = 1050;
			float gui_network_top = 20;
			float gui_network_width = 580;
			float gui_network_height = 680;
			int newLineCount = 1;
			
			float scrollViewOrganicHeight;
			if(relevantSettlements.Count * 25 < 200) scrollViewOrganicHeight = 200f; else scrollViewOrganicHeight = relevantSettlements.Count * 25;
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,(200)), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(scrollViewOrganicHeight)),false,true);
			
			for (int i = 0; i < relevantSettlements.Count; i++){
						GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*(newLineCount+1)),gui_network_width, 20), "Ask about " + relevantSettlements[i].name + " for " + costForHints[i] + " drachma ?");
						if (GUI.Button (new Rect (gui_network_left + 320,gui_network_top + 280+ (25*(newLineCount+1)),80, 20), "YES" ) ){
							if (MGV.playerShipVariables.ship.currency < costForHints[i]){
								MGV.showNotification = true;
								MGV.notificationMessage = "Not enough money to buy this information!";
							} else {
								MGV.playerShipVariables.ship.currency -= costForHints[i];
								MGV.showNotification = true;
								MGV.notificationMessage = GetInfoOnNetworkedSettlementResource(relevantSettlements[i].cargo[Random.Range (0,relevantSettlements[i].cargo.Length)]);
							}
						}
						newLineCount++;
					
				}
			GUI.EndScrollView();
		}
		
		
	//================================================================================================================
	// HIRE CREW PANEL
		void SetupHireCrewMemberPanelVariables(){
			//First clear the costs list
			hireCrewCosts.Clear ();
			for (int i = 0; i < MGV.currentlyAvailableCrewMembersAtPort.Count; i++){
			//TODO Temporary solution--need to add a clout check modifier
				hireCrewCosts.Add (MGV.currentlyAvailableCrewMembersAtPort[i].clout *2);
			}
		}
		
		void BuildHireCrewMemberPanel(){
			float gui_network_left = 1050;
			float gui_network_top = 40;
			float gui_network_width = 580;
			float gui_network_height = 680;
			int newLineCount = 1;
			
			float scrollViewOrganicHeight;
			if(MGV.currentlyAvailableCrewMembersAtPort.Count * 50 < 200) scrollViewOrganicHeight = 200f; else scrollViewOrganicHeight = MGV.currentlyAvailableCrewMembersAtPort.Count * 170;
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,200), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(scrollViewOrganicHeight)),false,true);
			
			//Create a list of 5 crew members to hire
			for (int i = 0; i < MGV.currentlyAvailableCrewMembersAtPort.Count; i++){
				CrewMember currentMember = MGV.currentlyAvailableCrewMembersAtPort[i];
				string title = MGV.GetCloutTitleEquivalency(currentMember.clout) + " " + currentMember.name + ", the " + MGV.GetJobClassEquivalency(currentMember.typeOfCrew) + " from " +GetSettlementFromID(currentMember.originCity).name + ".";
				int costToHire = hireCrewCosts[i]; 
				GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*newLineCount),gui_network_width, 20), title);
				GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*(newLineCount+1)),gui_network_width - 40, 80), currentMember.backgroundInfo);
				GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*(newLineCount+4)),gui_network_width, 20), "Hire this "+ currentMember.name +" for: " + costToHire + " ?");
				if (GUI.Button (new Rect (gui_network_left + 215,gui_network_top + 280+ (25*(newLineCount+4)),80, 20), "YES" ) ){
					//Check to see if player has enough money to hire
					if(MGV.playerShipVariables.ship.currency >= costToHire){
						//Now check to see if there is room to hire a new crew member!
						if (MGV.playerShipVariables.ship.crewRoster.Count < MGV.playerShipVariables.ship.crewCapacity){
							MGV.playerShipVariables.ship.crewRoster.Add(currentMember);
							MGV.currentlyAvailableCrewMembersAtPort.Remove(currentMember);
							//remove matching crew cost from array
							hireCrewCosts.RemoveAt(i);
							//Subtract the cost from the ship's money
							MGV.playerShipVariables.ship.currency -= costToHire; 
						//If there isn't room, then let the player know
						} else {
							MGV.showNotification = true;
							MGV.notificationMessage = "You don't have room on the ship to hire " + currentMember.name + ".";						
						}
					//If not enough money, then let the player know
					} else {
						MGV.showNotification = true;
						MGV.notificationMessage = "You can't afford to hire " + currentMember.name + ".";
					}
				}
				newLineCount = newLineCount + 6;
			}
			GUI.EndScrollView();
		}
		
	//================================================================================================================
	// FIRE CREW PANEL
		void SetupFireCrewMemberPanelVariables(){
		//nothing variables wise needed at the moment but added for ease of expansion later
		}
		void BuildFireCrewMemberPanel(){
			float gui_network_left = 1050;
			float gui_network_top = 40;
			float gui_network_width = 580;
			float gui_network_height = 680;
			int newLineCount = 1;
			
			float scrollViewOrganicHeight;
			if(MGV.playerShipVariables.ship.crewRoster.Count * 50 < 200) scrollViewOrganicHeight = 200f; else scrollViewOrganicHeight = MGV.playerShipVariables.ship.crewRoster.Count * 170;
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,(200)), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(scrollViewOrganicHeight)),false,true);
			
			//Create a list of 5 crew members to hire
			foreach (CrewMember currentMember in MGV.playerShipVariables.ship.crewRoster){
				//Make sure they are removable crew members and not quest related
				if(currentMember.isKillable){
					string title = MGV.GetCloutTitleEquivalency(currentMember.clout) + " " + currentMember.name + ", the " + MGV.GetJobClassEquivalency(currentMember.typeOfCrew) + " from " +GetSettlementFromID(currentMember.originCity).name + ".";
					GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*newLineCount),gui_network_width, 20), title);
					GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*(newLineCount+1)),gui_network_width - 40, 80), currentMember.backgroundInfo);
					GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*(newLineCount+4)),gui_network_width, 20), "Fire " + currentMember.name + " ?");
					if (GUI.Button (new Rect (gui_network_left + 215,gui_network_top + 280+ (25*(newLineCount+4)),80, 20), "YES" ) ){
						MGV.playerShipVariables.ship.crewRoster.Remove (currentMember);
						MGV.showNotification = true;
						MGV.notificationMessage = currentMember.name + " looked at you sadly and said before leaving, 'I thought I was doing so well. I'm sorry I let you down. Guess I'll go drink some cheap wine...";						
							
					}
					newLineCount = newLineCount + 6;
				}
			}
			GUI.EndScrollView();
			
		}
	//=================================================================================================================
	// HIRE NAVIGATOR PANEL

		void SetupNavigatorPanelVariables(){
		//First clear the settlement list
		relevantSettlements.Clear ();
		foreach(int settlementID in MGV.playerShipVariables.ship.playerJournal.knownSettlements){
			//make sure not to list the current settlement the player is at
			if(MGV.currentSettlement.settlementID != settlementID)
				relevantSettlements.Add(GetSettlementFromID(settlementID));
		}
		//Clear the nav menu costs
			navPanelCosts.Clear();
			//right now cost is set to ~1 currency per .1km and the total is modified by subtracting the total clout influence percentage
			for (int i = 0; i < relevantSettlements.Count; i++){
				float costToHire = MGV.GetDistanceBetweenTwoLatLongCoordinates(MGV.currentSettlement.location_longXlatY, relevantSettlements[i].location_longXlatY) / 1000f;
				costToHire = costToHire - (costToHire * MGV.GetOverallCloutModifier(relevantSettlements[i].settlementID));
				navPanelCosts.Add (Mathf.RoundToInt(costToHire));
			}
		
		}
		void BuildHireNavigatorPanel(){
			float gui_network_left = 1050;
			float gui_network_top = 40;
			float gui_network_width = 580;
			float gui_network_height = 680;
			int newLineNavCount = 1;
			int lineSpacing = 5;
			
			float scrollViewOrganicHeight;
			if(relevantSettlements.Count * 25 < 200) scrollViewOrganicHeight = 200f; else scrollViewOrganicHeight = relevantSettlements.Count * 25;
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,(200)), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(scrollViewOrganicHeight)),false,true);
			//In this scroll view we need to list out all the available settlements that are available in a players journal.
			//It should be a list of City names with a button to the right with the cost to hire a navigator which is determined by the distance from current port.
			for (int i = 0; i < relevantSettlements.Count; i++){
				float costToHire = navPanelCosts[i];
				GUI.Label (new Rect (gui_network_left + 5,gui_network_top + 280+ (25*newLineNavCount),gui_network_width, 20), relevantSettlements[i].name);
				if (GUI.Button (new Rect (gui_network_left + 205,gui_network_top + 280+ (25*newLineNavCount),80, 20), "" + (int) costToHire ) ){
					//Do this if button pressed
					//Check to see if player has enough money to hire
					if(MGV.playerShipVariables.ship.currency >= costToHire){
						//subtract the cost from the players currency
						MGV.playerShipVariables.ship.currency -= (int) costToHire;
						//change location of beacon
						Vector3 location = Vector3.zero;
						for( int x = 0; x < MGV.settlement_masterList_parent.transform.childCount; x++) 
							if (MGV.settlement_masterList_parent.transform.GetChild(x).GetComponent<script_settlement_functions>().thisSettlement.settlementID == relevantSettlements[i].settlementID)
								location = MGV.settlement_masterList_parent.transform.GetChild(x).position;
						MGV.navigatorBeacon.transform.position = location;
						MGV.navigatorBeacon.GetComponent<LineRenderer>().SetPosition(0, new Vector3(location.x, 0, location.z));
						MGV.navigatorBeacon.GetComponent<LineRenderer>().SetPosition(1, location + new Vector3(0,400,0));
						MGV.playerShipVariables.UpdateNavigatorBeaconAppearenceBasedOnDistance();
						MGV.playerShipVariables.ship.currentNavigatorTarget = relevantSettlements[i].settlementID;
						MGV.ShowANotificationMessage("You hired a navigator to " + relevantSettlements[i].name + " for " + costToHire + " drachma.");
					//If not enough money, then let the player know
					} else {
					MGV.showNotification = true;
					MGV.notificationMessage = "You can't afford to hire a navigator to " + relevantSettlements[i].name + ".";
					}
				}
				newLineNavCount++;
			}
			GUI.EndScrollView();
		}
		
	//=================================================================================================================
	// REPAIR SHIP PANEL		
		void SetupRepairShipPanelVariables(){
		
		//We need to do a clout check as well as a network checks
		int baseModifier = Mathf.CeilToInt(2 - MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID));
		if(MGV.CheckIfCityIDIsPartOfNetwork(MGV.currentSettlement.settlementID)){
			costToRepair = Mathf.CeilToInt(MGV.currentSettlement.tax_network * baseModifier * 1);
		} else {
			costToRepair = Mathf.CeilToInt(MGV.currentSettlement.tax_neutral * baseModifier * 1);
		}
		
		}
		void BuildRepairShipPanel(){
			float gui_network_left = 1050;
			float gui_network_top = 40;
			float gui_network_width = 580;
			float gui_network_height = 680;
		
			scrollPosition = GUI.BeginScrollView(new Rect(gui_network_left,gui_network_top + 300,gui_network_width,(200)), scrollPosition, new Rect(gui_network_left,gui_network_top+300,gui_network_width-20,(200)),false,true);
			GUI.Label (new Rect (gui_network_left,gui_network_top + 280,gui_network_width, 20), "Your ship's current status: " + MGV.playerShipVariables.ship.health  + " / 100 hp" );
			GUI.Label (new Rect (gui_network_left,gui_network_top + 300,gui_network_width, 20), "Cost: " + costToRepair + "drachma");
			if(GUI.Button(new Rect(gui_network_left+150,gui_network_top + 300,100,25), "Repair 1HP ?")){
						MGV.playerShipVariables.ship.health += 1f;
						//make sure the hp can't go above 100
						if (MGV.playerShipVariables.ship.health > 100){
							MGV.playerShipVariables.ship.health = 100;
							MGV.showNotification = true;
							MGV.notificationMessage = "Your ship is already fully repaired";						
						} else {
							MGV.playerShipVariables.ship.currency -= costToRepair;
						}
					}
					
			GUI.Label (new Rect (gui_network_left,gui_network_top + 330,gui_row_width, 20), "Cost: " + Mathf.CeilToInt(100-MGV.playerShipVariables.ship.health) * costToRepair + "drachma");
			if(GUI.Button(new Rect(gui_network_left+150,gui_network_top + 330,100,25), "Repair All")){
				if(MGV.playerShipVariables.ship.health >= 100){
					MGV.showNotification = true;
					MGV.notificationMessage = "Your ship is already fully repaired";						
				} else {
				MGV.playerShipVariables.ship.currency -= (int) (costToRepair * Mathf.CeilToInt(100-MGV.playerShipVariables.ship.health));
					MGV.playerShipVariables.ship.health = 100f;
				}
			}
			GUI.EndScrollView();
		}
	//#####################################################################
	//	END OF TAVERN MENU PANEL BUILDER FUNCTIONS
	//#####################################################################

}
