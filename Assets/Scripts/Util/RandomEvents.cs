using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class RandomEvents
{
	//#########################################################################################################
	//	RANDOM  EVENT  FUNCTION
	//=========================
	//		--This function determines whether or not a random event will happen to the ship. Regardless of 
	//		--whether or not a random event occurs, it will trigger journal messages based on whether or not
	//		--the ship is in open sea or near a sphere of influence of a settlement/location of interest
	//
	//#########################################################################################################

	// it makes sense for this to have access to the ship, the ship's movement data and position, and the crew, as well as things like clout
	// doesn't need access to things like the GUI
	public static void WillARandomEventHappen(GameVars gameVars, Ship ship, ShipSpeedModifiers shipSpeedModifiers, Transform shipTransform) {


		//Random Events have a chance to occur every half day of travel
		//-------------------------------------------------------------
		//These values help determine the half day of travel
		float tenthPlaceTemp = (ship.totalNumOfDaysTraveled - Mathf.FloorToInt(ship.totalNumOfDaysTraveled));
		tenthPlaceTemp *= 10;
		//Debug.Log (tenthPlaceTemp + "  " + hundredthPlaceTemp);
		//If we are at a half day's travel, then see if a random event occurs
		if ((Mathf.FloorToInt(tenthPlaceTemp) == 5 || Mathf.FloorToInt(tenthPlaceTemp) == 9) && !gameVars.isPerformingRandomEvent) {
			gameVars.isPerformingRandomEvent = true;
			float chanceOfEvent = .95f; //0 - 1 value representing chance of a random event occuring
										//We determine if the 
			if (Random.Range(0f, 1f) <= chanceOfEvent) {
				//Debug.Log ("Triggering Random Event");
				//When we trigger a random event, let's make the ship drop anchor!
				gameVars.playerShipVariables.rayCheck_stopShip = true;

				//We separate Random events into two possible categories: Positive, and Negative.
				//First we need to determine if the player has a positive or negative event occur
				//--The basic chance is a 50/50 chance of either or, but we need to figure out if the
				//--crew makeup has any augers, and if so, each auger decreases the chance of a negative (now controlled by PostiveEvent modifiers)
				//--event by 10%. We then roll an aggregate clout score to further reduce the chance by a maximum of 20%

				//Get the 0-1 aggregate clout score. Here we use the current zone of influence's network id to check
				int currentZoneID = 0;
				//TODO Right now this just uses the relevant city's ID to check--but in the aggregate score function--it should start using the networks--not the city.
				if (gameVars.activeSettlementInfluenceSphereList.Count > 0) currentZoneID = gameVars.activeSettlementInfluenceSphereList[0];
				float aggregateCloutScore = gameVars.GetOverallCloutModifier(currentZoneID);
				//Now determine the final weighted chance score that will be .5f and under
				chanceOfEvent = .5f - ship.crewRoster.Sum(c => c.changeOnHire.PositiveEvent / 100f) - (.2f * aggregateCloutScore);


				//If we roll under our range, that means we hit a NEGATIVE random event
				//**************************
				//******NEGATIVE EVENT
				//**************************
				if (Random.Range(0f, 1f) <= chanceOfEvent) {

					int numOfEvents = Random.Range(1, 4);
					//Here are negative random events that can happen while traveling
					//---------------------------------------------------------------		
					//*pirate attack: You lose cargo and/or crew members, and ship hp is reduced
					//*storm: your ship is moved to a random location within half a days travel, ship hp is reduced
					//*crewmember is sick: they may or may not die, but temporarily acts as one less crew member, and uses twice as much water
					//*military request: War trireme may demand supplies for war effort and ask for crew who might randomly join them
					switch (numOfEvents) {

						//======================
						//PIRATE ATTACK
						case 1:
							//We need to determine whether or not the player successfully defends against the pirate attack
							//The first check is to see if the pirates are part of the same network as the player--if they are, they apologize and leave the player alone
							//if they aren't in the same network, the player has a base 20% chance of succeeding plus 5% per warrior present on board, plus a max of 20% based on aggregate clout
							int numOfWarriors = 0;
							foreach (CrewMember crewman in ship.crewRoster) { if (crewman.typeOfCrew == CrewType.Warrior) numOfWarriors++; }
							//TODO Right now we'll just assume they aren't in the network
							chanceOfEvent = .2f + (.05f * numOfWarriors) + (.2f * aggregateCloutScore);
							//Damage the ship regardlesss of the outcome
							gameVars.AdjustPlayerShipHealth(-20);
							//If the roll is lower than the chanceOfEvent variable--the pirates were unsuccessful
							if (Random.Range(0f, 1f) <= chanceOfEvent) {
								//Raise the player and crews clout after the successful event
								gameVars.AdjustCrewsClout(3);
								gameVars.AdjustPlayerClout(3);

								//Despite their lack of success--there might be a chance of losing a crewman to the attack
								//The chance of a death is 20%  minus a total of 10% from the agregate clout score.
								chanceOfEvent = .2f - (.1f * aggregateCloutScore);
								//If a crew member dies
								if (Random.Range(0f, 1f) < chanceOfEvent) {
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
							break;

						//=====================	
						//STORM AT SEA	
						case 2:
							StormAtSea(ship, gameVars, shipTransform, chanceOfEvent, aggregateCloutScore);
							break;


						//SICK CREW MEMBER		
						case 3:
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
							break;


						//MILITARY CONFISCATION		
						case 4:
							//A fleet of triremes heading to battle stop your ship and demand some of your stores for their journey
							finalMessage = "You spot a small fleet of ships in the distance closing in fast on your own vessel. The crew is terribly worried it may be pirates!" +
							"As they approach you realize it's not pirates--but it may as well be--a military expedition! They hail you and explain their war efforts." +
							"You acknowledge their courage and praise their victories to come, all the while waiting for the captain to make his demands upon your ship." +
							" Everyone is a pirate these days it seems!";

							//Find a random resource on board the ship that is available and remove half of it.
							bool whileBreaker = true;
							while (whileBreaker) {
								int cargoIndex = Random.Range(0, 14);
								if (ship.cargo[cargoIndex].amount_kg > 0) {
									int amountToRemove = Mathf.CeilToInt(ship.cargo[cargoIndex].amount_kg / 2f);
									whileBreaker = false;
									ship.cargo[cargoIndex].amount_kg /= 2;
									finalMessage += " The troops demand a manifest and upon inspection, determine they require " + amountToRemove + "kg of " + ship.cargo[cargoIndex].name + " from your stores. You grit your teeth but smile and agree. You're in no position to argue!";
								}


							}
							//Remove a random crew member who is taken for the war effort
							CrewMember tempCrew = RemoveRandomCrewMember(ship);
							//if the removed crewmember isn't flagged as null, then there are crewmembers to lose
							if (tempCrew.ID != -1) {
								finalMessage += "The captain also eyes your crew before explaining how he needs another set of strong arms to man an oar on his trireme! He looks at " +
								tempCrew.name + " and demands he come aboard his ship. " + tempCrew.name + " looks at you and sighs--knowing there's nothing that can be done! He wishes you the best and thanks you for the stores and crewman!";

								//otherwise, the military doesn't want any of your crew
							}
							else {
								finalMessage += "The captain eyes over your crew and makes a strange sound of displeasure. He explains none of your crew seem capable enough for the war effort and that they'll be on their way. They wish you luck on your journey!";
							}

							finalMessage += "You think to yourself, as the commander sails away with his small fleet, how odd it is to thank someone for stealing from them. Your crew seems equally frustrated, but equally glad they aren't sailing to some unknown battle against some unknown king. You unfurl the sails and go about your journey!";

							gameVars.ShowANotificationMessage(finalMessage);
							break;







					}


					//**************************
					//*****POSITIVE EVENT
					//**************************
				}
				else {
					//TODO This is a temporary fix to show something positive happened
					//	MGV.notificationMessage = 
					//	MGV.showNotification = true;	


					Debug.Log("POSITIVE EVENT TRIGGER");
					//Here are positive random events that can happen while traveling
					//---------------------------------------------------------------		
					//friendly ship: offer out of network information--if low on water/Provisions they may offer you some stores and suggest a port to visit
					//Abandoned/Raided Ship: crew is dead but you find left over stores to add to your cargo
					//Favor of the Gods: Your crew feels suddenly uplifted and courageous after the siting of a mysterious event and the ship's base speed is permanently increased by 1km an hour for the next 12 hours.
					//Poseidon's Bounty: The crew realizes there is an abundance of fish so you stop to cast nets and add additional units of Provisions to your stores.
					int numOfEvents = Random.Range(1, 5);
					switch (numOfEvents) {
						//=====================	
						//FRIENDLY SHIP
						case 1:
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
							break;
						//=====================	
						//ABADONED SHIP		
						case 2:
							finalMessage = "You come upon a derelict ship floating in the distance. Cautiously you approach it--wary of piracy--but the" +
							" smell of the salty air can't mask the stench from the corpses laying about...drying like grapes in the sun. Suddenly one of the bodies" +
							" had a voice and with his dying gasps, the sailor begs you to sink the ship with them aboard to make peace with Poseidon. Your crew" +
							" makes the final preparations, but search the ship's stores for anything useful.";

							//first determine the type of cargo to add to the ship's stores.
							int typeOfCargo = Random.Range(2, 14);
							//Now determine how much cargo the player can hold
							int amountCanHold = (int)(ship.cargo_capicity_kg - ship.GetTotalCargoAmount());

							//If there is room on board(There will almost ALWAYS be some room) then tell the player how much they found
							if (amountCanHold > 0) {
								int amountToAdd = (int)(Random.Range(1, amountCanHold) * aggregateCloutScore);
								finalMessage += "The crew finds " + amountToAdd + " kg of " + ship.cargo[typeOfCargo].name + ". What luck! I'm sure Poseidon won't mind if we just take a little something for our troubles! This should fetch a fair price at the market!";
								ship.cargo[typeOfCargo].amount_kg += amountToAdd;
							}
							else {
								finalMessage += "The crew finds some " + ship.cargo[typeOfCargo].name + " but there isn't room on board! It's probably for the best--we shouldn't take from Poseidon.";
							}

							//now add the final bit to the event
							finalMessage += "We watch the ship sink as we sail away--ever mindful that if we aren't careful, the same could happen to us!";
							gameVars.ShowANotificationMessage(finalMessage);
							break;
						//=====================	
						//FAVOR OF GODS		
						case 3:
							finalMessage = "Your crew suddenly felt uneasy causing you to drop anchor, but there were no pirates or storms in sight. " +
											"After prayers to Poseidon for your good fortune, a group of dolphins jump about your ship playing for a moment before disappearing. " +
											"Your crew takes it as a good sign and their spirits are lifted! As they begin to raise anchor, you notice the ship feels a bit faster than before." +
											" The waters seem to push you forward in a suspicious but fortunate manner!";
							shipSpeedModifiers.Event++;

							gameVars.ShowANotificationMessage(finalMessage);
							break;
						//=====================	
						//POSEIDON'S BOUNTY		
						case 4:

							finalMessage = "The crew stirs you from deep contemplation to yell about an abnormal abundance of fish jumping out of the water--practically onto the boat itself!" +
							" They want to drop anchor and reap the bounty that Poseidon has deemed the crew worthy of! You agree to drop anchor and cast nets--all the while wary of the tricks" +
							" the gods play upon mortals.";

							//Determine how much cargo the player can hold
							amountCanHold = (int)(ship.cargo_capicity_kg - ship.GetTotalCargoAmount());

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
							break;
						//=====================	
						//ZEUS'S BOUNTY		
						case 5:

							finalMessage = "As you stare at cloudy skies, wondering if it's an ill omen, a ray of sun shoots through the clouds" +
								" and a calm light rain of fresh water pours on the ship and its crew! The crew asks you if they should drop anchor and catch the gift of water from Zeus!" +
									" You look over the cargo holds and check the supplies before answering--always cautious not to anger the gods.";

							//Determine how much cargo the player can hold
							amountCanHold = (int)(ship.cargo_capicity_kg - ship.GetTotalCargoAmount());

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
							break;

					}

				}








			}
			//If we do or don't get a random event, we should always get a message from the crew--let's call them tales
			//here they describe things like any cities nearby if the crew is familiar or snippets of greek mythology, or they
			//may be from a list of messages concering any nearby zones of influence from passing settlements/locations of interest
			ship.shipCaptainsLog.Add(gameVars.currentLogPool[Random.Range(0, gameVars.currentLogPool.Count)]);
			ship.shipCaptainsLog[ship.shipCaptainsLog.Count - 1].dateTimeOfEntry = ship.totalNumOfDaysTraveled + " days";
			gameVars.currentCaptainsLog = ship.shipCaptainsLog[ship.shipCaptainsLog.Count - 1].dateTimeOfEntry + "\n" + ship.shipCaptainsLog[ship.shipCaptainsLog.Count - 1].logEntry + "\n\n" + gameVars.currentCaptainsLog;
		}


		//let's make sure the trigger for a new log  / event doesn't happen again until needed by
		//	by turning it off when the the trigger number changes--which means it won't take effect
		//	again until the next time the trigger number occurs
		//Debug.Log (Mathf.FloorToInt(tenthPlaceTemp));
		if (Mathf.FloorToInt(tenthPlaceTemp) != 5 && Mathf.FloorToInt(tenthPlaceTemp) != 9) gameVars.isPerformingRandomEvent = false;

	}

	static CrewMember RemoveRandomCrewMember(Ship ship) {
		//Find a random crewmember to kill if they can be killed

		CrewMember killedMate = new CrewMember(-1);
		List<int> listOfPossibleCrewToKill = new List<int>();

		//generate a list of possible crew that can be killed
		for (int i = 0; i < ship.crewRoster.Count; i++) {
			if (ship.crewRoster[i].isKillable) listOfPossibleCrewToKill.Add(i);
		}

		//if we don't find any available crewmembers to kill, return an empty crewman as a flag that none exist to be killed
		if (listOfPossibleCrewToKill.Count != 0) {
			int randomMemberToKill = listOfPossibleCrewToKill[Random.Range(0, listOfPossibleCrewToKill.Count - 1)];
			//Store the crewman in a temp variable
			killedMate = ship.crewRoster[randomMemberToKill];
			//Remove the crewmember
			ship.crewRoster.Remove(killedMate);
			//return the removed crewmember
			return killedMate;


		}

		//If there are no available members then just return a null flagged member initialize in the beggining of this function
		return killedMate;


	}

	public static void StormAtSea(Ship ship, GameVars gameVars, Transform shipTransform, float chanceOfEvent, float aggregateCloutScore) {

		//We need to dtermine whether or not the player sucessfully navigates through the storm.
		//The player has a 20% chance of succeeding plus 1% per sailor on board (now controlled by Navigation modifiers), plus a max of 20% based on aggregate clout
		chanceOfEvent = .2f + ship.crewRoster.Sum(c => c.changeOnHire.Navigation / 100f) + (.2f * aggregateCloutScore);
		Debug.Log(chanceOfEvent);
		//If the roll is lower than the chanceOfEvent variable--the storm was unsuccessful in throwing the player off course
		if (Random.Range(0f, 1f) <= chanceOfEvent) {
			//Adjust crew clout
			gameVars.AdjustCrewsClout(3);
			gameVars.AdjustPlayerClout(3);

			//Despite their lack of success--there might be a chance of losing a crewman to the storm
			//The chance of a death is 20%  minus a total of 10% from the agregate clout score.
			gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. The waves crash upon your ship and your sails whip in the winds, but your crew holds fast and successfully navigates the storm!");

			//Otherwise the storm was successful and the necessary penalties occur
		}
		else {
			//Adjust crew clout
			gameVars.AdjustCrewsClout(-3);
			gameVars.AdjustPlayerClout(-3);

			//The first penalty is a possibility for the death of 3 crew members
			//second penalty is the movement of the ship in a random direction for 50 in-game units ~50km or until a shoreline is reached
			//--this is accomplished through a raycast shot in a random direction from the ship
			//set the distance to 50
			float offCourseDistance = 50f;
			//set the layer mask to only check for collisions on layer 10 ("terrain")--this helps ignore the multiple irrelevant hit boxes that exist in the environment(including the player)
			int terrainLayerMask = 1 << 10;
			//We get a random directional vector3 by keeping the y at 0 and providing a value of -1 to 1 for the x and z values
			Vector3 offCourseDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
			//Set the origin to be at the base of the ship
			Vector3 rayOrigin = shipTransform.transform.position;// + new Vector3(0,-.23f,0);

			RaycastHit possibleTerrain;
			//If we get a hit then push the players ship 5 units (~5km) before the impact off the shore
			if (Physics.Raycast(rayOrigin, offCourseDirection, out possibleTerrain, offCourseDistance, terrainLayerMask)) {
				//Determine the location of the shore hit
				Vector3 hitLocation = possibleTerrain.point;
				//Determine the location of the off-shore hit by cycling back 5 units in the direction of the ray origin
				Vector3 courseDirectionOpposite = offCourseDirection * -1f;
				Vector3 adjustedLocation = hitLocation + (courseDirectionOpposite * 5f);
				//Move the player's ship to the location
				shipTransform.transform.position = adjustedLocation;



				//If we don't get a hit, then just move the player ship to that position.
			}
			else {
				//move the player to the new position
				Vector3 offCoursePosition = rayOrigin + (offCourseDirection * 50f);
				shipTransform.transform.position = offCoursePosition;
			}

			//TODO Turn off the ghost trail path of the player to reduce their ability to find their location--this will turn on when revisiting a known settlement.
			gameVars.playerGhostRoute.SetActive(false);

			//Kill up to 6 crew members normally!
			int numOfCrewToKill = Random.Range(1, 6);

			// kill EVERYONE if you end up plopped onto land, this will trigger a game over
			var isShipwrecked = false;
			if (gameVars.playerShipVariables.IsOnLand(shipTransform.transform.position)) {
				numOfCrewToKill = ship.crew;
				isShipwrecked = true;
			}

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
			//Display message telling the player what occured
			if (lostCrew.Count == 0) {
				gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
										 "until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
										"it will, leaving you lost asea without any known bearings across the waters.");
			}
			else if(isShipwrecked) {
				gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
									"until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
									"it will, leaving you shipwrecked. Unfortunately, you lose ALL of your crew to the storm's wrath!");
				gameVars.isGameOver = true;
			}
			else if(ship.crew == 0) {
				gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
									"until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
									"it will. Unfortunately, you lose ALL of your crew to the storm's wrath!");
				gameVars.isGameOver = true;
			}
			else {
				gameVars.ShowANotificationMessage("A storm suddenly surges across the seas filling your crew with worry. They struggle for hours " +
									"until the storm overcomes their senses and abilities. You all hold tight and let the storm take your ship where " +
									"it will, leaving you lost asea without any known bearings across the waters. Unfortunately, you lose " + lostNames + " to the storm's wrath! You say a few prayers to Poseidon " +
									"and struggle onward to find your way!");
			}


		}
	}
}
