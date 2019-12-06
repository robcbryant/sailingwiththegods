using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//*pirate attack: You lose cargo and/or crew members, and ship hp is reduced
public class PirateAttack : RandomEvents.NegativeEvent
{
	public override void Execute() {
		//We need to determine whether or not the player successfully defends against the pirate attack
		//The first check is to see if the pirates are part of the same network as the player--if they are, they apologize and leave the player alone
		//if they aren't in the same network, the player has a base 20% chance of succeeding plus 5% per warrior present on board, plus a max of 20% based on aggregate clout
		int numOfWarriors = 0;
		foreach (CrewMember crewman in ship.crewRoster) { if (crewman.typeOfCrew == CrewType.Warrior) numOfWarriors++; }
		//TODO Right now we'll just assume they aren't in the network
		var chance = .2f + (.05f * numOfWarriors) + (.2f * aggregateCloutScore);
		//Damage the ship regardlesss of the outcome
		gameVars.AdjustPlayerShipHealth(-20);
		//If the roll is lower than the chanceOfEvent variable--the pirates were unsuccessful
		if (Random.Range(0f, 1f) <= chance) {
			//Raise the player and crews clout after the successful event
			gameVars.AdjustCrewsClout(3);
			gameVars.AdjustPlayerClout(3);

			//Despite their lack of success--there might be a chance of losing a crewman to the attack
			//The chance of a death is 20%  minus a total of 10% from the agregate clout score.
			chance = .2f - (.1f * aggregateCloutScore);
			//If a crew member dies
			if (Random.Range(0f, 1f) < chance) {
				//get a random crewmember
				CrewMember crewToKill = RemoveRandomCrewMember(ship);
				//If there is a crewmember to kill
				if (crewToKill.ID != -1) {
					gameVars.ShowANotificationMessage("Your crew spots a distant ship closing in fast--definitely pirates! By the gods! You manage to escape after fending off their attack! As your crew cheers with victorious honor, "
											+ "they suddenly stop upon realizing they lost a good crewman... " + crewToKill.name + "'s sacrifice has earned him great honor. You prepare the proper rites and cast the sailor asea.");
					//otherwise there is no crewmember to kill--either from not having enough crewmembers or no crewmembers that are killable so just give the success response without a death
				}
				else {
					gameVars.ShowANotificationMessage("Your crew spots a distant ship closing in fast--definitely pirates! By the gods! After a heated battle, you manage to escape after fending off their attack without any casualties! Your crew cheers with victorious honor!");
				}
				//No crewmembers died so it was a perfect defensive victory	
			}
			else {
				gameVars.ShowANotificationMessage("Your crew spots a distant ship closing in fast--definitely pirates! By the gods! After a heated battle, you manage to escape after fending off their attack without any casualties! Your crew cheers with victorious honor!");
			}


			//Otherwise the pirates were successful and the necessary penalties occur
		}
		else {
			//Reduce the clout of the player and crew
			gameVars.AdjustCrewsClout(-3);
			gameVars.AdjustPlayerClout(-3);

			// penalty:loss of half the ship's cargo across the board.
			foreach (Resource resource in ship.cargo) {
				int newAmount = (int)(resource.amount_kg / 2);
				resource.amount_kg -= newAmount;

			}

			// penalty:the death of up to 6 crew members if available/killable
			int numOfCrewToKill = Random.Range(1, 6);
			List<CrewMember> lostCrew = new List<CrewMember>();
			string lostNames = "";
			//Fill a list of killed crew
			for (int i = 0; i <= numOfCrewToKill; i++) {
				CrewMember temp = RemoveRandomCrewMember(ship);
				//if the removed crewmember is flagged as null, then there are no crewmembers to kill
				if (temp.ID != -1) {
					lostCrew.Add(temp);
					//add the name to the compiled string with a comma if not the last index
					if (i != numOfCrewToKill) lostNames += temp.name + ", "; else lostNames += temp.name;
				}
			}
			//If the list of killed crew is empty, then we didn't kill any crewmembers so add a message that explains why
			if (lostCrew.Count == 0) {
				gameVars.ShowANotificationMessage("Your crew spots a distant ship closing in fast--they are definitely pirates! Your crew prepares for battle as the ship rams at full speed into your hull." +
										  " Your crew fights valiantly against the onslaught, but the pirates successfully bring you to your knees! They rummage through your holds and take half your supplies with them as a generous bounty." +
										  " They intended to take a number of you prisoner, but the pirate captain sensed a strange omen surrounding you and wanted no part in your crew's fate. They leave your dishonored ship with their newly acquired supplies.");
				//Otherwise members died so alert the player
			}
			else {
				gameVars.ShowANotificationMessage("Your crew spots a distant ship closing in fast--they are definitely pirates! Your crew prepares for battle as the ship rams at full speed into your hull." +
											" Your crew fights valiantly against the onslaught, but the pirates successfully bring you to your knees! They rummage through your holds and take half your supplies with them as a generous bounty." +
											" Unfortunately, you lose " + lostNames + " to death and kidnapping. The remaining crew are unsettled but fortunate for their lives.");

			}


		}
	}
}
