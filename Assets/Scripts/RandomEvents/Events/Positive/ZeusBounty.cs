using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ZeusBounty : RandomEvents.PositiveEvent
{
	public override void Execute() {
		var finalMessage = "As you stare at cloudy skies, wondering if it's an ill omen, a ray of sun shoots through the clouds" +
							" and a calm light rain of fresh water pours on the ship and its crew! The crew asks you if they should drop anchor and catch the gift of water from Zeus!" +
							" You look over the cargo holds and check the supplies before answering--always cautious not to anger the gods.";

		//Determine how much cargo the player can hold
		var amountCanHold = (int)(ship.cargo_capicity_kg - ship.GetTotalCargoAmount());

		//If there is room on board(There will almost ALWAYS be some room so let's say at least 50kg) then tell the player how much they found
		//if there is less than 50kg of room, but the ship is low on water, then the crew can have the water
		if (amountCanHold > 50 || ship.cargo[0].amount_kg <= 100) {
			int amountToAdd = (int)(Random.Range(1, amountCanHold) * gameVars.GetOverallCloutModifier(gameVars.currentSettlement.settlementID));
			finalMessage += "The crew catches " + amountToAdd + " kg of water from the rain. What luck! Praise be to Poseidon! Hopefully this isn't one of his tricks!";
			ship.cargo[0].amount_kg += amountToAdd;
		}
		else {
			finalMessage += " Suddenly you stop the crew, shouting that the stores are already full enough! It would be foolish to take the bounty--" +
				"obviously it's a gift to Zeus' brother--not to us and not worth the risk! We continue on our journey--despite the grumblings of the crew.";
		}

		gameVars.ShowANotificationMessage(finalMessage);
	}
}
