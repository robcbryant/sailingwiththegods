using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class RandomEvents
{
	//#########################################################################################################
	//	RANDOM  EVENT  FUNCTION
	//=========================
	//		--This function determines whether or not a random event will happen to the ship. Regardless of 
	//		--whether or not a random event occurs, it will trigger journal messages based on whether or not
	//		--the ship is in open sea or near a sphere of influence of a settlement/location of interest
	//
	//#########################################################################################################

	// it makes sense for this to have access to the ship, the ship's movement data and position, and the crew, as well as things like clout
	// doesn't need access to things like the GUI
	public static void WillARandomEventHappen(GameVars gameVars, Ship ship, ShipSpeedModifiers shipSpeedModifiers, Transform shipTransform) {


		//Random Events have a chance to occur every half day of travel
		//-------------------------------------------------------------
		//These values help determine the half day of travel
		float tenthPlaceTemp = (ship.totalNumOfDaysTraveled - Mathf.FloorToInt(ship.totalNumOfDaysTraveled));
		tenthPlaceTemp *= 10;
		//Debug.Log (tenthPlaceTemp + "  " + hundredthPlaceTemp);

		//If we are at a half day's travel, then see if a random event occurs
		if ((Mathf.FloorToInt(tenthPlaceTemp) == 5 || Mathf.FloorToInt(tenthPlaceTemp) == 9) && !gameVars.isPerformingRandomEvent) {
			gameVars.isPerformingRandomEvent = true;
			float chanceOfEvent = .95f; //0 - 1 value representing chance of a random event occuring
										//We determine if the 
			if (Random.Range(0f, 1f) <= chanceOfEvent) {
				//Debug.Log ("Triggering Random Event");
				//When we trigger a random event, let's make the ship drop anchor!
				gameVars.playerShipVariables.rayCheck_stopShip = true;

				//We separate Random events into two possible categories: Positive, and Negative.
				//First we need to determine if the player has a positive or negative event occur
				//--The basic chance is a 50/50 chance of either or, but we need to figure out if the
				//--crew makeup has any augers, and if so, each auger decreases the chance of a negative (now controlled by PostiveEvent modifiers)
				//--event by 10%. We then roll an aggregate clout score to further reduce the chance by a maximum of 20%

				//Get the 0-1 aggregate clout score. Here we use the current zone of influence's network id to check
				int currentZoneID = 0;
				//TODO Right now this just uses the relevant city's ID to check--but in the aggregate score function--it should start using the networks--not the city.
				if (gameVars.activeSettlementInfluenceSphereList.Count > 0) currentZoneID = gameVars.activeSettlementInfluenceSphereList[0];
				float aggregateCloutScore = gameVars.GetOverallCloutModifier(currentZoneID);
				//Now determine the final weighted chance score that will be .5f and under
				chanceOfEvent = .5f - ship.crewRoster.Sum(c => c.changeOnHire.PositiveEvent / 100f) - (.2f * aggregateCloutScore);


				//If we roll under our range, that means we hit a NEGATIVE random event
				if (Random.Range(0f, 1f) <= chanceOfEvent) {
					ExecuteEvent(GetSubclassesOfType<NegativeEvent>(), gameVars, ship, shipSpeedModifiers, shipTransform, aggregateCloutScore);
				}
				else {
					ExecuteEvent(GetSubclassesOfType<PositiveEvent>(), gameVars, ship, shipSpeedModifiers, shipTransform, aggregateCloutScore);
				}

			}
			//If we do or don't get a random event, we should always get a message from the crew--let's call them tales
			//here they describe things like any cities nearby if the crew is familiar or snippets of greek mythology, or they
			//may be from a list of messages concering any nearby zones of influence from passing settlements/locations of interest
			var log = gameVars.GetRandomCaptainsLogFromPool();
			if(log != null) {
				ship.shipCaptainsLog.Add(log);
				log.dateTimeOfEntry = ship.totalNumOfDaysTraveled + " days";
				gameVars.AddToCaptainsLog(log.dateTimeOfEntry + "\n" + log.logEntry);
			}
		}


		//let's make sure the trigger for a new log  / event doesn't happen again until needed by
		//	by turning it off when the the trigger number changes--which means it won't take effect
		//	again until the next time the trigger number occurs
		//Debug.Log (Mathf.FloorToInt(tenthPlaceTemp));
		if (Mathf.FloorToInt(tenthPlaceTemp) != 5 && Mathf.FloorToInt(tenthPlaceTemp) != 9) gameVars.isPerformingRandomEvent = false;

	}

	public abstract class PositiveEvent : Event { }
	public abstract class NegativeEvent : Event { }
	public abstract class Event
	{
		protected GameVars gameVars { get; private set; }
		protected Ship ship { get; private set; }
		protected ShipSpeedModifiers shipSpeedModifiers { get; private set; }
		protected Transform shipTransform { get; private set; }
		protected float aggregateCloutScore { get; private set; }

		public void Init(GameVars gameVars, Ship ship, ShipSpeedModifiers shipSpeedModifiers, Transform shipTransform, float aggregateCloutScore) {
			this.gameVars = gameVars;
			this.ship = ship;
			this.shipSpeedModifiers = shipSpeedModifiers;
			this.shipTransform = shipTransform;
			this.aggregateCloutScore = aggregateCloutScore;
		}

		public abstract void Execute();

		// HELPER FUNCTIONS THAT MAY BE USEFUL FOR ALL SUBCLASSES

		protected static CrewMember RemoveRandomCrewMember(Ship ship) {
			//Find a random crewmember to kill if they can be killed

			CrewMember killedMate = new CrewMember(-1);
			List<int> listOfPossibleCrewToKill = new List<int>();

			//generate a list of possible crew that can be killed
			for (int i = 0; i < ship.crewRoster.Count; i++) {
				if (ship.crewRoster[i].isKillable) listOfPossibleCrewToKill.Add(i);
			}

			//if we don't find any available crewmembers to kill, return an empty crewman as a flag that none exist to be killed
			if (listOfPossibleCrewToKill.Count != 0) {
				int randomMemberToKill = listOfPossibleCrewToKill[Random.Range(0, listOfPossibleCrewToKill.Count - 1)];
				//Store the crewman in a temp variable
				killedMate = ship.crewRoster[randomMemberToKill];
				//Remove the crewmember
				ship.crewRoster.Remove(killedMate);
				//return the removed crewmember
				return killedMate;


			}

			//If there are no available members then just return a null flagged member initialize in the beggining of this function
			return killedMate;


		}
	}

	static IEnumerable<System.Type> GetSubclassesOfType<T>() {
		return System.Reflection.Assembly.GetAssembly(typeof(T))
			.GetTypes()
			.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));
	}

	static Event CreateEvent(System.Type eventType, GameVars gameVars, Ship ship, ShipSpeedModifiers shipSpeedModifiers, Transform shipTransform, float aggregateCloutScore) {
		var result = System.Activator.CreateInstance(eventType) as Event;
		result.Init(gameVars, ship, shipSpeedModifiers, shipTransform, aggregateCloutScore);
		return result;
	}

	static void ExecuteEvent(IEnumerable<System.Type> options, GameVars gameVars, Ship ship, ShipSpeedModifiers shipSpeedModifiers, Transform shipTransform, float aggregateCloutScore) {
		var eventObj = CreateEvent(options.RandomElement(), gameVars, ship, shipSpeedModifiers, shipTransform, aggregateCloutScore);
		eventObj.Execute();
	}
}
