//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

////*military request: War trireme may demand supplies for war effort and ask for crew who might randomly join them
//public class MilitaryRequest : RandomEvents.NegativeEvent
//{
//	public override void Execute() {
//		//A fleet of triremes heading to battle stop your ship and demand some of your stores for their journey
//		var finalMessage = "You spot a small fleet of ships in the distance closing in fast on your own vessel. The crew is terribly worried it may be pirates!" +
//		"As they approach you realize it's not pirates--but it may as well be--a military expedition! They hail you and explain their war efforts." +
//		"You acknowledge their courage and praise their victories to come, all the while waiting for the captain to make his demands upon your ship." +
//		" Everyone is a pirate these days it seems!";

//		//Find a random resource on board the ship that is available and remove half of it.
//		bool whileBreaker = true;
//		while (whileBreaker) {
//			int cargoIndex = Random.Range(0, 14);
//			if (ship.cargo[cargoIndex].amount_kg > 0) {
//				int amountToRemove = Mathf.CeilToInt(ship.cargo[cargoIndex].amount_kg / 2f);
//				whileBreaker = false;
//				ship.cargo[cargoIndex].amount_kg /= 2;
//				finalMessage += " The troops demand a manifest and upon inspection, determine they require " + amountToRemove + "kg of " + ship.cargo[cargoIndex].name + " from your stores. You grit your teeth but smile and agree. You're in no position to argue!";
//			}


//		}
//		//Remove a random crew member who is taken for the war effort
//		CrewMember tempCrew = RemoveRandomCrewMember(ship);
//		//if the removed crewmember isn't flagged as null, then there are crewmembers to lose
//		if (tempCrew.ID != -1) {
//			finalMessage += "The captain also eyes your crew before explaining how he needs another set of strong arms to man an oar on his trireme! He looks at " +
//			tempCrew.name + " and demands he come aboard his ship. " + tempCrew.name + " looks at you and sighs--knowing there's nothing that can be done! He wishes you the best and thanks you for the stores and crewman!";

//			//otherwise, the military doesn't want any of your crew
//		}
//		else {
//			finalMessage += "The captain eyes over your crew and makes a strange sound of displeasure. He explains none of your crew seem capable enough for the war effort and that they'll be on their way. They wish you luck on your journey!";
//		}

//		finalMessage += "You think to yourself, as the commander sails away with his small fleet, how odd it is to thank someone for stealing from them. Your crew seems equally frustrated, but equally glad they aren't sailing to some unknown battle against some unknown king. You unfurl the sails and go about your journey!";

//		gameVars.ShowANotificationMessage(finalMessage);
//	}
//}
