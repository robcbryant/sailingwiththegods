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
	public float timeLimit = 5f;
	[Range(0f, 1f)]
	public float noResourcesMod = 0.5f;
	public MiniGameInfoScreen mgInfo;
	[TextArea(2, 15)]
	public string instructionsText;
	public Sprite stormIcon;

	[Header("Clout")]
	public int refusalLoss = 15;
	public Vector2Int survivalGain = new Vector2Int(5, 25);

	[Header("End-Game Health")]
	public float[] damageLevelPercents;
	[TextArea(2, 10)]
	public string[] damageLevelText;

	[Header("Buttons")]
	public ButtonExplanation performButton;
	public ButtonExplanation rejectButton;
	public Button startButton;
	public Button finishButton;
	public string winFinishText = "You escaped!";
	public string loseFinishText = "Game over!";

	private Ritual currentRitual;
	private CrewMember currentCrew;
	private int cloutChange;
	private RandomizerForStorms rfs;

	private void Start() 
	{
		rfs = GetComponent<RandomizerForStorms>();
	}


	private void OnEnable() 
	{
		currentRitual = null;
		currentCrew = null;
		GetComponent<StormMGmovement>().ToggleMovement(false);
		DisplayStartingText();
		cloutChange = 0;
	}

	public void DisplayStartingText() 
	{
		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[0], 
			Globals.GameVars.stormSubtitles[0], 
			Globals.GameVars.stormStartText[0] + "\n\n" + instructionsText + "\n\n" + Globals.GameVars.stormStartText[Random.Range(1, Globals.GameVars.stormStartText.Count)], 
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
		string cloutText = "";

		if (action >= 0) {
			//Ritual is being performed
			float mod = CheckResources() ? 1 : noResourcesMod;
			float check = Random.Range(0.0f, 1.0f);
			result = check < (currentRitual.SuccessChance * mod) ? RandomizerForStorms.StormDifficulty.Easy : RandomizerForStorms.StormDifficulty.Medium;
			if (result == RandomizerForStorms.StormDifficulty.Easy) {
				cloutText = $"\n\nYour successful ritual has raised your clout by {currentRitual.CloutGain}.";
				cloutChange = currentRitual.CloutGain;
			}
			else {
				cloutText = $"\n\nYour failed ritual has lowered your clout by {currentRitual.CloutLoss}.";
				cloutChange = -currentRitual.CloutLoss;
			}
			SubtractCosts();
		}
		else {
			//Ritual was rejected
			result = RandomizerForStorms.StormDifficulty.Hard;
			cloutText = $"\n\nYou decision to reject the gods and refuse to perform a ritual has made some of your crew nervous, and your clout has decreased by {refusalLoss}";
			cloutChange = -refusalLoss;
		}

		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[2], 
			Globals.GameVars.stormSubtitles[2], 
			result != RandomizerForStorms.StormDifficulty.Error ? extraText + Globals.GameVars.stormRitualResultsText[(int)result] + cloutText : "something went wrong", 
			stormIcon, 
			MiniGameInfoScreen.MiniGame.Start);

		startButton.onClick.RemoveAllListeners();
		startButton.onClick.AddListener(mgInfo.CloseDialog);
		startButton.onClick.AddListener(rfs.StartDamageTimer);
		startButton.onClick.AddListener(() => GetComponent<StormMGmovement>().ToggleMovement(true));

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
		rfs.StopDamageTimer();
		ShipHealth h = GetComponent<ShipHealth>();
		float percentDamage = h.Health / h.MaxHealth;
		int damageBracket = RandomizerForStorms.GetBracket(damageLevelPercents, percentDamage);
		int cloutGained = Mathf.CeilToInt((survivalGain.y - survivalGain.x) * percentDamage + survivalGain.x);
		string cloutText = damageLevelText[damageBracket] + "\n\n" + $"For making your way out of the storm with your ship intact, your clout has risen {Mathf.RoundToInt(cloutGained)}." + 
			$" Combined with the {cloutChange} from the ritual, your clout has changed a total of {Mathf.RoundToInt(cloutGained + cloutChange)}.";
		Globals.GameVars.AdjustPlayerClout(cloutGained + cloutChange);

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[3], 
			Globals.GameVars.stormSubtitles[3], 
			Globals.GameVars.stormSuccessText[0] + "\n\n" + cloutText + "\n\n" + Globals.GameVars.stormSuccessText[Random.Range(1, Globals.GameVars.stormSuccessText.Count)], 
			stormIcon, 
			MiniGameInfoScreen.MiniGame.Finish);
		finishButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = winFinishText;
		finishButton.onClick.RemoveAllListeners();
		finishButton.onClick.AddListener(UnloadMinigame);
		GetComponent<StormMGmovement>().ToggleMovement(false);
	}

	public void LoseGame() 
	{
		rfs.StopDamageTimer();
		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.stormTitles[3], 
			Globals.GameVars.stormSubtitles[3], 
			Globals.GameVars.stormFailureText[0] + "\n\n" + Globals.GameVars.stormFailureText[Random.Range(1, Globals.GameVars.stormFailureText.Count)], 
			stormIcon, 
			MiniGameInfoScreen.MiniGame.Finish);
		finishButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = loseFinishText;
		finishButton.onClick.RemoveAllListeners();
		finishButton.onClick.AddListener(UnloadMinigame);
		finishButton.onClick.AddListener(EndGame);
		GetComponent<StormMGmovement>().ToggleMovement(false);
	}

	public void EndGame() 
	{
		Globals.GameVars.isGameOver = true;
	}

	public void UnloadMinigame() 
	{
		//UNLOAD MINIGAME CODE GOES HERE
		mgInfo.CloseDialog();
		gameObject.SetActive(false);
	}
}
