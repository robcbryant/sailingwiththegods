using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
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
	#region Triggers
	public abstract class Trigger
	{
		public abstract TriggerType Type { get; }
	}

	public class CityTrigger : Trigger
	{
		public override TriggerType Type => TriggerType.City;

		public readonly int DestinationId;

		public CityTrigger(int destinationId) : base() {
			DestinationId = destinationId;
		}
	}

	public class CoordTrigger : Trigger
	{
		public override TriggerType Type => TriggerType.Coord;

		public readonly Vector2 LongXLatY;

		public CoordTrigger(Vector2 longXLatY) : base() {
			LongXLatY = longXLatY;
		}
	}

	public class UpgradeShipTrigger : Trigger
	{
		public override TriggerType Type => TriggerType.UpgradeShip;
	}

	public class NoneTrigger : Trigger
	{
		public override TriggerType Type => TriggerType.None;
	}

	public enum TriggerType
	{
		None,
		City,
		Coord,
		UpgradeShip
	}
	#endregion

	#region Arrival Events

	public abstract class ArrivalEvent
	{
		protected QuestSegment Segment { get; private set; }
		public abstract ArrivalEventType Type { get; }
		public abstract void Execute(QuestSegment segment);
	}

	public class MessageArrivalEvent : ArrivalEvent
	{
		public override ArrivalEventType Type => ArrivalEventType.Message;

		public override void Execute(QuestSegment segment) {

			Globals.UI.Show<QuestScreen, QuizScreenModel>(new QuizScreenModel(
				title: QuestSystem.QuestMessageIntro,
				message: Message,
				caption: segment.caption,
				icon: segment.image,
				choices: new ObservableCollection<ButtonViewModel> {
					new ButtonViewModel { Label = "OK", OnClick = () => Globals.UI.Hide<QuestScreen>() }
				}
			));

			Globals.Quests.CompleteQuestSegment(segment);
		}

		public readonly string Message;

		public MessageArrivalEvent(string message) {
			Message = message;
		}
	}

	public class QuizArrivalEvent : ArrivalEvent
	{
		public override ArrivalEventType Type => ArrivalEventType.Quiz;

		readonly string QuizName;

		public override void Execute(QuestSegment segment) {
			Quizzes.QuizSystem.StartQuiz(QuizName, () => Globals.Quests.CompleteQuestSegment(segment));
		}

		public QuizArrivalEvent(string quizName) {
			QuizName = quizName;
		}
	}

	public class NoneArrivalEvent : ArrivalEvent
	{
		public override ArrivalEventType Type => ArrivalEventType.None;

		// just immediately start the next quest with no additional popups
		public override void Execute(QuestSegment segment) {
			Globals.Quests.CompleteQuestSegment(segment);
		}
	}

	public enum ArrivalEventType
	{
		None,
		Message,
		Quiz
	}

	#endregion

	public int segmentID;
	public Trigger trigger;
	public bool skippable;
	public string objective;
	public bool isFinalSegment;
	public List<int> crewmembersToAdd;
	public List<int> crewmembersToRemove;
	public string descriptionOfQuest;
	public ArrivalEvent arrivalEvent;
	public List<int> mentionedPlaces;
	public Sprite image;
	public string caption;

	public QuestSegment(int segmentID, Trigger trigger, bool skippable, string objective, string descriptionOfQuest, ArrivalEvent arrivalEvent, List<int> crewmembersToAdd, List<int> crewmembersToRemove, bool isFinalSegment, List<int> mentionedPlaces, Sprite image, string caption) {
		this.segmentID = segmentID;
		this.trigger = trigger;
		this.skippable = skippable;
		this.objective = objective;
		this.descriptionOfQuest = descriptionOfQuest;
		this.arrivalEvent = arrivalEvent;
		this.crewmembersToAdd = crewmembersToAdd;
		this.crewmembersToRemove = crewmembersToRemove;
		this.isFinalSegment = isFinalSegment;
		this.mentionedPlaces = mentionedPlaces;
		this.image = image;
		this.caption = caption;
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

	public ObservableCollection<int> knownSettlements;

	public Journal() {
		this.knownSettlements = new ObservableCollection<int>();
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

public class PirateType
{
	public int ID;
	public string name;
	public int difficulty;
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
	public bool isPirate;
	public PirateType pirateType;

	public bool isJason => name == "Jason";

	SkillModifiers _changeOnHire;
	public SkillModifiers changeOnHire { get { if(_changeOnHire == null) InitChangeOnHire(); return _changeOnHire; } }

	SkillModifiers _changeOnFire;
	public SkillModifiers changeOnFire { get { if (_changeOnFire == null) InitChangeOnFire(); return _changeOnFire; } }

	SkillModifiers _currentContribution;
	public SkillModifiers currentContribution { get { if (_currentContribution == null) InitCurrentContribution(); return _currentContribution; } }

	//0= sailor  1= warrior  2= slave  3= passenger 4= navigator 5= auger
	//A sailor is the base class--no benefits/detriments
	//	--navigators provide maps to different settlements and decrease negative random events
	//	--warriors make sure encounters with pirates or other raiding activities go better in your favor
	//	--slaves have zero clout--few benefits--but they never leave the ship unless they die
	public CrewMember(int ID, string name, int originCity, int clout, CrewType typeOfCrew, string backgroundInfo, bool isKillable, bool isPartOfMainQuest, bool isPirate, PirateType pirateType) {
		this.ID = ID;
		this.name = name;
		this.originCity = originCity;
		this.clout = clout;
		this.typeOfCrew = typeOfCrew;
		this.backgroundInfo = backgroundInfo;
		this.isKillable = isKillable;
		this.isPartOfMainQuest = isPartOfMainQuest;
		this.isPirate = isPirate;
		this.pirateType = pirateType;
	}

	//This is a helper class to create a void crewman
	public CrewMember(int id) {
		ID = id;
		_changeOnHire = new SkillModifiers();
		_changeOnFire = new SkillModifiers();
		_currentContribution = new SkillModifiers();
	}

	void InitChangeOnHire() {
		var gameVars = Globals.GameVars;

		_changeOnHire = new SkillModifiers {
			CitiesInNetwork = gameVars.Network.GetCrewMemberNetwork(this).Count(s => !gameVars.Network.MyCompleteNetwork.Contains(s)),
			BattlePercentChance = typeOfCrew == CrewType.Warrior ? 5 : 0,
			Navigation = typeOfCrew == CrewType.Sailor ? 1 : 0,
			PositiveEvent = typeOfCrew == CrewType.Guide ? 10 : 0
		};
	}

	void InitChangeOnFire() {
		var gameVars = Globals.GameVars;

		// the cities in network calculation is too expensive right now. disabled temporarily
		_changeOnFire = new SkillModifiers {
			CitiesInNetwork = -gameVars.Network.GetCrewMemberNetwork(this).Count(s => !gameVars.Network.CrewMembersWithNetwork(s).Any(crew => crew != this) && !gameVars.Network.MyImmediateNetwork.Contains(s)),
			BattlePercentChance = typeOfCrew == CrewType.Warrior ? -5 : 0,
			Navigation = typeOfCrew == CrewType.Sailor ? -1 : 0,
			PositiveEvent = typeOfCrew == CrewType.Guide ? -10 : 0
		};
	}

	void InitCurrentContribution() {
		var gameVars = Globals.GameVars;

		// very similar to changeOnFire, but shows it as positives. this is their contribution to your team, not what you'll lose if you fire them (but it's basically the same).
		_currentContribution = new SkillModifiers {
			CitiesInNetwork = gameVars.Network.GetCrewMemberNetwork(this).Count(s => !gameVars.Network.CrewMembersWithNetwork(s).Any(crew => crew != this) && !gameVars.Network.MyImmediateNetwork.Contains(s)),
			BattlePercentChance = typeOfCrew == CrewType.Warrior ? 5 : 0,
			Navigation = typeOfCrew == CrewType.Sailor ? 1 : 0,
			PositiveEvent = typeOfCrew == CrewType.Guide ? 10 : 0
		};
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

// KDTODO: This needs to be rewritten to something less error prone. Probably JSONUtility
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
			"CrewMemberIDs,UnityXYZ,Current_Questleg,ShipHP,Clout,PlayerNetwork,DaysStarving,DaysThirsty,Currency,LoanAmount,LoanOriginID,CurrentNavigatorTarget,KnownSettlements,CaptainsLog,upgradeLevel,crewCap,cargoCap\n";
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

		CSVstring += "," + playerShip.upgradeLevel;
		CSVstring += "," + playerShip.crewCapacity;
		CSVstring += "," + playerShip.cargo_capicity_kg;

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
	public string icon;

	public MetaResource(string name, int id, string description, string icon) {
		this.name = name;
		this.id = id;
		this.description = description;
		this.icon = icon;
	}

}


public class Resource : Model
{
	public const string Water = "Water";
	public const string Provisions = "Provisions";
	public const string Grain = "Grain";
	public const string Wine = "Wine";
	public const string Timber = "Timber";
	public const string Gold = "Gold";
	public const string Silver = "Silver";
	public const string Copper = "Copper";
	public const string Tin = "Tin";
	public const string Obsidian = "Obsidian";
	public const string Lead = "Lead";
	public const string Slaves = "Slaves";
	public const string Iron = "Iron";
	public const string Bronze = "Bronze";
	public const string PrestigeGoods = "Prestige Goods";

	public string name { get; private set; }

	private float _initial_amount_kg;
	public float initial_amount_kg { get => _initial_amount_kg; set { _initial_amount_kg = value; Notify(); } }

	private float _amount_kg;
	public float amount_kg { get => _amount_kg; set { _amount_kg = value; Notify(); } }

	public Resource(string name, float amount_kg) {
		this.name = name;
		this.amount_kg = amount_kg;
		this.initial_amount_kg = amount_kg;
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
	public Vector3 adjustedGamePosition;
	public float eulerY;
	public int typeOfSettlement;
	public string description;
	public List<int> networks;
	public ObservableCollection<CrewMember> availableCrew;
	public string prefabName;

	public Resource GetCargoByName(string name) => cargo.FirstOrDefault(c => c.name == name);

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
		availableCrew = new ObservableCollection<CrewMember>();
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
		availableCrew = new ObservableCollection<CrewMember>();

	}

	override public string ToString() {
		string mString = this.name + ":\n" + "Population: " + population + "\n\n RESOURCES \n";
		for (int i = 0; i < this.cargo.Length; i++) {
			mString += this.cargo[i].name + ":  " + this.cargo[i].amount_kg + "kg\n";
		}

		return mString;
	}

}

public class Ship : Model
{
	public const int StartingCrewCap = 10;		// 30
	public const int StartingCargoCap = 200;	// 1200
	public const int StartingWater = 50;		// 300
	public const int StartingFood = 50;			// 300
	public const int StartingCrewSize = 5;		// 30

	public string name;
	public float speed;
	public float cargo_capicity_kg;
	public Resource[] cargo;
	public int networkID;
	public List<CaptainsLogEntry> shipCaptainsLog;
	public Journal playerJournal;
	public int currentNavigatorTarget;
	public Loan currentLoan;
	public List<int> networks;
	public int originSettlement;

	public MainQuestLine mainQuest;

	// TODO: Reconcile mainQuest and objective concepts. These systems seem like they should be merged
	private string _objective;
	public string objective { get => _objective; set { _objective = value; Notify(); } }

	public int crew => crewRoster.Count;
	public ObservableCollection<CrewMember> crewRoster;

	private float _totalNumOfDaysTraveled;
	public float totalNumOfDaysTraveled { get => _totalNumOfDaysTraveled; set { _totalNumOfDaysTraveled = value; Notify(); } }

	private int _crewCapacity = StartingCrewCap;
	public int crewCapacity { get => _crewCapacity; set { _crewCapacity = value; Notify(); } }

	private bool _sailsAreUnfurled = true;
	public bool sailsAreUnfurled { get => _sailsAreUnfurled; set { _sailsAreUnfurled = value; Notify(); } }

	private int _upgradeLevel;
	public int upgradeLevel { get => _upgradeLevel; set { _upgradeLevel = value; Notify(); } }

	private float _health;
	public float health { get => _health; set { _health = value; Notify(); } }

	private string _builtMonuments = "";
	public string builtMonuments { get => _builtMonuments; set { _builtMonuments = value; Notify(); } }

	private int _currency;
	public int currency { get => _currency; set { _currency = value; Notify(); } }

	public float _playerClout;
	public float playerClout { get => _playerClout; set { _playerClout = value; Notify(); } }

	public float CurrentCargoKg => cargo.Sum(c => c.amount_kg);

	public Resource GetCargoByName(string name) => cargo.FirstOrDefault(c => c.name == name);

	public Ship(string name, float speed, int health, float cargo_capcity_kg) {

		this.name = name;
		this.speed = speed;
		this.health = health;
		this.cargo_capicity_kg = cargo_capcity_kg;
		this.shipCaptainsLog = new List<CaptainsLogEntry>();
		this.crewRoster = new ObservableCollection<CrewMember>();
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

		this.currency = 500;
		this.crewCapacity = StartingCrewCap;
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
