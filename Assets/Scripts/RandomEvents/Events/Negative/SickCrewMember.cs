using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//*crewmember is sick: they may or may not die, but temporarily acts as one less crew member, and uses twice as much water
public class SickCrewMember : RandomEvents.NegativeEvent
{
	//weight scaled down to zero to prevent the plague-like event on 08/07/2020
	//removed due to concern of reletivity to current events (Coronavirus Pandemic) 
	public override float Weight() {
		return 0f;
	}

	public override void Execute() {
		//A random crewmember gets sick, and there is a chance up to two members die from the event
		//Kill up to 2 crew members!
		int numOfSickCrewToKill = Random.Range(1, 2);
		List<CrewMember> sickCrew = new List<CrewMember>();
		string lostSickNames = "";
		//Fill a list of killed crew
		for (int i = 0; i <= numOfSickCrewToKill; i++) {
			CrewMember temp = RemoveRandomCrewMember(ship);
			//if the removed crewmember is flagged as null, then there are no crewmembers to kill
			if (temp.ID != -1) {
				sickCrew.Add(temp);
				//add the name to the compiled string with a 'and' if not the last index
				if (i != numOfSickCrewToKill) lostSickNames += temp.name + " & "; else lostSickNames += temp.name;
			}
		}
		string finalMessage = "The crew alerts you to a terrible predicament--someone has been sick and it may have spread!";

		//check to see if anyone dies
		//If no one dies, then let the player know they survived
		if (lostSickNames == "") {
			finalMessage += " Fortunately, the crew seems fine and the sickness doesn't seem to be disease. Maybe they are just overworked a bit." +
			"The members who feel ill, express their need for a little rest and they'll be fine. You agree and the journey continues!";
		}
		else {
			//Someone was sick!
			//If it was one person
			if (!lostSickNames.Contains("&")) {
				finalMessage += "When you inspect " + lostSickNames + ", you make the decision to throw him overboard--he's definitely diseased, and the crew can't afford to catch it." +
				"It may seem heartless, but he knew what he signed up for when he decided to go on this journey!";
			}
			else {
				finalMessage += "When you have a look at " + lostSickNames + ", you give a knowing sigh--that they both have a terrible plague, and the rest of the crew agrees to throw them off the ship. It's better for two to die, than the entire crew!";
			}

		}

		finalMessage += " You continue the journey with the crew--thankful that it wasn't any worse. A plague on a ship asea is quite dangerous.";
		gameVars.ShowANotificationMessage(finalMessage);
	}
}
