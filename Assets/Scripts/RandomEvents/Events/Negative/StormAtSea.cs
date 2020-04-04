using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//*storm: your ship is moved to a random location within half a days travel, ship hp is reduced
public class StormAtSea : RandomEvents.NegativeEvent
{
	public GameObject StormMiniGame;
	public GameObject currentCam;

	

	public override void Execute() {

		StormMiniGame.SetActive(true);
		currentCam.SetActive(false);

	//	//We need to dtermine whether or not the player sucessfully navigates through the storm.
	//	//The player has a 20% chance of succeeding plus 1% per sailor on board (now controlled by Navigation modifiers), plus a max of 20% based on aggregate clout
	//	var chance = .2f + ship.crewRoster.Sum(c => c.changeOnHire.Navigation / 100f) + (.2f * aggregateCloutScore);
	//	Debug.Log(chance);
	//	//If the roll is lower than the chanceOfEvent variable--the storm was unsuccessful in throwing the player off course
	//	if (Random.Range(0f, 1f) <= chance) {
	//		//Adjust crew clout
	//		gameVars.AdjustCrewsClout(3);
	//		gameVars.AdjustPlayerClout(3);

	//		//Despite their lack of success--there might be a chance of losing a crewman to the storm
	//		//The chance of a death is 20%  minus a total of 10% from the agregate clout score.
	//		gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. The waves crash upon your ship and your sails whip in the winds, but your crew holds fast and successfully navigates the storm!");

	//		//Otherwise the storm was successful and the necessary penalties occur
	//	}
	//	else {
	//		//Adjust crew clout
	//		gameVars.AdjustCrewsClout(-3);
	//		gameVars.AdjustPlayerClout(-3);

	//		//The first penalty is a possibility for the death of 3 crew members
	//		//second penalty is the movement of the ship in a random direction for 50 in-game units ~50km or until a shoreline is reached
	//		//--this is accomplished through a raycast shot in a random direction from the ship
	//		//set the distance to 50
	//		float offCourseDistance = 50f;
	//		//set the layer mask to only check for collisions on layer 10 ("terrain")--this helps ignore the multiple irrelevant hit boxes that exist in the environment(including the player)
	//		int terrainLayerMask = 1 << 10;
	//		//We get a random directional vector3 by keeping the y at 0 and providing a value of -1 to 1 for the x and z values
	//		Vector3 offCourseDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
	//		//Set the origin to be at the base of the ship
	//		Vector3 rayOrigin = shipTransform.transform.position;// + new Vector3(0,-.23f,0);

	//		RaycastHit possibleTerrain;
	//		//If we get a hit then push the players ship 5 units (~5km) before the impact off the shore
	//		if (Physics.Raycast(rayOrigin, offCourseDirection, out possibleTerrain, offCourseDistance, terrainLayerMask)) {
	//			//Determine the location of the shore hit
	//			Vector3 hitLocation = possibleTerrain.point;
	//			//Determine the location of the off-shore hit by cycling back 5 units in the direction of the ray origin
	//			Vector3 courseDirectionOpposite = offCourseDirection * -1f;
	//			Vector3 adjustedLocation = hitLocation + (courseDirectionOpposite * 5f);
	//			//Move the player's ship to the location
	//			shipTransform.transform.position = adjustedLocation;



	//			//If we don't get a hit, then just move the player ship to that position.
	//		}
	//		else {
	//			//move the player to the new position
	//			Vector3 offCoursePosition = rayOrigin + (offCourseDirection * 50f);
	//			shipTransform.transform.position = offCoursePosition;
	//		}

	//		//TODO Turn off the ghost trail path of the player to reduce their ability to find their location--this will turn on when revisiting a known settlement.
	//		gameVars.playerGhostRoute.SetActive(false);

	//		//Kill up to 6 crew members normally!
	//		int numOfCrewToKill = Random.Range(1, 6);

	//		// kill EVERYONE if you end up plopped onto land, this will trigger a game over
	//		var isShipwrecked = false;
	//		if (gameVars.playerShipVariables.IsOnLand(shipTransform.transform.position)) {
	//			numOfCrewToKill = ship.crew;
	//			isShipwrecked = true;
	//		}

	//		List<CrewMember> lostCrew = new List<CrewMember>();
	//		string lostNames = "";
	//		//Fill a list of killed crew
	//		for (int i = 0; i <= numOfCrewToKill; i++) {
	//			CrewMember temp = RemoveRandomCrewMember(ship);
	//			//if the removed crewmember is flagged as null, then there are no crewmembers to kill
	//			if (temp.ID != -1) {
	//				lostCrew.Add(temp);
	//				//add the name to the compiled string with a comma if not the last index
	//				if (i != numOfCrewToKill) lostNames += temp.name + ", "; else lostNames += temp.name;
	//			}
	//		}
	//		//Display message telling the player what occured
	//		if (lostCrew.Count == 0) {
	//			gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
	//									 "until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
	//									"it will, leaving you lost asea without any known bearings across the waters.");
	//		}
	//		else if (isShipwrecked) {
	//			gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
	//								"until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
	//								"it will, leaving you shipwrecked. Unfortunately, you lose ALL of your crew to the storm's wrath!");
	//			gameVars.isGameOver = true;
	//		}
	//		else if (ship.crew == 0) {
	//			gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
	//								"until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
	//								"it will. Unfortunately, you lose ALL of your crew to the storm's wrath!");
	//			gameVars.isGameOver = true;
	//		}
	//		else {
	//			gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
	//								"until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
	//								"it will, leaving you lost asea without any known bearings across the waters. Unfortunately, you lose " + lostNames + " to the storm's wrath! You say a few prayers to Poseidon " +
	//								"and struggle onward to find your way!");
	//		}


	//	}
	}
}