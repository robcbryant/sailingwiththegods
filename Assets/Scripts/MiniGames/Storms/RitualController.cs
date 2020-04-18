using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RitualController : MonoBehaviour
{

	public enum RitualResult
	{
		Success, Failure, Refusal
	}

	[Header("General")]
	public float noResourcesMod = 0.5f;
	public MiniGameInfoScreen mgInfo;

	public Sprite stormIcon;

	public ButtonExplanation performButton;
	public ButtonExplanation rejectButton;
	public Button finishButton;
	public string winFinishText = "You escaped!";
	public string loseFinishText = "Game over!";

	private Ritual currentRitual;
	private CrewMember currentCrew;

	private void OnEnable() 
	{
		currentRitual = null;
		currentCrew = null;
		DisplayStartingText();
	}

	public void DisplayStartingText() 
	{
		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[0], 
			Globals.GameVars.stormSubtitles[0], 
			Globals.GameVars.stormStartText[0] + "\n\n" + Globals.GameVars.stormStartText[Random.Range(1, Globals.GameVars.stormStartText.Count)], 
			stormIcon, 
			MiniGameInfoScreen.MiniGame.StormStart);
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
		currentCrew = Globals.GameVars.playerShipVariables.ship.crewRoster[RandomIndex(Globals.GameVars.playerShipVariables.ship.crewRoster)];

		DisplayRitualText();
	}

	public void DisplayRitualText() 
	{
		string ritualText = currentRitual.RitualText;
		string introText = currentRitual.HasSeer ? Globals.GameVars.stormSeerText[0] : Globals.GameVars.stormNoSeerText[0];
		string closeText = currentRitual.HasSeer ? Globals.GameVars.stormSeerText[Random.Range(1, Globals.GameVars.stormSeerText.Count)] : 
			Globals.GameVars.stormNoSeerText[Random.Range(1, Globals.GameVars.stormNoSeerText.Count)];
		string finalRitualText = introText + "\n\n" + ritualText.Replace("{0}", currentCrew.name) + "\n\n" + closeText;

		mgInfo.DisplayText(Globals.GameVars.stormTitles[1], Globals.GameVars.stormSubtitles[1], finalRitualText, stormIcon, MiniGameInfoScreen.MiniGame.Storm);

		bool hasResources = CheckResources();
		string performText = "";
		if (!hasResources) 
		{
			mgInfo.AddToText("\n\nUnfortunately, you are missing some needed resources and will have a harder time with the ritual!");
			performText += $"{currentRitual.SuccessChance * noResourcesMod * 100}% Success Chance";
		}
		else 
		{
			performText += $"{currentRitual.SuccessChance * 100}% Success Chance";
		}

		
		for (int i = 0; i < currentRitual.ResourceTypes.Length; i++) 
		{
			switch (currentRitual.ResourceTypes[i]) 
			{
				case (-1):
					performText += $"\n{currentCrew.name} will die as a sacrifice";
					break;
				case (-2):
					performText += $"\n-{currentRitual.ResourceAmounts[i]} Drachma";
					break;
				default:
					performText += $"\n-{currentRitual.ResourceAmounts[i]} {Globals.GameVars.masterResourceList[currentRitual.ResourceTypes[i]].name}";
					break;
			}
		}

		if (!hasResources) 
		{
			performText += "\nYou are missing resources, so you will use what you have and hope for the best";
		}

		performButton.SetExplanationText(performText);
		rejectButton.SetExplanationText("If you refuse to do the ritual, the storm will surely get worse!");
	}

	public void CalculateRitualResults(int action) 
	{
		RandomizerForStorms.StormDifficulty result = RandomizerForStorms.StormDifficulty.Error;
		string extraText = "";

		if (action >= 0) {
			//Ritual is being performed
			float mod = CheckResources() ? 1 : noResourcesMod;
			float check = Random.Range(0.0f, 1.0f);
			result = check < (currentRitual.SuccessChance * mod) ? RandomizerForStorms.StormDifficulty.Easy : RandomizerForStorms.StormDifficulty.Medium;
			SubtractCosts();
		}
		else {
			//Ritual was rejected
			result = RandomizerForStorms.StormDifficulty.Hard;
		}

		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[2], 
			Globals.GameVars.stormSubtitles[2], 
			result != RandomizerForStorms.StormDifficulty.Error ? extraText + Globals.GameVars.stormRitualResultsText[(int)result] : "something went wrong", 
			stormIcon, 
			MiniGameInfoScreen.MiniGame.Start);

		//Send the result to the difficulty calculator for the storm
		GetComponent<RandomizerForStorms>().SetDifficulty(result);
	}

	private bool CheckResources() 
	{
		//Make sure you remember: -1 is a crewmember, -2 is money
		bool hasResources = true;

		for (int i = 0; i < currentRitual.ResourceTypes.Length; i++) 
		{
			if (currentRitual.ResourceTypes[i] == -2) {
				hasResources = hasResources && (Globals.GameVars.playerShipVariables.ship.currency > currentRitual.ResourceAmounts[i]);
			}
			else if (currentRitual.ResourceTypes[i] == -1) {
				//skip - you know they have a crewmember
			}
			else {
				hasResources = hasResources && (Globals.GameVars.playerShipVariables.ship.cargo[currentRitual.ResourceTypes[i]].amount_kg > currentRitual.ResourceAmounts[i]);
			}
		}
		return hasResources;
	}

	private void SubtractCosts() 
	{
		for (int i = 0; i < currentRitual.ResourceTypes.Length; i++) {
			switch (currentRitual.ResourceTypes[i]) 
			{
				case (-2):
					Globals.GameVars.playerShipVariables.ship.currency = Mathf.Max(0, Globals.GameVars.playerShipVariables.ship.currency - currentRitual.ResourceAmounts[i]);
					break;
				case (-1):
					Globals.GameVars.playerShipVariables.ship.crewRoster.Remove(currentCrew);
					break;
				default:
					int j = currentRitual.ResourceTypes[i];
					Globals.GameVars.playerShipVariables.ship.cargo[j].amount_kg = Mathf.Max(0f, Globals.GameVars.playerShipVariables.ship.cargo[j].amount_kg - currentRitual.ResourceAmounts[i]);
					break;
			}
		}
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

	public void WinGame()
	{
		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[3], 
			Globals.GameVars.stormSubtitles[3], 
			Globals.GameVars.stormSuccessText[0] + "\n\n" + Globals.GameVars.stormSuccessText[Random.Range(1, Globals.GameVars.stormSuccessText.Count)], 
			stormIcon, 
			MiniGameInfoScreen.MiniGame.Finish);
		finishButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = winFinishText;
	}

	public void LoseGame() 
	{
		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[3], 
			Globals.GameVars.stormSubtitles[3], 
			Globals.GameVars.stormFailureText[0] + "\n\n" + Globals.GameVars.stormFailureText[Random.Range(1, Globals.GameVars.stormFailureText.Count)], 
			stormIcon, 
			MiniGameInfoScreen.MiniGame.Finish);
		finishButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = loseFinishText;
		finishButton.onClick.AddListener(EndGame);
	}

	public void EndGame() 
	{
		Globals.GameVars.isGameOver = true;
	}
}
