using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualController : MonoBehaviour
{
	public enum RitualResult
	{
		Success, Failure, Refusal
	}
	public MiniGameInfoScreen mgInfo;

	public Sprite stormIcon;

	public ButtonExplanation performButton;
	public ButtonExplanation rejectButton;
	
	[Tooltip("Ordered: start, ritual, ritual results, storm results")]
	[TextArea(1, 3)]
	public string[] titles;

	[Tooltip("Ordered: start, ritual, ritual results, storm results")]
	[TextArea(1, 3)]
	public string[] subtitles;

	[Tooltip("Unordered, will be chosen at random")]
	[TextArea(2, 10)]
	public string[] startingText;

	[Tooltip("Ordered: success, failure, refusal")]
	[TextArea(2, 10)]
	public string[] ritualResultsText;

	[Tooltip("Ordered by success, from no damage to game loss")]
	[TextArea(2, 10)]
	public string[] stormResultsText;

	private Ritual currentRitual;
	private CrewMember chosenCrew;

	private void OnEnable() 
	{
		currentRitual = null;
		chosenCrew = null;
		DisplayStartingText();
	}

	public void DisplayStartingText() 
	{
		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(titles[0], subtitles[0], startingText[RandomIndex(startingText)], stormIcon, MiniGameInfoScreen.MiniGame.StormStart);
	}

	public void ChooseRitual() 
	{
		//Determine if the player has a seer or not
		List<Ritual> possibleRituals = new List<Ritual>();

		bool hasSeer = CheckForSeer();
		
		for (int i = 0; i < Globals.GameVars.stormRituals.Count; i++) 
		{
			if (Globals.GameVars.stormRituals[i].HasSeer == hasSeer) 
			{
				possibleRituals.Add(Globals.GameVars.stormRituals[i]);
			}
		}

		//Select an appropriate ritual
		currentRitual = possibleRituals[RandomIndex(possibleRituals)];
		chosenCrew = Globals.GameVars.playerShipVariables.ship.crewRoster[RandomIndex(Globals.GameVars.playerShipVariables.ship.crewRoster)];

		DisplayRitualText();
	}

	public void DisplayRitualText() 
	{
		string ritualText = currentRitual.RitualText;
		string finalRitualText = ritualText.Replace("{0}", chosenCrew.name);

		mgInfo.DisplayText(titles[1], subtitles[1], finalRitualText, stormIcon, MiniGameInfoScreen.MiniGame.Storm);

		string performText = $"{currentRitual.SuccessChance * 100}% Success Chance";
		for (int i = 0; i < currentRitual.ResourceTypes.Length; i++) 
		{
			string resourceName = "";
			switch (currentRitual.ResourceTypes[i]) 
			{
				case (-1):
					resourceName = "Crewmember";
					break;
				case (-2):
					resourceName = "Drachma";
					break;
				default:
					resourceName = Globals.GameVars.masterResourceList[currentRitual.ResourceTypes[i]].name;
					break;
			}
			performText += $"\n-{currentRitual.ResourceAmounts[i]} {resourceName}";
		}

		performButton.SetExplanationText(performText);
		rejectButton.SetExplanationText("If you refuse to do the ritual, the storm will surely get worse!");
	}

	public void CalculateRitualResults(int action) 
	{
		int result = -1;

		if (action >= 0) {
			//Ritual is being performed
			float check = Random.Range(0.0f, 1.0f);
			result = check < currentRitual.SuccessChance ? 0 : 1;
			Debug.Log($"{check} : {currentRitual.SuccessChance} so ritual {result}");
		}
		else {
			//Ritual was rejected
			result = 2;
		}

		mgInfo.DisplayText(titles[2], subtitles[2], result > -1 ? ritualResultsText[result] : "something went wrong", stormIcon, MiniGameInfoScreen.MiniGame.Start);

		//Send the result to the difficulty calculator for the storm
	}

	private bool CheckResources() {
		//Check if the player has the needed resources

		//Make sure you remember: -1 is a crewmember, -2 is money
		return true;
	}

	private int RandomIndex<T>(IList<T> array) 
	{
		return Random.Range(0, array.Count);
	}

	private bool CheckForSeer() 
	{
		bool hasSeer = false;

		for (int i = 0; i < Globals.GameVars.playerShipVariables.ship.crew; i++) 
		{
			hasSeer = hasSeer || (Globals.GameVars.playerShipVariables.ship.crewRoster[i].typeOfCrew == CrewType.Seer);
		}

		return hasSeer;
	}
}
