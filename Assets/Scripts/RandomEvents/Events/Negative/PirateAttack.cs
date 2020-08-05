using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//*pirate attack: You lose cargo and/or crew members, and ship hp is reduced
public class PirateAttack : RandomEvents.NegativeEvent
{
	public override bool isValid() {
		if (Globals.GameVars.playerShipVariables.zonesList.Count > 0) { return base.isValid(); }
		else { return false; }
	}
	public override void Execute() {		
		Globals.MiniGames.Enter("Pirate Game/Pirate Game");
	}
}
