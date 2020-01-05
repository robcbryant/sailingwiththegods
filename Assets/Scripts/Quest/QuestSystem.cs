using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
	public const string QuestMessageIntro = "The Argonautica Quest: ";

	GameVars gameVars => Globals.GameVars;
	GameObject playerShip => gameVars.playerShip;
	MainQuestLine quest => playerShipVariables.ship.mainQuest;
	script_player_controls playerShipVariables => gameVars.playerShipVariables;

	const float CoordTriggerDistance = 2;

	bool IsComplete(QuestSegment s) => s.segmentID < quest.currentQuestSegment;

	QuestSegment CurrSegment => quest.questSegments.ElementAtOrDefault(quest.currentQuestSegment);

	// the quest is split into two "legs". up through Aea and after Aea
	// First we determine which part of the questleg the player is in which determines which part of the quest array the player can access
	// If the player is in the first half before Aea--then only search for these quest segments, else only search for the last quest segments
	const int FirstLegEndId = 15;
	IEnumerable<QuestSegment> CurrLeg =>
		CurrSegment.segmentID < FirstLegEndId ?
		quest.questSegments.Take(FirstLegEndId + 1) :
		quest.questSegments.Skip(FirstLegEndId + 1);

	IEnumerable<QuestSegment> CurrLegRemaining =>
		CurrLeg
		.Where(seg => !IsComplete(seg));

	private void Awake() {
		Globals.Register(this);
	}

	public QuestSegment CompleteQuestSegment(QuestSegment thisQuest) {
		// this should be called by the arrival events to show the completion mesage, quiz, or whatever other arrival events there are. lost/gained crew will be baked into the message text

		//add the arrival message to Captain's log
		// TODO: For now, the only segment trigger types that support the captain's log are cities. this is because the captain's log spreadsheet assumes a city id as the trigger
		// we also only support message arrival events for now because we are also assuming there's a concrete completion message
		if (thisQuest.trigger is QuestSegment.CityTrigger cityTrigger && thisQuest.arrivalEvent is QuestSegment.MessageArrivalEvent arrivalMessage) {
			var nextDestEntry = new CaptainsLogEntry(cityTrigger.DestinationId, QuestMessageIntro + arrivalMessage.Message) {
				dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days"
			};
			playerShipVariables.ship.shipCaptainsLog.Add(nextDestEntry);
			gameVars.AddToCaptainsLog(nextDestEntry.dateTimeOfEntry + "\n" + nextDestEntry.logEntry);
		}

		//Remove any crew members if the questline calls for it
		foreach (int crewID in thisQuest.crewmembersToRemove) {
			Debug.Log("CREW ID REMOVING: " + crewID);
			//Make sure the crew ID values are not -1(a null value which means no changes)
			if (crewID != -1)
				playerShipVariables.ship.crewRoster.Remove(gameVars.GetCrewMemberFromID(crewID));
		}

		//Add any new crew members if the questline calls for it
		foreach (int crewID in thisQuest.crewmembersToAdd) {
			//Make sure the crew ID values are not -1(a null value which means no changes)
			if (crewID != -1)
				playerShipVariables.ship.crewRoster.Add(gameVars.GetCrewMemberFromID(crewID));
		}

		//Then increment the questline to the in succession and update the player captains log with the new information for the next quest line
		return StartQuestSegment(thisQuest.segmentID + 1);
	}

	public QuestSegment StartQuestSegment(int segmentId) {
		quest.currentQuestSegment = segmentId;

		var nextSegment = quest.questSegments[quest.currentQuestSegment];

		// TODO: For now, the only segment trigger types that support the captain's log are cities. this is because the captain's log spreadsheet assumes a city id as the trigger
		// TODO: Need to pull this out into a function so we can fix the name conflict on cityTrigger
		if (nextSegment.trigger is QuestSegment.CityTrigger cityTrigger2) {
			var nextDestEntry = new CaptainsLogEntry(cityTrigger2.DestinationId, QuestMessageIntro + nextSegment.descriptionOfQuest) {
				dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days"
			};
			playerShipVariables.ship.shipCaptainsLog.Add(nextDestEntry);
			gameVars.AddToCaptainsLog(nextDestEntry.dateTimeOfEntry + "\n" + nextDestEntry.logEntry);

			//Now add the city name of the next journey quest to the players known settlements
			// this is only valid for city destinations, obviously
			playerShipVariables.ship.playerJournal.AddNewSettlementToLog(cityTrigger2.DestinationId);
			Debug.Log("next seg: " + cityTrigger2.DestinationId);
			//Now add the mentioned places attached to this quest leg
			foreach (int i in nextSegment.mentionedPlaces) {
				Debug.Log("mentioning: " + i);
				//Make sure we don't add any null values--a -1 represents no mentions of any settlements
				if (i != -1)
					playerShipVariables.ship.playerJournal.AddNewSettlementToLog(i);
			}
		}

		// TODO: Need to add a new Objective field since this will pretty always give a non-good string, but it'll work as a standin for now
		playerShipVariables.ship.objective = string.Concat(nextSegment.descriptionOfQuest
			.SkipWhile(c => c == '"')
			.TakeWhile(c => c != '!')
		);

		return nextSegment;
	}

	#region Trigger Checks

	//We need to cylcle through each quest destination and see if this current area matches one of the destinations. Preferably the players should go in order--but we are designing it to allow players to
	//skip ahead and in theory--go directly to the end destination. One of the issues is the removal and addition of crewmembers along the way that are important for the plot. Because the questline is a series
	//of stock messages for each destination, parsing out the narrative that talks about non-existent crewmen is difficult. For now--the narrative will remain unchanged, but hercules might never actually leave
	//the ship if they don't stop at a specific destination where he leaves. Some narratives might discuss sailors that aren't actually on the ship. Additionally--the endpoint of the questline is the origin so the questline needs to be
	//split into 2 parts--the first ends at aea and the player can sell straight there, but must go there in order to return back to Pagasse and can return there directly or follow the questline back there.
	//ALSO--once the player reachees a certain point in the questline, the player can't return to older points in the quest so the beginning id should start at the current quest segment

	public void CheckCityTriggers(int settlementID) {
		//First determine if the player has finished the entire questline yet (null). We'll use the Count without a -1 to make sure the incremented quest leg is higher thant he last available leg
		//If the current settlement matches the id of any target in the quest line, increment the quest line to that point--preferably we want it to be the next one in sequence--but we're expanding player behavioral choices.
		var match = CurrLegRemaining.FirstOrDefault(seg => 
			seg.trigger is QuestSegment.CityTrigger city && 
			city.DestinationId == settlementID);

		if (match != null) {
			match.arrivalEvent.Execute(match);
		}
	}

	public void CheckCoordTriggers(Vector2 position) {
		var match = CurrLegRemaining.FirstOrDefault(seg => 
			seg.trigger is QuestSegment.CoordTrigger coord &&
			Vector2.Distance(position, coord.LatLongCoord) < CoordTriggerDistance);

		if (match != null) {
			match.arrivalEvent.Execute(match);
		}
	}

	public void CheckUpgradeShipTriggers() {
		var match = CurrLegRemaining.FirstOrDefault(seg => seg.trigger is QuestSegment.UpgradeShipTrigger);
		if (match != null) {
			match.arrivalEvent.Execute(match);
		}
	}

	#endregion

	public void InitiateMainQuestLineForPlayer() {
		var playerShipVariables = gameVars.playerShipVariables;

		//For the argonautica, let's set the crew capacity to 30
		playerShipVariables.ship.crewCapacity = Ship.StartingCrewCap;

		//Now let's add all the initial crew from the start screen selection and start the first leg of the quest
		for (int i = 0; i < gameVars.newGameAvailableCrew.Count; i++) {
			if (gameVars.newGameCrewSelectList[i]) {
				playerShipVariables.ship.crewRoster.Add(gameVars.newGameAvailableCrew[i]);
			}
		}

		//Let's increase the ships cargo capacity
		playerShipVariables.ship.cargo_capicity_kg = Ship.StartingCargoCap;

		//Let's increase the ships Provisions and water base to reflect starting crew members
		playerShipVariables.ship.cargo[0].amount_kg = Ship.StartingWater;
		playerShipVariables.ship.cargo[1].amount_kg = Ship.StartingFood;

		//Increase the quest counter because the start screen takes care of the first leg
		// KD: Your first quest is to find pagasae, not tomb of dolops
		// KDTODO: Maybe should come back??????????? Think about this.
		// now we start on quest 0 at the beginning of the game, with an upgradeship trigger assumed.
		Debug.Log(quest.currentQuestSegment);
		//quest.currentQuestSegment++;

		var segment = quest.questSegments[quest.currentQuestSegment];

		//first show a window for the welcome message, and if there are any crew member changes, then let the player know.
		// KD: Removed the concept of the first welcome message
		// KDTODO: Maybe should come back??????????? Think about this.
		//notificationMessage = segment.descriptionAtCompletion;
		//showNotification = true;

		// TODO: KD: I believe this whole block is unnecessary now that the first quest segment is to upgrade your ship and not a city. just leaving the log blank since
		// only cities are supported in log entries at the moment. i imagine this will come back in some form, but it doesn't make sense for now.
		// if it DOES come back, we should call StartNextQuestSegment instead of copy pasting all that code in here
		{ 
			/*
			//Add this message to the captain's log
			playerShipVariables.ship.shipCaptainsLog.Add(new CaptainsLogEntry(segment.destinationID, segment.descriptionOfQuest));
			playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
			currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
			//Now add the mentioned places attached to this quest leg
			foreach (int i in segment.mentionedPlaces) {
				//Make sure we don't add any null values--a -1 represents no mentions of any settlements
				if (i != -1)
					playerShipVariables.ship.playerJournal.AddNewSettlementToLog(i);
			}
			Debug.Log(quest.currentQuestSegment);

			//Then increment the questline to the in succession and update the player captains log with the new information for the next quest line
			quest.currentQuestSegment++;

			// TODO: This line bad. It used to set a field and actually change the quest segment, but i think we don't want this anymore anyway so disabling it.
			//segment = quest.questSegments[quest.currentQuestSegment];

			playerShipVariables.ship.shipCaptainsLog.Add(new CaptainsLogEntry(segment.destinationID, segment.descriptionOfQuest));
			playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = playerShipVariables.ship.totalNumOfDaysTraveled + " days";
			currentCaptainsLog = playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + playerShipVariables.ship.shipCaptainsLog[playerShipVariables.ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + currentCaptainsLog;
			//Now add the mentioned places attached to this quest leg
			foreach (int i in segment.mentionedPlaces) {
				//Make sure we don't add any null values--a -1 represents no mentions of any settlements
				if (i != -1)
					playerShipVariables.ship.playerJournal.AddNewSettlementToLog(i);
			}
			//Now add the city name of the next journey quest to the players known settlements
			playerShipVariables.ship.playerJournal.AddNewSettlementToLog(segment.destinationID);
			*/
		}

		//Now teleport the player ship to an appropriate location near the first target
		playerShip.transform.position = new Vector3(1702.414f, playerShip.transform.position.y, 2168.358f);
		//Set the player's initial position to the new position
		playerShipVariables.lastPlayerShipPosition = playerShip.transform.position;

		//Setup Difficulty Level
		gameVars.SetupBeginnerGameDifficulty();

		// setup each city with 5 crew available and for now, they never regenerate.
		foreach (var settlement in gameVars.settlement_masterList) {
			settlement.availableCrew.Clear();
			gameVars.GenerateRandomCrewMembers(5).ForEach(c => settlement.availableCrew.Add(c));
		}

		Debug.Log(quest.currentQuestSegment);

		//Flag the main GUI scripts to turn on
		gameVars.runningMainGameGUI = true;
	}

}
