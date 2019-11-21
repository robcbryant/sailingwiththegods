using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Favor of the Gods: Your crew feels suddenly uplifted and courageous after the siting of a mysterious event and the ship's base speed is permanently increased by 1km an hour for the next 12 hours.
public class FavorOfGods : RandomEvents.PositiveEvent
{
	public override void Execute() {
		var finalMessage = "Your crew suddenly felt uneasy causing you to drop anchor, but there were no pirates or storms in sight. " +
							"After prayers to Poseidon for your good fortune, a group of dolphins jump about your ship playing for a moment before disappearing. " +
							"Your crew takes it as a good sign and their spirits are lifted! As they begin to raise anchor, you notice the ship feels a bit faster than before." +
							" The waters seem to push you forward in a suspicious but fortunate manner!";
		shipSpeedModifiers.Event++;

		gameVars.ShowANotificationMessage(finalMessage);
	}
}
