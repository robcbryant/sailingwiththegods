using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//*pirate attack: You lose cargo and/or crew members, and ship hp is reduced
public class PirateAttack : RandomEvents.NegativeEvent
{
	//increased weight to have minigame occur more often
	public override float Weight() {
		return 2f;
	}


	//checks to see if the player is wihtin one of the pirate zones
	//if player is not in a pirate zone, the minigame will not occur during gameplay
	public override bool isValid() {
		if (Globals.GameVars.playerShipVariables.zonesList.Count > 0) { return base.isValid(); }
		else { return false; }
	}
	public override void Execute() {		
		Globals.MiniGames.Enter("Pirate Game/Pirate Game");
	}
}
