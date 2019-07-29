//=======================================================================================================================================
//
//  globalVariables.cs   -- Main Global Variable code interface
//
//    --This script is for setting up several things:
//
//      Game Inisitialization: This sets up the entire game world, and when finished, allows the player to interact with the game.  
//          --This involves building the wind and water current vectors, settlements, and other game objects that populate the world
//      Global Classes: Eventually all classes should reside here.
//      Global Variables: All variables that need to easily be accessed by all scripts are stored here for centralized access.
//          --This includes stored references to other script objects with variables, e.g. if the GUI needs to access variables
//          --of the player's current Ship object attached to the player_controls script
//      Global Functions: The bulk of this script is to handle most of the non-control logic of the game, e.g. running algorithms to
//          --determine aggregate clout scores, or the costs of various resources or other utilities the player can access.
//
//      NOTE: This script does not have an update function. It performs no loop in the game's core logic. It initializes the game world,
//              --and then acts as a resevoir of functions and data for the other scripts to access
//
//======================================================================================================================================

/*
 * TODO:
 * These are all getting modified on every play through. Need to instance them before changing the material at runtime.
 *  modified:   Assets/Materials/mat_blue.mat
 *  modified:   Assets/Materials/mat_cursor_ring.mat
 *  modified:   Assets/Materials/mat_skybox_clouds_trans 1.mat
 *  modified:   Assets/Materials/mat_skybox_moon.mat
 *  modified:   Assets/Materials/mat_skybox_skycolor.mat
 *  modified:   Assets/Materials/mat_skybox_sun.mat
 *  modified:   Assets/Materials/mat_water.mat
 *  modified:   Assets/Materials/mat_water_sprite.mat
 */



using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


//======================================================================================================================================================================
//======================================================================================================================================================================
//  SETUP ALL GLOBAL VARIABLES
//======================================================================================================================================================================
//======================================================================================================================================================================

public class GameVars : MonoBehaviour
{
	const int windZoneColumns = 64;
	const int windZoneRows = 32;

	const int currentZoneColumns = 128;
	const int currentZoneRows = 64;

	// TODO: Is this a bug? These never change.
	public const bool IS_NEW_GAME = true;
	public const bool IS_NOT_NEW_GAME = false;

	// TODO: Is this a bug? These never change.
	public const string TD_year = "2000";
	public const string TD_month = "1";
	public const string TD_day = "1";
	public const string TD_hour = "0";
	public const string TD_minute = "0";
	public const string TD_second = "0";

	[Header("World Scene Refs")]
	public GameObject FPVCamera;
	public GameObject camera_Mapview;
	public GameObject terrain;
	public GameObject cityLightsParent;

	[Header("Ship Scene Refs")]
	public GameObject[] sails = new GameObject[6];
	public GameObject[] shipLevels;

	[Header("Ununorganized Scene Refs")]
	public List<CrewMember> currentlyAvailableCrewMembersAtPort; // updated every time ship docks at port

	[Header("GUI Scene Refs")]
	public GameObject MasterGUISystem;
	public GameObject GUI_PortMenu;
	public GameObject GUI_GameHUD;
	public GameObject selection_ring;

	[Header("Skybox Scene Refs")]
	public GameObject skybox_celestialGrid;
	public GameObject skybox_MAIN_CELESTIAL_SPHERE;
	public GameObject skybox_ecliptic_sphere;
	public GameObject skybox_clouds;
	public GameObject skybox_horizonColor;
	public GameObject skybox_sun;
	public GameObject skybox_moon;

	[Header("Material Asset Refs")]
	// TODO: instance these before modifying so they don't change on disk
	public Material mat_waterCurrents;
	public Material mat_water;

	[Header("Beacons")]
	public GameObject navigatorBeacon;
	public GameObject crewBeacon;

	// TODO: unorganized variables
	[HideInInspector] public GameObject mainCamera;
	[HideInInspector] public GameObject playerTrajectory;
	[HideInInspector] public GameObject playerGhostRoute;
	[HideInInspector] public WindRose[,] windrose_January = new WindRose[10, 8];
	[HideInInspector] public GameObject windZoneParent;
	[HideInInspector] public GameObject waterSurface;
	[HideInInspector] public CurrentRose[,] currentRose_January;
	[HideInInspector] public GameObject currentZoneParent;

	// settlements
	[HideInInspector] public Settlement[] settlement_masterList;
	[HideInInspector] public GameObject settlement_masterList_parent;
	[HideInInspector] public GameObject currentSettlementGameObject;
	[HideInInspector] public Settlement currentSettlement;
	[HideInInspector] public int currentPortTax = 0;		// this is derived from the currentSettlement. could be a getter on settlement object

	// ship
	[HideInInspector] public GameObject playerShip;
	[HideInInspector] public script_player_controls playerShipVariables;

	// captain's log
	[HideInInspector] public string currentCaptainsLog = "";
	[HideInInspector] public CaptainsLogEntry[] captainsLogEntries;
	[HideInInspector] public List<CaptainsLogEntry> currentLogPool = new List<CaptainsLogEntry>();

	// resources
	[HideInInspector] public List<MetaResource> masterResourceList = new List<MetaResource>();

	// game state
	[HideInInspector] public bool controlsLocked = false;
	[HideInInspector] public bool isGameOver = false;
	[HideInInspector] public bool justLeftPort = false;
	[HideInInspector] public bool menuControlsLock = false;
	[HideInInspector] public bool gameIsFinished = false;
	[HideInInspector] public bool isPerformingRandomEvent = false;
	[HideInInspector] public bool isPassingTime = false;

	// notifications
	//The main notifications are handled by the first two variables
	//	--to make sure multiple notifications can be seen that might overlap, e.g. the player triggers two notifications in an action
	//	--there are two and if the first is 'true' or showing a message, it will default to a secondary notification window
	[HideInInspector] public bool showNotification = false;
	[HideInInspector] public string notificationMessage = "";
	[HideInInspector] public bool showSecondaryNotification = false;
	[HideInInspector] public string secondaryNotificationMessage = "";

	// environment
	[HideInInspector] public Light mainLightSource;

	// title and start screens
	[HideInInspector] public bool startGameButton_isPressed = false;
	[HideInInspector] public bool isTitleScreen = true;
	[HideInInspector] public bool isStartScreen = false;
	[HideInInspector] public GameObject camera_titleScreen;
	[HideInInspector] public GameObject bg_titleScreen;
	[HideInInspector] public GameObject bg_startScreen;
	[HideInInspector] public bool isLoadedGame = false;

	//###################################
	//	Crew Member Variables
	//###################################
	[HideInInspector] public List<CrewMember> masterCrewList = new List<CrewMember>();

	//###################################
	//	GUI VARIABLES
	//###################################
	[HideInInspector] public bool runningMainGameGUI = false;
	[HideInInspector] public bool showSettlementGUI = false;
	[HideInInspector] public bool showSettlementTradeButton = false;
	[HideInInspector] public bool[] newGameCrewSelectList = new bool[40];
	[HideInInspector] public List<CrewMember> newGameAvailableCrew = new List<CrewMember>();
	[HideInInspector] public bool showPortDockingNotification = false;
	[HideInInspector] public bool gameDifficulty_Beginner = false;
	[HideInInspector] public bool showNonPortDockButton = false;
	[HideInInspector] public bool showNonPortDockingNotification = false;
	[HideInInspector] public bool updatePlayerCloutMeter = false;

	// high level game systems
	public Trade Trade { get; private set; }
	public Network Network { get; private set; }
	public bool isInNetwork => Network.CheckIfCityIDIsPartOfNetwork(currentSettlement.settlementID);

	//###################################
	//	RANDOM EVENT VARIABLES
	//###################################
	[HideInInspector] public List<int> activeSettlementInfluenceSphereList = new List<int>();

	//###################################
	//	DEBUG VARIABLES
	//###################################
	[ReadOnly] public int DEBUG_currentQuestLeg = 0;
	[HideInInspector] public bool DEBUG_MODE_ON = false;






	//======================================================================================================================================================================
	//======================================================================================================================================================================
	//  INITIALIZE THE GAME WORLD
	//======================================================================================================================================================================
	//======================================================================================================================================================================


	// Use this for initialization
	void Awake() {

		Globals.Register(this);

		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		playerShip = GameObject.FindGameObjectWithTag("playerShip");
		camera_titleScreen = GameObject.FindGameObjectWithTag("camera_titleScreen");
		waterSurface = GameObject.FindGameObjectWithTag("waterSurface");
		playerGhostRoute = GameObject.FindGameObjectWithTag("playerGhostRoute");
		playerTrajectory = GameObject.FindGameObjectWithTag("playerTrajectory");
		mainLightSource = GameObject.FindGameObjectWithTag("main_light_source").GetComponent<Light>();

		playerShipVariables = playerShip.GetComponent<script_player_controls>();

		Network = new Network(this);
		Trade = new Trade(this);

		//Load all txt database files
		masterCrewList = CSVLoader.LoadMasterCrewRoster();
		captainsLogEntries = CSVLoader.LoadCaptainsLogEntries();
		masterResourceList = CSVLoader.LoadResourceList();
		settlement_masterList = CSVLoader.LoadSettlementList();		// depends on resource list and crew list

		CreateSettlementsFromList();
		currentSettlementGameObject = settlement_masterList_parent.transform.GetChild(0).gameObject;
		currentSettlement = currentSettlementGameObject.GetComponent<script_settlement_functions>().thisSettlement;

		// wind and current init
		BuildWindZoneGameObjects();
		BuildCurrentZoneGameObjects();
		windrose_January = CSVLoader.LoadWindRoses(windZoneColumns, windZoneRows);
		currentRose_January = CSVLoader.LoadWaterZonesFromFile(currentZoneColumns, currentZoneRows);
		SetInGameWindZonesToWindRoseData();
		SetInGameWaterZonesToCurrentRoseData();

		//Load the basic log entries into the log pool
		AddEntriesToCurrentLogPool(0);
		StartPlayerShipAtOriginCity();
		GenerateCityLights();
	}


	//======================================================================================================================================================================
	//======================================================================================================================================================================
	//  THE REMAINDER OF THE SCRIPT IS ALL GLOBALLY ACCESSIBLE FUNCTIONS
	//======================================================================================================================================================================
	//======================================================================================================================================================================
	

	//====================================================================================================
	//      CSV / DATA LOADING FUNCTIONS
	//====================================================================================================


	public bool LoadSavedGame() {
		PlayerJourneyLog loadedJourney = new PlayerJourneyLog();
		Ship ship = playerShipVariables.ship;

		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { ',' };
		char[] recordDelimiter = new char[] { '_' };

		//Look for a save game file and tell the player if none is found.
		string saveText;
		try {
			saveText = File.ReadAllText(Application.persistentDataPath + "/player_save_game.txt");
		}
		catch (Exception error) {
			ShowANotificationMessage("Sorry! No load game 'player_save_game.txt' was found in the game directory '" + Application.persistentDataPath + "' or the save file is corrupt!\nError Code: " + error);
			return false;
		}
		//	TextAsset saveGame = (TextAsset)Resources.Load("player_save_game", typeof(TextAsset));
		string[] fileByLine = saveText.Split(splitFile, StringSplitOptions.None);
		Debug.Log("file://" + Application.persistentDataPath + "/player_save_game.txt");
		Debug.Log(saveText);

		if (fileByLine.Length == 0) return false;
		//start at index 1 to skip the record headers we have to then subtract 
		//one when adding NEW entries to the list to ensure we start at ZERO and not ONE
		//all past routes will be stored as text, but the last route(last line of file) will also be done this way, but will additionally be parsed out for editing in-game values
		for (int lineCount = 1; lineCount < fileByLine.Length; lineCount++) {
			string[] records = fileByLine[lineCount].Split(lineDelimiter, StringSplitOptions.None);

			//First Add the basic route
			Vector3 origin = new Vector3(float.Parse(records[2]), float.Parse(records[3]), float.Parse(records[4]));
			Vector3 destination = new Vector3(float.Parse(records[5]), float.Parse(records[6]), float.Parse(records[7]));
			float numOfDays = float.Parse(records[1]);

			loadedJourney.routeLog.Add(new PlayerRoute(origin, destination, numOfDays));

			//Next add the cargo manifest
			string CSVcargo = "";
			for (int i = 8; i < 23; i++) {
				CSVcargo += "," + records[i];
			}
			loadedJourney.cargoLog.Add(CSVcargo);

			//Next add the other attributes string
			// KDTODO: This needs to update whenever we add a new field right now. Needs a rewrite.
			string CSVotherAtt = "";
			for (int i = 23; i < 42; i++) {
				CSVotherAtt += "," + records[i];
			}
			loadedJourney.otherAttributes.Add(CSVotherAtt);

			//Update Ship Position
			string[] XYZ = records[27].Split(recordDelimiter, StringSplitOptions.None);
			loadedJourney.routeLog[loadedJourney.routeLog.Count - 1].UnityXYZEndPoint = new Vector3(float.Parse(XYZ[0]), float.Parse(XYZ[1]), float.Parse(XYZ[2]));

		}
		playerShipVariables.journey = loadedJourney;

		//Now use the last line of data to update the current player status and load the game
		string[] playerVars = fileByLine[fileByLine.Length - 1].Split(lineDelimiter, StringSplitOptions.None);

		//Update in game Time
		ship.totalNumOfDaysTraveled = float.Parse(playerVars[1]);
		//Update Sky to match time
		playerShipVariables.UpdateDayNightCycle(IS_NOT_NEW_GAME);

		//Update all Cargo Holds
		int fileStartIndex = 8;
		foreach (Resource resource in ship.cargo) {
			resource.amount_kg = float.Parse(playerVars[fileStartIndex]);
			fileStartIndex++;
		}

		//Update all Crewmen
		List<CrewMember> updatedCrew = new List<CrewMember>();
		string[] parsedCrew = playerVars[26].Split(recordDelimiter, StringSplitOptions.None);
		foreach (string crewID in parsedCrew) {
			updatedCrew.Add(GetCrewMemberFromID(int.Parse(crewID)));
		}
		ship.crewRoster.Clear();
		updatedCrew.ForEach(c => ship.crewRoster.Add(c));

		//Update Ship Position
		string[] parsedXYZ = playerVars[27].Split(recordDelimiter, StringSplitOptions.None);
		playerShip.transform.position = new Vector3(float.Parse(parsedXYZ[0]), float.Parse(parsedXYZ[1]), float.Parse(parsedXYZ[2]));

		//Update Current Quest Leg
		ship.mainQuest.currentQuestSegment = int.Parse(playerVars[28]);

		//Update Ship Health
		ship.health = float.Parse(playerVars[29]);

		//Update player clout
		ship.playerClout = float.Parse(playerVars[30]);

		//Update player networks
		List<int> loadedNetworks = new List<int>();
		string[] parsedNetworks = playerVars[31].Split(recordDelimiter, StringSplitOptions.None);
		foreach (string netID in parsedNetworks) {
			loadedNetworks.Add(int.Parse(netID));
		}
		ship.networks = loadedNetworks;

		//Update player starving and thirsty day counters
		playerShipVariables.dayCounterStarving = int.Parse(playerVars[32]);
		playerShipVariables.dayCounterThirsty = int.Parse(playerVars[33]);

		//Update Currency
		ship.currency = int.Parse(playerVars[34]);

		//Add any Loans
		//--If Loan exists then add otherwise make null
		if (int.Parse(playerVars[35]) != -1) {
			//TODO right now we aren't storing the loan variable properly so relaly a loaded game means a player can cheat currently--whoops--and have plenty of time to pay it back and their interest disappears. Need to put on fix list
			ship.currentLoan = new Loan(int.Parse(playerVars[35]), 0f, 0f, int.Parse(playerVars[36]));
		}
		else {
			ship.currentLoan = null;
		}

		//Add Current Navigator Destination
		int targetID = int.Parse(playerVars[37]);
		if (targetID != -1) {
			ship.currentNavigatorTarget = targetID;
			//change location of beacon
			Vector3 location = Vector3.zero;
			for (int x = 0; x < settlement_masterList_parent.transform.childCount; x++)
				if (settlement_masterList_parent.transform.GetChild(x).GetComponent<script_settlement_functions>().thisSettlement.settlementID == targetID)
					location = settlement_masterList_parent.transform.GetChild(x).position;
			MoveNavigatorBeacon(navigatorBeacon, location);
		}
		else {
			ship.currentNavigatorTarget = -1;
		}
		//Add the Known Settlements

		string[] parsedKnowns = playerVars[38].Split(recordDelimiter, StringSplitOptions.None);
		//Debug.Log ("PARSED KNOWNS: " + playerVars[38]);
		foreach (string settlementID in parsedKnowns) {
			//Debug.Log ("PARSED KNOWNS: " + settlementID);
			ship.playerJournal.knownSettlements.Add(int.Parse(settlementID));
		}
		//Add Captains Log
		string restoreCommasAndNewLines = playerVars[39].Replace('^', ',');
		currentCaptainsLog = restoreCommasAndNewLines.Replace('*', '\n');
		//Debug.Log (currentCaptainsLog);

		// KDTODO: This needs to be done every time we add a new field right now. Needs a rewrite.
		ship.upgradeLevel = int.Parse(playerVars[40]);
		ship.crewCapacity = int.Parse(playerVars[41]);
		ship.cargo_capicity_kg = int.Parse(playerVars[42]);
		SetShipModel(ship.upgradeLevel);

		// KDTODO: Once the save game routines are rewritten, need to save the crew available in each city instead of regenerating since this is exploitable
		// it's just too much hassle to support saving this right now because the save format is limiting
		// setup each city with 5 crew available and for now, they never regenerate.
		foreach (var settlement in settlement_masterList) {
			settlement.availableCrew.Clear();
			GenerateRandomCrewMembers(5).ForEach(c => settlement.availableCrew.Add(c));
		}

		//If no errors then return true
		return true;
	}

	public void MoveNavigatorBeacon(GameObject beacon, Vector3 location) {
		beacon.transform.position = location;
		beacon.GetComponent<LineRenderer>().SetPosition(0, new Vector3(location.x, 0, location.z));
		beacon.GetComponent<LineRenderer>().SetPosition(1, location + new Vector3(0, 400, 0));
		playerShipVariables.UpdateNavigatorBeaconAppearenceBasedOnDistance(beacon);
	}

	public void RotateCameraTowards(Vector3 target) {

		// rotate the camera's parent to look at the target (which eliminates the need for RotateAround below)
		// also rotate the 
		//FPVCamera.transform.parent.parent.parent.DOLookAt(target, 1f);

		CameraLookTarget = target;

	}

	public Vector3? CameraLookTarget;

	void UpdateCameraRotation() {

		if(CameraLookTarget.HasValue) {

			var camToTarget = CameraLookTarget.Value - FPVCamera.transform.parent.parent.transform.position;
			var angle = Vector3.SignedAngle(FPVCamera.transform.parent.parent.forward, camToTarget.normalized, Vector3.up);

			FPVCamera.transform.parent.parent.RotateAround(FPVCamera.transform.parent.parent.position, FPVCamera.transform.parent.parent.up, angle * Time.deltaTime);

			if(Mathf.Abs(angle) < 2f) {
				CameraLookTarget = null;
			}

		}

	}

	private void Update() {
		UpdateCameraRotation();
		DebugHotkeys();
	}

	void DebugHotkeys() {
#if UNITY_EDITOR
		if(Input.GetKeyUp(KeyCode.E)) {
			RandomEvents.StormAtSea(playerShipVariables.ship, Globals.GameVars, playerShip.transform, 1, 0);
		}
#endif
	}

	//====================================================================================================
	//      GAMEOBJECT BUILDING TO POPULATE WORLD FUNCTIONS
	//====================================================================================================

	public void CreateSettlementsFromList() {
		settlement_masterList_parent = Instantiate(new GameObject(), Vector3.zero, transform.rotation) as GameObject;
		settlement_masterList_parent.name = "Settlement Master List";
		foreach (Settlement settlement in settlement_masterList) {
			GameObject currentSettlement;
			//Here we add a model/prefab to the settlement based on it's
			try {
				//Debug.Log ("BEFORE TRYING TO LOAD SETTLEMENT PREFAB    " + settlement.prefabName + "  :   " + settlement.name);
				currentSettlement = Instantiate(Resources.Load("City Models/" + settlement.prefabName, typeof(GameObject))) as GameObject;
				//Debug.Log ("AFTER TRYING TO LOAD SETTLEMENT PREFAB    " + settlement.prefabName);
			}
			catch {
				currentSettlement = Instantiate(Resources.Load("City Models/PF_settlement", typeof(GameObject))) as GameObject;
			}
			//We need to check if the settlement has an adjusted position or not--if it does then use it, otherwise use the given lat long coordinate
			if (settlement.adjustedGamePosition.x == 0) {
				Vector2 tempXY = CoordinateUtil.Convert_WebMercator_UnityWorld(CoordinateUtil.ConvertWGS1984ToWebMercator(settlement.location_longXlatY));
				Vector3 tempPos = new Vector3(tempXY.x, terrain.GetComponent<Terrain>().SampleHeight(new Vector3(tempXY.x, 0, tempXY.y)), tempXY.y);
				currentSettlement.transform.position = tempPos;
			}
			else {
				currentSettlement.transform.position = settlement.adjustedGamePosition;
				currentSettlement.transform.eulerAngles = new Vector3(0, settlement.eulerY, 0);
			}
			currentSettlement.tag = "settlement";
			currentSettlement.name = settlement.name;
			currentSettlement.layer = 8;
			//Debug.Log ("*********************************************  <<>>>" + currentSettlement.name + "   :   " + settlement.settlementID);
			currentSettlement.GetComponent<script_settlement_functions>().thisSettlement = settlement;
			currentSettlement.transform.SetParent(settlement_masterList_parent.transform);
			settlement.theGameObject = currentSettlement;
		}
	}

	public void BuildWindZoneGameObjects() {
		//We need to create a gridded system of GameObjects to represent the windzones
		//It should be a Main Parent GameObject with a series of zones with a rotater and particle system
		//	--WindZones
		//		--0_0
		//			--Particle Rotater
		//				--Wind particle system
		windZoneParent = new GameObject();
		windZoneParent.name = "WindZones Parent Object";
		float originX = 0;
		float originZ = 4096; //Unity's 2D top-down Y axis is Z
		float zoneHeight = 128;
		float zoneWidth = 64;

		for (int col = 0; col < windZoneColumns; col++) {
			for (int row = 0; row < windZoneRows; row++) {
				GameObject newZone = new GameObject();
				GameObject rotater = new GameObject();
				GameObject windParticles;// = Instantiate(new GameObject(), Vector3.zero, transform.rotation) as GameObject;
				newZone.transform.position = new Vector3(originX + (col * zoneWidth), 0, originZ - (row * zoneHeight));
				newZone.transform.localScale = new Vector3(zoneWidth, 1f, zoneHeight);
				newZone.name = col + "_" + row;
				newZone.tag = "windDirectionVector";
				newZone.AddComponent<BoxCollider>();
				newZone.GetComponent<BoxCollider>().isTrigger = true;
				newZone.GetComponent<BoxCollider>().size = new Vector3(.95f, 10, .95f);
				newZone.layer = 20;
				rotater.AddComponent<script_WaterWindCurrentVector>();
				rotater.transform.position = newZone.transform.position;
				rotater.transform.rotation = newZone.transform.rotation;
				rotater.name = "Particle Rotater";
				windParticles = Instantiate(Resources.Load("PF_windParticles", typeof(GameObject))) as GameObject;
				windParticles.transform.position = new Vector3(newZone.transform.position.x, newZone.transform.position.y, newZone.transform.position.z - (zoneHeight / 2));

				windParticles.transform.parent = rotater.transform;
				rotater.transform.parent = newZone.transform;
				newZone.transform.parent = windZoneParent.transform;
				rotater.SetActive(false);
			}
		}
	}

	public void BuildCurrentZoneGameObjects() {
		//We need to create a gridded system of GameObjects to represent the windzones
		//It should be a Main Parent GameObject with a series of zones with a rotater and particle system
		//	--WindZones
		//		--0_0
		//			--Particle Rotater
		//				--Wind particle system
		currentZoneParent = new GameObject();
		currentZoneParent.name = "CurrentZones Parent Object";
		float originX = 0;
		float originZ = 4096; //Unity's 2D top-down Y axis is Z
		float zoneHeight = 64;
		float zoneWidth = 32;

		for (int col = 0; col < currentZoneColumns; col++) {
			for (int row = 0; row < currentZoneRows; row++) {
				GameObject newZone = new GameObject();
				GameObject rotater = new GameObject();
				GameObject currentParticles;// = Instantiate(new GameObject(), Vector3.zero, transform.rotation) as GameObject;
				newZone.transform.position = new Vector3(originX + (col * zoneWidth), 0, originZ - (row * zoneHeight));
				newZone.transform.localScale = new Vector3(zoneWidth, 1f, zoneHeight);
				newZone.name = col + "_" + row;
				newZone.tag = "currentDirectionVector";
				newZone.AddComponent<BoxCollider>();
				newZone.GetComponent<BoxCollider>().isTrigger = true;
				newZone.GetComponent<BoxCollider>().size = new Vector3(.95f, 10, .95f);
				newZone.layer = 19;
				rotater.AddComponent<script_WaterWindCurrentVector>();
				rotater.transform.position = newZone.transform.position;
				rotater.transform.rotation = newZone.transform.rotation;
				rotater.name = "Particle Rotater";
				currentParticles = Instantiate(Resources.Load("PF_currentParticles", typeof(GameObject))) as GameObject;
				currentParticles.transform.position = new Vector3(newZone.transform.position.x, newZone.transform.position.y, newZone.transform.position.z - (zoneHeight / 2));
				currentParticles.transform.Translate(-transform.forward * .51f, Space.Self);
				currentParticles.transform.parent = rotater.transform;
				rotater.transform.parent = newZone.transform;
				newZone.transform.parent = currentZoneParent.transform;
				rotater.SetActive(false);

			}
		}
	}

	public void GenerateCityLights() {
		for (int i = 0; i < settlement_masterList_parent.transform.childCount; i++) {
			GameObject currentCityLight = Instantiate(Resources.Load("PF_cityLights", typeof(GameObject))) as GameObject;
			currentCityLight.transform.position = settlement_masterList_parent.transform.GetChild(i).position;
			currentCityLight.transform.parent = cityLightsParent.transform;
		}
	}

	public void SetInGameWindZonesToWindRoseData() {

		//For each of the zones in the Wind Zone parent GameObject, we need to loop through them
		//	--and set the rotation of each to match the windrose data
		for (int currentZone = 0; currentZone < windZoneParent.transform.childCount; currentZone++) {
			string zoneID = windZoneParent.transform.GetChild(currentZone).name;
			//Debug.Log(zoneID);
			int col = int.Parse(zoneID.Split('_')[0]);
			int row = int.Parse(zoneID.Split('_')[1]);

			//Find the matching wind rose in the month of january
			float speed = 1;
			float direction = UnityEngine.Random.Range(0f, 90f);
			if (windrose_January[col, row] != null) {
				speed = windrose_January[col, row].speed;
				direction = windrose_January[col, row].direction;
			}
			windZoneParent.transform.GetChild(currentZone).GetChild(0).transform.eulerAngles = new Vector3(0, -1f * (direction - 90f), 0); //We subtract 90 because Unity's 'zero' is set at 90 degrees and Unity's positive angle is CW and not CCW like normal trig
			windZoneParent.transform.GetChild(currentZone).GetChild(0).GetComponent<script_WaterWindCurrentVector>().currentMagnitude = speed;
			//if (speed == 0) windZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(false);
			//else windZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(true);
		}

	}

	public void SetInGameWaterZonesToCurrentRoseData() {

		//For each of the zones in the Wind Zone parent GameObject, we need to loop through them
		//	--and set the rotation of each to match the windrose data
		for (int currentZone = 0; currentZone < currentZoneParent.transform.childCount; currentZone++) {
			string zoneID = currentZoneParent.transform.GetChild(currentZone).name;
			//Debug.Log(zoneID);
			int col = int.Parse(zoneID.Split('_')[0]);
			int row = int.Parse(zoneID.Split('_')[1]);

			//Find the matching current rose in the month of january
			float speed = 1;
			float direction = UnityEngine.Random.Range(0f, 90f);
			if (currentRose_January[col, row] != null) {
				speed = currentRose_January[col, row].speed;
				direction = currentRose_January[col, row].direction;
			}
			currentZoneParent.transform.GetChild(currentZone).GetChild(0).transform.eulerAngles = new Vector3(0, -1f * (direction - 90f), 0); //We subtract 90 because Unity's 'zero' is set at 90 degrees and Unity's positive angle is CW and not CCW like normal trig
			currentZoneParent.transform.GetChild(currentZone).GetChild(0).GetComponent<script_WaterWindCurrentVector>().currentMagnitude = speed;
			//if (speed == 0) currentZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(false);
			//else currentZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(true);
			//Debug.Log ("Turning water on?");
		}

	}




	//====================================================================================================
	//      DATA SAVING FUNCTIONS
	//====================================================================================================
	public void SaveUserGameData(bool isRestart) {
		string delimitedData = playerShipVariables.journey.ConvertJourneyLogToCSVText();
		Debug.Log(delimitedData);
		string filePath = Application.persistentDataPath + "/";

		string fileNameServer = "";
		if (DEBUG_MODE_ON)
			fileNameServer += "DEBUG_DATA_" + SystemInfo.deviceUniqueIdentifier + "_player_data_" + System.DateTime.UtcNow.ToString("HH-mm-ss_dd_MMMM_yyyy") + ".csv";
		else
			fileNameServer += SystemInfo.deviceUniqueIdentifier + "_player_data_" + System.DateTime.UtcNow.ToString("HH-mm-ss_dd_MMMM_yyyy") + ".csv";

		string fileName = "player_save_game.txt";

		//Adding a try/catch block around this write because if someone tries playing the game out of zip on mac--it throws an error that is unoticeable but also
		//causes the code to fall short and quit before saving to the server
		try {
			//save a backup before Joanna's edits
			System.IO.File.WriteAllText(Application.persistentDataPath + "/BACKUP-" + SystemInfo.deviceUniqueIdentifier + "_player_data_" + System.DateTime.UtcNow.ToString("HH-mm-ss_dd_MMMM_yyyy") + ".csv", delimitedData);
			//Only save the game for loading if it's not a restart--otherwise if the player loads, it will load right where the player restarted the game
			if (!isRestart) System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName, delimitedData);
			//TODO Temporary addition for joanna to remove the captains log from the server upload
			string fileToUpload = RemoveCaptainsLogForJoanna(delimitedData);
			System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileNameServer, fileToUpload);
			Debug.Log(Application.persistentDataPath);
		}
		catch (Exception e) {
			showSecondaryNotification = true;
			secondaryNotificationMessage = "ERROR: a backup wasn't saved at: " + Application.persistentDataPath + "  - which means it may not have uploaded either: " + e.Message;
		}
		//Only upload to the server is the DebugMode is OFF
		if (!DEBUG_MODE_ON) SaveUserGameDataToServer(filePath, fileNameServer);

	}

	public void SaveUserGameDataToServer(string localPath, string localFile) {
		Debug.Log("Starting FTP");
		string user = "SamoGameBot";
		string pass = "%Mgn~WxH+CRzj>4Z";
		string host = "34.193.207.222";
		string initialPath = "";

		FileInfo file = new FileInfo(localPath + localFile);
		if (!file.Exists) {
			Debug.LogError("No save file created, so could not save game data to server: " + localFile);
			return;
		}

		Uri address = new Uri("ftp://" + host + "/" + Path.Combine(initialPath, file.Name));
		FtpWebRequest request = FtpWebRequest.Create(address) as FtpWebRequest;

		// Upload options:

		// Provide credentials
		request.Credentials = new NetworkCredential(user, pass);

		// Set control connection to closed after command execution
		request.KeepAlive = false;

		// Specify command to be executed
		request.Method = WebRequestMethods.Ftp.UploadFile;

		// Specify data transfer type
		request.UseBinary = true;

		// Notify server about size of uploaded file
		request.ContentLength = file.Length;

		//Make sure we have a timeout for the connection because the default is Infinite--for instance, if the
		//player is offline, the timeout will never happen if there isn't a value set. We'll set it
		//to 5 seconds(5000ms) as a time
		request.Timeout = 5000;

		// Set buffer size to 2KB.
		var bufferLength = 2048;
		var buffer = new byte[bufferLength];
		var contentLength = 0;

		// Open file stream to read file
		var fs = file.OpenRead();

		try {
			// Stream to which file to be uploaded is written.
			var stream = request.GetRequestStream();

			// Read from file stream 2KB at a time.
			contentLength = fs.Read(buffer, 0, bufferLength);

			// Loop until stream content ends.
			while (contentLength != 0) {
				//Debug.Log("Progress: " + ((fs.Position / fs.Length) * 100f));
				// Write content from file stream to FTP upload stream.
				stream.Write(buffer, 0, contentLength);
				contentLength = fs.Read(buffer, 0, bufferLength);
			}

			// Close file and request streams
			stream.Close();
			fs.Close();
		}
		catch (Exception e) {
			Debug.LogError("Error uploading file: " + e.Message);
			showNotification = true;
			notificationMessage = "ERROR: No Upload--The server timed out or you currently do not have a stable internet connection\n" + e.Message;
			return;
		}

		Debug.Log("Upload successful.");
		showNotification = true;
		notificationMessage = "File: '" + localFile + "' successfully uploaded to the server!";
	}

	//TODO: This is an incredibly specific function that won't be needed later
	public string RemoveCaptainsLogForJoanna(string file) {
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		string newFile = "";
		string[] fileByLine = file.Split(splitFile, StringSplitOptions.None);

		//For each line of the save file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			int index = fileByLine[row].LastIndexOf(",");
			newFile += fileByLine[row].Substring(0, index) + "\n";
			//Debug.Log (fileByLine [row]); 
			//Debug.Log (fileByLine [row].Substring (0, index));
		}

		return newFile;

	}

	// TODO: Apparently this isn't hooked up anymore. Need to fix this tool so we can adjust current directions in the editor
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void SaveWaterCurrentZones() {

		string waterRoseData = "";
		int rowCounter = 0;
		Transform waterZone;

		//Loop through all of the child objects of the current zone parent object
		//The parent stores them in a sequential list so every 40 objects represents a new line in the spread sheet csv file
		//The coordinate for the zones is 0,0 for the top left, ending with 39,39 on the bottom right
		for (int currentZone = 0; currentZone < currentZoneParent.transform.childCount; currentZone++) {
			waterZone = currentZoneParent.transform.GetChild(currentZone);
			waterRoseData += waterZone.GetChild(0).transform.localRotation.eulerAngles.y;
			waterRoseData += ",";
			waterRoseData += waterZone.GetChild(0).GetComponent<script_WaterWindCurrentVector>().currentMagnitude;
			rowCounter++;
			//If we've hit 40 objects, it's time to start a new row in the csv file
			if (rowCounter == 40) {
				rowCounter = 0;
				waterRoseData += "\n";
			}
			//only write a comma if we aren't at the last entry for the row
			else
				waterRoseData += ",";
		}
		//Debug.Log(waterRoseData);
		StreamWriter sw = new StreamWriter(@Application.persistentDataPath + "/" + "waterzones_january.txt");
		sw.Write(waterRoseData);
		sw.Close();


	}

	// TODO: Apparently this isn't hooked up anymore. Need to fix this tool so we can adjust the settlement unity position offsets in the editor
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void Tool_SaveCurrentSettlementPositionsToFile() {
		string ID = "";
		string unityX = "";
		string unityY = "";
		string unityZ = "";
		string unityEulerY = "";
		string writeToFile = "";
		for (int i = 0; i < settlement_masterList_parent.transform.childCount; i++) {
			ID = settlement_masterList_parent.transform.GetChild(i).GetComponent<script_settlement_functions>().thisSettlement.settlementID.ToString();
			//if(ID == "309"){Debug.Log ("We At 309!!!!!!");}
			unityX = settlement_masterList_parent.transform.GetChild(i).transform.position.x.ToString();
			unityY = settlement_masterList_parent.transform.GetChild(i).transform.position.y.ToString();
			unityZ = settlement_masterList_parent.transform.GetChild(i).transform.position.z.ToString();
			unityEulerY = settlement_masterList_parent.transform.GetChild(i).transform.eulerAngles.y.ToString();
			string test = ((ID + "," + unityX + "," + unityY + "," + unityZ + "," + unityEulerY));
			//perform a quick check to make sure we aren't at the end of the file: if we are don't add a new line
			if (i != settlement_masterList_parent.transform.childCount - 1)
				test += "\n";
			writeToFile += test;
		}

		//Write the string to file now
		StreamWriter sw = new StreamWriter(@"H:\sailingwiththegods\Assets\Resources\settlement_unity_position_offsets.txt");
		sw.Write(writeToFile);
		sw.Close();
	}

	//====================================================================================================
	//      PLAYER INITIALIZATION FUNCTIONS
	//====================================================================================================

	void StartPlayerShipAtOriginCity() {
		//first set the origin city to the first available as a default
		GameObject originCity = settlement_masterList_parent.transform.GetChild(0).gameObject;
		foreach (Transform child in settlement_masterList_parent.transform) {
			//if the settlement we want exists, then use it as the default instead
			if (child.name == "Samothrace") {
				originCity = child.gameObject;
				break;
			}
		}
		//now set the player ship to the origin city coordinate
		//!TODO This is arbotrarily set to samothrace right now
		playerShip.transform.position = new Vector3(1939.846f, .23f, 2313.506f);
		//mainCamera.transform.position = new Vector3(originCity.transform.position.x, 30f, originCity.transform.position.z);
	}

	public void RestartGame() {

		//Debug.Log ("Quest Seg: " + playerShipVariables.ship.mainQuest.currentQuestSegment);
		//First we need to save the game that just ended
		SaveUserGameData(true);
		//Then we need to re-initialize all the player's variables
		playerShipVariables.Reset();

		//Reset Other Player Ship Variables
		playerShipVariables.numOfDaysTraveled = 0;
		playerShipVariables.numOfDaysWithoutProvisions = 0;
		playerShipVariables.numOfDaysWithoutWater = 0;
		playerShipVariables.dayCounterStarving = 0;
		playerShipVariables.dayCounterThirsty = 0;

		//Take player back to title screen
		//Debug.Log ("GOING TO TITLE SCREEN");
		Globals.UI.HideAll();
		Globals.UI.Show<TitleScreen, GameViewModel>(new GameViewModel());
		controlsLocked = true;
		isTitleScreen = true;
		RenderSettings.fog = false;
		FPVCamera.SetActive(false);
		camera_titleScreen.SetActive(true);
		isTitleScreen = true;
		runningMainGameGUI = false;


		//clear captains log
		currentCaptainsLog = "";

		GUI_PortMenu.SetActive(false);
		GUI_GameHUD.SetActive(false);



	}

	public void InitiateMainQuestLineForPlayer() {
		Debug.Log("Main Quest TESTER");
		//For the argonautica, let's set the crew capacity to 30
		playerShipVariables.ship.crewCapacity = Ship.StartingCrewCap;

		//Now let's add all the initial crew from the start screen selection and start the first leg of the quest
		for (int i = 0; i < newGameAvailableCrew.Count; i++) {
			if (newGameCrewSelectList[i]) {
				playerShipVariables.ship.crewRoster.Add(newGameAvailableCrew[i]);
				//Debug.Log (newGameCrewSelectList[i]);	
			}
		}
		//Debug.Log (playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd.Count + "<<<<<<<<<<<<<CREW");
		//	foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd){
		//		playerShipVariables.ship.crewRoster.Add (GetCrewMemberFromID(crewID));
		//	}

		//Let's increase the ships cargo capacity
		playerShipVariables.ship.cargo_capicity_kg = Ship.StartingCargoCap;

		//Let's increase the ships Provisions and water base to reflect starting crew members
		playerShipVariables.ship.cargo[0].amount_kg = Ship.StartingWater;
		playerShipVariables.ship.cargo[1].amount_kg = Ship.StartingFood;

		//Increase the quest counter because the start screen takes care of the first leg

		Debug.Log(playerShipVariables.ship.mainQuest.currentQuestSegment);
		playerShipVariables.ship.mainQuest.currentQuestSegment++;

		//first show a window for the welcome message, and if there are any crew member changes, then let the player know.
		notificationMessage = playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionAtCompletion;
		showNotification = true;
		//Add this message to the captain's log
		playerShipVariables.ship.shipCaptainsLog.Add(new CaptainsLogEntry(playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID, playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionOfQuest));
		playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
		currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
		//Now add the mentioned places attached to this quest leg
		foreach (int i in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].mentionedPlaces) {
			//Make sure we don't add any null values--a -1 represents no mentions of any settlements
			if (i != -1)
				playerShipVariables.ship.playerJournal.AddNewSettlementToLog(i);
		}
		Debug.Log(playerShipVariables.ship.mainQuest.currentQuestSegment);
		//Then increment the questline to the in succession and update the player captains log with the new information for the next quest line
		playerShipVariables.ship.mainQuest.currentQuestSegment++;
		playerShipVariables.ship.shipCaptainsLog.Add(new CaptainsLogEntry(playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID, playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionOfQuest));
		playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
		currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
		//Now add the mentioned places attached to this quest leg
		foreach (int i in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].mentionedPlaces) {
			//Make sure we don't add any null values--a -1 represents no mentions of any settlements
			if (i != -1)
				playerShipVariables.ship.playerJournal.AddNewSettlementToLog(i);
		}
		//Now add the city name of the next journey quest to the players known settlements
		playerShipVariables.ship.playerJournal.AddNewSettlementToLog(playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID);
		//Now teleport the player ship to an appropriate location near the first target
		playerShip.transform.position = new Vector3(1702.414f, playerShip.transform.position.y, 2168.358f);
		//Set the player's initial position to the new position
		playerShipVariables.lastPlayerShipPosition = playerShip.transform.position;

		//Setup Difficulty Level
		SetupBeginnerGameDifficulty();

		// setup each city with 5 crew available and for now, they never regenerate.
		foreach(var settlement in settlement_masterList) {
			settlement.availableCrew.Clear();
			GenerateRandomCrewMembers(5).ForEach(c => settlement.availableCrew.Add(c));
		}

		Debug.Log(playerShipVariables.ship.mainQuest.currentQuestSegment);

		//Flag the main GUI scripts to turn on
		runningMainGameGUI = true;
	}

	public void UpgradeShip(int costToBuyUpgrade) {
		playerShipVariables.ship.upgradeLevel = 1;
		playerShipVariables.ship.currency -= costToBuyUpgrade;

		// TODO: These should be defined per uprade level, but until we have a better idea how upgrades will work long term, just hard here
		playerShipVariables.ship.crewCapacity = 30;
		playerShipVariables.ship.cargo_capicity_kg = 1200;

		// add all the non-fireable story crew members now that you have your big boy ship
		FillBeginStoryCrew();

		Globals.UI.Hide<RepairsView>();
		Globals.UI.Show<InfoScreen, InfoScreenModel>(new InfoScreenModel {
			Title = "Welcome to the Argonautica!",
			Message = "Find your way through the dangerous seas to complete your quest! You have found yourself at Pagasae, where King Pelias has given you the task of sailing across " +
					"the Aegean and the Black Sea to retrieve the Golden Fleece. This is the hide of the flying ram that brought Phrixus from Boetia to Aea. The hide now hangs on a tree on the other side of the Black" +
					" Sea in the city of Aea. The great lord Aeetes prizes the fleece, and a very large dragon guards it.\n\n" +
					"The task seems impossible!But you do not sail alone, Jason.You have assembled a group of the most powerful warriors, sailors, and prophets in Greece to help you in your quest. " +
					"Most are the sons of royal families, and each one has a unique skill.Once the heroes have all arrived, your crew stocks the ships and the people of Pagasae greet you all."
		});

		// show the new ship model
		SetShipModel(playerShipVariables.ship.upgradeLevel);
	}

	void SetShipModel(int shipLevel) {
		foreach (var upgradeLevel in shipLevels) {
			upgradeLevel.SetActive(false);
		}
		shipLevels[shipLevel].SetActive(true);
	}

	// when you purchase the trireme, we add all the MUST have story crew members. 
	public void FillBeginStoryCrew() {

		foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd) {
			CrewMember currentMember = GetCrewMemberFromID(crewID);
			if (!currentMember.isKillable && !currentMember.isJason) {
				playerShipVariables.ship.crewRoster.Add(currentMember);
			}
		}

	}

	public void FillNewGameCrewRosterAvailability() {
		//We need to fill a list of 40 crewmembers for the player to choose from on a new game start
		//--The first set will come from the Argonautica, and the top of the list will be populated with necessary characters for the plot
		//--The remainder will be filled from the remaining available argonautica start crew and then randomly generated crew to choose from to create 40 members

		//initialize a fresh List of crew and corresponding array of 40 bools
		newGameAvailableCrew = new List<CrewMember>();
		newGameCrewSelectList = new bool[40];

		//TODO FIX THIS LATER Let's remove the randomly generated crew--this is just a safety precaution--might not be needed.
		playerShipVariables.ship.crewRoster.Clear();

		//Let's add all the optional crew from the Argonautica
		foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd) {
			CrewMember currentMember = GetCrewMemberFromID(crewID);
			if (currentMember.isKillable && !currentMember.isJason) {
				newGameAvailableCrew.Add(currentMember);
			}
		}

		//Now let's add all the possible non-quest historical people for hire
		foreach (CrewMember thisMember in masterCrewList) {
			//make sure we don't go over 40 listings
			if (newGameAvailableCrew.Count == 40)
				break;

			if (!thisMember.isPartOfMainQuest) {
				newGameAvailableCrew.Add(thisMember);
			}
		}


		//Now let's add randomly generated crew to the list until the quota of 40 is fulfilled
		while (newGameAvailableCrew.Count < 40) {
			newGameAvailableCrew.Add(GenerateRandomCrewMembers(1)[0]);
		}

		// filter out people who don't have connections at the ports in your starting bay or have overwhelmingly large networks
		// prefer random people with small networks over argonautica crew who have very large networks. you should have to hire these people later
		var nearestToStart = new string[] { "Pagasae", "Iolcus", "Pherai (Thessaly)", "Phylace", "Tisaia", "Histiaia/Oreos" };
		var bestOptions = from c in newGameAvailableCrew
						  let network = Network.GetCrewMemberNetwork(c)
						  where network.Any(s => nearestToStart.Contains(s.name)) && network.Count() < 10
						  select c;

		// use people with low # connections as backup options. this is just to keep the early game from being confusing
		var backupOptions = from c in newGameAvailableCrew
							let network = Network.GetCrewMemberNetwork(c)
							where network.Count() < 10
							select c;

		var remainingNeeded = Ship.StartingCrewSize - bestOptions.Count();
		if(remainingNeeded > 0) {
			newGameAvailableCrew = bestOptions.Concat(backupOptions.Take(remainingNeeded)).ToList();
		}
		else {
			newGameAvailableCrew = bestOptions.ToList();
		}

	}

	public List<CrewMember> GenerateRandomCrewMembers(int numberOfCrewmanNeeded) {
		//This function pulls from the list of available crewmembers in the world and selects random crewman from that list of a defined
		//	--size that isn't already on board the ship and returns it. This may not return a full list if the requested number is too high--it will return
		//	--the most it has available
		List<CrewMember> availableCrew = new List<CrewMember>();
		int numOfIterations = 0;
		while (numberOfCrewmanNeeded != availableCrew.Count) {
			CrewMember thisMember = masterCrewList[UnityEngine.Random.Range(0, masterCrewList.Count)];
			if (!thisMember.isPartOfMainQuest) {
				//Now make sure this crewmember isn't already in the current crew
				if(!playerShipVariables.ship.crewRoster.Contains(thisMember)) {
					availableCrew.Add(thisMember);
				}
			}
			//Break from the main loop if we've tried enough crewman
			if (masterCrewList.Count == numOfIterations)
				break;
			numOfIterations++;
		}

		//Return the final List of crewman--it might not be the full amount requested if there aren't enough to pull form
		return availableCrew;

	}

	public void SetupBeginnerGameDifficulty() {
		//Set difficulty level variables
		if (gameDifficulty_Beginner)
			camera_Mapview.GetComponent<Camera>().enabled = true;
		else
			camera_Mapview.GetComponent<Camera>().enabled = false;
	}

	public void LoadSavedGhostRoute() {
		//For the loadgame function--it just fills the ghost trail with the routes that exist
		playerGhostRoute.GetComponent<LineRenderer>().positionCount = playerShipVariables.journey.routeLog.Count;
		for (int routeIndex = 0; routeIndex < playerShipVariables.journey.routeLog.Count; routeIndex++) {
			Debug.Log("GhostRoute Index: " + routeIndex);
			playerGhostRoute.GetComponent<LineRenderer>().SetPosition(routeIndex, playerShipVariables.journey.routeLog[routeIndex].UnityXYZEndPoint - new Vector3(0, playerShip.transform.position.y, 0));
			//set player last origin point for next route add on
			if (routeIndex == playerShipVariables.journey.routeLog.Count - 1) {
				playerShipVariables.travel_lastOrigin = playerShipVariables.journey.routeLog[routeIndex].UnityXYZEndPoint - new Vector3(0, playerShip.transform.position.y);
				playerShipVariables.originOfTrip = playerShipVariables.journey.routeLog[routeIndex].UnityXYZEndPoint - new Vector3(0, playerShip.transform.position.y);
			}
		}
	}

	//====================================================================================================
	//    LOOKUP / DICTIONARY FUNCTIONS
	//====================================================================================================   

	public string GetJobClassEquivalency(CrewType jobCode) {
		//This function simply returns a predefined string based on the number code of a crewmembers job
		//	--based on their constant values held in the global variables 
		string title = "";
		switch (jobCode) {
			case CrewType.Sailor: title = "Sailor"; break;
			case CrewType.Warrior: title = "Warrior"; break;
			case CrewType.Slave: title = "Slave"; break;
			case CrewType.Passenger: title = "Passenger"; break;
			case CrewType.Navigator: title = "Navigator"; break;
			case CrewType.Assistant: title = "Assistant"; break;
			case CrewType.Guide: title = "Guide"; break;
			case CrewType.Lawyer: title = "Lawyer"; break;
			case CrewType.Royalty: title = "Royalty"; break;
			case CrewType.Seer: title = "Seer"; break;
		}
		return title;
	}



	public string GetCloutTitleEquivalency(int clout) {
		//This function simply returns a predefined string based on the number value of the clout provided
		string title = "";
		if (clout > 1 && clout <= 499) title = "Goatherd";
		else if (clout > 500 && clout <= 999) title = "Farmer";
		else if (clout > 1000 && clout <= 1499) title = "Merchant";
		else if (clout > 1500 && clout <= 1999) title = "Mercenary";
		else if (clout > 2000 && clout <= 2499) title = "Knight";
		else if (clout > 2500 && clout <= 2999) title = "War Chief";
		else if (clout > 3000 && clout <= 3499) title = "Boule Leader";
		else if (clout > 3500 && clout <= 3999) title = "Ambassador";
		else if (clout > 4000 && clout <= 4499) title = "Prince";
		else if (clout > 4500 && clout <= 4999) title = "King";
		else if (clout >= 5000) title = "The God";
		else title = "ERROR: clout is not between 0 and 100";
		return title;
	}

	public Settlement GetSettlementFromID(int ID) {
		//Debug.Log (settlement_masterList.Length);
		foreach (Settlement city in settlement_masterList) {
			//Debug.Log ("DEBUG: city: " + city.name);
			if (city.settlementID == ID) {
				return city;
			}
		}
		//if no matches(this shouldn't be possible--return a fake settlement rather than a null
		//	--this is more sophisticated than a null--it won't crash but the error is obvious.
		Debug.Log("ERROR: DIDNT FIND ID MATCH IN GetSettlementFromID Function: Looking for settlement ID:  " + ID);
		return new Settlement(-1, "ERROR", -1);
	}

	public CrewMember GetCrewMemberFromID(int ID) {
		foreach (CrewMember crewman in masterCrewList) {
			if (crewman.ID == ID)
				return crewman;
		}
		return null;
	}



	//====================================================================================================
	//    QUEST FUNCTIONS
	//====================================================================================================   

	public void CheckIfCurrentSettlementIsPartOfMainQuest(int settlementID) {

		//We need to cylcle through each quest destination and see if this current area matches one of the destinations. Preferably the players should go in order--but we are designing it to allow players to
		//skip ahead and in theory--go directly to the end destination. One of the issues is the removal and addition of crewmembers along the way that are important for the plot. Because the questline is a series
		//of stock messages for each destination, parsing out the narrative that talks about non-existent crewmen is difficult. For now--the narrative will remain unchanged, but hercules might never actually leave
		//the ship if they don't stop at a specific destination where he leaves. Some narratives might discuss sailors that aren't actually on the ship. Additionally--the endpoint of the questline is the origin so the questline needs to be
		//split into 2 parts--the first ends at aea and the player can sell straight there, but must go there in order to return back to Pagasse and can return there directly or follow the questline back there.
		//ALSO--once the player reachees a certain point in the questline, the player can't return to older points in the quest so the beginning id should start at the current quest segment

		//create a bool that can be accessed by this function's loops etc.
		bool matchFound = false;

		//First determine if the player has finished the entire questline or yet. We'll use the Count without a -1 to make sure the incremented quest leg is higher thant he last available leg
		if (playerShipVariables.ship.mainQuest.currentQuestSegment < playerShipVariables.ship.mainQuest.questSegments.Count) {
			int currentQuestEnd;
			int aeaID = 15;

			//First we determine which part of the questleg the player is in which determines which part of the quest array the player can access
			//If the player is in the first half before Aea--then only search for these quest segments, else only search for the last quest segments
			if (playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].segmentID < aeaID) {
				//currentQuestBeginning = 0;
				currentQuestEnd = aeaID;
			}
			else {
				//currentQuestBeginning = aeaID;
				//set the end destination to the last position of the array
				currentQuestEnd = playerShipVariables.ship.mainQuest.questSegments.Count - 1;
			}


			//we add a +1 to the current questline so that the player can't continuously perform the quest at the same stop
			for (int index = playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].segmentID; index <= currentQuestEnd; index++) {
				QuestSegment thisQuest = playerShipVariables.ship.mainQuest.questSegments[index];
				Debug.Log(settlementID + "   : =? :   " + thisQuest.destinationID);
				//If the current settlement matches the id of any target in the quest line, increment the quest line to that point--preferably we want it to be the next one in sequence--but we're expanding player behavioral choices.
				if (settlementID == thisQuest.destinationID) {
					//If there is a match, the player has moved on to a new leg of the quest line
					//first show a window for the completion message, and if there are any crew member changes, then let the player know.
					string questMessageIntro = "The Argonautica Quest: ";
					notificationMessage = questMessageIntro + thisQuest.descriptionAtCompletion;
					showNotification = true;

					//add the arrival message to Captain's log
					playerShipVariables.ship.shipCaptainsLog.Add(new CaptainsLogEntry(thisQuest.destinationID, questMessageIntro + thisQuest.descriptionAtCompletion));
					playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
					currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;

					//Remove any crew members if the questline calls for it
					foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].crewmembersToRemove) {
						Debug.Log("CREW ID REMOVING: " + crewID);
						//Make sure the crew ID values are not -1(a null value which means no changes)
						if (crewID != -1)
							//foreach (CrewMember currentMember in playerShipVariables.ship.crewRoster){
							//	Debug.Log ("Checking =  : " + currentMember.ID + " : " + crewID);
							//	if (currentMember.ID == crewID)
							playerShipVariables.ship.crewRoster.Remove(GetCrewMemberFromID(crewID));
						//}
					}

					//Add any new crew members if the questline calls for it
					foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].crewmembersToAdd) {
						//Make sure the crew ID values are not -1(a null value which means no changes)
						if (crewID != -1)
							playerShipVariables.ship.crewRoster.Add(GetCrewMemberFromID(crewID));
					}

					//Then increment the questline to the in succession and update the player captains log with the new information for the next quest line
					playerShipVariables.ship.mainQuest.currentQuestSegment = thisQuest.segmentID + 1;
					playerShipVariables.ship.shipCaptainsLog.Add(new CaptainsLogEntry(playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID, questMessageIntro + playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionOfQuest));
					playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
					currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;


					//Now add the city name of the next journey quest to the players known settlements
					playerShipVariables.ship.playerJournal.AddNewSettlementToLog(playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID);
					Debug.Log("next seg: " + playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID);
					//Now add the mentioned places attached to this quest leg
					foreach (int i in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].mentionedPlaces) {
						Debug.Log("mentioning: " + i);
						//Make sure we don't add any null values--a -1 represents no mentions of any settlements
						if (i != -1)
							playerShipVariables.ship.playerJournal.AddNewSettlementToLog(i);
					}
					//If we find a match, set the appropriate bool flag, and break from the loop
					matchFound = true;
					break;
				}


			}
		}
		if (!matchFound) {
			//if it's not a quest line, just tell the player nothing
			//notificationMessage = "You found " + currentSettlement.name + "!";
			//showNotification = true;
		}

	}



	//====================================================================================================
	//    PLAYER MODIFICATION FUNCTIONS
	//====================================================================================================   

	public void AdjustPlayerClout(int cloutAdjustment) {
		int cloutModifier = 100; //We have a modifier to help link the new system in with the old functions.
		int clout = (int)playerShipVariables.ship.playerClout;
		//adjust the players clout by the given amount
		playerShipVariables.ship.playerClout += (cloutAdjustment * cloutModifier);
		//if the player's clout exceeds 100 after the adjustment, then reduce it back to 100 as a cap
		if (playerShipVariables.ship.playerClout > 5000)
			playerShipVariables.ship.playerClout = 5000;
		//if the player's clout is reduced below 0 after the adjustment, then increase it to 0 again
		if (playerShipVariables.ship.playerClout < 0)
			playerShipVariables.ship.playerClout = 0;
		Debug.Log(playerShipVariables.ship.playerClout);
		//First check if a player reaches a new clout level
		//If the titles don't match after adjustment then we have a change!
		if (GetCloutTitleEquivalency(clout) != GetCloutTitleEquivalency((int)playerShipVariables.ship.playerClout)) {
			updatePlayerCloutMeter = true;
			//Next we need to determine whether or not it was a level down or level up
			//If it was an increase then show a positive message
			if (clout < (clout + cloutAdjustment)) {
				Debug.Log("Gained a level");
				notificationMessage = "Congratulations! You have reached a new level of influence! Before this day you were Jason, " + GetCloutTitleEquivalency(clout) + ".....But now...You have become Jason " + GetCloutTitleEquivalency((int)playerShipVariables.ship.playerClout) + "!";
				showNotification = true;
				//If it was a decrease then show a negative message to the player
			}
			else {
				Debug.Log("Lost a level");
				notificationMessage = "Unfortunately you sunk to a new low level of respect in the world! Before this day you were Jason, " + GetCloutTitleEquivalency(clout) + ".....But now...You have become Jason " + GetCloutTitleEquivalency((int)playerShipVariables.ship.playerClout) + "!";
				showNotification = true;
			}
			MasterGUISystem.GetComponent<script_GUI>().GUI_UpdatePlayerCloutMeter();
		}


	}

	public void AdjustCrewsClout(int cloutAdjustment) {
		foreach (CrewMember crew in playerShipVariables.ship.crewRoster) {
			//adjust the crews clout by the given amount
			crew.clout += cloutAdjustment;
			//if the crew's clout exceeds 100 after the increase, then reduce it back to 100 as a cap
			if (crew.clout > 5000)
				crew.clout = 5000;
			//if the crew's clout is reduced below 0 after the adjustment, then increase it to 0 again
			if (crew.clout < 0)
				crew.clout = 0;
		}

	}

	public void AdjustPlayerShipHealth(int healthAdjustment) {
		//adjust the health by the given amount
		playerShipVariables.ship.health += healthAdjustment;
		//if the health exceeds 100 after the adjustment, then reduce it back to 100 as a cap
		if (playerShipVariables.ship.health > 100)
			playerShipVariables.ship.health = 100;
		//if the health is reduced below 0 after the adjustment, then increase it back to 0
		if (playerShipVariables.ship.health < 0)
			playerShipVariables.ship.health = 0;
	}

	public void AddEntriesToCurrentLogPool(int logID) {
		for (int i = 0; i < captainsLogEntries.Length; i++) {
			if (captainsLogEntries[i].settlementID == logID) {
				currentLogPool.Add(captainsLogEntries[i]);
			}
		}
	}

	public void RemoveEntriesFromCurrentLogPool(int logID) {
		currentLogPool.RemoveAll(entry => entry.settlementID == logID);
	}



	//====================================================================================================
	//    OTHER FUNCTIONS
	//====================================================================================================   
	
	public float GetOverallCloutModifier(int settlementID) {
		//This is the main function that processes ALL clout-based modifiers and returns a floating point value 0-1
		//	--to represent the influence level the player has at any particular moment in an interaction. This
		//	--interaction might be through the buying and selling of goods, interactions with other settlements,
		//	--or interactions with pirates and random events.
		float finalModifier = 0;

		float playerClout = 0;
		float calculatedCrewClout = 0;
		float playerNetworkModifier = 0;
		float playerOriginNetworkModifier = 0;
		float crewMembersNetworkModifier = 0;

		//###### First get the player's clout and convert it to a 0-100 value
		playerClout = playerShipVariables.ship.playerClout;

		playerClout = (int)Math.Floor((playerClout / 5000) * 100); //5000 is the cap so we divide the current amount to get the 0/1 ratio

		//###### Next we need to cycle through all the crew members and tally up the clout there
		//	--This will be a 1 - 100 value that is a sum of of percentage of total possible clout
		//	--e.g. if there are 10 crew members, the total possible clout is 50,000--if it adds
		//	--up to 25,000, the returned clout is 50--or 50%
		float sumOfCrewClout = 0;
		foreach (CrewMember member in playerShipVariables.ship.crewRoster) {
			sumOfCrewClout += member.clout;
		}
		//Here's where we divide the sum by the total possible clout on board the ship--clout will ALWAYS be between 1 - 100
		calculatedCrewClout = sumOfCrewClout / (playerShipVariables.ship.crewRoster.Count * 100);

		//###### Now we need to determine whether or not the current city (or representative thereof) 
		//	--is part of the player's individual network--if it is the value is 100, otherwise it's 50.
		//	--the player's individual network is ALL the settlements he/she knows of in their knowledgebase--not necessarily IN it's main attached network like the Samothrace network of influence
		foreach (int playerNetworkID in playerShipVariables.ship.playerJournal.knownSettlements) {
			if (playerNetworkID == settlementID) {
				playerNetworkModifier = 100f;
			}
			else {
				playerNetworkModifier = 0f;
			}
		}

		//###### Now we need to determine whether or not the current city (or representative thereof) 
		//	--is part of the player's hometown/main network(s), e.g. if the player and city is part of the Samothracian network
		Debug.Log("DEBUG:  " + settlementID);
		//If there is no city attached(settlementID == 0) then we are in open waters so return 0
		if (settlementID != 0) {
			foreach (int playerNetworkID in playerShipVariables.ship.networks) {
				foreach (int settlementNetID in GetSettlementFromID(settlementID).networks) {
					//If we find a network match through the network ID then return the city's population to represent its influence in the network
					//	--if we don't find a match, a value of 0 is returned			
					if (playerNetworkID == settlementNetID) {
						playerOriginNetworkModifier = GetSettlementFromID(settlementID).population;
					}
					else {
						playerOriginNetworkModifier = 0f;
					}
				}

			}
		}
		else {
			playerNetworkModifier = 0f;
		}

		//###### Now let's find out if ANY of the crewmembers' origin towns are connected with this settlement ID of interest
		//	--If there is a member, then return a value of 50, otherwise a value of 0. This value (50) is less than the
		//	--players home town/network(s), because the player is the captain--and assumedly has more influence in that sense.
		//If there is no city attached(settlementID == 0) then we are in open waters so return 0
		if (settlementID != 0) {
			foreach (CrewMember member in playerShipVariables.ship.crewRoster) {
				foreach (int crewNetworkID in GetSettlementFromID(member.originCity).networks) {
					foreach (int settlementNetID in GetSettlementFromID(settlementID).networks) {
						//If we find a network match through the network ID then return the city's population / 2 to represent its influence in the network
						//	--if we don't find a match, a value of 0 is returned			
						if (crewNetworkID == settlementNetID) {
							crewMembersNetworkModifier = GetSettlementFromID(settlementID).population / 2f;
						}
						else {
							crewMembersNetworkModifier = 0f;
						}
					}
				}

			}
		}

		//###### Now that we have all of our clout values, the sum total will be out of 500 possible points
		//	--the player's own clout is weighted at double, the crew modifiers are weighted both at half, and the players network modifers are weighted as normal (1)
		finalModifier = (playerClout * 2) + (calculatedCrewClout / 2) + playerNetworkModifier + playerOriginNetworkModifier + (crewMembersNetworkModifier / 2);
		finalModifier /= 500;
		//We return the final percentage point modifier. It will be between 0-1
		return finalModifier;

	}


	public bool CheckIfShipBackAtLoanOriginPort() {
		bool isAtPort = false;
		if (playerShipVariables.ship.currentLoan.settlementOfOrigin == currentSettlement.settlementID) {
			isAtPort = true;
		}
		else {
			isAtPort = false;
		}

		return isAtPort;


	}


	public void ShowANotificationMessage(string message) {
		//First check if we have a primary message going already
		if (showNotification) {
			//if we do then queue up a secondary message
			showSecondaryNotification = true;
			secondaryNotificationMessage = message;
			//otherwise show a normal primary message
		}
		else {
			showNotification = true;
			notificationMessage = message;
		}
	}

















}///////// END OF FILE
