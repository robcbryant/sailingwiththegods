using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CSVLoader
{

	public static Settlement[] LoadSettlementList() {
		char[] lineDelimiter = new char[] { '@' };
		char[] recordDelimiter = new char[] { '_' };

		string filename = "settlement_list_newgame";
		string[] fileByLine = TryLoadListFromGameFolder(filename);

		//subtract one to account for the header line
		var settlement_masterList = new Dictionary<int, Settlement>();
		//start at index 1 to skip the record headers we have to then subtract 
		//one when adding NEW settlements to the list to ensure we start at ZERO and not ONE
		for (int lineCount = 1; lineCount < fileByLine.Length; lineCount++) {
			//Debug.Log("-->" + fileByLine[lineCount]);
			string[] records = fileByLine[lineCount].Split(lineDelimiter, StringSplitOptions.None);
			//Debug.Log (records[1] + "  " + records[2] + " " + records[3] + " " + records[4]);
			//NAME | LAT LONG | POPULATION | ELEVATION
			int id;
			if (records[0] == null)
				id = -1;
			else
				id = int.Parse(records[0]);
			string name;
			if (records[1] == null)
				name = "NO NAME";
			else
				name = records[1];
			Vector2 longXlatY;
			try {
				longXlatY = new Vector2(float.Parse(records[3]), float.Parse(records[2]));
			}
			catch {
				longXlatY = Vector2.zero;
			}
			float elevation;
			try {
				elevation = float.Parse(records[4]);
			}
			catch {
				elevation = 0f;
			}
			int population;
			population = int.Parse(records[5]);

			var settlement = new Settlement(id, name, longXlatY, elevation, population);
			settlement_masterList.Add(id, settlement);

			//Grab the networks it belongs to
			//List<int> networks = new List<int>();
			string[] parsedNetworks = records[7].Split(recordDelimiter, StringSplitOptions.None);
			foreach (string networkID in parsedNetworks)
				settlement.networks.Add(int.Parse(networkID));

			//load settlement's in/out network taxes
			settlement.tax_neutral = float.Parse(records[9]);
			settlement.tax_network = float.Parse(records[10]);
			//load the settlement type, e.g. port, no port
			settlement.typeOfSettlement = int.Parse(records[26]);
			//add resources to settlement (records length - 2 is confusing, but there are items after the last resource--can probably change this later)
			for (int recordIndex = 11; recordIndex < records.Length - 3; recordIndex++) {
				settlement.cargo[recordIndex - 11].probabilityOfAvailability = float.Parse(records[recordIndex]);
				//TODO The probability values are 1-100 and population affects the amount
				//  Population/2 x (probabilityOfResource/100)
				float amount = (settlement.population / 2) * (settlement.cargo[recordIndex - 11].probabilityOfAvailability / 1.5f);
				settlement.cargo[recordIndex - 11].amount_kg = amount;
			}
			//Add model/prefab name to settlement
			settlement.prefabName = records[records.Length - 2];
			Debug.Log("********PREFAB NAME:     " + settlement.prefabName);
			//Add description to settlement
			settlement.description = records[records.Length - 1];

			//Debug.Log (settlement_masterList[lineCount-1].ToString());
			//Vector2 test = CoordinateUtil.ConvertWGS1984ToWebMercator(longXlatY);
			//Debug.Log (records[1] + " : " + test.x + " , " + test.y);


		}

		LoadAdjustedSettlementLocations(settlement_masterList);

		return settlement_masterList.Values.ToArray();

	}

	public static WindRose[,] LoadWindRoses(int width, int height) {
		char[] lineDelimiter = new char[] { ',' };

		string filename = "windroses_january";

		string[] fileByLine = TryLoadListFromGameFolder(filename);

		var windrose_January = new WindRose[width, height];

		//For each line of the wind rose file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			//Debug.Log("-->" + fileByLine[lineCount]);
			string[] records = fileByLine[row].Split(lineDelimiter, StringSplitOptions.None);
			//Now loop through each column of the line and assign it to a windrose within January
			for (int col = 0; col < records.Length / 2; col++) {
				float direction = float.Parse(records[col * 2]);//there are double the amount of columns in the file--these formulas account for that
				float speed = float.Parse(records[(col * 2) + 1]);
				windrose_January[col, row] = new WindRose(direction, speed);
				//Debug.Log (col + " " + row + "   :   " + windrose_January[col,row].direction + " -> " + windrose_January[col,row].speed);
			}
		}

		return windrose_January;
	}

	public static CurrentRose[,] LoadWaterZonesFromFile(int width, int height) {
		char[] lineDelimiter = new char[] { ',' };

		string filename = "waterzones_january";

		string[] fileByLine = TryLoadListFromGameFolder(filename);

		var currentRose_January = new CurrentRose[width, height];

		//For each line of the wind rose file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			//Debug.Log("-->" + fileByLine[lineCount]);
			string[] records = fileByLine[row].Split(lineDelimiter, StringSplitOptions.None);
			//Now loop through each column of the line and assign it to a windrose within January
			for (int col = 0; col < records.Length / 2; col++) {
				float direction = float.Parse(records[col * 2]);//there are double the amount of columns in the file--these formulas account for that
				float speed = float.Parse(records[(col * 2) + 1]);
				currentRose_January[col, row] = new CurrentRose(direction, speed);
				//Debug.Log (col + " " + row + "   :   " + currentRose_January[col,row].direction + " -> " + currentRose_January[col,row].speed);
			}
		}

		return currentRose_January;
	}
	public static CaptainsLogEntry[] LoadCaptainsLogEntries() {
		
		char[] lineDelimiter = new char[] { '@' };
		string filename = "captains_log_database";

		string[] fileByLine = TryLoadListFromGameFolder(filename);

		var captainsLogEntries = new CaptainsLogEntry[fileByLine.Length];
		//For each line of the wind rose file (the row)
		for (int row = 0; row < fileByLine.Length; row++) {
			//Debug.Log (captainsLogEntries.Length + "  :  " + row);
			string[] records = fileByLine[row].Split(lineDelimiter, StringSplitOptions.None);
			captainsLogEntries[row] = new CaptainsLogEntry(int.Parse(records[0]), records[1]);

		}

		//Debugging
		//for (int i = 0; i < captainsLogEntries.Length; i++)
		//Debug.Log (captainsLogEntries[i].settlementID + "  :  " + captainsLogEntries[i].logEntry);

		return captainsLogEntries;
	}

	//This loads the main quest line from a CSV file in the resources
	public static MainQuestLine LoadMainQuestLine() {
		MainQuestLine mainQuest = new MainQuestLine();
		char[] lineDelimiter = new char[] { '@' };
		char[] lineDelimiterB = new char[] { '_' };
		string filename = "main_questline_database";

		string[] fileByLine = TryLoadListFromGameFolder(filename);

		//start at index 1 to skip the record headers
		//For each line of the main quest file (the row)
		for (int row = 1; row < fileByLine.Length; row++) {
			string[] records = fileByLine[row].Split(lineDelimiter, StringSplitOptions.None);
			//Debug.Log (row);
			//Debug.Log
			//let's parse out all the crew roster changes
			//Debug.Log ("LOADQUEST--Leg: " + records[0] + " )( removals: " + records[6]);
			string[] crewRosterAdd = records[5].Split(lineDelimiterB, StringSplitOptions.None);
			string[] crewRosterRemove = records[6].Split(lineDelimiterB, StringSplitOptions.None);
			string[] mentionedSpots = records[4].Split(lineDelimiterB, StringSplitOptions.None);

			List<int> crewToAdd = new List<int>();
			foreach (string id in crewRosterAdd) {
				crewToAdd.Add(int.Parse(id));
			}
			List<int> crewToRemove = new List<int>();
			foreach (string id in crewRosterRemove) {
				crewToRemove.Add(int.Parse(id));
			}
			List<int> mentionedPlaces = new List<int>();
			foreach (string id in mentionedSpots) {
				mentionedPlaces.Add(int.Parse(id));
			}
			//now let's see if we're on the last segment of the questline
			bool isEnd = false;
			if (row == fileByLine.Length - 1)
				isEnd = true;
			//Debug.Log("***************************");
			//Debug.Log (records[1]);
			//now add the segment to the main questline
			mainQuest.questSegments.Add(new QuestSegment(int.Parse(records[0]), int.Parse(records[1]), records[2], records[3], crewToAdd, crewToRemove, isEnd, mentionedPlaces));
		}

		return mainQuest;
	}

	public static List<CrewMember> LoadMasterCrewRoster() {
		
		char[] lineDelimiter = new char[] { '@' };

		string filename = "crewmembers_database";

		string[] fileByLine = TryLoadListFromGameFolder(filename);

		var masterCrewList = new List<CrewMember>();

		//start at index 1 to skip the record headers
		//For each line of the main quest file (the row)
		for (int row = 1; row < fileByLine.Length; row++) {
			string[] records = fileByLine[row].Split(lineDelimiter, StringSplitOptions.None);

			bool isKillable = false;
			bool isPartOfMainQuest = false;
			if (int.Parse(records[6]) == 1)
				isKillable = true;
			if (int.Parse(records[7]) == 1)
				isPartOfMainQuest = true;
			//Let's add a crewmember to the master roster
			// TODO: Change CrewType in CSV to a string so it's more readable and use Enum.Parse
			masterCrewList.Add(new CrewMember(int.Parse(records[0]), records[1], int.Parse(records[2]), int.Parse(records[3]), (CrewType)int.Parse(records[4]), records[5], isKillable, isPartOfMainQuest));
		}

		return masterCrewList;

	}

	static void LoadAdjustedSettlementLocations(Dictionary<int, Settlement> settlements) {
		
		char[] lineDelimiter = new char[] { ',' };
		int currentID = 0;
		string filename = "settlement_unity_position_offsets";

		string[] fileByLine = TryLoadListFromGameFolder(filename);

		for (int row = 0; row < fileByLine.Length; row++) {
			string[] records = fileByLine[row].Split(lineDelimiter, StringSplitOptions.None);
			currentID = int.Parse(records[0]);

			if (settlements.ContainsKey(currentID)) {
				var thisSettlement = settlements[currentID];
				thisSettlement.adjustedGamePosition = new Vector3(float.Parse(records[1]), float.Parse(records[2]), float.Parse(records[3]));
				thisSettlement.eulerY = float.Parse(records[4]);
			}
		}

	}

	public static List<MetaResource> LoadResourceList() {
		char[] lineDelimiter = new char[] { '@' };
		string filename = "resource_list";

		string[] fileByLine = TryLoadListFromGameFolder(filename);

		var masterResourceList = new List<MetaResource>();

		//start at index 1 to skip the record headers we have to then subtract 
		for (int lineCount = 1; lineCount < fileByLine.Length; lineCount++) {
			string[] records = fileByLine[lineCount].Split(lineDelimiter, StringSplitOptions.None);
			masterResourceList.Add(new MetaResource(records[1], int.Parse(records[0]), records[3], records[2]));
		}

		return masterResourceList;
	}

	static string TryLoadFromGameFolder(string filename) {
		try {
			WWW localFile = new WWW("file://" + Application.dataPath + "/" + filename + ".txt");

			while (!localFile.isDone) {
				Debug.Log("Progress of Load File: " + localFile.progress);
			}
			Debug.Log(Application.dataPath + "/" + filename + ".txt");
			Debug.Log(localFile.text);
			if (localFile.text == "") {
				TextAsset file = (TextAsset)Resources.Load(filename, typeof(TextAsset));
				return file.text;
			}
			return localFile.text;

		}
		catch (Exception error) {
			Debug.Log("Sorry! No file: " + filename + " was found in the game directory '" + Application.dataPath + "' or the save file is corrupt!\nError Code: " + error);
			TextAsset file = (TextAsset)Resources.Load(filename, typeof(TextAsset));
			return file.text;
		}

	}

	static string[] TryLoadListFromGameFolder(string filename) {
		string[] splitFile = new string[] { "\r\n", "\r", "\n" };

		string filetext = TryLoadFromGameFolder(filename);
		string[] fileByLine = filetext.Split(splitFile, StringSplitOptions.None);

		// remove any trailing newlines since the parsers assume there's no newline at the end of the file, but VS auto-adds one
		return fileByLine
			.Where(line => !string.IsNullOrEmpty(line))
			.ToArray();
	}
}
