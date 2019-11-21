using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Abandoned/Raided Ship: crew is dead but you find left over stores to add to your cargo
public class AbandonedShip : RandomEvents.PositiveEvent
{
	public override void Execute() {
		var finalMessage = "You come upon a derelict ship floating in the distance. Cautiously you approach it--wary of piracy--but the" +
							" smell of the salty air can't mask the stench from the corpses laying about...drying like grapes in the sun. Suddenly one of the bodies" +
							" had a voice and with his dying gasps, the sailor begs you to sink the ship with them aboard to make peace with Poseidon. Your crew" +
							" makes the final preparations, but search the ship's stores for anything useful.";

		//first determine the type of cargo to add to the ship's stores.
		int typeOfCargo = Random.Range(2, 14);
		//Now determine how much cargo the player can hold
		int amountCanHold = (int)(ship.cargo_capicity_kg - ship.GetTotalCargoAmount());

		//If there is room on board(There will almost ALWAYS be some room) then tell the player how much they found
		if (amountCanHold > 0) {
			int amountToAdd = (int)(Random.Range(1, amountCanHold) * aggregateCloutScore);
			finalMessage += "The crew finds " + amountToAdd + " kg of " + ship.cargo[typeOfCargo].name + ". What luck! I'm sure Poseidon won't mind if we just take a little something for our troubles! This should fetch a fair price at the market!";
			ship.cargo[typeOfCargo].amount_kg += amountToAdd;
		}
		else {
			finalMessage += "The crew finds some " + ship.cargo[typeOfCargo].name + " but there isn't room on board! It's probably for the best--we shouldn't take from Poseidon.";
		}

		//now add the final bit to the event
		finalMessage += "We watch the ship sink as we sail away--ever mindful that if we aren't careful, the same could happen to us!";
		gameVars.ShowANotificationMessage(finalMessage);
	}
}
