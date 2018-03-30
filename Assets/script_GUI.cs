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

public class script_GUI : MonoBehaviour {

//======================================================================================================================================================================
//======================================================================================================================================================================
//  SETUP ALL VARIABLES FOR THE GUI
//======================================================================================================================================================================
//======================================================================================================================================================================


	//-----------------------------------------------------------
	// Title Screen Variables
	//-----------------------------------------------------------
		public GameObject title_start;
		public GameObject title_newgame_button;
		public GameObject title_newgame_beginner_button;
		public GameObject title_loadgame_button;
		public GameObject title_loadgame_beginner_button;
		public GameObject title_crew_select;
		public GameObject title_crew_select_story;
		public GameObject title_crew_select_start_game;

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
				
	//-----------------------------------------------------------
	// Player Notification Variables
	//-----------------------------------------------------------
		public GameObject notice_notificationParent;
		public GameObject notiec_notificationTemplate;
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
		public GameObject hud_button_dropAnchor;
		public GameObject hud_button_rest;
		public GameObject hud_button_saveGame;
		public GameObject hud_button_restartGame;
		
		public GameObject hud_captainsLog;
		
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
	//REPAIR SHIP PANEL VARIABLES
	int costToRepair;

    
    //===================================
    // OTHER VARS
    globalVariables MGV;
    public GameObject GUI_port_menu;
    public GameObject all_trade_rows;
    public GameObject player_currency;
    public GameObject player_current_cargo;
    public GameObject player_max_cargo;
    
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
    
    Vector2 scrollPosition = new Vector2(0,0);
    Vector2 captainsLogScrollPosition = Vector2.zero;
    Vector2 hireNavigatorScrollPosition = Vector2.zero;    
    
    
    
//======================================================================================================================================================================
//  INITIALIZE ANY NECESSARY VARIABLES
//======================================================================================================================================================================
	void Start () {
        MGV = GameObject.FindGameObjectWithTag("global_variables").GetComponent<globalVariables>();
        style_background.fontSize = (int) yUnit * 3;
        style_background.normal.textColor = Color.white;
        style_button.fontSize = (int) yUnit * 3;
        style_button.normal.textColor = Color.white;
        style_label.fontSize = (int) yUnit * 3;
        style_label.normal.textColor = Color.white;
        xUnit = Screen.width / 100f;
        yUnit = Screen.height / 100f;
	}

    
    
    
    
    
    
    
//======================================================================================================================================================================
//======================================================================================================================================================================
//  START OF THE MAIN UPDATE LOOP
//======================================================================================================================================================================
//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
	
void OnGUI(){



	//This is NEW--This is a quick/easy way to make sure the necessary labels are constantly updated without too much
	//	--overhead to worry about.
    updateLabelsForPlayerVariables();

	//************************************************************
    //
    // THE Rest of this until the Title menu will be deprecated once the title menu is switched over to the new GUI system
    //
    //
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
	//  IF WE ARE AT THE TITLE SCREEN OR START SCREEN
	//=====================================================================================================================================	

	if (!MGV.runningMainGameGUI) {

	//=====================================================================================================================================
	// IF WE ARE AT THE TITLE SCREEN
	if (MGV.isTitleScreen) {	
		title_start.SetActive (true);
		title_newgame_button.GetComponent<Button> ().onClick.AddListener (() => GUI_startNewGame (1));
		title_newgame_beginner_button.GetComponent<Button> ().onClick.AddListener (() => GUI_startNewGame (0));
		title_loadgame_button.GetComponent<Button> ().onClick.AddListener (() => GUI_loadGame (1));
		title_loadgame_beginner_button.GetComponent<Button> ().onClick.AddListener (() => GUI_loadGame (0));
		MGV.isTitleScreen = false;

	}		
			
	//=====================================================================================================================================	
	// IF WE ARE AT THE START SCREEN	-- SHOW START SCREEN GUI
	 
	
	if (MGV.isStartScreen) {

				//===========================================================
				//HERE WE GIVE THE FIRST LEG OF THE ARGONAUTICA QUEST / INTRO
				GUI.Label (new Rect (540, 150, 340, 500), "Welcome to the Argonautica Jason! Find your way through the dangerous seas to complete your quest! You have found yourself at Pagasae, where King Pelias has given you the task of sailing across " +
					"the Aegean and the Black Sea to retrieve the Golden Fleece.  This is the hide of the flying ram that brought Phrixus from Boetia to Aea. The hide now hangs on a tree on the other side of the Black " +
					"Sea in the city of Aea. The great lord Aeetes prizes the fleece, and a very large dragon guards it. \n\nThe task seems impossible! But you do not sail alone, Jason. You have assembled a group of the most " +
					"powerful warriors, sailors, and prophets in Greece to help you in your quest.  Most are the sons of royal families, and each one has a unique skill. Once the heroes have all arrived, your crew stocks " +
					"the ships and the people of Pagasae greet you all.");
			
				//======================================================================
				//HERE WE DISPLAY A LIST OF CREW FOR THE PLAYER TO CONSTRUCT A CREW FROM
			
				//Update the number of currently selected crewmen
				int numOfCrew = 0;
				foreach (bool i in MGV.newGameCrewSelectList)
					if (i)
						numOfCrew++;
				GUI.Label (new Rect (960, 145, 590, 500), "It's up to you, Jason, to select your crew! Certain members, integral to your journey, must be chosen...the others are up to you! How do you build a good crew?ou start with 20 crewmen, but it is best to hire men at port and keep as close to a full crew of 30 as possible.Keep in mind that one man working hard at the oars drinks about 5 kg of water a day and eats about 0.7 kg of food a day, so you’ll need to stock up!");
				GUI.Label (new Rect (960, 220, 590, 500), "You currently have " + numOfCrew + " / 20 crewmembers!\n___________________________________________________________________________________");
			
			
				//_____________________________________
				//START OF THE CREW ROSTER SCROLL VIEW
				scrollPosition = GUI.BeginScrollView (new Rect (960, 250, 620, 380), scrollPosition, new Rect (960, 250, 600, 4000), false, true);
				
				for (int i = 0; i < MGV.newGameAvailableCrew.Count; i++) {
					//If the crewmember is necessary for the quest--lock the selection in as true
					if (!MGV.newGameAvailableCrew [i].isKillable) {
						MGV.newGameCrewSelectList [i] = GUI.Toggle (new Rect (960, 250 + (i * 100), 50, 50), true, "");
						//Otherwise let the player toggle on/off the selection
					} else {
						//We also need to run a check on whether or not we have 30 members--if we do, then hide the check box if it's 'false'
						if (numOfCrew != 20 || MGV.newGameCrewSelectList [i]) {
							MGV.newGameCrewSelectList [i] = GUI.Toggle (new Rect (960, 250 + (i * 100), 50, 50), MGV.newGameCrewSelectList [i], "");
						}
					}
					//Show the Crewmember's information / backstory
					string title = MGV.GetCloutTitleEquivalency (MGV.newGameAvailableCrew [i].clout) + " " + MGV.newGameAvailableCrew [i].name + ", the " + MGV.GetJobClassEquivalency (MGV.newGameAvailableCrew [i].typeOfCrew) + " from " + MGV.GetSettlementFromID (MGV.newGameAvailableCrew [i].originCity).name + ".";
					GUI.Label (new Rect (1010, 255 + (i * 100), 520, 60), title);
					GUI.Label (new Rect (1010, 257 + (i * 100), 520, 60), "___________________________________________________________________");
					GUI.Label (new Rect (1010, 270 + (i * 100), 520, 100), MGV.newGameAvailableCrew [i].backgroundInfo);

				
				
				}						
				
				GUI.EndScrollView ();
			
				if (numOfCrew == 20 || MGV.DEBUG_MODE_ON) {
					if (GUI.Button (new Rect (1060, 640, 400, 40), "-----START GAME-----")) {
						MGV.startGameButton_isPressed = true;
						//Turn on the ship HUD
						player_hud_parent.SetActive (true);
					}
				}
	

		}


	//=====================================================================================================================================	
	//  IF WE AREN'T AT THE TITLE SCREEN OR START SCREEN
	//=====================================================================================================================================	
	} else if (MGV.runningMainGameGUI) {
        
        //----------------------------------------------------------------------------------------------------------
        //      ALL static GUI elements go here for normail gameplay, e.g. ship stats, etc.
        //----------------------------------------------------------------------------------------------------------
        
        //`````````````````````````````````````````````````````````````````
        //  CAPTAINS LOG
        hud_captainsLog.GetComponent<Text>().text = MGV.currentCaptainsLog;
 		//`````````````````````````````````````````````````````````````````
		// 	SHIP STATS GUI
        hud_waterStores.GetComponent<Text>().text = ((int)MGV.playerShipVariables.ship.cargo[0].amount_kg).ToString ();
        hud_provisions.GetComponent<Text>().text = ((int)MGV.playerShipVariables.ship.cargo[1].amount_kg).ToString ();
        hud_shipHealth.GetComponent<Text>().text = ((int)MGV.playerShipVariables.ship.health).ToString ();
        hud_daysTraveled.GetComponent<Text>().text = (Mathf.Round(MGV.playerShipVariables.ship.totalNumOfDaysTraveled * 1000.0f) / 1000.0f).ToString ();
        hud_daysThirsty.GetComponent<Text>().text = (MGV.playerShipVariables.dayCounterThirsty).ToString ();
        hud_daysStarving.GetComponent<Text>().text = (MGV.playerShipVariables.dayCounterStarving).ToString ();
        hud_currentSpeed.GetComponent<Text>().text = (Mathf.Round(MGV.playerShipVariables.current_shipSpeed_Magnitude * 1000.0f) / 1000.0f).ToString ();
        hud_crewmember_count.GetComponent<Text>().text =(MGV.playerShipVariables.ship.crew).ToString ();
        hud_playerClout.GetComponent<Text>().text = MGV.GetCloutTitleEquivalency((int)(Mathf.Round(MGV.playerShipVariables.ship.playerClout * 1000.0f) / 1000.0f));			
		//`````````````````````````````````````````````````````````````````
		// DOCKING BUTTON -- other GUI button click handlers are done in the editor--These are done here because the button's behavior changes based on other variables. The others do not
		if (MGV.showSettlementTradeButton){ hud_button_dock.transform.GetChild(0).GetComponent<Text>().text = "CLICK TO \n  DOCK WITH \n" + MGV.currentSettlement.name; hud_button_dock.GetComponent<Button>().onClick.RemoveAllListeners();hud_button_dock.GetComponent<Button>().onClick.AddListener(() => GUI_checkOutOrDockWithPort(true));}
		else if (MGV.showNonPortDockButton){hud_button_dock.transform.GetChild(0).GetComponent<Text>().text = "CHECK OUT \n" + MGV.currentSettlement.name; hud_button_dock.GetComponent<Button>().onClick.RemoveAllListeners();hud_button_dock.GetComponent<Button>().onClick.AddListener(() => GUI_checkOutOrDockWithPort(true));}
		else {hud_button_dock.transform.GetChild(0).GetComponent<Text>().text = "DOCKING \n CURRENTLY \n UNAVAILABLE"; hud_button_dock.GetComponent<Button>().onClick.RemoveAllListeners(); }
		       
        
        //----------------------------------------------------------------------------------------------------------
        //      The remaining part of this block is for listeners that change the GUI based on variable flags
        //----------------------------------------------------------------------------------------------------------        
        
        //`````````````````````````````````````````````````````````````````
        //WE ARE SHOWING A YES / NO  PORT TAX NOTIFICATION POP UP	?
        if(MGV.showPortDockingNotification){	
            MGV.showPortDockingNotification = false;
            GUI_ShowPortDockingNotification();	
        } else if(MGV.showNonPortDockingNotification){
            MGV.showNonPortDockingNotification = false;
            GUI_ShowNonPortDockingNotification();
        }
	
        //`````````````````````````````````````````````````````````````````
		//Check to see if we need to show any generic notifications ?
		if (MGV.showNotification) {
			ShowNotification(MGV.notificationMessage);
			MGV.showNotification = false;
		}
						
		//`````````````````````````````````````````````````````````````````
		// GAME OVER GUI
		if (MGV.isGameOver){
			GUI_ShowGameOverNotification();
			MGV.isGameOver = false;
		}
		
        //`````````````````````````````````````````````````````````````````
        //  DEPRECATED -- This will be moved to buttons in the editor, and the functions will look different
        if(!MGV.showHelpGUI && !MGV.showSettlementTradeGUI){
            if(GUI.Button(new Rect(10,10,120,25), "?")){
                MGV.showHelpGUI = true;
                //Adjust the attributes of the GUI splash screen so it only shows the left portion of the background
                MGV.bg_startScreen.transform.localPosition = new Vector3(-6.18f, 0, 1);
                MGV.bg_startScreen.transform.localScale = new Vector3(5.69f, 10.2f, 0);
                MGV.bg_startScreen.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex", new Vector2(0.315f, 1f));
                MGV.bg_startScreen.SetActive(true);
            }
        }
        
        if (MGV.showHelpGUI && !MGV.showSettlementTradeGUI){
            if(GUI.Button(new Rect(10,10,120,25), "X")){
                MGV.showHelpGUI = false;
                //reset the splash screen to its defaults so that it will be normal again if the player restarts a new game
                MGV.bg_startScreen.SetActive(false);
                MGV.bg_startScreen.transform.localPosition = new Vector3(0, 0, 1);
                MGV.bg_startScreen.transform.localScale = new Vector3(18.09f, 10.2f, 0);
                MGV.bg_startScreen.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex", new Vector2(1f, 1f));
            }

        }
        
        // END OF DEPRECATED FUNCTIONS		
		
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
    //  Processing Based Functions (Ideally these will all be moved to the globalvariables  script
    //=====================================================================================================================================	
	string GetDirectionsToSettlement(Vector3 target){
		string directions = "";
		float angle =  Mathf.Atan2(target.z-MGV.currentSettlementGameObject.transform.position.z, target.x-MGV.currentSettlementGameObject.transform.position.x) * Mathf.Rad2Deg;
		if (angle < 0) angle += 360;

		if(angle >=0 && angle < 20)directions += "East:  ";
		else if(angle >= 20 && angle < 70)  directions+= "Northeast:   ";
		else if(angle >= 70 && angle < 110)	directions+= "North:   ";		
		else if(angle >= 110 && angle < 160)directions+= "Northwest:   ";
		else if(angle >= 160 && angle < 200)directions+= "West:   ";
		else if(angle >= 200 && angle < 250)directions+= "Southwest:   ";
		else if(angle >= 250 && angle < 290)directions+= "South:   ";
		else if(angle >= 290 && angle < 340)directions+= "Southeast:   ";	
		else if(angle >= 340 && angle < 360)directions+= "East:   ";		
			
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
		int price = (int) Mathf.Floor( 1000 * Mathf.Pow(1.2f,(-.1f*amount)) );
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
			
		//Add the players clout modifier. It will be a 0-100 percent reduction of the current tax rate
		
		float taxReductionAmount = taxRateToApply * (-1*MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID));
		float newTaxRate = taxRateToApply + taxReductionAmount;
		MGV.taxRateMessage = ": " + taxRateToApply.ToString("0.000") + " was reduced by: " + taxReductionAmount.ToString("0.000") + " because of your crew's clout to a final tax of: "+ newTaxRate.ToString("0.000");
		
		
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
		Debug.Log (cargoIndex + "  :  " + MGV.playerShipVariables.ship.cargo[cargoIndex].amount_kg + "  :  " + changeAmount);
		MGV.playerShipVariables.ship.cargo[cargoIndex].amount_kg += changeAmount;
		//we use a (-) change amount here because the changeAmount reflects the direction of the goods
		//e.g. if the player is selling--they are negative in cargo---but their currency is positive and vice versa.
		MGV.playerShipVariables.ship.currency += (int)(price * -changeAmount); 
	}
		
	void ShowShipResources(){
		for(int i = 0; i < MGV.playerShipVariables.ship.cargo.Length; i++){
			Text currentCargoLabel = (Text) all_trade_rows.transform.GetChild(i).GetChild (6).GetComponent<Text>();
			currentCargoLabel.text = (int)MGV.playerShipVariables.ship.cargo[i].amount_kg + " kg";
		}
	}	
	
	string GetCurrentMonth(){
		int numOfDaysTraveled = (int) MGV.playerShipVariables.ship.totalNumOfDaysTraveled;
		string monthName = "";
               if (numOfDaysTraveled <=30)  {monthName = "January";
		} else if (numOfDaysTraveled <=60)  {monthName = "February";
		} else if (numOfDaysTraveled <=90)  {monthName = "March";
		} else if (numOfDaysTraveled <=120) {monthName = "April";
		} else if (numOfDaysTraveled <=150) {monthName = "May";
		} else if (numOfDaysTraveled <=180) {monthName = "June";
		} else if (numOfDaysTraveled <=210) {monthName = "July";
		} else if (numOfDaysTraveled <=240) {monthName = "August";
		} else if (numOfDaysTraveled <=270) {monthName = "September";
		} else if (numOfDaysTraveled <=300) {monthName = "October";
		} else if (numOfDaysTraveled <=330) {monthName = "November";
		} else if (numOfDaysTraveled <=360) {monthName = "December";
		}		
		return monthName;
	}
	
    
    
	//=====================================================================================================================================	
    //  GUI Interaction Functions are the remaining code below. All of these functions control some aspect of the GUI based on state changes
    //=====================================================================================================================================	


	
	//-------------------------------------------------------------------------------------------------------------------------
	//   TITLE SCREEN FUNCTIONS AND COMPONENTS


	public void GUI_startNewGame(int difficulty){
		MGV.isTitleScreen = false;
		MGV.isStartScreen = true;
		MGV.bg_titleScreen.SetActive(false);
		MGV.bg_startScreen.SetActive(true);
		MGV.FillNewGameCrewRosterAvailability();
		if (difficulty == 0) MGV.gameDifficulty_Beginner = false;
		else MGV.gameDifficulty_Beginner = true;
	}
	
	public void GUI_loadGame (int difficulty){
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
		if (difficulty == 0) MGV.gameDifficulty_Beginner = false;
		else MGV.gameDifficulty_Beginner = true;
		MGV.SetupBeginnerGameDifficulty();
		
		//Turn on the ship HUD
		player_hud_parent.SetActive(true);
		
		MGV.controlsLocked = false;
	}











    //-------------------------------------------------------------------------------------------------------------------------
    //   GAME OVER NOTIFICATIONS AND COMPONENTS
	
	public void GUI_ShowGameOverNotification(){
		MGV.controlsLocked = true;
		//Set the notification window as active
		gameover_main.SetActive(true);
		//Setup the GameOver Message
		gameover_message.GetComponent<Text>().text = "You have lost! Your crew has died! Your adventure ends here!";
		//GUI_TAB_SetupAShrinePanel the GUI_restartGame button
		gameover_restart.GetComponent<Button>().onClick.AddListener(() => GUI_RestartGame() );

    }
    
    public void GUI_RestartGame(){
		gameover_main.SetActive(false);
        //Restart from Beginning
		MGV.RestartGame();
    }
    
    
    //-------------------------------------------------------------------------------------------------------------------------
    //   DOCKING INFO PANEL AND COMPONENTS    
  
    public void GUI_ShowNonPortDockingNotification(){
		//Show the non port notification window
		nonport_info_main.SetActive(true);
		//Set the title
		nonport_info_name.GetComponent<Text>().text = MGV.currentSettlement.name;
		//Set the description
		nonport_info_notification.GetComponent<Text>().text = MGV.currentSettlement.description;
		//Setup the okay button
		nonport_info_okay.GetComponent<Button>().onClick.AddListener(() => GUI_ExitPortNotification() );
		
	}
	
	public void GUI_ShowPortDockingNotification(){
		MGV.controlsLocked = true;
		Debug.Log ("Locked down all controls");
	
		//Show the port notification pop up
		port_info_main.SetActive(true);
		//Set the title
		port_info_name.GetComponent<Text>().text = MGV.currentSettlement.name;
		//Setup the message for the scroll view
		string portMessage = "";
		portMessage += MGV.currentSettlement.description;
		portMessage += "\n\n";
		if (MGV.isInNetwork){
			portMessage += "This Port is part of your network!\n";
			if (MGV.crewMemberWithNetwork != null) portMessage += "Your crewman, " + MGV.crewMemberWithNetwork.name + " assures you their connections here are strong! They should welcome you openly and waive your port taxes on entering!";
			else portMessage += "You know this port as captain very well! You expect that your social connections here will soften the port taxes in your favor!";
		} else {
			portMessage += "This port is outside your social network!\n";
			if(MGV.currentPortTax != 0){
				portMessage += "If you want to dock here, your tax for entering will be " + MGV.currentPortTax + " drachma. \n";
				//If the port tax will make the player go negative--alert them as they enter
				if (MGV.playerShipVariables.ship.currency - MGV.currentPortTax < 0) portMessage += "Docking here will put you in debt for " + (MGV.playerShipVariables.ship.currency - MGV.currentPortTax) + "drachma, and you may lose your ship!\n";
			} else {
				portMessage += "You only have food and water stores on board, with no taxable goods. Thankfully you will dock for free!";
			}
			portMessage += "\n\nThe portmaster informed you that the usual tax of" + MGV.taxRateMessage;
        }
        port_info_notification.GetComponent<Text>().text = portMessage;
        port_info_enter.GetComponent<Button>().onClick.AddListener(() => GUI_EnterPort() );
        port_info_leave.GetComponent<Button>().onClick.AddListener(() => GUI_ExitPortNotification() );
	}

                        //-------------------------------------------------------------------------------------------------------------------------
                        //   DOCKING INFO PANEL AND COMPONENTS    HELPER FUNCTIONS	
                        
                        public void GUI_ExitPortNotification(){
                            //Turn off both nonport AND port notification windows
                            port_info_main.SetActive(false); 
                            MGV.showPortDockingNotification = false;
                            nonport_info_main.SetActive(false);
                            MGV.showNonPortDockingNotification = false;
                            MGV.controlsLocked = false;
                        }			
                                
                        public void GUI_EnterPort(){
                            //Turn off port welcome screen
                            MGV.showPortDockingNotification = false;
                            port_info_main.SetActive(false);
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
                            
                            //-------------------------------------------------
                            //NEW GUI FUNCTIONS FOR SETTING UP TAB CONTENT
                            //Show Port Menu
                            MGV.GUI_PortMenu.SetActive(true);
                            
                            //Load Resource labels
                            ShowShipResources();
                            //Check Button availability
                            GUI_CheckAllResourceButtonsForValidity();
                            ShowSettlementResources();
                            //Setup port panels
                            GUI_TAB_SetupLoanManagementPanel();
                            GUI_TAB_SetupAShrinePanel();
                            GUI_TAB_SetupShipRepairInformation();
                            GUI_TAB_SetupTavernInformation();
                            GUI_TAB_SetupCrewManagementPanel();
                            //Show the coin image associated with this settlement
                            //Get the settlement ID as a string
                            string currentID = MGV.currentSettlement.settlementID.ToString();
                            Texture currentCoinTex =  (Texture) Resources.Load("settlement_coins/" + currentID);
                            //Now test if it exists, if the settlement does not have a matching texture, then default to a basic one
                            if (currentCoinTex){MGV.coinImage.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex",currentCoinTex); } 
                            else {MGV.coinImage.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", (Texture) Resources.Load("settlement_coins/default_coin_texture"));}
                            //Figure out the tax on the cargo hold
                            if (MGV.isInNetwork) MGV.currentPortTax = 0;
                            else MGV.currentPortTax = GetTaxRateOnCurrentShipManifest();
                            //Add a new route to the player journey log as a port entry
                            MGV.playerShipVariables.journey.AddRoute(new PlayerRoute( MGV.playerShip.transform.position, Vector3.zero, MGV.currentSettlement.settlementID, MGV.currentSettlement.name, false, MGV.playerShipVariables.ship.totalNumOfDaysTraveled), MGV.playerShipVariables, MGV.currentCaptainsLog);
                            //We should also update the ghost trail with this route otherwise itp roduce an empty 0,0,0 position later
                            MGV.playerShipVariables.UpdatePlayerGhostRouteLineRenderer(MGV.IS_NOT_NEW_GAME);
                            
                        }					
	
	
    //=================================================================================================================
	// NOTIFICATION POP-UP SYSTEM
	//=================================================================================================================	
	
	void ShowNotification(string message){
		//Declare our message as a string to ensure we have a unique instance passed
		string notificationMessage = message;
		
		//Set the Notification System to Active
		notice_notificationSystem.SetActive(true);
		
		//Clone the first child of the "Current Notifications" parent(This will be the template used to instantiate more notifications as needed
		GameObject newNotification = Instantiate((GameObject)notice_notificationParent.transform.GetChild(0).gameObject) as GameObject;
		newNotification.transform.SetParent( (Transform) notice_notificationParent.transform);
		newNotification.SetActive(true);
		
		//Reset its transform values
		newNotification.GetComponent<RectTransform>().localScale = Vector3.one;
		newNotification.GetComponent<RectTransform>().localPosition = Vector3.zero;
		
		//If there are more than 2 notifications present(there are more than 2 children--1 template + 1 notification) then let's
		//	--give the second notification a random location within a specified range of  -600 < x < 600  and -180 < y < 180
		if (notice_notificationParent.transform.childCount > 2) {
			newNotification.GetComponent<RectTransform>().localPosition = new Vector3(Random.Range(-600,600),Random.Range(-180,180),0);
		}
		//Setup the Message Text
		newNotification.transform.FindChild("ScrollView/Content/Message").GetComponent<Text>().text = notificationMessage;
		
		//Setup the confirmation button to destroy this specific notification instance
		Button notificationButton = (Button) newNotification.transform.FindChild("Confirm Button").GetComponent<Button>();
		notificationButton.onClick.AddListener(() => GUI_RemoveNotification(newNotification));
		//That's it
	}
	
	public void GUI_RemoveNotification(GameObject notification){
		//If there are only 2 children (the hidden template notification and the one we are about to delete), then turn off the notification system window and set it to active = false
		if (notice_notificationParent.transform.childCount == 2) {
			notice_notificationSystem.SetActive(false);
		}
		//Remove the current notification that flagged this event
		GameObject.Destroy(notification);
	}


	
	//=================================================================================================================
	// HELPER FUNCTIONS FOR IN-PORT TRADE WINDOW
	//=================================================================================================================	
	
	public void ShowSettlementResources(){
		
		for(int i = 0; i < MGV.currentSettlement.cargo.Length; i++){
			Text currentExchangeRate = (Text) all_trade_rows.transform.GetChild(i).GetChild (3).GetComponent<Text>();
			currentExchangeRate.text = GetPriceOfResource(MGV.currentSettlement.cargo[i].amount_kg) + "d/kg";
		}
		
	}
	
	public void GUI_Button_TryToLeavePort(){
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
			MGV.playerShipVariables.journey.AddRoute(new PlayerRoute(new Vector3(MGV.playerShip.transform.position.x, MGV.playerShip.transform.position.y, MGV.playerShip.transform.position.z), Vector3.zero, MGV.currentSettlement.settlementID, MGV.currentSettlement.name, true, MGV.playerShipVariables.ship.totalNumOfDaysTraveled), MGV.playerShipVariables, MGV.currentCaptainsLog);
			//We should also update the ghost trail with this route otherwise itp roduce an empty 0,0,0 position later
			MGV.playerShipVariables.UpdatePlayerGhostRouteLineRenderer(MGV.IS_NOT_NEW_GAME);
			
			//Turn off the coin image texture
			MGV.GUI_PortMenu.SetActive(false);
			
		} else {//Debug.Log ("Not Enough Drachma to Leave the Port!");
			MGV.showNotification = true;
			MGV.notificationMessage = "Not Enough Drachma to pay the port tax and leave!";
		}
	}
	
	public void GUI_Buy_Resources(string idXamount){
		int id = int.Parse(idXamount.Split(',')[0]) - 1;
		float amount = float.Parse (idXamount.Split (',')[1]);
		Text currentCargoLabel = (Text) all_trade_rows.transform.GetChild(id).GetChild (6).GetComponent<Text>();
				
		Debug.Log(id + " : " + amount);

		if (CheckSettlementResourceAvailability((int)(amount),id)) {ChangeShipCargo(id, amount); ChangeSettlementCargo(id, -amount);}
		
		currentCargoLabel.text = (int)MGV.playerShipVariables.ship.cargo[id].amount_kg + " kg";
		
		GUI_CheckAllResourceButtonsForValidity(id);
		ShowSettlementResources();
	}
	
	public void GUI_Sell_Resources(string idXamount){
		int id = int.Parse(idXamount.Split(',')[0]) - 1;
		float amount = float.Parse (idXamount.Split (',')[1]);
		Text currentCargoLabel = (Text) all_trade_rows.transform.GetChild(id).GetChild (6).GetComponent<Text>();

		Debug.Log(id + " : " + amount);
		
		if (CheckShipResourceAvailability((int)(amount),id)) {ChangeShipCargo(id, -amount); ChangeSettlementCargo(id, amount); }

		currentCargoLabel.text = (int)MGV.playerShipVariables.ship.cargo[id].amount_kg + " kg";		
		
		GUI_CheckAllResourceButtonsForValidity(id);
		ShowSettlementResources();
	}
	//This function, if an ID value IS given, goes through the buttons associated with that resource ID and determines
	//	--whether or not the buttons should be disabled or not
	public void GUI_CheckAllResourceButtonsForValidity(int id){
		Button currentCargoButton_one = (Button) all_trade_rows.transform.GetChild(id).GetChild (4).GetComponent<Button>();
		Button currentCargoButton_ten = (Button) all_trade_rows.transform.GetChild(id).GetChild (5).GetComponent<Button>();
		Button currentSettlementButton_one = (Button) all_trade_rows.transform.GetChild(id).GetChild(1).GetComponent<Button>(); 
		Button currentSettlementButton_ten = (Button) all_trade_rows.transform.GetChild(id).GetChild(2).GetComponent<Button>();
		
		if (CheckShipResourceAvailability((int)(1),id)){currentCargoButton_one.interactable = true;} else {currentCargoButton_one.interactable = false;}
		if (CheckShipResourceAvailability((int)(10),id)){currentCargoButton_ten.interactable = true;} else {currentCargoButton_ten.interactable = false;}
		if (CheckSettlementResourceAvailability((int)(1),id)){currentSettlementButton_one.interactable = true;} else {currentSettlementButton_one.interactable = false;}
		if (CheckSettlementResourceAvailability((int)(10),id)){currentSettlementButton_ten.interactable = true;} else {currentSettlementButton_ten.interactable = false;}
	}


	//This Function, if no ID given, goes through ALL id values and their corresponding buttons and determines
	//	--whether or not the button should be disabled or not
	public void GUI_CheckAllResourceButtonsForValidity(){
		for (int id = 0; id < 15; id++){
			Button currentCargoButton_one = (Button) all_trade_rows.transform.GetChild(id).GetChild (4).GetComponent<Button>();
			Button currentCargoButton_ten = (Button) all_trade_rows.transform.GetChild(id).GetChild (5).GetComponent<Button>();
			Button currentSettlementButton_one = (Button) all_trade_rows.transform.GetChild(id).GetChild(1).GetComponent<Button>(); 
			Button currentSettlementButton_ten = (Button) all_trade_rows.transform.GetChild(id).GetChild(2).GetComponent<Button>();
			
			if (CheckShipResourceAvailability((int)(1),id)){currentCargoButton_one.interactable = true;} else {currentCargoButton_one.interactable = false;}
			if (CheckShipResourceAvailability((int)(10),id)){currentCargoButton_ten.interactable = true;} else {currentCargoButton_ten.interactable = false;}
			if (CheckSettlementResourceAvailability((int)(1),id)){currentSettlementButton_one.interactable = true;} else {currentSettlementButton_one.interactable = false;}
			if (CheckSettlementResourceAvailability((int)(10),id)){currentSettlementButton_ten.interactable = true;} else {currentSettlementButton_ten.interactable = false;}
		}
	}


	//This function updates the player cargo labels after any exchange between money and resources has been made
	public void updateLabelsForPlayerVariables(){
		player_currency.GetComponent<Text>().text = MGV.playerShipVariables.ship.currency.ToString();
		player_current_cargo.GetComponent<Text>().text = Mathf.CeilToInt(MGV.playerShipVariables.ship.GetTotalCargoAmount()).ToString();
		player_max_cargo.GetComponent<Text>().text = Mathf.CeilToInt(MGV.playerShipVariables.ship.cargo_capicity_kg).ToString();
	
	}
	
	//This function activates the docking element when the dock button is clicked. A bool is passed to determine whether or not the button is responsive
	public void GUI_checkOutOrDockWithPort(bool isAvailable){
		if (isAvailable){
			MGV.showPortDockingNotification = true;
			//Figure out if this settlement is part of the player's network
			MGV.isInNetwork = MGV.CheckIfCityIDIsPartOfNetwork(MGV.currentSettlement.settlementID);
			//Figure out the tax on the cargo hold
			MGV.currentPortTax = GetTaxRateOnCurrentShipManifest();
		}
		//Else do nothing
	}


    //============================================================================================================================================================================
    //============================================================================================================================================================================
    //  THESE CONSTRUCT ALL OF THE PANELS WITHIN THE PORT MENU GUI SYSTEM
    //============================================================================================================================================================================

	
	//=================================================================================================================
	// SETUP THE CREW MANAGEMENT PANEL
	//=================================================================================================================	
	public void GUI_TAB_SetupCrewManagementPanel(){
		
		//================================================================================================================
		// HIRE CREW PANEL

			//Create a list of 5 crew members to hire
			for (int i = 0; i < MGV.currentlyAvailableCrewMembersAtPort.Count; i++){
			
				//First let's get a clone of our hidden row in the tavern scroll view
				GameObject currentMemberRow = Instantiate((GameObject)tab_crew_hireScrollWindow.transform.GetChild(0).gameObject) as GameObject;
				currentMemberRow.transform.SetParent( (Transform) tab_crew_hireScrollWindow.transform);
				//Set the current clone to active
				currentMemberRow.SetActive(true);
				currentMemberRow.GetComponent<RectTransform>().localScale = Vector3.one;
				Text memberName = (Text) currentMemberRow.transform.FindChild("Crew Name").GetComponent<Text>();
				Text memberJob = (Text) currentMemberRow.transform.FindChild("Sailor Job/Job Title").GetComponent<Text>();
				Text memberCost = (Text) currentMemberRow.transform.FindChild("Pay Demand/Pay Amount").GetComponent<Text>();
				Text memberHome = (Text) currentMemberRow.transform.FindChild("Home Town/Home Town Name").GetComponent<Text>();
				Text memberClout = (Text) currentMemberRow.transform.FindChild("Clout/Clout Title").GetComponent<Text>();
				Button hireMember = (Button) currentMemberRow.transform.FindChild("Hire Button").GetComponent<Button>();
				Button moreMemberInfo = (Button) currentMemberRow.transform.FindChild("Backstory/Backstory Button").GetComponent<Button>();
			
				CrewMember currentMember = MGV.currentlyAvailableCrewMembersAtPort[i];
				memberName.text = currentMember.name;
				memberJob.text = MGV.GetJobClassEquivalency(currentMember.typeOfCrew);
				memberCost.text = (currentMember.clout *2).ToString();//TODO Temporary solution--need to add a clout check modifier
				memberHome.text = MGV.GetSettlementFromID(currentMember.originCity).name;
				memberClout.text = MGV.GetCloutTitleEquivalency(currentMember.clout);


				hireMember.onClick.AddListener(() => GUI_HireCrewMember(currentMember, currentMemberRow, int.Parse(memberCost.text)));
				moreMemberInfo.onClick.AddListener(() => GUI_GetBackgroundInfo(currentMember.backgroundInfo));

			}
			
		
		//================================================================================================================
		// FIRE CREW PANEL

		//Create a list of 5 crew members to hire
		foreach (CrewMember member in MGV.playerShipVariables.ship.crewRoster){
			//We have to re-declare the CrewMember argument here or else when we apply the variable to the onClick() handler
			//	--all onClick()'s in this loop will reference the last CrewMember instance in the loop rather than their
			//	--respective iterated instances
			CrewMember currentMember = member;
			
			//First let's get a clone of our hidden row in the tavern scroll view
			GameObject currentMemberRow = Instantiate((GameObject)tab_crew_fireScrollWindow.transform.GetChild(0).gameObject) as GameObject;
			currentMemberRow.transform.SetParent( (Transform) tab_crew_fireScrollWindow.transform);
			//Set the current clone to active
			currentMemberRow.SetActive(true);
			currentMemberRow.GetComponent<RectTransform>().localScale = Vector3.one;
			Text memberName = (Text) currentMemberRow.transform.FindChild("Crew Name").GetComponent<Text>();
			Text memberJob = (Text) currentMemberRow.transform.FindChild("Sailor Job/Job Title").GetComponent<Text>();
			Text memberHome = (Text) currentMemberRow.transform.FindChild("Home Town/Home Town Name").GetComponent<Text>();
			Text memberClout = (Text) currentMemberRow.transform.FindChild("Clout/Clout Title").GetComponent<Text>();
			Button fireMember = (Button) currentMemberRow.transform.FindChild("Fire Button").GetComponent<Button>();
			Button moreMemberInfo = (Button) currentMemberRow.transform.FindChild("Backstory/Backstory Button").GetComponent<Button>();
			
			memberName.text = currentMember.name;
			memberJob.text = MGV.GetJobClassEquivalency(currentMember.typeOfCrew);
			memberHome.text = MGV.GetSettlementFromID(currentMember.originCity).name;
			memberClout.text = MGV.GetCloutTitleEquivalency(currentMember.clout);
			
			
			fireMember.onClick.AddListener(() => GUI_FireCrewMember(currentMember, currentMemberRow));
			moreMemberInfo.onClick.AddListener(() => GUI_GetBackgroundInfo(currentMember.backgroundInfo));
			
		}
		
	}
                        //----------------------------------------------------------------------------
                        //----------------------------CREW PANEL HELPER FUNCTIONS		
						
						public void GUI_FireCrewMember(CrewMember crewman, GameObject currentRow){
							GameObject.Destroy(currentRow);
							MGV.playerShipVariables.ship.crewRoster.Remove (crewman);
							MGV.showNotification = true;
							MGV.notificationMessage = crewman.name + " looked at you sadly and said before leaving, 'I thought I was doing so well. I'm sorry I let you down. Guess I'll go drink some cheap wine...";						
							
						}
	
		
						public void GUI_HireCrewMember(CrewMember crewman, GameObject currentRow, int costToHire ){
							//Check to see if player has enough money to hire
							if(MGV.playerShipVariables.ship.currency >= costToHire){
								//Now check to see if there is room to hire a new crew member!
								if (MGV.playerShipVariables.ship.crewRoster.Count < MGV.playerShipVariables.ship.crewCapacity){
									MGV.playerShipVariables.ship.crewRoster.Add(crewman);
									
									//Subtract the cost from the ship's money
									MGV.playerShipVariables.ship.currency -= costToHire; 
									
									//Remove Row
									GameObject.Destroy(currentRow);
									
									
									//If there isn't room, then let the player know
								} else {
									MGV.showNotification = true;
									MGV.notificationMessage = "You don't have room on the ship to hire " + crewman.name + ".";						
								}
								//If not enough money, then let the player know
							} else {
								MGV.showNotification = true;
								MGV.notificationMessage = "You can't afford to hire " + crewman.name + ".";
							}				
						}
						
						public void GUI_GetBackgroundInfo(string info){
							MGV.showNotification = true;
							MGV.notificationMessage = info;					
							
						}

	//=================================================================================================================
	// SETUP THE LOAN MANAGEMENT PANEL
	//=================================================================================================================	
	
	public void GUI_TAB_SetupLoanManagementPanel(){

	
		//-------NEW LOAN-----------------------
		if(MGV.playerShipVariables.ship.currentLoan == null){
			//Turn on the panel we need and the others off
			tab_loan_new_parent.SetActive(true);
			tab_loan_old_parent.SetActive(false);
			tab_loan_elsewhere_parent.SetActive(false);
								
			//Setup the initial term to repay the loan
			float numOfDaysToPayOffLoan = 10;
			//Determine the base loan amount off the city's population
			float baseLoanAmount = 500 * (MGV.currentSettlement.population / 1000);
			//If base loan amount is less than 200 then make it 200 as the smallest amount available
			if (baseLoanAmount < 200f) baseLoanAmount = 200f;
			//Determine the actual loan amount off the player's clout
			int loanAmount = (int) (baseLoanAmount + (baseLoanAmount * MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID)));
			//Determmine the base interest rate of the loan off the city's population
			float baseInterestRate = 10 + (MGV.currentSettlement.population / 1000);
			//Determine finalized interest rate after determining player's clout
			float finalInterestRate = (float)System.Math.Round(baseInterestRate - (baseInterestRate * MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID)),3);
			//Determine the precompiled interest if returned on time
			int totalAmountDueAtTerm = Mathf.CeilToInt(loanAmount + (loanAmount * (finalInterestRate/100)));
						
			tab_loan_new_dueDate.GetComponent<Text>().text = numOfDaysToPayOffLoan.ToString();
			tab_loan_new_interestRate.GetComponent<Text>().text = finalInterestRate.ToString();
			tab_loan_new_loanAmount.GetComponent<Text>().text = loanAmount.ToString();
			tab_loan_new_totalOwed.GetComponent<Text>().text = totalAmountDueAtTerm.ToString();
			//Create the Loan object for our button to process		
			Loan newLoan = new Loan (loanAmount, finalInterestRate, numOfDaysToPayOffLoan, MGV.currentSettlement.settlementID);
			
			tab_loan_new_takeLoanButton.GetComponent<Button>().onClick.AddListener(() => GUI_TakeOutLoan(loanAmount, newLoan));
		
		//-------OLD LOAN-----------------------	
		//If the player does have a loan already, then we need to make them aware of that
		//--If the player is back at the origin settlement of the loan, then the player should be allowed to pay the loan back
		} else if (MGV.CheckIfShipBackAtLoanOriginPort()){
			//Turn on the panel we need and the others off
			tab_loan_new_parent.SetActive(false);
			tab_loan_old_parent.SetActive(true);
			tab_loan_elsewhere_parent.SetActive(false);
			
			int loanAmount = MGV.playerShipVariables.ship.currentLoan.GetTotalAmountDueWithInterest();
			tab_loan_old_dueDate.GetComponent<Text>().text = MGV.playerShipVariables.ship.currentLoan.numOfDaysUntilDue.ToString();
			tab_loan_old_loanAmount.GetComponent<Text>().text = loanAmount.ToString();

			tab_loan_old_payBackButton.GetComponent<Button>().onClick.AddListener(() => GUI_PayBackLoan(loanAmount));			
			
		//-------LOAN IS ELSEWHERE-----------------------
		//If at a different settlement, the player needs to be made aware that they can only take a loan out from a single settlement at a time.
		} else {
			//Turn on the panel we need and the others off
			tab_loan_new_parent.SetActive(false);
			tab_loan_old_parent.SetActive(false);
			tab_loan_elsewhere_parent.SetActive(true);
			
			tab_loan_elsewhere_dueDate.GetComponent<Text>().text = MGV.playerShipVariables.ship.currentLoan.numOfDaysUntilDue.ToString();
			tab_loan_elsewhere_loanAmount.GetComponent<Text>().text = MGV.playerShipVariables.ship.currentLoan.GetTotalAmountDueWithInterest().ToString();
			tab_loan_elsewhere_loanOrigin.GetComponent<Text>().text = MGV.GetSettlementFromID(MGV.playerShipVariables.ship.currentLoan.settlementOfOrigin).name;
			
		}
		
	
	}
							//----------------------------------------------------------------------------
							//----------------------------LOAN PANEL HELPER FUNCTIONS	
							
							public void GUI_PayBackLoan(int amountDue){
								//Pay the loan back if the player has the currency to do it
								if (MGV.playerShipVariables.ship.currency > amountDue){
									MGV.playerShipVariables.ship.currency -= amountDue;
									MGV.playerShipVariables.ship.currentLoan = null;
									MGV.showNotification = true;
									MGV.notificationMessage = "You paid back your loan and earned a little respect!";
									//give a boost to the players clout for paying back loan
									MGV.AdjustPlayerClout(3);
									//Reset our loan panel
									GUI_TAB_SetupLoanManagementPanel();
								//Otherwise let player know they can't afford to pay the loan back
								} else {
									MGV.showNotification = true;
									MGV.notificationMessage = "You currently can't afford to pay your loan back! Better make some more money!";
								}							
							}
							
							public void GUI_TakeOutLoan(int loanAmount, Loan loan){
								MGV.playerShipVariables.ship.currentLoan = loan;
								MGV.playerShipVariables.ship.currency += loanAmount;
								MGV.showNotification = true;
								MGV.notificationMessage = "You took out a loan of " + loanAmount +  " drachma! Remember to pay it back in due time!";	
								//Reset our loan panel
								GUI_TAB_SetupLoanManagementPanel();						
							}
	


//=================================================================================================================
// SETUP THE BUILD A MONUMENT PANEL
//=================================================================================================================	
public void GUI_TAB_SetupAShrinePanel(){
	
	int baseCost = 0; 
	//We need to do a clout check as well as a network checks
	int baseModifier = Mathf.CeilToInt(1000 - (200 * MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID)));
	if(MGV.CheckIfCityIDIsPartOfNetwork(MGV.currentSettlement.settlementID)){
		baseCost = Mathf.CeilToInt(MGV.currentSettlement.tax_network * baseModifier * 1);
	} else {
		baseCost = Mathf.CeilToInt(MGV.currentSettlement.tax_neutral * baseModifier * 1);
	}
	
	int statueCost =baseCost/2;
	int shrineCost =baseCost;
	int templeCost =baseCost*2;
	tab_monument_buildStatueButton.transform.GetChild (0).GetComponent<Text>().text = statueCost.ToString () + " DR";
	tab_monument_buildStatueButton.GetComponent<Button>().onClick.AddListener(() => GUI_BuildAStatue(statueCost));
	tab_monument_buildShrineButton.transform.GetChild (0).GetComponent<Text>().text = shrineCost.ToString () + " DR";
	tab_monument_buildShrineButton.GetComponent<Button>().onClick.AddListener(() => GUI_BuildAShrine(shrineCost));
	tab_monument_buildTempleButton.transform.GetChild (0).GetComponent<Text>().text = templeCost.ToString () + " DR";
	tab_monument_buildTempleButton.GetComponent<Button>().onClick.AddListener(() => GUI_BuildATemple(templeCost));
	
}
							//----------------------------------------------------------------------------
							//----------------------------MONUMENT PANEL HELPER FUNCTIONS
							public void GUI_BuildAStatue(int cost){
								
								if (MGV.playerShipVariables.ship.currency > cost){
									MGV.playerShipVariables.ship.currency -= cost;
									MGV.showNotification = true;
									MGV.notificationMessage = "You built a statue for " + MGV.currentSettlement.name +  "! You've earned a little clout!";
									MGV.AdjustPlayerClout(1);
								} else {
									MGV.showNotification = true;
									MGV.notificationMessage = "You don't have enough money to build a statue for " + MGV.currentSettlement.name;
								}
							}
							public void GUI_BuildAShrine(int cost){
								
								if (MGV.playerShipVariables.ship.currency > cost){
									MGV.playerShipVariables.ship.currency -= cost;
									MGV.showNotification = true;
									MGV.notificationMessage = "You built a shrine for " + MGV.currentSettlement.name +  "! You've earned quite a bit of clout!";
									MGV.AdjustPlayerClout(4);
								} else {
									MGV.showNotification = true;
									MGV.notificationMessage = "You don't have enough money to build a Shrine for " + MGV.currentSettlement.name;
								}
							}
							public void GUI_BuildATemple(int cost){
								
								if (MGV.playerShipVariables.ship.currency > cost){
									MGV.playerShipVariables.ship.currency -= cost;
									MGV.showNotification = true;
									MGV.notificationMessage = "You built a temple for " + MGV.currentSettlement.name +  "! You've earned a tremendous amount of clout!";
									MGV.AdjustPlayerClout(8);
									} else {
									MGV.showNotification = true;
									MGV.notificationMessage = "You don't have enough money to build a Temple for " + MGV.currentSettlement.name;
								}
							}



	//=================================================================================================================
	// SETUP THE TAVERN PANEL
	//=================================================================================================================	
	public void GUI_TAB_SetupTavernInformation(){
		
		//Let's clear our current list of settlements
		for(int i = 1; i < tab_tavern_scrollWindow.transform.childCount;i++){
			GameObject.Destroy(tab_tavern_scrollWindow.transform.GetChild(i).gameObject);
		}
		
		//First clear the settlement list
		relevantSettlements.Clear ();
		foreach(int settlementID in MGV.playerShipVariables.ship.playerJournal.knownSettlements){
			Settlement settlement = MGV.GetSettlementFromID(settlementID);
				//make sure not to list the current settlement the player is at
				if(MGV.currentSettlement.settlementID != settlementID){	
					//First let's get a clone of our hidden row in the tavern scroll view
					GameObject currentPort = Instantiate((GameObject)tab_tavern_scrollWindow.transform.GetChild(0).gameObject) as GameObject;
					currentPort.transform.SetParent( (Transform) tab_tavern_scrollWindow.transform);
					//Set the current clone to active
					currentPort.SetActive(true);
					currentPort.GetComponent<RectTransform>().localScale = Vector3.one;
					Text portLabel = (Text) currentPort.transform.FindChild("Port Name").GetComponent<Text>();
					Button hintCostBut = (Button) currentPort.transform.FindChild("Hint Button").GetComponent<Button>();
					Button navCostBut = (Button) currentPort.transform.FindChild("Hire Navigator Button").GetComponent<Button>();
					//Setup the Port Name
					portLabel.text = settlement.name;
					//Let's setup the Navigator hiring information
					//-- Figure out the Cost to Hire a navigator for this city
					float initialCost = MGV.GetDistanceBetweenTwoLatLongCoordinates(MGV.currentSettlement.location_longXlatY, settlement.location_longXlatY) / 1000f;
					int costToHire = Mathf.RoundToInt(initialCost - (initialCost * MGV.GetOverallCloutModifier(settlement.settlementID)));
					//--Set the button label to show the cost
					navCostBut.transform.FindChild("Cost For Navigator").GetComponent<Text>().text = costToHire.ToString() + "Dr";
					navCostBut.onClick.AddListener(() => GUI_HireANavigator(settlement, costToHire));
					
					//Now let's setup a hint button if it's a city. If it's not a city, then there is no trading and nothign to ask about
					if (settlement.typeOfSettlement == 1){
						//Set the button to Active is we are using it!
						hintCostBut.interactable = true;
						//Now figure out the costs for questions for the relevant settlements
						initialCost = MGV.GetDistanceBetweenTwoLatLongCoordinates(MGV.currentSettlement.location_longXlatY, settlement.location_longXlatY) / 10000f;
						int costForHint = Mathf.RoundToInt(initialCost - (initialCost * MGV.GetOverallCloutModifier(settlement.settlementID)));
						//--Set the Button label to show the cost
						hintCostBut.transform.FindChild("Cost For Hint").GetComponent<Text>().text = costForHint.ToString() + "Dr";
						hintCostBut.onClick.AddListener(() => GUI_BuyHint(settlement, costForHint));						
									
					} else {
						//turn off the button if it's not a port
						hintCostBut.interactable = false;
					}
				}
		}
	}
					//----------------------------------------------------------------------------
					//----------------------------TAVERN PANEL HELPER FUNCTIONS
		
					public void GUI_BuyHint(Settlement currentSettlement, int hintCost){
				
						if (MGV.playerShipVariables.ship.currency < hintCost){
							MGV.showNotification = true;
							MGV.notificationMessage = "Not enough money to buy this information!";
						} else {
							MGV.playerShipVariables.ship.currency -= hintCost;
							MGV.showNotification = true;
							MGV.notificationMessage = GetInfoOnNetworkedSettlementResource(currentSettlement.cargo[Random.Range (0,currentSettlement.cargo.Length)]);
						}
						
					}
					public void GUI_HireANavigator(Settlement thisSettlement, int costToHire){
						//Do this if button pressed
						//Check to see if player has enough money to hire
						if(MGV.playerShipVariables.ship.currency >= costToHire){
							//subtract the cost from the players currency
							MGV.playerShipVariables.ship.currency -= (int) costToHire;
							//change location of beacon
							Vector3 location = Vector3.zero;
							for( int x = 0; x < MGV.settlement_masterList_parent.transform.childCount; x++) 
								if (MGV.settlement_masterList_parent.transform.GetChild(x).GetComponent<script_settlement_functions>().thisSettlement.settlementID == thisSettlement.settlementID)
									location = MGV.settlement_masterList_parent.transform.GetChild(x).position;
							MGV.navigatorBeacon.transform.position = location;
							MGV.navigatorBeacon.GetComponent<LineRenderer>().SetPosition(0, new Vector3(location.x, 0, location.z));
							MGV.navigatorBeacon.GetComponent<LineRenderer>().SetPosition(1, location + new Vector3(0,400,0));
							MGV.playerShipVariables.UpdateNavigatorBeaconAppearenceBasedOnDistance();
							MGV.playerShipVariables.ship.currentNavigatorTarget = thisSettlement.settlementID;
							MGV.ShowANotificationMessage("You hired a navigator to " + thisSettlement.name + " for " + costToHire + " drachma.");
							//If not enough money, then let the player know
						} else {
							MGV.showNotification = true;
							MGV.notificationMessage = "You can't afford to hire a navigator to " + thisSettlement.name + ".";
						}	
					}
	

	//=================================================================================================================
	// SETUP THE SHIP REPAIR PANEL
	//=================================================================================================================	
	public void GUI_TAB_SetupShipRepairInformation(){

			//We need to do a clout check as well as a network checks
			//costToRepair is a GLOBAL var to this script
			int baseModifier = Mathf.CeilToInt(2 - MGV.GetOverallCloutModifier(MGV.currentSettlement.settlementID));
			if(MGV.CheckIfCityIDIsPartOfNetwork(MGV.currentSettlement.settlementID)){
				costToRepair = Mathf.CeilToInt(MGV.currentSettlement.tax_network * baseModifier * 1);
			} else {
				costToRepair = Mathf.CeilToInt(MGV.currentSettlement.tax_neutral * baseModifier * 1);
			}
	
			Text cost_allHP = (Text) tab_shiprepair_cost_allhp.GetComponent<Text>();
			Text cost_1HP = (Text) tab_shiprepair_cost_onehp.GetComponent<Text>();
			Text currentHP = (Text) tab_shiprepair_shiphealth.GetComponent<Text>();
			
			string oneHPCost = Mathf.CeilToInt(costToRepair).ToString();
			string allHPCost = (Mathf.CeilToInt(100-Mathf.CeilToInt(MGV.playerShipVariables.ship.health)) * costToRepair).ToString();
			string shipCurrent = Mathf.CeilToInt(MGV.playerShipVariables.ship.health).ToString();
			
			//If the ship is at 100HP already, then let's not worry about giving the player the costs--we'll replace the costs by an X
			//	--and disable the repair buttons
			if(shipCurrent == "100"){
				cost_1HP.text = "X";
				cost_allHP.text = "X";
				currentHP.text = shipCurrent;
				tab_shiprepair_repairOneButton.GetComponent<Button>().interactable = false;
				tab_shiprepair_repairAllButton.GetComponent<Button>().interactable = false;			
			} else {
				cost_1HP.text = oneHPCost;
				cost_allHP.text = allHPCost;
				currentHP.text = shipCurrent;
				tab_shiprepair_repairOneButton.GetComponent<Button>().interactable = true;
				tab_shiprepair_repairAllButton.GetComponent<Button>().interactable = true;
			}
	}
	
	
                    //----------------------------------------------------------------------------
                    //----------------------------SHIP REPAIR PANEL HELPER FUNCTIONS		
					public void GUI_RepairShipByOneHP(){
						MGV.playerShipVariables.ship.health += 1f;
						//make sure the hp can't go above 100
						if (MGV.playerShipVariables.ship.health > 100){
							MGV.playerShipVariables.ship.health = 100;
							MGV.showNotification = true;
							MGV.notificationMessage = "Your ship is already fully repaired";						
						} else {
							MGV.playerShipVariables.ship.currency -= costToRepair;
						}
						GUI_TAB_SetupShipRepairInformation();
					}
					
					public void GUI_RepairShipByAllHP(){
						if(Mathf.CeilToInt(MGV.playerShipVariables.ship.health) >= 100){
							MGV.showNotification = true;
							MGV.notificationMessage = "Your ship is already fully repaired";						
						} else {
							MGV.playerShipVariables.ship.currency -= (int) (costToRepair * Mathf.CeilToInt(100-MGV.playerShipVariables.ship.health));
							MGV.playerShipVariables.ship.health = 100f;
						}
						GUI_TAB_SetupShipRepairInformation();		
					}	
	

    
    

    //============================================================================================================================================================================
    //============================================================================================================================================================================
	// ADDITIONAL FUNCTIONS FOR GUI BUTTONS (These are linked from the Unity Editor)
    //============================================================================================================================================================================

    //-----------------------------------------------------
	//THIS IS THE UNFURLING / FURLING OF SAILS BUTTON
	public void GUI_furlOrUnfurlSails(){
		if(MGV.sailsAreUnfurled){
			hud_button_furlSails.transform.GetChild(0).GetComponent<Text>().text = "Furl Sails";
			MGV.sailsAreUnfurled = false;
			foreach(GameObject sail in MGV.sails)
				sail.SetActive(false);			
		} else {
			hud_button_furlSails.transform.GetChild(0).GetComponent<Text>().text = "Unfurl Sails";
			MGV.sailsAreUnfurled = true;
			foreach(GameObject sail in MGV.sails)
				sail.SetActive(true);
			
		}	
	}

	
	//-----------------------------------------------------
	//THIS IS THE DROP ANCHOR BUTTON
	public void GUI_dropAnchor(){
		//If the controls are locked--we are traveling so force it to stop
		if(MGV.controlsLocked && !MGV.showSettlementTradeGUI)
			MGV.playerShipVariables.rayCheck_stopShip = true;
	}
	
    //-----------------------------------------------------
	//THIS IS THE REST BUTTON
	
	public void GUI_restOverNight(){
		//If the controls are locked--we are traveling so force it to stop
		if(MGV.controlsLocked && !MGV.showSettlementTradeGUI)
			MGV.playerShipVariables.rayCheck_stopShip = true;
		//Run a script on the player controls that fast forwards time by a quarter day
		MGV.isPassingTime = true;
		MGV.controlsLocked = true;
		StartCoroutine(MGV.playerShipVariables.WaitForTimePassing(.25f, false));
	}	
	
	//-----------------------------------------------------
	//THIS IS THE SAVE DATA BUTTON
	public void GUI_saveGame(){
		MGV.notificationMessage = "Saved Data File 'player_save_game.txt' To: " + Application.persistentDataPath + "/" ;
		MGV.showNotification = true;
		MGV.SaveUserGameData(false);
	}
	
	//-----------------------------------------------------
	//THIS IS THE RESTART GAME BUTTON	
	public void GUI_restartGame(){
		MGV.RestartGame();
	}

}
