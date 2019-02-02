using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Loan
{
	public int amount;
	public float interestRate;
	public float numOfDaysUntilDue;
	public float numOfDaysPassedUntilDue;
	public int settlementOfOrigin;

	public Loan(int amount, float interestRate, float numOfDaysUntilDue, int settlementOfOrigin) {
		this.amount = amount;
		this.interestRate = interestRate;
		this.numOfDaysUntilDue = numOfDaysUntilDue;
		this.numOfDaysPassedUntilDue = 0;
		this.settlementOfOrigin = settlementOfOrigin;
	}

	public int GetTotalAmountDueWithInterest() {
		return (int)(amount + (amount * (interestRate / 100)));
	}
}

public class QuestSegment
{
	public int segmentID;
	public int destinationID;
	public bool isFinalSegment;
	public List<int> crewmembersToAdd;
	public List<int> crewmembersToRemove;
	public string descriptionOfQuest;
	public string descriptionAtCompletion;
	public List<int> mentionedPlaces;

	public QuestSegment(int segmentID, int destinationID, string descriptionOfQuest, string descriptionAtCompletion, List<int> crewmembersToAdd, List<int> crewmembersToRemove, bool isFinalSegment, List<int> mentionedPlaces) {
		this.segmentID = segmentID;
		this.destinationID = destinationID;
		this.descriptionOfQuest = descriptionOfQuest;
		this.descriptionAtCompletion = descriptionAtCompletion;
		this.crewmembersToAdd = crewmembersToAdd;
		this.crewmembersToRemove = crewmembersToRemove;
		this.isFinalSegment = isFinalSegment;
		this.mentionedPlaces = mentionedPlaces;
	}
}

public class MainQuestLine
{

	public List<QuestSegment> questSegments;
	public int currentQuestSegment;

	public MainQuestLine() {
		questSegments = new List<QuestSegment>();
		currentQuestSegment = 0;
	}


}

public class Journal
{

	public List<int> knownSettlements;

	public Journal() {
		this.knownSettlements = new List<int>();
	}

	public void AddNewSettlementToLog(int settlementID) {
		//Debug.Log ("Adding New SET ID:  -->   " + settlementID);
		//First Check to see if there are any settlements in the log yet
		if (knownSettlements.Count > 0) {
			//check to make sure the id doesn't already exist
			for (int i = 0; i < knownSettlements.Count; i++) {
				if (knownSettlements[i] == settlementID)
					break;//if we find a match break the loop and exit
				else if (i == knownSettlements.Count - 1)//else if there are no matches and we're at the end of the list, add the ID
					this.knownSettlements.Add(settlementID);
			}
		}
		else {//if there are no settlements then just add the settlement
			this.knownSettlements.Add(settlementID);
		}
	}
}

public enum CrewType
{
	Sailor = 0,
	Warrior = 1,
	Slave = 2,
	Passenger = 3,
	Navigator = 4,
	Guide = 5,
	Assistant = 6,
	Royalty = 7,
	Seer = 8,
	Lawyer = 9
}

public class CrewMember
{
	public int ID;
	public string name;
	public int originCity;
	public int clout;
	public string backgroundInfo;
	public bool isKillable;
	public bool isPartOfMainQuest;
	public CrewType typeOfCrew;
	//0= sailor  1= warrior  2= slave  3= passenger 4= navigator 5= auger
	//A sailor is the base class--no benefits/detriments
	//	--navigators provide maps to different settlements and decrease negative random events
	//	--warriors make sure encounters with pirates or other raiding activities go better in your favor
	//	--slaves have zero clout--few benefits--but they never leave the ship unless they die
	public CrewMember(int ID, string name, int originCity, int clout, CrewType typeOfCrew, string backgroundInfo, bool isKillable, bool isPartOfMainQuest) {
		this.ID = ID;
		this.name = name;
		this.originCity = originCity;
		this.clout = clout;
		this.typeOfCrew = typeOfCrew;
		this.backgroundInfo = backgroundInfo;
		this.isKillable = isKillable;
		this.isPartOfMainQuest = isPartOfMainQuest;
	}

	//This is a helper class to create a void crewman
	public CrewMember(int id) {
		this.ID = id;
	}



}

public class CaptainsLogEntry
{
	public int settlementID;
	public string logEntry;
	public string dateTimeOfEntry;

	public CaptainsLogEntry(int settlementID, string logEntry) {
		this.settlementID = settlementID;
		this.logEntry = logEntry;
	}

}

public class CurrentRose
{
	public float direction;
	public float speed;

	public CurrentRose(float direction, float speed) {
		this.direction = direction;
		this.speed = speed;

	}
}

public class WindRose
{
	public float direction;
	public float speed;

	public WindRose(float direction, float speed) {
		this.direction = direction;
		this.speed = speed;

	}
}

public class PlayerRoute
{
	//TODO This method as well as the Player Journey Log is a pretty dirty solution that needs some serious clean up and tightening. It's a bit brute force and messy right now.
	public Vector3[] theRoute;
	public int settlementID;
	public string settlementName;
	public bool isLeaving;
	public float timeStampInDays;
	public Vector3 UnityXYZEndPoint;

	public PlayerRoute(Vector3 origin, Vector3 destination, float timeStampInDays) {
		this.theRoute = new Vector3[] { origin, destination };
		this.settlementID = -1;
		this.timeStampInDays = timeStampInDays;
		this.UnityXYZEndPoint = destination;
		Debug.Log("Getting called...." + origin.x + "  " + origin.z);

	}

	public PlayerRoute(Vector3 origin, Vector3 destination, int settlementID, string settlementName, bool isLeaving, float timeStampInDays) {
		this.theRoute = new Vector3[] { origin, destination };
		this.settlementID = settlementID;
		this.isLeaving = isLeaving;
		this.settlementName = settlementName;
		this.timeStampInDays = timeStampInDays;
		this.UnityXYZEndPoint = origin;
		Debug.Log("Getting called 2...." + origin.x + "  " + origin.z);
		//TODO this is a dirty fix--under normal route conditions the Unity XYZ is changed to XZY to match 3d geometry conventions (z is up and not forward like in Unity)
		//TODO --I'm manually making this port stop XZY here so I don't have to change to much of the ConvertJourneyLogToCSVText() function in the PlayerJourneyLog Class
		//TODO --We only need to change the 'origin' XYZ to XZY because port stops will have Vector3.zero (0,0,0) for the destination values always 
		//TODO --Maybe it's lazy--but I've been at this for about 15 hours non-stop and I'm running out of sophisticated brain power. Probably the narrative of all this code.
		//this.theRoute[0] = new Vector3(this.theRoute[0].x,this.theRoute[0].z,this.theRoute[0].y);
		//this.theRoute[1] = new Vector3(this.theRoute[1].x,this.theRoute[1].z,this.theRoute[1].y);
	}

}

public class PlayerJourneyLog
{
	//TODO This whole function needs retooling--rather htan 3 separate arrays, the PlayerRoute object should store all the necessary variables--In fact this could be replaced with a simple List of Player Routes rather than having two separate objects no apparent reason.
	public List<PlayerRoute> routeLog;
	public List<string> cargoLog;
	//This is a quick dirty solution TODO need to make it a bit easier with current timelines of work
	public List<string> otherAttributes;
	public string CSVheader;

	public PlayerJourneyLog() {
		this.routeLog = new List<PlayerRoute>();
		this.cargoLog = new List<string>();
		this.otherAttributes = new List<string>();
		this.CSVheader = "Unique_Machine_ID,timestamp,originE,originN,originZ,endE,endN,endZ," +
			"Water_kg,Provisions_kg,Grain_kg,Wine_kg,Timber_kg,Gold_kg,Silver_kg," +
			"Copper_kg,Tin_kg,Obsidian_kg,Lead_kg,Slaves_kg,Iron_kg,Bronze_kg,Luxury_kg,Is_Leaving_Port,PortID,PortName," +
			"CrewMemberIDs,UnityXYZ,Current_Questleg,ShipHP,Clout,PlayerNetwork,DaysStarving,DaysThirsty,Currency,LoanAmount,LoanOriginID,CurrentNavigatorTarget,KnownSettlements,CaptainsLog\n";
	}

	public void AddRoute(PlayerRoute routeToAdd, script_player_controls playerShipVars, string captainsLog) {
		this.routeLog.Add(routeToAdd);
		Debug.Log("Getting called 3...." + routeToAdd.theRoute[0].x + "  " + routeToAdd.theRoute[0].z);
		AddCargoManifest(playerShipVars.ship.cargo);
		AddOtherAttributes(playerShipVars, captainsLog, routeToAdd);
	}

	public void AddCargoManifest(Resource[] cargoToAdd) {
		string CSVstring = "";
		foreach (Resource resource in cargoToAdd) {
			CSVstring += "," + resource.amount_kg;
		}
		this.cargoLog.Add(CSVstring);
	}

	public void AddOtherAttributes(script_player_controls playerShipVars, string captainsLog, PlayerRoute currentRoute) {
		string CSVstring = "";
		Ship playerShip = playerShipVars.ship;
		//Add the applicable port docking info
		//If it isn't -1, then it's a port stop
		if (currentRoute.settlementID != -1) {
			CSVstring += "," + currentRoute.isLeaving + "," + currentRoute.settlementID + "," + currentRoute.settlementName;
		}
		else {
			CSVstring += "," + -1 + "," + -1 + "," + -1;
		}

		//Add the crewID's 
		CSVstring += ",";
		for (int index = 0; index < playerShip.crewRoster.Count; index++) {
			//Debug.Log ("ID: "  + playerShip.crewRoster[index].ID);
			CSVstring += playerShip.crewRoster[index].ID;
			if (index < playerShip.crewRoster.Count - 1)
				CSVstring += "_";
		}

		//Add the Unity XYZ coordinate of ship
		Vector3 playerLocation = playerShipVars.transform.position;
		CSVstring += "," + playerLocation.x + "_" + playerLocation.y + "_" + playerLocation.z;
		//Add the current questleg
		CSVstring += "," + playerShip.mainQuest.currentQuestSegment;
		//Add Ship HP
		CSVstring += "," + playerShip.health;
		//Add Player Clout
		CSVstring += "," + playerShip.playerClout;
		//Add Player Networks
		CSVstring += ",";
		for (int index = 0; index < playerShip.networks.Count; index++) {
			CSVstring += playerShip.networks[index];
			if (index < playerShip.networks.Count - 1)
				CSVstring += "_";
		}
		//Add Days Starving
		CSVstring += "," + playerShipVars.numOfDaysWithoutProvisions;
		//Add Days Thirsty
		CSVstring += "," + playerShipVars.numOfDaysWithoutWater;
		//Add currency
		CSVstring += "," + playerShip.currency;
		//Add Loan Amount Owed
		if (playerShip.currentLoan != null)
			CSVstring += "," + playerShip.currentLoan.amount;
		else
			CSVstring += ",-1";
		//Add Loan Origin City
		if (playerShip.currentLoan != null)
			CSVstring += "," + playerShip.currentLoan.settlementOfOrigin;
		else
			CSVstring += ",-1";
		//Add Current Navigator Target
		CSVstring += "," + playerShip.currentNavigatorTarget;
		//Add the list of known settlements in the player's acquired journal settlement knowledge
		CSVstring += ",";
		//Debug.Log (CSVstring);
		foreach (int settlementID in playerShip.playerJournal.knownSettlements)
			CSVstring += settlementID + "_";
		//remove trailing '_' from list of known settlements
		CSVstring = CSVstring.Remove(CSVstring.Length - 1);
		//Debug.Log ("After substring: " + CSVstring);
		//Add Captains Log: first we need to switch commas in the log to a "|" so it doesn't hurt the delimeters TODO This could be nicer but is fine for now until we get a better database setup
		//--also need tp scrub newlines
		string scrubbedLog = captainsLog.Replace(',', '^');
		scrubbedLog = scrubbedLog.Replace('\n', '*');
		CSVstring += "," + scrubbedLog;

		//Add a new row to match the route of all these attributes
		this.otherAttributes.Add(CSVstring);

	}

	public string ConvertJourneyLogToCSVText() {
		string CSVfile = "";
		//Ship playerShip = playerShipVars.ship;
		//Let's first add the headings to each column
		CSVfile += this.CSVheader;
		Debug.Log("Route Count: " + routeLog.Count);
		//first make sure the route isn't empty--if it is, then return a blank file
		if (routeLog.Count == 0) {
			CSVfile = "There is no player data to save currently";
		}
		else {

			//Loop through each route in the list and respectively the cargo(they will always be the same size so a single for loop index can handle both at once
			//	Each loop represents a single line in the CSV file
			for (int i = 0; i < routeLog.Count; i++) {
				//First add the player's unique machine ID--this will be different depending on the Operating System of the user, but will always be a consistent unique ID based on the users hardware(assuming there aren't major hardware changes)
				CSVfile += SystemInfo.deviceUniqueIdentifier + ",";
				//we're converting it to Web Mercator before saving it
				Vector3 mercator_origin = new Vector3((routeLog[i].theRoute[0].x * 1193.920898f) + (526320 - 0), (routeLog[i].theRoute[0].z * 1193.920898f) + (2179480 - 0), routeLog[i].theRoute[0].y);
				Vector3 mercator_end = new Vector3((routeLog[i].theRoute[1].x * 1193.920898f) + (526320 - 0), (routeLog[i].theRoute[1].z * 1193.920898f) + (2179480 - 0), routeLog[i].theRoute[1].y);

				Vector2 longXlatY_origin = CoordinateUtil.ConvertWebMercatorToWGS1984(new Vector2(mercator_origin.x, mercator_origin.y));
				Vector2 longXlatY_end = CoordinateUtil.ConvertWebMercatorToWGS1984(new Vector2(mercator_end.x, mercator_end.y));


				//TODO: This needs to be cleaned up below--the lat/long bit so it's not wasting resources on a pointless conversion above
				//TODO -- Seriously this XYZ / XZY business is a frankenstein monster of confusion--I can't even fathom in my current sleep deprived state why I'm having to put the y as a z here. My god. Someone Save me!
				//If this isn't a player travel route, but a port stop, then we don't need to worry about the conversion of of lat / long--it's already in lat long
				if (routeLog[i].settlementID != -1) {
					longXlatY_origin = new Vector2(routeLog[i].theRoute[0].x, routeLog[i].theRoute[0].z);
					longXlatY_end = Vector2.zero;

				}

				CSVfile += routeLog[i].timeStampInDays + "," + longXlatY_origin.x + "," + longXlatY_origin.y + "," + mercator_origin.z + "," +
					longXlatY_end.x + "," + longXlatY_end.y + "," + mercator_end.z;

				/*CSVfile += ((routeLog[i].theRoute[0].x * 1193.920898f) + (526320 - 0)) + "," + 
						   ((routeLog[i].theRoute[0].z * 1193.920898f) + (2179480 - 0)) + "," + 
						    routeLog[i].theRoute[0].y + "," +
						    
						   ((routeLog[i].theRoute[1].x * 1193.920898f) + (526320 - 0)) + "," + 
						   ((routeLog[i].theRoute[1].z * 1193.920898f) + (2179480 - 0)) + "," + 
						   routeLog[i].theRoute[1].y;
				*/



				//Add the Resources to the line record
				CSVfile += cargoLog[i];
				CSVfile += otherAttributes[i];


				//Add a newline if not on last route
				if (i != (routeLog.Count - 1)) {
					CSVfile += "\n";
					//Debug.Log ("Adding a NEW Line?");	
				}

				//Debug.Log (CSVfile);
			}
		}

		return CSVfile;
	}
}

public class MetaResource
{
	public string name;
	public int id;
	public string description;

	public MetaResource(string name, int id, string description) {
		this.name = name;
		this.id = id;
		this.description = description;
	}

}


public class Resource
{
	public string name;
	public float amount_kg;
	public float probabilityOfAvailability = 0;

	public Resource(string name, float amount_kg) {
		this.name = name;
		this.amount_kg = amount_kg;
	}
}

public class Settlement
{

	public int settlementID;
	public Vector2 location_longXlatY;
	public string name;
	public int population;
	public float elevation;
	public Resource[] cargo;
	public float tax_neutral;
	public float tax_network;
	public GameObject theGameObject;
	public int[] networkHintResources;
	public Vector3 adjustedGamePosition;
	public float eulerY;
	public int typeOfSettlement;
	public string description;
	public List<int> networks;
	public string prefabName;

	public Settlement(int settlementID, string name, Vector2 location_longXlatY, float elevation, int population) {
		this.settlementID = settlementID;
		this.location_longXlatY = location_longXlatY;
		this.elevation = elevation;
		this.name = name;
		this.population = population;
		cargo = new Resource[] {
		new Resource ("Water", 0f),
		new Resource ("Provisions", 0f),
		new Resource ("Grain", 0f),
		new Resource ("Wine", 0f),
		new Resource ("Timber", 0f),
		new Resource ("Gold", 0f),
		new Resource ("Silver", 0f),
		new Resource ("Copper", 0f),
		new Resource ("Tin", 0f),
		new Resource ("Obsidian", 0f),
		new Resource ("Lead", 0f),
		new Resource ("Slaves", 0f),
		new Resource ("Iron", 0f),
		new Resource ("Bronze", 0f),
		new Resource ("Prestige Goods", 0f),
		};
		networks = new List<int>();
	}

	//This is a debug class to make a blank settlement for testing
	public Settlement(int id, string name, int networkID) {
		this.settlementID = id;
		this.name = name;
		this.population = 0;
		this.elevation = 0;
		this.tax_network = 0;
		this.tax_neutral = 0;
		this.description = "FAKE SETTLEMENT--LOOK INTO THIS ERROR";
		this.networks = new List<int>();

	}

	override public string ToString() {
		string mString = this.name + ":\n" + "Population: " + population + "\n\n RESOURCES \n";
		for (int i = 0; i < this.cargo.Length; i++) {
			mString += this.cargo[i].name + ":  " + this.cargo[i].amount_kg + "kg\n";
		}

		return mString;
	}

}

public class Ship
{
	public string name;
	public float speed;
	public float health;
	public float cargo_capicity_kg;
	public float current_cargo_kg;
	public Resource[] cargo;
	public int currency;
	public int crewCapacity;
	public int crew;
	public float totalNumOfDaysTraveled;
	public int networkID;
	public float playerClout;
	public List<CaptainsLogEntry> shipCaptainsLog;
	public List<CrewMember> crewRoster;
	public Journal playerJournal;
	public int currentNavigatorTarget;
	public Loan currentLoan;
	public MainQuestLine mainQuest;
	public List<int> networks;
	public int originSettlement;
	public string builtMonuments = "";

	public Ship(string name, float speed, int health, float cargo_capcity_kg) {

		this.name = name;
		this.speed = speed;
		this.health = health;
		this.cargo_capicity_kg = cargo_capcity_kg;
		this.shipCaptainsLog = new List<CaptainsLogEntry>();
		this.crewRoster = new List<CrewMember>();
		this.playerJournal = new Journal();

		cargo = new Resource[] {
			new Resource ("Water", 100f),
			new Resource ("Provisions", 100f),
			new Resource ("Grain", 0f),
			new Resource ("Wine", 0f),
			new Resource ("Timber", 0f),
			new Resource ("Gold", 0f),
			new Resource ("Silver", 0f),
			new Resource ("Copper", 0f),
			new Resource ("Tin", 0f),
			new Resource ("Obsidian", 0f),
			new Resource ("Lead", 0f),
			new Resource ("Slaves", 0f),
			new Resource ("Iron", 0f),
			new Resource ("Bronze", 0f),
			new Resource ("Prestige Goods", 0f),
		};

		this.current_cargo_kg = 0;
		this.currency = 500;
		this.crewCapacity = 30;
		this.crew = 0;
		this.playerClout = 50f;
		this.currentNavigatorTarget = -1;
		this.totalNumOfDaysTraveled = 0;
		this.networks = new List<int>();
		this.originSettlement = 246; //TODO Default set to Iolcus--Jason's hometown
		this.networks.Add(1);//TODO Default set to Samothrace network.
	}

	public float GetTotalCargoAmount() {
		float total = 0;
		foreach (Resource resource in cargo) {
			total += resource.amount_kg;
		}
		return total;
	}
}
