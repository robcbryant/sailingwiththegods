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


//======================================================================================================================================================================
//======================================================================================================================================================================
//  SETUP ALL GLOBAL CLASSES
//======================================================================================================================================================================
//======================================================================================================================================================================

public class Loan
{
	public int amount;
	public float interestRate;
	public float numOfDaysUntilDue;
	public float numOfDaysPassedUntilDue;
	public int settlementOfOrigin;
	
	public Loan (int amount, float interestRate, float numOfDaysUntilDue, int settlementOfOrigin)
	{
		this.amount = amount;
		this.interestRate = interestRate;
		this.numOfDaysUntilDue = numOfDaysUntilDue;
		this.numOfDaysPassedUntilDue = 0;
		this.settlementOfOrigin = settlementOfOrigin;
	}
	
	public int GetTotalAmountDueWithInterest ()
	{
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
	
	public QuestSegment (int segmentID, int destinationID, string descriptionOfQuest, string descriptionAtCompletion, List<int> crewmembersToAdd, List<int>crewmembersToRemove, bool isFinalSegment, List<int> mentionedPlaces)
	{
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
	
	public MainQuestLine ()
	{
		questSegments = new List<QuestSegment> ();
		currentQuestSegment = 0;
	}
	

}

public class Journal
{
	
	public List<int> knownSettlements;
	
	public Journal ()
	{
		this.knownSettlements = new List<int> ();
	}
	
	public void AddNewSettlementToLog (int settlementID)
	{
		//Debug.Log ("Adding New SET ID:  -->   " + settlementID);
		//First Check to see if there are any settlements in the log yet
		if (knownSettlements.Count > 0) {
			//check to make sure the id doesn't already exist
			for (int i = 0; i <knownSettlements.Count; i++) {
				if (knownSettlements [i] == settlementID)
					break;//if we find a match break the loop and exit
				else if (i == knownSettlements.Count - 1)//else if there are no matches and we're at the end of the list, add the ID
					this.knownSettlements.Add (settlementID);
			}
		} else {//if there are no settlements then just add the settlement
			this.knownSettlements.Add (settlementID);
		}
	}
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
	public int typeOfCrew; 
	//0= sailor  1= warrior  2= slave  3= passenger 4= navigator 5= auger
	//A sailor is the base class--no benefits/detriments
	//	--navigators provide maps to different settlements and decrease negative random events
	//	--warriors make sure encounters with pirates or other raiding activities go better in your favor
	//	--slaves have zero clout--few benefits--but they never leave the ship unless they die
	public CrewMember (int ID, string name, int originCity, int clout, int typeOfCrew, string backgroundInfo, bool isKillable, bool isPartOfMainQuest)
	{
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
	public CrewMember (int id)
	{
		this.ID = id;	
	}
	
	
	
}

public class CaptainsLogEntry
{
	public int settlementID;
	public string logEntry;
	public string dateTimeOfEntry;
	
	public CaptainsLogEntry (int settlementID, string logEntry)
	{
		this.settlementID = settlementID;
		this.logEntry = logEntry;
	}

}

public class CurrentRose
{
	public float direction;
	public float speed;
	
	public CurrentRose (float direction, float speed)
	{
		this.direction = direction;
		this.speed = speed;
		
	}
}

public class WindRose
{
	public float direction;
	public float speed;

	public WindRose (float direction, float speed)
	{
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
	
	public PlayerRoute (Vector3 origin, Vector3 destination, float timeStampInDays)
	{
		this.theRoute = new Vector3[]{origin, destination};
		this.settlementID = -1;
		this.timeStampInDays = timeStampInDays;
		this.UnityXYZEndPoint = destination;
		Debug.Log ("Getting called...." + origin.x + "  " + origin.z);
		
	}

	public PlayerRoute (Vector3 origin, Vector3 destination, int settlementID, string settlementName, bool isLeaving, float timeStampInDays)
	{
		this.theRoute = new Vector3[]{origin, destination};
		this.settlementID = settlementID;
		this.isLeaving = isLeaving;
		this.settlementName = settlementName;
		this.timeStampInDays = timeStampInDays;
		this.UnityXYZEndPoint = origin;
		Debug.Log ("Getting called 2...." + origin.x + "  " + origin.z);	
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
	   
	public PlayerJourneyLog ()
	{
		this.routeLog = new List<PlayerRoute> ();
		this.cargoLog = new List<string> ();
		this.otherAttributes = new List<string> ();
		this.CSVheader = "Unique_Machine_ID,timestamp,originE,originN,originZ,endE,endN,endZ," +
			"Water_kg,Provisions_kg,Grain_kg,Wine_kg,Timber_kg,Gold_kg,Silver_kg," +
			"Copper_kg,Tin_kg,Obsidian_kg,Lead_kg,Slaves_kg,Iron_kg,Bronze_kg,Luxury_kg,Is_Leaving_Port,PortID,PortName," +
			"CrewMemberIDs,UnityXYZ,Current_Questleg,ShipHP,Clout,PlayerNetwork,DaysStarving,DaysThirsty,Currency,LoanAmount,LoanOriginID,CurrentNavigatorTarget,KnownSettlements,CaptainsLog\n";
	}
	
	public void AddRoute (PlayerRoute routeToAdd, script_player_controls playerShipVars, string captainsLog)
	{
		this.routeLog.Add (routeToAdd);
		Debug.Log ("Getting called 3...." + routeToAdd.theRoute [0].x + "  " + routeToAdd.theRoute [0].z);	
		AddCargoManifest (playerShipVars.ship.cargo);
		AddOtherAttributes (playerShipVars, captainsLog, routeToAdd);
	}
	
	public void AddCargoManifest (Resource[] cargoToAdd)
	{
		string CSVstring = "";
		foreach (Resource resource in cargoToAdd) {
			CSVstring += "," + resource.amount_kg;
		}
		this.cargoLog.Add (CSVstring);
	}
	
	public void AddOtherAttributes (script_player_controls playerShipVars, string captainsLog, PlayerRoute currentRoute)
	{
		string CSVstring = "";
		Ship playerShip = playerShipVars.ship;
		//Add the applicable port docking info
		//If it isn't -1, then it's a port stop
		if (currentRoute.settlementID != -1) {
			CSVstring += "," + currentRoute.isLeaving + "," + currentRoute.settlementID + "," + currentRoute.settlementName;
		} else {
			CSVstring += "," + -1 + "," + -1 + "," + -1;
		}
		
		//Add the crewID's 
		CSVstring += ",";
		for (int index = 0; index < playerShip.crewRoster.Count; index++) {
			//Debug.Log ("ID: "  + playerShip.crewRoster[index].ID);
			CSVstring += playerShip.crewRoster [index].ID;
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
			CSVstring += playerShip.networks [index];
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
		CSVstring = CSVstring.Remove (CSVstring.Length - 1);
		//Debug.Log ("After substring: " + CSVstring);
		//Add Captains Log: first we need to switch commas in the log to a "|" so it doesn't hurt the delimeters TODO This could be nicer but is fine for now until we get a better database setup
		//--also need tp scrub newlines
		string scrubbedLog = captainsLog.Replace (',', '^');
		scrubbedLog = scrubbedLog.Replace ('\n', '*');
		CSVstring += "," + scrubbedLog;

		//Add a new row to match the route of all these attributes
		this.otherAttributes.Add (CSVstring);
	
	}
	
	public string ConvertJourneyLogToCSVText ()
	{
		string CSVfile = "";
		//Ship playerShip = playerShipVars.ship;
		//Let's first add the headings to each column
		CSVfile += this.CSVheader;
		Debug.Log ("Route Count: " + routeLog.Count);
		//first make sure the route isn't empty--if it is, then return a blank file
		if (routeLog.Count == 0) {
			CSVfile = "There is no player data to save currently";
		} else {

			//Loop through each route in the list and respectively the cargo(they will always be the same size so a single for loop index can handle both at once
			//	Each loop represents a single line in the CSV file
			for (int i = 0; i < routeLog.Count; i++) {
				//First add the player's unique machine ID--this will be different depending on the Operating System of the user, but will always be a consistent unique ID based on the users hardware(assuming there aren't major hardware changes)
				CSVfile += SystemInfo.deviceUniqueIdentifier + ",";
				//we're converting it to Web Mercator before saving it
				Vector3 mercator_origin = new Vector3 ((routeLog [i].theRoute [0].x * 1193.920898f) + (526320 - 0), (routeLog [i].theRoute [0].z * 1193.920898f) + (2179480 - 0), routeLog [i].theRoute [0].y);
				Vector3 mercator_end = new Vector3 ((routeLog [i].theRoute [1].x * 1193.920898f) + (526320 - 0), (routeLog [i].theRoute [1].z * 1193.920898f) + (2179480 - 0), routeLog [i].theRoute [1].y);
				
				Vector2 longXlatY_origin = GV_CONST.ConvertWebMercatorToWGS1984 (new Vector2 (mercator_origin.x, mercator_origin.y));
				Vector2 longXlatY_end = GV_CONST.ConvertWebMercatorToWGS1984 (new Vector2 (mercator_end.x, mercator_end.y));
				
				
				//TODO: This needs to be cleaned up below--the lat/long bit so it's not wasting resources on a pointless conversion above
				//TODO -- Seriously this XYZ / XZY business is a frankenstein monster of confusion--I can't even fathom in my current sleep deprived state why I'm having to put the y as a z here. My god. Someone Save me!
				//If this isn't a player travel route, but a port stop, then we don't need to worry about the conversion of of lat / long--it's already in lat long
				if (routeLog [i].settlementID != -1) {
					longXlatY_origin = new Vector2 (routeLog [i].theRoute [0].x, routeLog [i].theRoute [0].z);
					longXlatY_end = Vector2.zero;
					
				}
				
				CSVfile += routeLog [i].timeStampInDays + "," + longXlatY_origin.x + "," + longXlatY_origin.y + "," + mercator_origin.z + "," +
					longXlatY_end.x + "," + longXlatY_end.y + "," + mercator_end.z;
				
				/*CSVfile += ((routeLog[i].theRoute[0].x * 1193.920898f) + (526320 - 0)) + "," + 
						   ((routeLog[i].theRoute[0].z * 1193.920898f) + (2179480 - 0)) + "," + 
						    routeLog[i].theRoute[0].y + "," +
						    
						   ((routeLog[i].theRoute[1].x * 1193.920898f) + (526320 - 0)) + "," + 
						   ((routeLog[i].theRoute[1].z * 1193.920898f) + (2179480 - 0)) + "," + 
						   routeLog[i].theRoute[1].y;
				*/
						   
						   
						   
				//Add the Resources to the line record
				CSVfile += cargoLog [i];
				CSVfile += otherAttributes [i];


				//Add a newline if not on last route
				if (i != (routeLog.Count - 1)){
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
	
	public MetaResource(string name, int id, string description)
	{
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
	
	public Resource (string name, float amount_kg)
	{
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
	
	public Settlement (int settlementID, string name, Vector2 location_longXlatY, float elevation, int population)
	{
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
		networks = new List<int> ();
	}
	
	//This is a debug class to make a blank settlement for testing
	public Settlement (int id, string name, int networkID)
	{
		this.settlementID = id;
		this.name = name;
		this.population = 0;
		this.elevation = 0;
		this.tax_network = 0;
		this.tax_neutral = 0;
		this.description = "FAKE SETTLEMENT--LOOK INTO THIS ERROR";
		this.networks = new List<int> ();
		
	}
	
	override public string ToString ()
	{
		string mString = this.name + ":\n" + "Population: " + population + "\n\n RESOURCES \n";
		for (int i = 0; i < this.cargo.Length; i++) {
			mString += this.cargo [i].name + ":  " + this.cargo [i].amount_kg + "kg\n";
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

	public Ship (string name, float speed, int health, float cargo_capcity_kg)
	{
	            
		this.name = name;
		this.speed = speed;
		this.health = health;
		this.cargo_capicity_kg = cargo_capcity_kg;
		this.shipCaptainsLog = new List<CaptainsLogEntry> ();
		this.crewRoster = new List<CrewMember> ();
		this.playerJournal = new Journal ();
		
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
		this.networks = new List<int> ();
		this.originSettlement = 246; //TODO Default set to Iolcus--Jason's hometown
		this.networks.Add (1);//TODO Default set to Samothrace network.
	}
	
	public float GetTotalCargoAmount ()
	{
		float total = 0;
		foreach (Resource resource in cargo) {
			total += resource.amount_kg;
		}
		return total;
	}
}

//####################################################################
//  This is a list of all constant variables accessible to all scripts
public static class GV_CONST
{
	public const int TAVERNSTATE_DEFAULT = 0;
	public const int TAVERNSTATE_CITY = 1;
	public const int TAVERNSTATE_NAVIGATOR = 2;
	public const int TAVERNSTATE_SHRINE = 3;
	public const int TAVERNSTATE_LOAN = 4;
	public const int TAVERNSTATE_HIRECREW = 5;
	public const int TAVERNSTATE_JOBS = 6;
	public const int TAVERNSTATE_REPAIRS = 7;
	public const int TAVERNSTATE_FIRECREW = 8;
	public const int CREWTYPE_SAILOR = 0;
	public const int CREWTYPE_WARRIOR = 1;
	public const int CREWTYPE_SLAVE = 2;
	public const int CREWTYPE_PASSENGER = 3;
	public const int CREWTYPE_NAVIGATOR = 4;
	public const int CREWTYPE_GUIDE = 5;
	public const int CREWTYPE_ASSISTANT = 6;
	public const int CREWTYPE_ROYALTY = 7;
	public const int CREWTYPE_SEER = 8;
	public const int CREWTYPE_LAWYER = 9;

	public static Vector2 ConvertWebMercatorToWGS1984 (Vector2 MercatorEastNorth)
	{
		double mercatorX_lon = MercatorEastNorth.x; 
		double mercatorY_lat = MercatorEastNorth.y;
		//if (Math.Abs(mercatorX_lon) < 180 && Math.Abs(mercatorY_lat) < 90)
		//		return Vector2.zero;
		
		//if ((Math.Abs(mercatorX_lon) > 20037508.3427892) )
		//TODO figure out what to do if the coordinate is OUTSIDE the extents of the projection
		//TODO	--E.G. if we sail through the north pole and start heading south--unity world coordinates need to reflect this
		//TODO	--We shouldn't need to worry about this unless we do globe circumnavigation--which we aren't
		//If we are outside the northern extent
		//if(Math.Abs(mercatorY_lat) > 20037508.3427892)
		//	mercatorY_lat = 20037508.3427892 - (mercatorY_lat - 20037508.3427892) - 1;
		//else if (Math.Abs(mercatorY_lat) < -20037508.3427892)
		
		double x = mercatorX_lon;
		double y = mercatorY_lat;
		double num3 = x / 6378137.0;
		double num4 = num3 * 57.295779513082323;
		double num5 = Math.Floor ((double)((num4 + 180.0) / 360.0));
		double num6 = num4 - (num5 * 360.0);
		double num7 = 1.5707963267948966 - (2.0 * Math.Atan (Math.Exp ((-1.0 * y) / 6378137.0)));
		mercatorX_lon = num6;
		mercatorY_lat = num7 * 57.295779513082323;
		
		return new Vector2 ((float)mercatorX_lon, (float)mercatorY_lat);
	}
}








//======================================================================================================================================================================
//======================================================================================================================================================================
//  SETUP ALL GLOBAL VARIABLES
//======================================================================================================================================================================
//======================================================================================================================================================================

public class globalVariables : MonoBehaviour
{
	//Easting (x) will always equal Unity X
	//Northing (y) will always equal Unity Z
	//Elevation (z) will always equal Unity Y
	Vector2 rasterMapOriginMeter = new Vector2 (526320, 2179480);//in meters
	public float unityWorldUnitResolution = 1193.920898f;//in meters
	float unityOrigin = 0;//This will always be 0,0 for both x and y
	public Settlement[] settlement_masterList;
	public GameObject settlement_masterList_parent;
	public GameObject playerShip;
	public script_player_controls playerShipVariables;
	public GameObject mainCamera;
	public GameObject FPVCamera;
	public GameObject terrain;
	public GameObject cityLightsParent;
	public GameObject selection_ring;
	public GameObject currentSettlementGameObject;
	public Settlement currentSettlement;
	
	
	//###################################
	//	TODO UNSORTED VARIABLES TODO
	//###################################
	
	public List<MetaResource> masterResourceList = new List<MetaResource>();
	public bool sailsAreUnfurled = true;
	public GameObject[] sails = new GameObject[6];
	public int currentTavernMenuState;
	public GameObject playerTrajectory;
	public GameObject playerGhostRoute;
	public GameObject navigatorBeacon;
	public string currentCaptainsLog = "";
	public bool isPerformingRandomEvent = false;
	public CaptainsLogEntry[] captainsLogEntries;
	public List<CaptainsLogEntry> currentLogPool = new List<CaptainsLogEntry> ();
	public WindRose[,] windrose_January = new WindRose[10, 8];
	public GameObject windZoneParent;
	public GameObject waterSurface;
	public CurrentRose[,] currentRose_January;
	public GameObject currentZoneParent;
	public List<Settlement> currentNetworkSettlements;//this is not a complete list--but rather a list of settlements generated through probability
	public List<CrewMember> currentlyAvailableCrewMembersAtPort; // updated every time ship docks at port
	public bool controlsLocked = false;
	public bool isGameOver = false;
	public bool justLeftPort = false;
	public bool menuControlsLock = false;
	public bool gameIsFinished = false;
	
	//The main notifications are handled by the first two variables
	//	--to make sure multiple notifications can be seen that might overlap, e.g. the player triggers two notifications in an action
	//	--there are two and if the first is 'true' or showing a message, it will default to a secondary notification window
	public bool showNotification = false;
	public string notificationMessage = "";
	public bool showSecondaryNotification = false;
	public string secondaryNotificationMessage = "";
	public int currentPortTax = 0;
	public bool IS_NEW_GAME = true;
	public bool IS_NOT_NEW_GAME = false;
	public string TD_year = "2000";
	public string TD_month = "1";
	public string TD_day = "1";
	public string TD_hour = "0";
	public string TD_minute = "0";
	public string TD_second = "0";
	public Light mainLightSource;
	public bool startGameButton_isPressed = false;
	public bool isTitleScreen = true;
	public bool isStartScreen = false;
	public GameObject camera_titleScreen;
	public GameObject bg_titleScreen;
	public GameObject bg_startScreen;
	public bool isLoadedGame = false;
	public bool isPassingTime = false;
	
	//###################################
	//	Crew Member Variables
	//###################################
	public List<CrewMember> masterCrewList = new List<CrewMember> ();
	
	//###################################
	//	GUI VARIABLES
	//###################################
	public bool runningMainGameGUI = false;
	public bool showSettlementTradeGUI = false;
	public bool showSettlementTradeButton = false;
	public bool[] newGameCrewSelectList = new bool[40];
	public List<CrewMember> newGameAvailableCrew = new List<CrewMember> ();
	public bool showPortDockingNotification = false;
	public bool isInNetwork = false;
	public CrewMember crewMemberWithNetwork;
	public bool gameDifficulty_Beginner = false;
	public bool showNonPortDockButton = false;
	public bool showNonPortDockingNotification = false;
	
	public GameObject MasterGUISystem;
	public GameObject GUI_PortMenu;
	public GameObject GUI_GameHUD;
	public bool updatePlayerCloutMeter = false;
	
	
	
	
	
	
	//###################################
	//	SKYBOX VARIABLES
	//###################################
	public GameObject skybox_celestialGrid;
	public GameObject skybox_MAIN_CELESTIAL_SPHERE;
	public GameObject skybox_ecliptic_sphere;
	public GameObject skybox_clouds;
	public GameObject skybox_horizonColor;
	public GameObject skybox_sun;
	public GameObject skybox_moon;
	
	//###################################
	//	MATERIALS
	//###################################
	public Material mat_waterCurrents;
	public Material mat_water;
	
	//###################################
	//	RANDOM EVENT VARIABLES
	//###################################
	public List<int> activeSettlementInfluenceSphereList = new List<int> ();
				
	//###################################
	//	DEBUG VARIABLES
	//###################################
	public int DEBUG_currentQuestLeg = 0;		// only in inspector for debug display
	public GameObject camera_Mapview;
	public bool DEBUG_MODE_ON = false;






//======================================================================================================================================================================
//======================================================================================================================================================================
//  INITIALIZE THE GAME WORLD
//======================================================================================================================================================================
//======================================================================================================================================================================

	
	// Use this for initialization
	void Awake ()
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		playerShip = GameObject.FindGameObjectWithTag ("playerShip");
		camera_titleScreen = GameObject.FindGameObjectWithTag ("camera_titleScreen");
		waterSurface = GameObject.FindGameObjectWithTag ("waterSurface");
		playerGhostRoute = GameObject.FindGameObjectWithTag ("playerGhostRoute");
		playerTrajectory = GameObject.FindGameObjectWithTag ("playerTrajectory");
		navigatorBeacon = GameObject.FindGameObjectWithTag ("navigatorBeacon");
		mainLightSource = GameObject.FindGameObjectWithTag ("main_light_source").GetComponent<Light> ();

		//Load all txt database files
		LoadMasterCrewRoster ();
		LoadCaptainsLogEntries ();
		LoadSettlementList ();
		LoadAdjustedSettlementLocations ();
		CreateSettlementsFromList ();
		LoadResourceList();
		currentSettlementGameObject = settlement_masterList_parent.transform.GetChild (0).gameObject;
		currentSettlement = currentSettlementGameObject.GetComponent<script_settlement_functions> ().thisSettlement;

		//Perform other initialization functions
		BuildWindZoneGameObjects ();
		BuildCurrentZoneGameObjects ();
		LoadWindRoses ();
		LoadWaterZonesFromFile ();
		SetInGameWindZonesToWindRoseData ();
		SetInGameWaterZonesToCurrentRoseData ();

		//Load the basic log entries into the log pool
		AddEntriesToCurrentLogPool (0);
		StartPlayerShipAtOriginCity ();
		playerShipVariables = playerShip.GetComponent<script_player_controls> ();
		GenerateCityLights ();
	}


//======================================================================================================================================================================
//======================================================================================================================================================================
//  THE REMAINDER OF THE SCRIPT IS ALL GLOBALLY ACCESSIBLE FUNCTIONS
//======================================================================================================================================================================
//======================================================================================================================================================================



	//====================================================================================================
	//      GEOSPATIAL / PROJECTION FUNCTIONS
    //====================================================================================================
	
		
	public Vector2 Convert_WebMercator_UnityWorld (Vector2 WTM_Coordinate)
	{

		Vector2 convertedCoordinate;
		convertedCoordinate.x = (WTM_Coordinate.x - (rasterMapOriginMeter.x - unityOrigin)) / unityWorldUnitResolution;
		convertedCoordinate.y = (WTM_Coordinate.y - (rasterMapOriginMeter.y - unityOrigin)) / unityWorldUnitResolution;	
		//Debug.Log (convertedCoordinate.x + " : " + convertedCoordinate.y);
		//Debug.Log (WTM_Coordinate.x + " : " + WTM_Coordinate.y);
		return convertedCoordinate;	
	}
	public Vector3 Convert_UnityWorld_WebMercator (Vector3 unity_coordinate)
	{
		
		Vector3 convertedCoordinate;
		convertedCoordinate.x = (unity_coordinate.x * unityWorldUnitResolution) + (rasterMapOriginMeter.x - unityOrigin);
		convertedCoordinate.y = (unity_coordinate.z * unityWorldUnitResolution) + (rasterMapOriginMeter.y - unityOrigin);
		convertedCoordinate.z = unity_coordinate.y;
		return convertedCoordinate;	
	}
	public Vector2 ConvertWGS1984ToWebMercator (Vector2 WGSLongLat)
	{

		
		if ((Math.Abs (WGSLongLat.x) > 180 || Math.Abs (WGSLongLat.y) > 90))
			return Vector2.zero;
			
		double num = WGSLongLat.x * 0.017453292519943295;
		double x = 6378137.0 * num;
		double a = WGSLongLat.y * 0.017453292519943295;
			
		WGSLongLat.x = (float)x;
		WGSLongLat.y = (float)(3189068.5 * Math.Log ((1.0 + Math.Sin (a)) / (1.0 - Math.Sin (a))));
			
		return WGSLongLat;
		
	}
	public Vector2 ConvertWebMercatorToWGS1984 (Vector2 MercatorEastNorth)
	{
		double mercatorX_lon = MercatorEastNorth.x; 
		double mercatorY_lat = MercatorEastNorth.y;
		//if (Math.Abs(mercatorX_lon) < 180 && Math.Abs(mercatorY_lat) < 90)
		//		return Vector2.zero;
		
		//if ((Math.Abs(mercatorX_lon) > 20037508.3427892) )
		//TODO figure out what to do if the coordinate is OUTSIDE the extents of the projection
		//TODO	--E.G. if we sail through the north pole and start heading south--unity world coordinates need to reflect this
		//TODO	--We shouldn't need to worry about this unless we do globe circumnavigation--which we aren't
		//If we are outside the northern extent
		//if(Math.Abs(mercatorY_lat) > 20037508.3427892)
		//	mercatorY_lat = 20037508.3427892 - (mercatorY_lat - 20037508.3427892) - 1;
		//else if (Math.Abs(mercatorY_lat) < -20037508.3427892)
		
		double x = mercatorX_lon;
		double y = mercatorY_lat;
		double num3 = x / 6378137.0;
		double num4 = num3 * 57.295779513082323;
		double num5 = Math.Floor ((double)((num4 + 180.0) / 360.0));
		double num6 = num4 - (num5 * 360.0);
		double num7 = 1.5707963267948966 - (2.0 * Math.Atan (Math.Exp ((-1.0 * y) / 6378137.0)));
		mercatorX_lon = num6;
		mercatorY_lat = num7 * 57.295779513082323;
		
		return new Vector2 ((float)mercatorX_lon, (float)mercatorY_lat);
	}    
	public float GetDistanceBetweenTwoLatLongCoordinates (Vector2 lonXlatY_A, Vector2 lonXlatY_B)
	{
		//This distance formula uses a Haversine formula to calculate the distance
		//It assumes a spherical earth rather than ellipsoidal which can give distance errors of roughly 0.3%
		//	--source:www.movable-type.co.uk/scripts/latlong.html -- accessed May 24 2016
		//We need to make sure our angles are in radians
		float latA = Mathf.Deg2Rad * lonXlatY_A.y;
		float latB = Mathf.Deg2Rad * lonXlatY_B.y;
		float changeOfLat = (lonXlatY_B.y - lonXlatY_A.y) * Mathf.Deg2Rad;
		float changeOfLon = (lonXlatY_B.x - lonXlatY_A.x) * Mathf.Deg2Rad;
		float earthRadius = 6371000f; // in meters
	
		//This is the Haversine implementation
		float a = Mathf.Pow (Mathf.Sin (changeOfLat / 2), 2) + 
			Mathf.Cos (latA) * Mathf.Cos (latB) * 
			Mathf.Pow (Mathf.Sin (changeOfLon / 2), 2);
		float c = 2 * Mathf.Atan2 (Mathf.Sqrt (a), Mathf.Sqrt (1 - a));
		float distance = earthRadius * c;
	
		//return the distance
		return distance;
	}


    
    //====================================================================================================
	//      CSV / DATA LOADING FUNCTIONS
    //====================================================================================================

	public void LoadSettlementList ()
	{
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { '@' };
		char[] recordDelimiter = new char[] { '_' };
		
		string filename = "settlement_list_newgame";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		//subtract one to account for the header line
		settlement_masterList = new Settlement[fileByLine.Length - 1];
		//start at index 1 to skip the record headers we have to then subtract 
		//one when adding NEW settlements to the list to ensure we start at ZERO and not ONE
		for (int lineCount = 1; lineCount < fileByLine.Length; lineCount++) {
			//Debug.Log("-->" + fileByLine[lineCount]);
			string[] records = fileByLine [lineCount].Split (lineDelimiter, StringSplitOptions.None);
			//Debug.Log (records[1] + "  " + records[2] + " " + records[3] + " " + records[4]);
			//NAME | LAT LONG | POPULATION | ELEVATION
			int id;
			if (records [0] == null)
				id = -1;
			else
				id = int.Parse (records [0]);
			string name;
			if (records [1] == null)
				name = "NO NAME";
			else
				name = records [1]; 
			Vector2 longXlatY;
			try {
				longXlatY = new Vector2 (float.Parse (records [3]), float.Parse (records [2]));
			} catch {
				longXlatY = Vector2.zero;
			}
			float elevation;
			try {
				elevation = float.Parse (records [4]);
			} catch {
				elevation = 0f;
			}
			int population;
			population = int.Parse (records [5]);
			settlement_masterList [lineCount - 1] = new Settlement (id, name, longXlatY, elevation, population);
			
			//Grab the networks it belongs to
			//List<int> networks = new List<int>();
			string[] parsedNetworks = records [7].Split (recordDelimiter, StringSplitOptions.None);
			foreach (string networkID in parsedNetworks)
				settlement_masterList [lineCount - 1].networks.Add (int.Parse (networkID));
			
			//load settlement's in/out network taxes
			settlement_masterList [lineCount - 1].tax_neutral = float.Parse (records [9]);
			settlement_masterList [lineCount - 1].tax_network = float.Parse (records [10]);
			//load the settlement type, e.g. port, no port
			settlement_masterList [lineCount - 1].typeOfSettlement = int.Parse (records [26]);
			//add resources to settlement (records length - 2 is confusing, but there are items after the last resource--can probably change this later)
			for (int recordIndex = 11; recordIndex < records.Length - 3; recordIndex++) {
				settlement_masterList [lineCount - 1].cargo [recordIndex - 11].probabilityOfAvailability = float.Parse (records [recordIndex]);
				//TODO The probability values are 1-100 and population affects the amount
				//  Population/2 x (probabilityOfResource/100)
				float amount = (settlement_masterList [lineCount - 1].population / 2) * (settlement_masterList [lineCount - 1].cargo [recordIndex - 11].probabilityOfAvailability / 1.5f);
				settlement_masterList [lineCount - 1].cargo [recordIndex - 11].amount_kg = amount;
			}
			//Add model/prefab name to settlement
			settlement_masterList [lineCount - 1].prefabName = records [records.Length - 2];
			Debug.Log ("********PREFAB NAME:     " + settlement_masterList [lineCount - 1].prefabName);
			//Add description to settlement
			settlement_masterList [lineCount - 1].description = records [records.Length - 1];
			
			//Debug.Log (settlement_masterList[lineCount-1].ToString());
			//Vector2 test = ConvertWGS1984ToWebMercator(longXlatY);
			//Debug.Log (records[1] + " : " + test.x + " , " + test.y);
			
			
		}		

	}

    public void LoadWindRoses ()
	{
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { ',' };
		
		string filename = "windroses_january";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		
		//For each line of the wind rose file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			//Debug.Log("-->" + fileByLine[lineCount]);
			string[] records = fileByLine [row].Split (lineDelimiter, StringSplitOptions.None);
			//Now loop through each column of the line and assign it to a windrose within January
			for (int col = 0; col < records.Length/2; col++) {
				float direction = float.Parse (records [col * 2]);//there are double the amount of columns in the file--these formulas account for that
				float speed = float.Parse (records [(col * 2) + 1]);
				windrose_January [col, row] = new WindRose (direction, speed); 
				//Debug.Log (col + " " + row + "   :   " + windrose_January[col,row].direction + " -> " + windrose_January[col,row].speed);
			}
		}
	}

	public void LoadWaterZonesFromFile ()
	{
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { ',' };
		
		string filename = "waterzones_january";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		
		//For each line of the wind rose file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			//Debug.Log("-->" + fileByLine[lineCount]);
			string[] records = fileByLine [row].Split (lineDelimiter, StringSplitOptions.None);
			//Now loop through each column of the line and assign it to a windrose within January
			for (int col = 0; col < records.Length/2; col++) {
				float direction = float.Parse (records [col * 2]);//there are double the amount of columns in the file--these formulas account for that
				float speed = float.Parse (records [(col * 2) + 1]);
				currentRose_January [col, row] = new CurrentRose (direction, speed); 
				//Debug.Log (col + " " + row + "   :   " + currentRose_January[col,row].direction + " -> " + currentRose_January[col,row].speed);
			}
		}
	}
	public void LoadCaptainsLogEntries ()
	{
		
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { '@' };
		string filename = "captains_log_database";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		
		captainsLogEntries = new CaptainsLogEntry[fileByLine.Length];
		//For each line of the wind rose file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			//Debug.Log (captainsLogEntries.Length + "  :  " + row);
			string[] records = fileByLine [row].Split (lineDelimiter, StringSplitOptions.None);
			captainsLogEntries [row] = new CaptainsLogEntry (int.Parse (records [0]), records [1]);
			
		}
		
		//Debugging
		//for (int i = 0; i < captainsLogEntries.Length; i++)
		//Debug.Log (captainsLogEntries[i].settlementID + "  :  " + captainsLogEntries[i].logEntry);
	}	

	//This loads the main quest line from a CSV file in the resources
	public MainQuestLine LoadMainQuestLine ()
	{
		MainQuestLine mainQuest = new MainQuestLine ();
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { '@' };
		char[] lineDelimiterB = new char[] { '_' };
		string filename = "main_questline_database";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		
		//start at index 1 to skip the record headers
		//For each line of the main quest file (the row)
		for (int row = 1; row < fileByLine.Length; row++) {
			string[] records = fileByLine [row].Split (lineDelimiter, StringSplitOptions.None);
			//Debug.Log (row);
			//Debug.Log
			//let's parse out all the crew roster changes
			//Debug.Log ("LOADQUEST--Leg: " + records[0] + " )( removals: " + records[6]);
			string[] crewRosterAdd = records [5].Split (lineDelimiterB, StringSplitOptions.None);
			string[] crewRosterRemove = records [6].Split (lineDelimiterB, StringSplitOptions.None);
			string[] mentionedSpots = records [4].Split (lineDelimiterB, StringSplitOptions.None);
			
			List<int> crewToAdd = new List<int> ();
			foreach (string id in crewRosterAdd) {
				crewToAdd.Add (int.Parse (id));
			}
			List<int> crewToRemove = new List<int> ();
			foreach (string id in crewRosterRemove) {
				crewToRemove.Add (int.Parse (id));
			}
			List<int> mentionedPlaces = new List<int> ();
			foreach (string id in mentionedSpots) {
				mentionedPlaces.Add (int.Parse (id));
			}
			//now let's see if we're on the last segment of the questline
			bool isEnd = false;
			if (row == fileByLine.Length - 1)
				isEnd = true;
			//Debug.Log("***************************");
			//Debug.Log (records[1]);
			//now add the segment to the main questline
			mainQuest.questSegments.Add (new QuestSegment (int.Parse (records [0]), int.Parse (records [1]), records [2], records [3], crewToAdd, crewToRemove, isEnd, mentionedPlaces));
		}
		
		return mainQuest;
	}

	public void LoadMasterCrewRoster ()
	{
		
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { '@' };
		
		string filename = "crewmembers_database";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		
		//start at index 1 to skip the record headers
		//For each line of the main quest file (the row)
		for (int row = 1; row < fileByLine.Length; row++) {
			string[] records = fileByLine [row].Split (lineDelimiter, StringSplitOptions.None);

			bool isKillable = false;
			bool isPartOfMainQuest = false;
			if (int.Parse (records [6]) == 1)
				isKillable = true;
			if (int.Parse (records [7]) == 1)
				isPartOfMainQuest = true;
			//Let's add a crewmember to the master roster
			masterCrewList.Add (new CrewMember (int.Parse (records [0]), records [1], int.Parse (records [2]), int.Parse (records [3]), int.Parse (records [4]), records [5], isKillable, isPartOfMainQuest));
		}
		
	}

	public void LoadAdjustedSettlementLocations ()
	{
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { ',' };
		int currentID = 0;
		string filename = "settlement_unity_position_offsets";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		
		for (int row = 0; row < fileByLine.Length; row++) {
			string[] records = fileByLine [row].Split (lineDelimiter, StringSplitOptions.None);
			currentID = int.Parse (records [0]);
			Settlement thisSettlement = GetSettlementFromID (currentID);
			thisSettlement.adjustedGamePosition = new Vector3 (float.Parse (records [1]), float.Parse (records [2]), float.Parse (records [3]));
			thisSettlement.eulerY = float.Parse (records [4]);
		}
		
	}
	
	public string TryLoadFromGameFolder (string filename)
	{
		try {
			WWW localFile = new WWW ("file://" + Application.dataPath + "/" + filename + ".txt");
			
			while (!localFile.isDone) {
				Debug.Log ("Progress of Load File: " + localFile.progress);
			}
			Debug.Log (Application.dataPath + "/" + filename + ".txt");
			Debug.Log (localFile.text);
			if (localFile.text == "") {
				TextAsset file = (TextAsset)Resources.Load (filename, typeof(TextAsset));
				return file.text;
			} 
			return localFile.text;
			
		} catch (Exception error) {
			Debug.Log ("Sorry! No file: " + filename + " was found in the game directory '" + Application.dataPath + "' or the save file is corrupt!\nError Code: " + error);
			ShowANotificationMessage ("Sorry! No file: " + filename + " was found in the game directory '" + Application.dataPath + "' or the save file is corrupt!\nError Code: " + error);
			TextAsset file = (TextAsset)Resources.Load (filename, typeof(TextAsset));
			return file.text;
		}
	
	}

	public void LoadResourceList ()
	{
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { '@' };
		string filename = "resource_list";
		
		string filetext = TryLoadFromGameFolder (filename);
		string[] fileByLine = filetext.Split (splitFile, StringSplitOptions.None);
		
		
		//start at index 1 to skip the record headers we have to then subtract 
		for (int lineCount = 1; lineCount < fileByLine.Length; lineCount++) {
			string[] records = fileByLine [lineCount].Split (lineDelimiter, StringSplitOptions.None);
			masterResourceList.Add(new MetaResource(records[1],int.Parse(records[0]), records[2]));
		}
	}	

	public bool LoadSavedGame ()
	{
		PlayerJourneyLog loadedJourney = new PlayerJourneyLog ();
		Ship ship = playerShipVariables.ship;
		
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		char[] lineDelimiter = new char[] { ',' };
		char[] recordDelimiter = new char[] { '_' };
		
		//Look for a save game file and tell the player if none is found.
		WWW saveFile;
		string saveText;
		try {
			saveFile = new WWW ("file://" + Application.dataPath + "/player_save_game.txt");
			saveText = System.IO.File.ReadAllText(Application.persistentDataPath + "/player_save_game.txt");	
			while (!saveFile.isDone) {
				Debug.Log ("Progress of Load File: " + saveFile.progress);
			}
		} catch (Exception error) {
			ShowANotificationMessage ("Sorry! No load game 'player_save_game.txt' was found in the game directory '" + Application.persistentDataPath + "' or the save file is corrupt!\nError Code: " + error);
			return false;
		}				
		//	TextAsset saveGame = (TextAsset)Resources.Load("player_save_game", typeof(TextAsset));
		string[] fileByLine = saveText.Split (splitFile, StringSplitOptions.None);
		Debug.Log ("file://" + Application.persistentDataPath + "/player_save_game.txt");
		Debug.Log(saveText);

		if(fileByLine.Length == 0) return false;
		//start at index 1 to skip the record headers we have to then subtract 
			//one when adding NEW entries to the list to ensure we start at ZERO and not ONE
			//all past routes will be stored as text, but the last route(last line of file) will also be done this way, but will additionally be parsed out for editing in-game values
			for (int lineCount = 1; lineCount < fileByLine.Length; lineCount++) {
				string[] records = fileByLine [lineCount].Split (lineDelimiter, StringSplitOptions.None);
					
				//First Add the basic route
				Vector3 origin = new Vector3 (float.Parse (records [2]), float.Parse (records [3]), float.Parse (records [4]));
				Vector3 destination = new Vector3 (float.Parse (records [5]), float.Parse (records [6]), float.Parse (records [7]));
				float numOfDays = float.Parse (records [1]);
					
				loadedJourney.routeLog.Add (new PlayerRoute (origin, destination, numOfDays));
					
				//Next add the cargo manifest
				string CSVcargo = "";
				for (int i = 8; i < 23; i++) {
					CSVcargo += "," + records [i];
				}
				loadedJourney.cargoLog.Add (CSVcargo);
					
				//Next add the other attributes string
				string CSVotherAtt = "";
				for (int i = 23; i < 39; i++) {
					CSVotherAtt += "," + records [i];
				}
				loadedJourney.otherAttributes.Add (CSVotherAtt);
					
				//Update Ship Position
				string[] XYZ = records [27].Split (recordDelimiter, StringSplitOptions.None);
				loadedJourney.routeLog [loadedJourney.routeLog.Count - 1].UnityXYZEndPoint = new Vector3 (float.Parse (XYZ [0]), float.Parse (XYZ [1]), float.Parse (XYZ [2]));
					
			}
			playerShipVariables.journey = loadedJourney;
				
			//Now use the last line of data to update the current player status and load the game
			string[] playerVars = fileByLine [fileByLine.Length - 1].Split (lineDelimiter, StringSplitOptions.None);
				
			//Update in game Time
			ship.totalNumOfDaysTraveled = float.Parse (playerVars [1]);
			//Update Sky to match time
			playerShipVariables.UpdateDayNightCycle (IS_NOT_NEW_GAME);
				
			//Update all Cargo Holds
			int fileStartIndex = 8;
			foreach (Resource resource in ship.cargo) {
				resource.amount_kg = float.Parse (playerVars [fileStartIndex]);
				fileStartIndex++;
			}
				
			//Update all Crewmen
			List<CrewMember> updatedCrew = new List<CrewMember> ();
			string[] parsedCrew = playerVars [26].Split (recordDelimiter, StringSplitOptions.None);
			foreach (string crewID in parsedCrew) {
				updatedCrew.Add (GetCrewMemberFromID (int.Parse (crewID)));
			}
			ship.crewRoster = updatedCrew;
				
			//Update Ship Position
			string[] parsedXYZ = playerVars [27].Split (recordDelimiter, StringSplitOptions.None);
			playerShip.transform.position = new Vector3 (float.Parse (parsedXYZ [0]), float.Parse (parsedXYZ [1]), float.Parse (parsedXYZ [2]));
			
			//Update Current Quest Leg
			ship.mainQuest.currentQuestSegment = int.Parse (playerVars [28]);
				
			//Update Ship Health
			ship.health = float.Parse (playerVars [29]);
				
			//Update player clout
			ship.playerClout = float.Parse (playerVars [30]);
				
			//Update player networks
			List<int> loadedNetworks = new List<int> ();
			string[] parsedNetworks = playerVars [31].Split (recordDelimiter, StringSplitOptions.None);
			foreach (string netID in parsedNetworks) {
				loadedNetworks.Add (int.Parse (netID));
			}
			ship.networks = loadedNetworks;
				
			//Update player starving and thirsty day counters
			playerShipVariables.dayCounterStarving = int.Parse (playerVars [32]);
			playerShipVariables.dayCounterThirsty = int.Parse (playerVars [33]);
				
			//Update Currency
			ship.currency = int.Parse (playerVars [34]);
				
			//Add any Loans
			//--If Loan exists then add otherwise make null
			if (int.Parse (playerVars [35]) != -1) {
				//TODO right now we aren't storing the loan variable properly so relaly a loaded game means a player can cheat currently--whoops--and have plenty of time to pay it back and their interest disappears. Need to put on fix list
				ship.currentLoan = new Loan (int.Parse (playerVars [35]), 0f, 0f, int.Parse (playerVars [36]));
			} else {
				ship.currentLoan = null;
			}
				
			//Add Current Navigator Destination
			int targetID = int.Parse (playerVars [37]);
			if (targetID != -1) {
				ship.currentNavigatorTarget = targetID;
				//change location of beacon
				Vector3 location = Vector3.zero;
				for (int x = 0; x < settlement_masterList_parent.transform.childCount; x++) 
					if (settlement_masterList_parent.transform.GetChild (x).GetComponent<script_settlement_functions> ().thisSettlement.settlementID == targetID)
						location = settlement_masterList_parent.transform.GetChild (x).position;
				navigatorBeacon.transform.position = location;
				navigatorBeacon.GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (location.x, 0, location.z));
				navigatorBeacon.GetComponent<LineRenderer> ().SetPosition (1, location + new Vector3 (0, 400, 0));
				playerShipVariables.UpdateNavigatorBeaconAppearenceBasedOnDistance ();
			} else {
				ship.currentNavigatorTarget = -1;
			}
			//Add the Known Settlements

			string[] parsedKnowns = playerVars [38].Split (recordDelimiter, StringSplitOptions.None);
			//Debug.Log ("PARSED KNOWNS: " + playerVars[38]);
			foreach (string settlementID in parsedKnowns) {
				//Debug.Log ("PARSED KNOWNS: " + settlementID);
				ship.playerJournal.knownSettlements.Add (int.Parse (settlementID));
			}
			//Add Captains Log
			string restoreCommasAndNewLines = playerVars [39].Replace ('^', ',');
			currentCaptainsLog = restoreCommasAndNewLines.Replace ('*', '\n');
			//Debug.Log (currentCaptainsLog);
		


		//If no errors then return true
		return true;
	}		



    //====================================================================================================
	//      GAMEOBJECT BUILDING TO POPULATE WORLD FUNCTIONS
    //====================================================================================================

	public void CreateSettlementsFromList ()
	{
		settlement_masterList_parent = Instantiate (new GameObject (), Vector3.zero, transform.rotation) as GameObject;
		settlement_masterList_parent.name = "Settlement Master List";
		foreach (Settlement settlement in settlement_masterList) {
			GameObject currentSettlement;
			//Here we add a model/prefab to the settlement based on it's
			try { 
				//Debug.Log ("BEFORE TRYING TO LOAD SETTLEMENT PREFAB    " + settlement.prefabName + "  :   " + settlement.name);
				currentSettlement = Instantiate (Resources.Load ("City Models/" + settlement.prefabName, typeof(GameObject))) as GameObject;
				//Debug.Log ("AFTER TRYING TO LOAD SETTLEMENT PREFAB    " + settlement.prefabName);
			} catch {
				currentSettlement = Instantiate (Resources.Load ("City Models/PF_settlement", typeof(GameObject))) as GameObject;
			}
			//We need to check if the settlement has an adjusted position or not--if it does then use it, otherwise use the given lat long coordinate
			if (settlement.adjustedGamePosition.x == 0) {
				Vector2 tempXY = Convert_WebMercator_UnityWorld (ConvertWGS1984ToWebMercator (settlement.location_longXlatY));
				Vector3 tempPos = new Vector3 (tempXY.x, terrain.GetComponent<Terrain> ().SampleHeight (new Vector3 (tempXY.x, 0, tempXY.y)), tempXY.y);
				currentSettlement.transform.position = tempPos;
			} else {
				currentSettlement.transform.position = settlement.adjustedGamePosition;
				currentSettlement.transform.eulerAngles = new Vector3 (0, settlement.eulerY, 0);
			}
			currentSettlement.tag = "settlement"; 
			currentSettlement.name = settlement.name;
			currentSettlement.layer = 8;
			//Debug.Log ("*********************************************  <<>>>" + currentSettlement.name + "   :   " + settlement.settlementID);
			currentSettlement.GetComponent<script_settlement_functions> ().thisSettlement = settlement;
			currentSettlement.transform.SetParent (settlement_masterList_parent.transform);
			settlement.theGameObject = currentSettlement;
		}
	}	
   
	public void BuildWindZoneGameObjects ()
	{
		//We need to create a gridded system of GameObjects to represent the windzones
		//It should be a Main Parent GameObject with a series of zones with a rotater and particle system
		//	--WindZones
		//		--0_0
		//			--Particle Rotater
		//				--Wind particle system
		windrose_January = new WindRose[64, 32];
		windZoneParent = new GameObject ();
		windZoneParent.name = "WindZones Parent Object";
		float originX = 0;
		float originZ = 4096; //Unity's 2D top-down Y axis is Z
		float zoneHeight = 128;
		float zoneWidth = 64;
		
		for (int col = 0; col < windrose_January.GetLength(0); col++) {
			for (int row = 0; row < windrose_January.GetLength(1); row++) {
				GameObject newZone = new GameObject ();
				GameObject rotater = new GameObject ();
				GameObject windParticles;// = Instantiate(new GameObject(), Vector3.zero, transform.rotation) as GameObject;
				newZone.transform.position = new Vector3 (originX + (col * zoneWidth), 0, originZ - (row * zoneHeight));
				newZone.transform.localScale = new Vector3 (zoneWidth, 1f, zoneHeight);
				newZone.name = col + "_" + row;
				newZone.tag = "windDirectionVector";
				newZone.AddComponent<BoxCollider> ();
				newZone.GetComponent<BoxCollider> ().isTrigger = true;
				newZone.GetComponent<BoxCollider> ().size = new Vector3 (.95f, 10, .95f);
				newZone.layer = 20;
				rotater.AddComponent<script_WaterWindCurrentVector> ();
				rotater.transform.position = newZone.transform.position;
				rotater.transform.rotation = newZone.transform.rotation;
				rotater.name = "Particle Rotater";
				windParticles = Instantiate (Resources.Load ("PF_windParticles", typeof(GameObject))) as GameObject;
				windParticles.transform.position = new Vector3 (newZone.transform.position.x, newZone.transform.position.y, newZone.transform.position.z - (zoneHeight / 2));

				windParticles.transform.parent = rotater.transform;
				rotater.transform.parent = newZone.transform;
				newZone.transform.parent = windZoneParent.transform;
				rotater.SetActive (false);
			}
		}
	}
	
	public void BuildCurrentZoneGameObjects ()
	{
		//We need to create a gridded system of GameObjects to represent the windzones
		//It should be a Main Parent GameObject with a series of zones with a rotater and particle system
		//	--WindZones
		//		--0_0
		//			--Particle Rotater
		//				--Wind particle system
		currentRose_January = new CurrentRose[128, 64];
		currentZoneParent = new GameObject ();
		currentZoneParent.name = "CurrentZones Parent Object";
		float originX = 0;
		float originZ = 4096; //Unity's 2D top-down Y axis is Z
		float zoneHeight = 64;
		float zoneWidth = 32;
		
		for (int col = 0; col < currentRose_January.GetLength(0); col++) {
			for (int row = 0; row < currentRose_January.GetLength(1); row++) {
				GameObject newZone = new GameObject ();
				GameObject rotater = new GameObject ();
				GameObject currentParticles;// = Instantiate(new GameObject(), Vector3.zero, transform.rotation) as GameObject;
				newZone.transform.position = new Vector3 (originX + (col * zoneWidth), 0, originZ - (row * zoneHeight));
				newZone.transform.localScale = new Vector3 (zoneWidth, 1f, zoneHeight);
				newZone.name = col + "_" + row;
				newZone.tag = "currentDirectionVector";
				newZone.AddComponent<BoxCollider> ();
				newZone.GetComponent<BoxCollider> ().isTrigger = true;
				newZone.GetComponent<BoxCollider> ().size = new Vector3 (.95f, 10, .95f);
				newZone.layer = 19;
				rotater.AddComponent<script_WaterWindCurrentVector> ();
				rotater.transform.position = newZone.transform.position;
				rotater.transform.rotation = newZone.transform.rotation;
				rotater.name = "Particle Rotater";
				currentParticles = Instantiate (Resources.Load ("PF_currentParticles", typeof(GameObject))) as GameObject;
				currentParticles.transform.position = new Vector3 (newZone.transform.position.x, newZone.transform.position.y, newZone.transform.position.z - (zoneHeight / 2));
				currentParticles.transform.Translate (-transform.forward * .51f, Space.Self);
				currentParticles.transform.parent = rotater.transform;
				rotater.transform.parent = newZone.transform;
				newZone.transform.parent = currentZoneParent.transform;
				rotater.SetActive (false);
				
			}
		}
	}    
    
    public void GenerateCityLights ()
	{
		for (int i = 0; i <  settlement_masterList_parent.transform.childCount; i++) {
			GameObject currentCityLight = Instantiate (Resources.Load ("PF_cityLights", typeof(GameObject))) as GameObject;
			currentCityLight.transform.position = settlement_masterList_parent.transform.GetChild (i).position;
			currentCityLight.transform.parent = cityLightsParent.transform;
		}
	}
	
	public void SetInGameWindZonesToWindRoseData ()
	{
		
		//For each of the zones in the Wind Zone parent GameObject, we need to loop through them
		//	--and set the rotation of each to match the windrose data
		for (int currentZone = 0; currentZone < windZoneParent.transform.childCount; currentZone++) {
			string zoneID = windZoneParent.transform.GetChild (currentZone).name;
			//Debug.Log(zoneID);
			int col = int.Parse (zoneID.Split ('_') [0]);
			int row = int.Parse (zoneID.Split ('_') [1]);
		
			//Find the matching wind rose in the month of january
			float speed = 1;
			float direction = UnityEngine.Random.Range (0f, 90f);
			if (windrose_January [col, row] != null) {
				speed = windrose_January [col, row].speed;
				direction = windrose_January [col, row].direction;
			}
			windZoneParent.transform.GetChild (currentZone).GetChild (0).transform.eulerAngles = new Vector3 (0, -1f * (direction - 90f), 0); //We subtract 90 because Unity's 'zero' is set at 90 degrees and Unity's positive angle is CW and not CCW like normal trig
			windZoneParent.transform.GetChild (currentZone).GetChild (0).GetComponent<script_WaterWindCurrentVector> ().currentMagnitude = speed;
			//if (speed == 0) windZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(false);
			//else windZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(true);
		}
	
	}
	
	public void SetInGameWaterZonesToCurrentRoseData ()
	{
	
		//For each of the zones in the Wind Zone parent GameObject, we need to loop through them
		//	--and set the rotation of each to match the windrose data
		for (int currentZone = 0; currentZone < currentZoneParent.transform.childCount; currentZone++) {
			string zoneID = currentZoneParent.transform.GetChild (currentZone).name;
			//Debug.Log(zoneID);
			int col = int.Parse (zoneID.Split ('_') [0]);
			int row = int.Parse (zoneID.Split ('_') [1]);
			
			//Find the matching current rose in the month of january
			float speed = 1;
			float direction = UnityEngine.Random.Range (0f, 90f);
			if (currentRose_January [col, row] != null) {
				speed = currentRose_January [col, row].speed;
				direction = currentRose_January [col, row].direction;
			}
			currentZoneParent.transform.GetChild (currentZone).GetChild (0).transform.eulerAngles = new Vector3 (0, -1f * (direction - 90f), 0); //We subtract 90 because Unity's 'zero' is set at 90 degrees and Unity's positive angle is CW and not CCW like normal trig
			currentZoneParent.transform.GetChild (currentZone).GetChild (0).GetComponent<script_WaterWindCurrentVector> ().currentMagnitude = speed;
			//if (speed == 0) currentZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(false);
			//else currentZoneParent.transform.GetChild (currentZone).GetChild(0).gameObject.SetActive(true);
			//Debug.Log ("Turning water on?");
		}
	
	}



    
    //====================================================================================================
	//      DATA SAVING FUNCTIONS
    //====================================================================================================
	public void SaveUserGameData (bool isRestart)
	{
		string delimitedData = playerShipVariables.journey.ConvertJourneyLogToCSVText ();
		Debug.Log (delimitedData);
		string filePath = Application.persistentDataPath + "/";
		
		string fileNameServer = "";
		if (DEBUG_MODE_ON)
			fileNameServer += "DEBUG_DATA_" + SystemInfo.deviceUniqueIdentifier + "_player_data_" + System.DateTime.UtcNow.ToString ("HH-mm-ss_dd_MMMM_yyyy") + ".csv";
		else
			fileNameServer += SystemInfo.deviceUniqueIdentifier + "_player_data_" + System.DateTime.UtcNow.ToString ("HH-mm-ss_dd_MMMM_yyyy") + ".csv";
		
		string fileName = "player_save_game.txt";
		
		//Adding a try/catch block around this write because if someone tries playing the game out of zip on mac--it throws an error that is unoticeable but also
		//causes the code to fall short and quit before saving to the server
		try {
			//save a backup before Joanna's edits
			System.IO.File.WriteAllText (Application.persistentDataPath + "/BACKUP-" + SystemInfo.deviceUniqueIdentifier + "_player_data_" + System.DateTime.UtcNow.ToString ("HH-mm-ss_dd_MMMM_yyyy") + ".csv", delimitedData);
			//Only save the game for loading if it's not a restart--otherwise if the player loads, it will load right where the player restarted the game
			if (!isRestart) System.IO.File.WriteAllText (Application.persistentDataPath + "/" + fileName, delimitedData);
			//TODO Temporary addition for joanna to remove the captains log from the server upload
			string fileToUpload = RemoveCaptainsLogForJoanna (delimitedData);
			System.IO.File.WriteAllText (Application.persistentDataPath + "/" + fileNameServer, fileToUpload);
			Debug.Log (Application.persistentDataPath);
		} catch (Exception e) {
			showSecondaryNotification = true;
			secondaryNotificationMessage = "ERROR: a backup wasn't saved at: " + Application.persistentDataPath + "  - which means it may not have uploaded either: " + e.Message;
		}
		//Only upload to the server is the DebugMode is OFF
		if (!DEBUG_MODE_ON)SaveUserGameDataToServer (filePath, fileNameServer);

	}
	
	public void SaveUserGameDataToServer (string localPath, string localFile)
	{
		Debug.Log ("Starting FTP");
		string user = "SamoGameBot";
		string pass = "%Mgn~WxH+CRzj>4Z";
		string host = "34.193.207.222";
		string initialPath = "";
	
		FileInfo file = new FileInfo (localPath + localFile);
		Uri address = new Uri ("ftp://" + host + "/" + Path.Combine (initialPath, file.Name));
		FtpWebRequest request = FtpWebRequest.Create (address) as FtpWebRequest;
		
		// Upload options:
		
		// Provide credentials
		request.Credentials = new NetworkCredential (user, pass);
		
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
		var fs = file.OpenRead ();
		
		try {
			// Stream to which file to be uploaded is written.
			var stream = request.GetRequestStream ();
			
			// Read from file stream 2KB at a time.
			contentLength = fs.Read (buffer, 0, bufferLength);
			
			// Loop until stream content ends.
			while (contentLength != 0) {
				//Debug.Log("Progress: " + ((fs.Position / fs.Length) * 100f));
				// Write content from file stream to FTP upload stream.
				stream.Write (buffer, 0, contentLength);
				contentLength = fs.Read (buffer, 0, bufferLength);
			}
			
			// Close file and request streams
			stream.Close ();
			fs.Close ();
		} catch (Exception e) {
			Debug.LogError ("Error uploading file: " + e.Message);
			showNotification = true;
			notificationMessage = "ERROR: No Upload--The server timed out or you currently do not have a stable internet connection\n" + e.Message;
			return;
		}
		
		Debug.Log ("Upload successful.");
		showNotification = true;
		notificationMessage = "File: '" + localFile + "' successfully uploaded to the server!";
	}
	
	//TODO: This is an incredibly specific function that won't be needed later
	public string RemoveCaptainsLogForJoanna (string file)
	{
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };
		string newFile = "";
		string[] fileByLine = file.Split (splitFile, StringSplitOptions.None);
		
		//For each line of the save file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			int index = fileByLine [row].LastIndexOf (",");
			newFile += fileByLine [row].Substring (0, index) + "\n";
			//Debug.Log (fileByLine [row]); 
			//Debug.Log (fileByLine [row].Substring (0, index));
		}	
		
		return newFile;
	
	}

	// TODO: Apparently this isn't hooked up anymore. Need to fix this tool so we can adjust current directions in the editor
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void SaveWaterCurrentZones ()
	{
		
		string waterRoseData = "";
		int rowCounter = 0;
		Transform waterZone;
		
		//Loop through all of the child objects of the current zone parent object
		//The parent stores them in a sequential list so every 40 objects represents a new line in the spread sheet csv file
		//The coordinate for the zones is 0,0 for the top left, ending with 39,39 on the bottom right
		for (int currentZone = 0; currentZone < currentZoneParent.transform.childCount; currentZone++) {
			waterZone = currentZoneParent.transform.GetChild (currentZone);
			waterRoseData += waterZone.GetChild (0).transform.localRotation.eulerAngles.y;
			waterRoseData += ",";
			waterRoseData += waterZone.GetChild (0).GetComponent<script_WaterWindCurrentVector> ().currentMagnitude;
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
		StreamWriter sw = new StreamWriter (@Application.persistentDataPath + "/" + "waterzones_january.txt");
		sw.Write (waterRoseData);
		sw.Close ();
		
	
	}

	// TODO: Apparently this isn't hooked up anymore. Need to fix this tool so we can adjust the settlement unity position offsets in the editor
	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void Tool_SaveCurrentSettlementPositionsToFile ()
	{
		string ID = "";
		string unityX = "";
		string unityY = "";
		string unityZ = "";
		string unityEulerY = "";
		string writeToFile = "";
		for (int i = 0; i < settlement_masterList_parent.transform.childCount; i++) {
			ID = settlement_masterList_parent.transform.GetChild (i).GetComponent<script_settlement_functions> ().thisSettlement.settlementID.ToString ();
			//if(ID == "309"){Debug.Log ("We At 309!!!!!!");}
			unityX = settlement_masterList_parent.transform.GetChild (i).transform.position.x.ToString ();
			unityY = settlement_masterList_parent.transform.GetChild (i).transform.position.y.ToString ();
			unityZ = settlement_masterList_parent.transform.GetChild (i).transform.position.z.ToString ();
			unityEulerY = settlement_masterList_parent.transform.GetChild (i).transform.eulerAngles.y.ToString ();
			string test = ((ID + "," + unityX + "," + unityY + "," + unityZ + "," + unityEulerY));
			//perform a quick check to make sure we aren't at the end of the file: if we are don't add a new line
			if (i != settlement_masterList_parent.transform.childCount - 1)
				test += "\n";
			writeToFile += test;
		}
		
		//Write the string to file now
		StreamWriter sw = new StreamWriter (@"H:\sailingwiththegods\Assets\Resources\settlement_unity_position_offsets.txt");
		sw.Write (writeToFile);
		sw.Close ();
	}

    //====================================================================================================
	//      PLAYER INITIALIZATION FUNCTIONS
    //====================================================================================================

	void StartPlayerShipAtOriginCity ()
	{
		//first set the origin city to the first available as a default
		GameObject originCity = settlement_masterList_parent.transform.GetChild (0).gameObject; 
		foreach (Transform child in settlement_masterList_parent.transform) {
			//if the settlement we want exists, then use it as the default instead
			if (child.name == "Samothrace") {
				originCity = child.gameObject;
				break;
			}
		}
		//now set the player ship to the origin city coordinate
		//!TODO This is arbotrarily set to samothrace right now
		playerShip.transform.position = new Vector3 (1939.846f, .23f, 2313.506f);
		//mainCamera.transform.position = new Vector3(originCity.transform.position.x, 30f, originCity.transform.position.z);
	}
	
	public void RestartGame ()
	{
        
		//Debug.Log ("Quest Seg: " + playerShipVariables.ship.mainQuest.currentQuestSegment);
		//First we need to save the game that just ended
		SaveUserGameData (true);
		//Then we need to re-initialize all the player's variables
		playerShipVariables.ship = new Ship ("Argo", 7.408f, 100, 500f);
        playerShipVariables.ship.networkID = 246;
        playerShipVariables.journey = new PlayerJourneyLog ();
		playerShipVariables.lastPlayerShipPosition = transform.position;
		playerShipVariables.ship.mainQuest = LoadMainQuestLine ();
		
		//Setup the day/night cycle
		playerShipVariables.UpdateDayNightCycle (IS_NEW_GAME);
		
		//initialize players ghost route
		playerShipVariables.UpdatePlayerGhostRouteLineRenderer (IS_NEW_GAME);
		
		//Reset Other Player Ship Variables
		playerShipVariables.numOfDaysTraveled = 0;
		playerShipVariables.numOfDaysWithoutProvisions = 0;
		playerShipVariables.numOfDaysWithoutWater = 0;
		playerShipVariables.dayCounterStarving = 0;
		playerShipVariables.dayCounterThirsty = 0;

        //Take player back to title screen
        //Debug.Log ("GOING TO TITLE SCREEN");
        camera_titleScreen.SetActive (true);
		bg_titleScreen.SetActive (true);
		bg_startScreen.SetActive (true);
		controlsLocked = true;
		isTitleScreen = true;
		RenderSettings.fog = false;
		FPVCamera.SetActive (false);
		isTitleScreen = true;
		runningMainGameGUI = false;
		
		
		//clear captains log
		currentCaptainsLog = "";
		
		GUI_PortMenu.SetActive(false);
		GUI_GameHUD.SetActive(false);



    }
    
	public void InitiateMainQuestLineForPlayer ()
	{
        Debug.Log("Main Quest TESTER");
        //For the argonautica, let's set the crew capacity to 30
        playerShipVariables.ship.crewCapacity = 30;

		//Now let's add all the initial crew from the start screen selection and start the first leg of the quest
		for (int i = 0; i < newGameAvailableCrew.Count; i++) {
			if (newGameCrewSelectList [i]){
				playerShipVariables.ship.crewRoster.Add (newGameAvailableCrew [i]);
				//Debug.Log (newGameCrewSelectList[i]);	
			}
		}
		//Debug.Log (playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd.Count + "<<<<<<<<<<<<<CREW");
		//	foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd){
		//		playerShipVariables.ship.crewRoster.Add (GetCrewMemberFromID(crewID));
		//	}
		playerShipVariables.ship.crew = playerShipVariables.ship.crewRoster.Count;
		
		//Let's increase the ships cargo capacity
		playerShipVariables.ship.cargo_capicity_kg = 1200f;
		
		//Let's increase the ships Provisions and water base to reflect 28 crew members
		playerShipVariables.ship.cargo [0].amount_kg = 300f;
		playerShipVariables.ship.cargo [1].amount_kg = 300f;

        //Increase the quest counter because the start screen takes care of the first leg

        Debug.Log(playerShipVariables.ship.mainQuest.currentQuestSegment);
		playerShipVariables.ship.mainQuest.currentQuestSegment++;
		
		//first show a window for the welcome message, and if there are any crew member changes, then let the player know.
		notificationMessage = playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionAtCompletion;
		showNotification = true;
		//Add this message to the captain's log
		playerShipVariables.ship.shipCaptainsLog.Add (new CaptainsLogEntry (playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID, playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionOfQuest));
		playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
		currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
		//Now add the mentioned places attached to this quest leg
		foreach (int i in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].mentionedPlaces) {
			//Make sure we don't add any null values--a -1 represents no mentions of any settlements
			if (i != -1)
				playerShipVariables.ship.playerJournal.AddNewSettlementToLog (i);
		}
        Debug.Log(playerShipVariables.ship.mainQuest.currentQuestSegment);
        //Then increment the questline to the in succession and update the player captains log with the new information for the next quest line
        playerShipVariables.ship.mainQuest.currentQuestSegment++;
		playerShipVariables.ship.shipCaptainsLog.Add (new CaptainsLogEntry (playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID, playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionOfQuest));
		playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
		currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
		//Now add the mentioned places attached to this quest leg
		foreach (int i in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].mentionedPlaces) {
			//Make sure we don't add any null values--a -1 represents no mentions of any settlements
			if (i != -1)
				playerShipVariables.ship.playerJournal.AddNewSettlementToLog (i);
		}
		//Now add the city name of the next journey quest to the players known settlements
		playerShipVariables.ship.playerJournal.AddNewSettlementToLog (playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID);
		//Now teleport the player ship to an appropriate location near the first target
		playerShip.transform.position = new Vector3 (1702.414f, playerShip.transform.position.y, 2168.358f);
		//Set the player's initial position to the new position
		playerShipVariables.lastPlayerShipPosition = playerShip.transform.position;
		
		//Setup Difficulty Level
		SetupBeginnerGameDifficulty ();

        Debug.Log(playerShipVariables.ship.mainQuest.currentQuestSegment);

        //Flag the main GUI scripts to turn on
        runningMainGameGUI = true;
	}
    
	public void FillNewGameCrewRosterAvailability ()
	{
		//We need to fill a list of 40 crewmembers for the player to choose from on a new game start
		//--The first set will come from the Argonautica, and the top of the list will be populated with necessary characters for the plot
		//--The remainder will be filled from the remaining available argonautica start crew and then randomly generated crew to choose from to create 40 members
		
		//initialize a fresh List of crew and corresponding array of 40 bools
		newGameAvailableCrew = new List<CrewMember> ();
		newGameCrewSelectList = new bool[40];
		
		//TODO FIX THIS LATER Let's remove the randomly generated crew--this is just a safety precaution--might not be needed.
		playerShipVariables.ship.crewRoster.Clear ();
		
		//First let's add all the MUST have crew
		int indexCounter = 0;
        Debug.Log(playerShipVariables.ship.mainQuest.questSegments.Count);
        Debug.Log(playerShipVariables.ship.mainQuest.questSegments[0]);

        foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd) {
			CrewMember currentMember = GetCrewMemberFromID (crewID);
			if (!currentMember.isKillable) {
				newGameAvailableCrew.Add (currentMember);
				//also make sure the matching list of bools is TRUE for quest crewman
				newGameCrewSelectList [indexCounter] = true;
				indexCounter++;
			}
		}
		
		//Now let's add all the optional crew from the Argonautica
		foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[0].crewmembersToAdd) {
			CrewMember currentMember = GetCrewMemberFromID (crewID);
			if (currentMember.isKillable) {
				newGameAvailableCrew.Add (currentMember);
			}
		}
		
		//Now let's add all the possible non-quest historical people for hire
		foreach (CrewMember thisMember in masterCrewList) {
			//make sure we don't go over 40 listings
			if (newGameAvailableCrew.Count == 40)
				break;
			
			if (!thisMember.isPartOfMainQuest) {
				newGameAvailableCrew.Add (thisMember);
			}
		}
		
		
		//Now let's add randomly generated crew to the list until the quota of 40 is fulfilled
		while (newGameAvailableCrew.Count < 40) {
			newGameAvailableCrew.Add (GenerateRandomCrewMembers (1) [0]);
		}

		
		
	}

	public void SetupBeginnerGameDifficulty ()
	{
		//Set difficulty level variables
		if (gameDifficulty_Beginner)
			camera_Mapview.GetComponent<Camera> ().enabled = true;
		else
			camera_Mapview.GetComponent<Camera> ().enabled = false;
	}

	public void LoadSavedGhostRoute ()
	{
		//For the loadgame function--it just fills the ghost trail with the routes that exist
		playerGhostRoute.GetComponent<LineRenderer> ().positionCount = playerShipVariables.journey.routeLog.Count;
		for (int routeIndex = 0; routeIndex < playerShipVariables.journey.routeLog.Count; routeIndex++) {
			Debug.Log ("GhostRoute Index: " + routeIndex);
			playerGhostRoute.GetComponent<LineRenderer> ().SetPosition (routeIndex, playerShipVariables.journey.routeLog [routeIndex].UnityXYZEndPoint - new Vector3 (0, playerShip.transform.position.y, 0));
			//set player last origin point for next route add on
			if (routeIndex == playerShipVariables.journey.routeLog.Count - 1) {
				playerShipVariables.travel_lastOrigin = playerShipVariables.journey.routeLog [routeIndex].UnityXYZEndPoint - new Vector3 (0, playerShip.transform.position.y);
				playerShipVariables.originOfTrip = playerShipVariables.journey.routeLog [routeIndex].UnityXYZEndPoint - new Vector3 (0, playerShip.transform.position.y);
			}
		}
	}    

    //====================================================================================================
	//     GUI ORIENTED FUNCTIONS
    //====================================================================================================   
    
    
 	public void GenerateProbableInfoListOfSettlementsInCurrentNetwork ()
	{
		List<Settlement> networkSettlements = new List<Settlement> ();
		int settlementListLimit = 0;
		//Debug.Log ("DEBUG: " + currentSettlement.settlementID);
		foreach (Settlement city in settlement_masterList) {
			if (city.name != currentSettlement.name && CheckForNetworkMatchBetweenTwoSettlements (currentSettlement.settlementID, city.settlementID) && settlementListLimit < 5) {
				int numOfResourcesToShow = 0;
				//determine how many resource hint slots this city will have based on influence TODO influence should be determined by population not a separate number
				for (int i = 0; i < 5; i++)
					if (currentSettlement.population >= UnityEngine.Random.Range (0f, 101f))
						numOfResourcesToShow ++;
						
				city.networkHintResources = new int[numOfResourcesToShow];
				//now fill in the available resource hint slots with random resources 0-14
				for (int i = 0; i < numOfResourcesToShow; i++)
					city.networkHintResources [i] = UnityEngine.Random.Range (0, 15); //15 because range is exclusive
				//Now add the city to the list
				networkSettlements.Add (city);
				playerShipVariables.ship.playerJournal.AddNewSettlementToLog (city.settlementID);					
				settlementListLimit++;
			}
		}
	
		currentNetworkSettlements = networkSettlements;
	}
	
	public void GenerateListOfAvailableCrewAtCurrentPort ()
	{
		currentlyAvailableCrewMembersAtPort = GenerateRandomCrewMembers (5);
	}

	public List<CrewMember> GenerateRandomCrewMembers (int numberOfCrewmanNeeded)
	{
		//This function pulls from the list of available crewmembers in the world and selects random crewman from that list of a defined
		//	--size that isn't already on board the ship and returns it. This may not return a full list if the requested number is too high--it will return
		//	--the most it has available
		List<CrewMember> availableCrew = new List<CrewMember> ();
		int numOfIterations = 0;
		while (numberOfCrewmanNeeded != availableCrew.Count) {
			CrewMember thisMember = masterCrewList [UnityEngine.Random.Range (0, masterCrewList.Count)];
			if (!thisMember.isPartOfMainQuest) {
				//Now make sure this crewmember isn't already in the current crew
				foreach (CrewMember boatCrewMan in playerShipVariables.ship.crewRoster) {
					//If we don't have a match, add the crewman to the list and break from the loop
					if (thisMember.ID != boatCrewMan.ID) {
						availableCrew.Add (thisMember);
						break;
					}
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
    

    
    //====================================================================================================
	//    LOOKUP / DICTIONARY FUNCTIONS
    //====================================================================================================   

	public string GetJobClassEquivalency (int jobCode)
	{
		//This function simply returns a predefined string based on the number code of a crewmembers job
		//	--based on their constant values held in the global variables 
		string title = "";
		switch (jobCode) {
		case GV_CONST.CREWTYPE_SAILOR:      title = "Sailor";   break;
		case GV_CONST.CREWTYPE_WARRIOR:     title = "Warrior";  break;
		case GV_CONST.CREWTYPE_SLAVE:       title = "Slave";    break;
		case GV_CONST.CREWTYPE_PASSENGER:   title = "Passenger";break;
		case GV_CONST.CREWTYPE_NAVIGATOR:   title = "Navigator";break;
		case GV_CONST.CREWTYPE_ASSISTANT:   title = "Assistant";break;
		case GV_CONST.CREWTYPE_GUIDE:       title = "Guide";    break;
		case GV_CONST.CREWTYPE_LAWYER:      title = "Lawyer";   break;
		case GV_CONST.CREWTYPE_ROYALTY:     title = "Royalty";  break;
		case GV_CONST.CREWTYPE_SEER:        title = "Seer";     break;
		}
		return title;
	}
	


	public string GetCloutTitleEquivalency (int clout)
	{
		//This function simply returns a predefined string based on the number value of the clout provided
		string title = "";
		if (clout > 1 && clout <= 499)          title = "Goatherd";
		else if (clout > 500 && clout <= 999)   title = "Farmer";
		else if (clout > 1000 && clout <= 1499) title = "Merchant";
		else if (clout > 1500 && clout <= 1999) title = "Mercenary";
		else if (clout > 2000 && clout <= 2499) title = "Knight";
		else if (clout > 2500 && clout <= 2999) title = "War Chief";
		else if (clout > 3000 && clout <= 3499) title = "Boule Leader";
		else if (clout > 3500 && clout <= 3999) title = "Ambassador";
		else if (clout > 4000 && clout <= 4499) title = "Prince";
		else if (clout > 4500 && clout <= 4999) title = "King";
		else if (clout >= 5000)                 title = "The God";
		else                                    title = "ERROR: clout is not between 0 and 100";
		return title;
	}	

	public Settlement GetSettlementFromID (int ID)
	{
		//Debug.Log (settlement_masterList.Length);
		foreach (Settlement city in settlement_masterList) {
			//Debug.Log ("DEBUG: city: " + city.name);
			if (city.settlementID == ID) {
				return city;
			}
		}
		//if no matches(this shouldn't be possible--return a fake settlement rather than a null
		//	--this is more sophisticated than a null--it won't crash but the error is obvious.
		Debug.Log ("ERROR: DIDNT FIND ID MATCH IN GetSettlementFromID Function: Looking for settlement ID:  " + ID);
		return new Settlement (-1, "ERROR", -1);
	}    
	
	public CrewMember GetCrewMemberFromID (int ID)
	{
		foreach (CrewMember crewman in masterCrewList) {
			if (crewman.ID == ID)
				return crewman;
		}
		return null;
	}



    //====================================================================================================
	//    QUEST FUNCTIONS
    //====================================================================================================   

	public void CheckIfCurrentSettlementIsPartOfMainQuest (int settlementID)
	{
	
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
			if (playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].segmentID < aeaID) {
				//currentQuestBeginning = 0;
				currentQuestEnd = aeaID;
			} else {
				//currentQuestBeginning = aeaID;
				//set the end destination to the last position of the array
				currentQuestEnd = playerShipVariables.ship.mainQuest.questSegments.Count - 1;
			}
			
			
			//we add a +1 to the current questline so that the player can't continuously perform the quest at the same stop
			for (int index = playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].segmentID; index <= currentQuestEnd; index++) {
				QuestSegment thisQuest = playerShipVariables.ship.mainQuest.questSegments [index];
				Debug.Log (settlementID + "   : =? :   " + thisQuest.destinationID);
				//If the current settlement matches the id of any target in the quest line, increment the quest line to that point--preferably we want it to be the next one in sequence--but we're expanding player behavioral choices.
				if (settlementID == thisQuest.destinationID) {
					//If there is a match, the player has moved on to a new leg of the quest line
					//first show a window for the completion message, and if there are any crew member changes, then let the player know.
					string questMessageIntro = "The Argonautica Quest: ";
					notificationMessage = questMessageIntro + thisQuest.descriptionAtCompletion;
					showNotification = true;
					
					//add the arrival message to Captain's log
					playerShipVariables.ship.shipCaptainsLog.Add (new CaptainsLogEntry (thisQuest.destinationID, questMessageIntro + thisQuest.descriptionAtCompletion));
					playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
					currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
					
					//Remove any crew members if the questline calls for it
					foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].crewmembersToRemove) {
						Debug.Log ("CREW ID REMOVING: " + crewID);
						//Make sure the crew ID values are not -1(a null value which means no changes)
						if (crewID != -1)
						//foreach (CrewMember currentMember in playerShipVariables.ship.crewRoster){
						//	Debug.Log ("Checking =  : " + currentMember.ID + " : " + crewID);
						//	if (currentMember.ID == crewID)
							playerShipVariables.ship.crewRoster.Remove (GetCrewMemberFromID (crewID));
						//}
					}
					
					//Add any new crew members if the questline calls for it
					foreach (int crewID in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].crewmembersToAdd) {
						//Make sure the crew ID values are not -1(a null value which means no changes)
						if (crewID != -1)
							playerShipVariables.ship.crewRoster.Add (GetCrewMemberFromID (crewID));
					}
					
					//Then increment the questline to the in succession and update the player captains log with the new information for the next quest line
					playerShipVariables.ship.mainQuest.currentQuestSegment = thisQuest.segmentID + 1;
					playerShipVariables.ship.shipCaptainsLog.Add (new CaptainsLogEntry (playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID, questMessageIntro + playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].descriptionOfQuest));
					playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
					currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog [playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
					
					
					//Now add the city name of the next journey quest to the players known settlements
					playerShipVariables.ship.playerJournal.AddNewSettlementToLog (playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID);
					Debug.Log ("next seg: " + playerShipVariables.ship.mainQuest.questSegments [playerShipVariables.ship.mainQuest.currentQuestSegment].destinationID);
					//Now add the mentioned places attached to this quest leg
					foreach (int i in playerShipVariables.ship.mainQuest.questSegments[playerShipVariables.ship.mainQuest.currentQuestSegment].mentionedPlaces) {
						Debug.Log ("mentioning: " + i);
						//Make sure we don't add any null values--a -1 represents no mentions of any settlements
						if (i != -1)
							playerShipVariables.ship.playerJournal.AddNewSettlementToLog (i);
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

	public void AdjustPlayerClout (int cloutAdjustment)
	{
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
		Debug.Log (playerShipVariables.ship.playerClout);
		//First check if a player reaches a new clout level
		//If the titles don't match after adjustment then we have a change!
		if (GetCloutTitleEquivalency (clout) != GetCloutTitleEquivalency ((int)playerShipVariables.ship.playerClout)) {
			updatePlayerCloutMeter = true;
			//Next we need to determine whether or not it was a level down or level up
			//If it was an increase then show a positive message
			if (clout < (clout + cloutAdjustment)) {
				Debug.Log ("Gained a level");
				notificationMessage = "Congratulations! You have reached a new level of influence! Before this day you were Jason, " + GetCloutTitleEquivalency (clout) + ".....But now...You have become Jason " + GetCloutTitleEquivalency ((int)playerShipVariables.ship.playerClout) + "!";				
				showNotification = true;
				//If it was a decrease then show a negative message to the player
			} else {
				Debug.Log ("Lost a level");
				notificationMessage = "Unfortunately you sunk to a new low level of respect in the world! Before this day you were Jason, " + GetCloutTitleEquivalency (clout) + ".....But now...You have become Jason " + GetCloutTitleEquivalency ((int)playerShipVariables.ship.playerClout) + "!";
				showNotification = true;
			}
            MasterGUISystem.GetComponent<script_GUI>().GUI_UpdatePlayerCloutMeter();
        }
		

	}
	
	public void AdjustCrewsClout (int cloutAdjustment)
	{
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
	
	public void AdjustPlayerShipHealth (int healthAdjustment)
	{
		//adjust the health by the given amount
		playerShipVariables.ship.health += healthAdjustment;
		//if the health exceeds 100 after the adjustment, then reduce it back to 100 as a cap
		if (playerShipVariables.ship.health > 100)
			playerShipVariables.ship.health = 100;
		//if the health is reduced below 0 after the adjustment, then increase it back to 0
		if (playerShipVariables.ship.health < 0)
			playerShipVariables.ship.health = 0;
	}

	public void AddEntriesToCurrentLogPool (int logID)
	{
		for (int i = 0; i < captainsLogEntries.Length; i++) {
			if (captainsLogEntries [i].settlementID == logID) {
				currentLogPool.Add (captainsLogEntries [i]);
			}
		}
	}
	
	public void RemoveEntriesFromCurrentLogPool (int logID)
	{
		currentLogPool.RemoveAll (entry => entry.settlementID == logID);
	}
	
	    
    
    //====================================================================================================
	//    OTHER FUNCTIONS
    //====================================================================================================   
	
	public bool CheckForNetworkMatchBetweenTwoSettlements (int cityA, int cityB)
	{
		int INDEPENDENT = 0;
		Settlement cityAObj = GetSettlementFromID (cityA);
		Settlement cityBObj = GetSettlementFromID (cityB);
		foreach (int cityA_ID in cityAObj.networks) {
			foreach (int cityB_ID in cityBObj.networks) {
				if (cityA_ID == cityB_ID && cityA_ID != INDEPENDENT) {
					return true;
				}
			}
		}
		return false;
	}
	

	public float GetOverallCloutModifier (int settlementID)
	{
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
		
		playerClout = (int)Math.Floor((playerClout / 5000)*100); //5000 is the cap so we divide the current amount to get the 0/1 ratio
		
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
			} else {
				playerNetworkModifier = 0f;
			}
		}

		//###### Now we need to determine whether or not the current city (or representative thereof) 
		//	--is part of the player's hometown/main network(s), e.g. if the player and city is part of the Samothracian network
		Debug.Log ("DEBUG:  " + settlementID);
		//If there is no city attached(settlementID == 0) then we are in open waters so return 0
		if (settlementID != 0) {
			foreach (int playerNetworkID in playerShipVariables.ship.networks) {
				foreach (int settlementNetID in GetSettlementFromID(settlementID).networks) {
					//If we find a network match through the network ID then return the city's population to represent its influence in the network
					//	--if we don't find a match, a value of 0 is returned			
					if (playerNetworkID == settlementNetID) {
						playerOriginNetworkModifier = GetSettlementFromID (settlementID).population;
					} else {
						playerOriginNetworkModifier = 0f;
					}
				}
			
			}
		} else {
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
							crewMembersNetworkModifier = GetSettlementFromID (settlementID).population / 2f;
						} else {
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
	

	public bool CheckIfShipBackAtLoanOriginPort ()
	{
		bool isAtPort = false;
		if (playerShipVariables.ship.currentLoan.settlementOfOrigin == currentSettlement.settlementID) {
			isAtPort = true;
		} else {
			isAtPort = false;
		}
		
		return isAtPort;
		
	
	}
	
	public bool FastApproximately (float a, float b, float threshold)
	{
		return ((a < b) ? (b - a) : (a - b)) <= threshold;
	}
	
	
	public bool CheckIfCityIDIsPartOfNetwork (int cityID)
	{
		//Debug.Log ("CITY PART OF NETWORK CEHCK: " + cityID);
		Settlement thisSettlement = GetSettlementFromID (cityID);
		int INDEPENDENT = 0;//Settlements who don't belong to a network are independent
		//Debug.Log ("DEBUG ID CHECK 2: " + thisSettlement.name);
		//First check if the player is part of the network
		foreach (int playerNetID in playerShipVariables.ship.networks) {
			//Debug.Log ("DEBUG ID CHECK: " + playerShipVariables.ship.networks);
			foreach (int cityNetID in thisSettlement.networks) {
				if (playerNetID == cityNetID && cityNetID != INDEPENDENT) {
					crewMemberWithNetwork = null;
					return true;
				}
			}
		}
		//Check if this is the player's hometown
		if (cityID == playerShipVariables.ship.originSettlement)
			return true;
		
		
		//Then Check are any crewmembers are part of the network
		foreach (CrewMember thisCrewMember in playerShipVariables.ship.crewRoster) {
			//Debug.Log (thisCrewMember.name);
			Settlement crewOriginCity = GetSettlementFromID (thisCrewMember.originCity);
			if (crewOriginCity.name != "ERROR") {
				
				//First check if the crewman is part of this towns network
				//	--we have to run through each network in the settlement's list against the networks in the crewman's origin city's list
				//TODO it will probably be useful down the road to ahve a crewman's origin city give extra information / bonuses beyond a network bonus
				foreach (int cityNetID in thisSettlement.networks) {
					foreach (int crewCityNetID in crewOriginCity.networks) {
						if (cityNetID == crewCityNetID && cityNetID != INDEPENDENT) {
							crewMemberWithNetwork = thisCrewMember;
							return true;
						}
					}
				}
			}
			//Check if this is the crewman's hometown
			if (cityID == thisCrewMember.originCity)
				return true;
		}
		
		//If we don't come up with any matches anywhere then return false
		return false;
	
	}
		
	
	public void ShowANotificationMessage (string message)
	{
		//First check if we have a primary message going already
		if (showNotification) {
			//if we do then queue up a secondary message
			showSecondaryNotification = true;
			secondaryNotificationMessage = message;
			//otherwise show a normal primary message
		} else {
			showNotification = true;
			notificationMessage = message;
		}
	}
	

	public float GetRange (float Xinput, float Xmax, float Xmin, float Ymax, float Ymin)
	{
		
		return (((Xinput - Xmin) / (Xmax - Xmin)) * (Ymax - Ymin)) + Ymin;
		
	}	

    
    
    
    
    
    
    
    
    
    
    
    
    
    
}///////// END OF FILE
