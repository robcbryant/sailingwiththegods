using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//*storm: your ship is moved to a random location within half a days travel, ship hp is reduced
public class StormAtSea : RandomEvents.NegativeEvent
{
	public override void Execute() {
		Globals.MiniGames.Enter("Storm MG/Storm Game");
	}
}