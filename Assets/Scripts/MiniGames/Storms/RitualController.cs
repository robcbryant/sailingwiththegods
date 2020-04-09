using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualController : MonoBehaviour
{
	private Ritual currentRitual;

	public void DisplayStartingText() 
	{
		//Show the start text

		//Show the storm start buttons
	}

	public void ChooseRitual() 
	{
		//Determine if the player has a seer or not

		//Select an appropriate ritual

		DisplayRitualText();
	}

	public void DisplayRitualText() 
	{
		//Show the ritual's flavor text

		//Show the ritual buttons

		//Set the ritual buttons' text
	}

	public void CalculateRitualResults(int action) 
	{
		int result = -1;
		string resultsText = "";

		if (action >= 0) 
		{
			//Ritual is being performed

			//Get a random number 0-1 and check it against the ritual's success chance
			
		}
		else 
		{
			//Ritual was rejected
			
		}

		//Based on the result, set the result text
		switch (result) 
		{
			case (0):
				//Easy difficulty (success)
				break;
			case (1):
				//Medium difficulty (failure)
				break;
			case (2):
				//Hard difficulty (rejection)
				break;
			default:
				//Something went wrong
				Debug.Log("Something went wrong with determining ritual results");
				break;
		}

		//Send the result to the difficulty calculator for the storm
	}

	private bool CheckResources() 
	{
		//Check if the player has the needed resources

		//Make sure you remember: -1 is a crewmember, -2 is money
		return true;
	}
}
