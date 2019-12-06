using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Poseidon's Bounty: The crew realizes there is an abundance of fish so you stop to cast nets and add additional units of Provisions to your stores.
public class PoseidonsBounty : RandomEvents.PositiveEvent
{
	public override void Execute() {
		var finalMessage = "The crew stirs you from deep contemplation to yell about an abnormal abundance of fish jumping out of the water--practically onto the boat itself!" +
		" They want to drop anchor and reap the bounty that Poseidon has deemed the crew worthy of! You agree to drop anchor and cast nets--all the while wary of the tricks" +
		" the gods play upon mortals.";

		//Determine how much cargo the player can hold
		var amountCanHold = (int)(ship.cargo_capicity_kg - ship.GetTotalCargoAmount());

		//If there is room on board(There will almost ALWAYS be some room so let's say at least 50kg) then tell the player how much they found
		//if there is less than 50kg of room, but the ship is low on Provisions, then the crew can have the Provisions
		if (amountCanHold > 50 || ship.cargo[1].amount_kg <= 100) {
			int amountToAdd = (int)(Random.Range(1, amountCanHold) * aggregateCloutScore);
			finalMessage += "The crew catches " + amountToAdd + " kg of Provisions from the fish. What luck! Praise be to Poseidon! Hopefully this isn't one of his tricks!";
			ship.cargo[1].amount_kg += amountToAdd;
		}
		else {
			finalMessage += " Suddenly you stop the crew, shouting that the stores are already full enough! It would be foolish to take the bounty--" +
			"obviously it is a test by Poseidon on our avarice! We continue on our journey--despite the grumblings of the crew.";
		}

		gameVars.ShowANotificationMessage(finalMessage);
	}
}
