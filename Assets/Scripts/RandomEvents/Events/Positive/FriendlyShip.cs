using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//friendly ship: offer out of network information--if low on water/Provisions they may offer you some stores and suggest a port to visit
public class FriendlyShip : RandomEvents.PositiveEvent
{
	public override void Execute() {
		string finalMessage = "You encounter a ship asea--worried at first that it seems like pirates! Fortunately it appears to be" +
							  " a friendly ship who says hello!";
		string messageWaterModifier = "";
		string messageProvisionsModifier = "";
		//First determine if the player is low on Provisions or water
		//--If the player is low on water
		if (ship.cargo[0].amount_kg <= 100) {
			//add a random amount of water to the stores between 30 and 60 and modified by clout
			int waterBonus = Mathf.FloorToInt(Random.Range(30, 60) * aggregateCloutScore);
			messageWaterModifier += " You received " + waterBonus + " kg of water ";

		}
		//--If the player is low on Provisions
		if (ship.cargo[1].amount_kg <= 100) {
			//add a random amount of Provisions to the stores between 30 and 60 and modified by clout
			int ProvisionsBonus = Mathf.FloorToInt(Random.Range(30, 60) * aggregateCloutScore);
			messageProvisionsModifier += "Thankfully you were given  " + ProvisionsBonus + " kg of Provisions ";

		}

		//Determine which message to show based on what the ship did for you!
		//If there are stores given--let the player know
		if (messageWaterModifier != "" || messageProvisionsModifier != "") {
			finalMessage += "They notice you are low on supplies and offer a bit of their own!";
		}
		else {
			finalMessage += "All they can offer are Provisions and water if you are in need, but your stores seem full enough!";
		}

		//Now add what Provisions and water they give you to the message
		finalMessage += messageWaterModifier + messageProvisionsModifier + " They bid you farewell and wish Poseidon's favor upon you!";
		gameVars.ShowANotificationMessage(finalMessage);
	}
}
