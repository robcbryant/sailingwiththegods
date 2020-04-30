using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class MiniGameManager : MonoBehaviour
{
	[Header("UI")]
	public MiniGameInfoScreen mgInfo;
	public Sprite pirateIcon;
	[TextArea(2, 15)]
	public string pirateInstructions;

	[Header("Buttons")]
	public ButtonExplanation[] runButtons;
	public ButtonExplanation[] negotiateButtons;
	public ButtonExplanation acceptNegotiationButton;
	public Button rejectNegotiationButton;
	public Button closeButton;
	public string acceptedNegotiationClose;
	public string rejectedNegotiationClose;
	public string failedRunClose;
	public string successRunClose;
	public string wonGameClose;
	public string lostGameClose;

	[Header("Gameplay")]
	public Vector2 runningBounds = new Vector2(0.1f, 0.9f);
	public GameObject piratesParent, crewParent;
	public List<GameObject> pirates, crew;
	public Transform[] pirateSpaces, crewSpaces;

	[Header("Clout")]
	public int wonFightClout;
	public int tookNegotiationClout;
	public int succeedRunClout;
	public int failedRunClout;

	private float runChance;
	private bool alreadyTriedRunning;
	private bool alreadyTriedNegotiating;
	private RandomSlotPopulator rsp;
	private int cloutChange;

	private void OnEnable() 
	{
		if (rsp == null) {
			rsp = GetComponent<RandomSlotPopulator>();
		}
		cloutChange = 0;

		alreadyTriedRunning = false;
		alreadyTriedNegotiating = false;

		foreach (ButtonExplanation button in negotiateButtons) 
		{
			button.SetExplanationText("The pirates may let you go\nYou will be known as a coward");
			button.GetComponentInChildren<Button>().interactable = true;
		}

		rsp.SetPirateType(Globals.GameVars.PirateTypes.RandomElement());
		runChance = CalculateRunChance();
		foreach (ButtonExplanation button in runButtons) 
		{
			button.SetExplanationText($"{runChance * 100}% success chance\nYou will be known as a coward");
			button.GetComponentInChildren<Button>().interactable = true;
		}

		string pirateTypeText = "";
		CrewMember pirateKnower = CrewFromPirateHometown(rsp.CurrentPirates);

		if (pirateKnower != null) 
		{
			string typeInfo = Globals.GameVars.pirateTypeIntroText[0];
			typeInfo = typeInfo.Replace("{0}", pirateKnower.name);
			typeInfo = typeInfo.Replace("{1}", rsp.CurrentPirates.name);

			pirateTypeText = typeInfo + " " + Globals.GameVars.pirateTypeIntroText[1 + rsp.CurrentPirates.ID];
		}
		else 
		{
			pirateTypeText = $"You ask around your crew, but it doesn't seem like any of them are from the same region as the pirates. You think they may be {rsp.CurrentPirates.name}. " +
				"Perhaps if you had more cities in your network you would have some advance warning of how they fight.";
		}

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[0], 
			Globals.GameVars.pirateSubtitles[0], 
			Globals.GameVars.pirateStartText[0] + "\n\n" + pirateTypeText + "\n\n" + pirateInstructions + "\n\n" + Globals.GameVars.pirateStartText[Random.Range(1, Globals.GameVars.pirateStartText.Count)], 
			pirateIcon, 
			MiniGameInfoScreen.MiniGame.Pirates);
	}

	public void GameOver() 
	{
		Globals.GameVars.isGameOver = true;
	}
	
	private string NetCloutText(int clout) 
	{
		string previousChange = "";

		if (cloutChange != 0) {
			previousChange = $"Combined with the earlier {cloutChange}, that is a net clout change of {clout + cloutChange}.";
		}

		return $"For sailing away with your lives, your clout has increased by {clout}. {previousChange}";
	}


	private CrewMember CrewFromPirateHometown(PirateType pirate) 
	{
		List<CrewMember> allPirates = Globals.GameVars.Pirates.Where(x => x.pirateType.Equals(pirate)).ToList();

		foreach (CrewMember c in Globals.GameVars.playerShipVariables.ship.crewRoster) 
		{
			IEnumerable<Settlement> crewNetwork = Globals.GameVars.Network.GetCrewMemberNetwork(c);
			foreach (CrewMember p in allPirates) 
			{
				if (crewNetwork.Contains(Globals.GameVars.GetSettlementFromID(p.originCity))) 
				{
					Debug.Log($"{c.name} knows the home city of Pirate {p.name}: {Globals.GameVars.GetSettlementFromID(c.originCity).name}");
					return c;
				}
			}
		}

		return null;
	}

	#region Negotiation
	int currentPlayerMoney = Globals.GameVars.playerShipVariables.ship.currency;
	int moneyPiratesWant = 0;

	public void OpenNegotiations() 
	{

		print(Globals.GameVars.playerShipVariables.ship.currency);

		if (!alreadyTriedNegotiating) 
		{
			//NEGOTIATION ALGORITHM GOES HERE

			//check for similar towns 
			//make easy/med/hard % demands

			//Figure out what they're offering

			//right now -- completely random---------------------------------------------------

			//Resource[] currentShipCargo = Globals.GameVars.playerShipVariables.ship.cargo;
			//int randNumberOfCargoItemsToTake = Random.Range(1, Globals.GameVars.playerShipVariables.ship.cargo.Length);

			//Resource[] cargoItemsPiratesWant = new Resource[randNumberOfCargoItemsToTake];
			//int[] cargoItemAmountPiratesWant = new int[randNumberOfCargoItemsToTake];

			//if (randNumberOfCargoItemsToTake > 0) {

			//	for (int x = 0; x < randNumberOfCargoItemsToTake; x++) {
			//		cargoItemsPiratesWant[x] = currentShipCargo[Random.Range(0, Globals.GameVars.playerShipVariables.ship.cargo.Length)];
			//		cargoItemAmountPiratesWant[x] = (int)Random.Range(0, Globals.GameVars.playerShipVariables.ship.cargo[Random.Range(0, Globals.GameVars.playerShipVariables.ship.cargo.Length)].amount_kg);
			//	}

			//}

			//TEMPORARY NEGOTIATION METHOD -- MONEY------------------------------------------
			//1/ 3/ 5 (e/ m/ h)

			//hard -- 75% money
			if (rsp.CurrentPirates.difficulty > 3) {
				moneyPiratesWant = (int)(currentPlayerMoney * .75);
			}
			//easy -- 25% money
			else if (rsp.CurrentPirates.difficulty < 3) {
				moneyPiratesWant = (int)(currentPlayerMoney * .25);
			}
			//med -- 50%
			else {
				moneyPiratesWant = (int)(currentPlayerMoney * .50);
			}

			//And put that into the button text
			acceptNegotiationButton.SetExplanationText("Cost: "+ moneyPiratesWant + " drachma");

			string deal = "This is what the pirates are offering: \n'We only want " + moneyPiratesWant + " drachma, and you may go freely.'";

			rejectNegotiationButton.onClick.RemoveAllListeners();
			rejectNegotiationButton.onClick.AddListener(mgInfo.CloseDialog);
			if (!rsp.Loaded) {
				rejectNegotiationButton.onClick.AddListener(rsp.StartMinigame);
			}

			mgInfo.gameObject.SetActive(true);
			mgInfo.DisplayText(
				Globals.GameVars.pirateTitles[1],
				Globals.GameVars.pirateSubtitles[1],
				Globals.GameVars.pirateNegotiateText[0] + "\n\n" + deal + "\n\nIf you take this deal, you will escape with your lives, but you will be thought a coward for avoiding a fight - your clout will go down!\n\n" +
					Globals.GameVars.pirateNegotiateText[Random.Range(1, Globals.GameVars.pirateNegotiateText.Count)],
				pirateIcon,
				MiniGameInfoScreen.MiniGame.Negotiation);

			foreach (ButtonExplanation button in negotiateButtons) 
			{
				button.SetExplanationText("You already rejected the pirates' deal!");
				button.GetComponentInChildren<Button>().interactable = false;
			}
		}
	}

	public void AcceptDeal() {
		//Subtract out resources

		Globals.GameVars.playerShipVariables.ship.currency -= moneyPiratesWant;
		print(Globals.GameVars.playerShipVariables.ship.currency);

		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = acceptedNegotiationClose;
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(UnloadMinigame);

		Globals.GameVars.AdjustPlayerClout(tookNegotiationClout + cloutChange, false);

		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[1],
			Globals.GameVars.pirateSubtitles[1],
			"You accepted the trade deal. You hand over what the pirates asked for and sail away. Though you have survived, your cowardly avoidance of a fight will stick with you. Your clout has gone down by" +
				$"{Mathf.Abs(tookNegotiationClout)}. Combined with the earlier {cloutChange}, that is a net clout change of {tookNegotiationClout + cloutChange}.",
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	public void RejectDeal() {
		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = rejectedNegotiationClose;
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(mgInfo.CloseDialog);

		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[1],
			Globals.GameVars.pirateSubtitles[1],
			$"You reject the pirate's deal and prepare to fight. Even if it was not your first choice, you will have to prove your heroism now.",
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	#endregion

	#region Running
	public void TryRunning() 
	{
		if (!alreadyTriedRunning) {
			//RUNNING CALCULATION GOES HERE
			bool check = runChance < Random.Range(0.0f, 1.0f);

			closeButton.onClick.RemoveAllListeners();

			if (check) 
			{
				RanAway();
			}
			else 
			{
				FailedRunning();
			}
		}
	}

	public void RanAway() 
	{
		Globals.GameVars.AdjustPlayerClout(succeedRunClout + cloutChange, false);
		string cloutText = $"Your cowardice has tarnished your reputation and your clout has gone down by {succeedRunClout}.";
		if (cloutChange != 0) {
			cloutText += $" Combined with the earlier {cloutChange}, that is a net clout change of {succeedRunClout + cloutChange}.";
		}

		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = successRunClose;
		closeButton.onClick.AddListener(UnloadMinigame);

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[2],
			Globals.GameVars.pirateSubtitles[2],
			Globals.GameVars.pirateRunSuccessText[0] + "\n\n" + cloutText + "\n\n" + Globals.GameVars.pirateRunSuccessText[Random.Range(1, Globals.GameVars.pirateRunSuccessText.Count)],
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	public void FailedRunning() 
	{
		string cloutText = $"Your failure to run has decreased your clout by {failedRunClout}.";

		cloutChange += failedRunClout;
		foreach (ButtonExplanation button in runButtons) {
			button.SetExplanationText($"There's no escape!");
			button.GetComponentInChildren<Button>().interactable = false;
		}

		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = failedRunClose;
		closeButton.onClick.AddListener(mgInfo.CloseDialog);
		if (!rsp.Loaded) {
			closeButton.onClick.AddListener(rsp.StartMinigame);
		}

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[2],
			Globals.GameVars.pirateSubtitles[2],
			Globals.GameVars.pirateRunFailText[0] + "\n\n" + cloutText + "\n\n" + Globals.GameVars.pirateRunFailText[Random.Range(1, Globals.GameVars.pirateRunFailText.Count)],
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	public float CalculateRunChance() 
	{
		int difficulty = rsp.CurrentPirates.difficulty;

		float baseShipSpeed = 7.408f;
		float crewMod = Globals.GameVars.playerShipVariables.shipSpeed_Actual / baseShipSpeed / 1.5f;
		float run = (1.0f / difficulty) * crewMod;
		run = Mathf.Max(runningBounds.x, run);
		run = Mathf.Min(runningBounds.y, run);

		Debug.Log($"{1.0f / difficulty} * {crewMod} = {run}");
		
		return run;
	}

	#endregion

	#region Fighting
	public void Fight() 
	{
		CrewCard crewMember, pirate;
		foreach(Transform p in piratesParent.transform) {
			pirates.Add(p.gameObject);
		}
		pirates = pirates.OrderBy(GameObject => GameObject.transform.position.x).ToList<GameObject>();
		foreach (Transform c in crewParent.transform) {
			crew.Add(c.gameObject);
		}
		crew = crew.OrderBy(GameObject => GameObject.transform.position.x).ToList<GameObject>();
		for (int index = 0; index <= crewParent.transform.childCount - 1; index++) {
			crewMember = crew[index].transform.GetComponent<CrewCard>();
			pirate = pirates[index].transform.GetComponent<CrewCard>();

			if (crewMember.gameObject.activeSelf && pirate.gameObject.activeSelf) {
				if (crewMember.power < pirate.power) {
					pirate.updatePower(pirate.power -= crewMember.power);
					crewMember.gameObject.SetActive(false);
				}
				else if (crewMember.power > pirate.power) {
					crewMember.updatePower(crewMember.power -= pirate.power);
					pirate.gameObject.SetActive(false);
				}
				else {
					crewMember.gameObject.SetActive(false);
					pirate.gameObject.SetActive(false);
				}
			}
		}
		int pirateCounter = 0;
		foreach(GameObject p in pirates) {
			if (p.activeInHierarchy) {
				pirateCounter++;
			}
		}
		int crewCounter = 0;
		foreach (GameObject c in crew) {
			if (c.activeInHierarchy) {
				crewCounter++;
			}
		}
		if (pirateCounter <= 0) {
			WinGame(crewCounter * 5);
		}
		if (crewCounter <= 0) {
			LoseGame();
		}
	}



	public void WinGame(int clout) 
	{
		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = wonGameClose;
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(UnloadMinigame);

		Globals.GameVars.AdjustPlayerClout(clout + cloutChange, false);

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[3],
			Globals.GameVars.pirateSubtitles[3],
			Globals.GameVars.pirateSuccessText[0] + "\n\n" + NetCloutText(clout) + "\n\n" + Globals.GameVars.pirateSuccessText[Random.Range(1, Globals.GameVars.pirateSuccessText.Count)],
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	public void LoseGame() 
	{
		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = lostGameClose;
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(GameOver);
		closeButton.onClick.AddListener(UnloadMinigame);

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[3],
			Globals.GameVars.pirateSubtitles[3],
			Globals.GameVars.pirateFailureText[0] + "\n\n" + Globals.GameVars.pirateFailureText[Random.Range(1, Globals.GameVars.pirateFailureText.Count)],
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	#endregion

	private void UnloadMinigame() {
		//UNLOAD MINIGAME CODE GOES HERE
		gameObject.SetActive(false);
	}
}
