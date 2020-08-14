using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

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
	public List<CardDropZone> crewSlots;
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

		//check true zones, out of those, have a pirate type spawn from one of the "true" areas
		//no pirates should appear if there aren't any actve zones
		if (Globals.GameVars.playerShipVariables.zonesList.Count > 0) {
			//below line is unneeded
			//int randomPirateTypeFromActiveZones = UnityEngine.Random.Range(1, Globals.GameVars.playerShipVariables.zonesList.Count);

			string randomPirateTypeName = Globals.GameVars.playerShipVariables.zonesList.RandomElement();

			PirateType theType = Globals.GameVars.PirateTypes.FirstOrDefault(t => t.name == randomPirateTypeName);
			//Debug.Log("theType = " + theType);
			rsp.SetPirateType(theType);
			//Debug.Log("Pirate type in game is: " + rsp.GetType());
		}
		else {
			//temporary code 
			//rsp.SetPirateType(Globals.GameVars.PirateTypes.RandomElement());
		}

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

			//commented out the "1 + " to test for text matching up to type of Pirate in PMG
			//works as of 08/14/2020
			pirateTypeText = typeInfo + " " + Globals.GameVars.pirateTypeIntroText[/*1 + */rsp.CurrentPirates.ID];
			Debug.Log("current pirate ID: " + rsp.CurrentPirates.ID);
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
			Globals.GameVars.pirateStartText[0] + "\n\n" + pirateTypeText + "\n\n" + pirateInstructions + "\n\n" + Globals.GameVars.pirateStartText[UnityEngine.Random.Range(1, Globals.GameVars.pirateStartText.Count)], 
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
					//Debug.Log($"{c.name} knows the home city of Pirate {p.name}: {Globals.GameVars.GetSettlementFromID(c.originCity).name}");
					return c;
				}
			}
		}

		return null;
	}

	public void InitilizePirates(List<CardDropZone> playableSlots) {

		//foreach (Transform p in piratesParent.transform) {
		//	pirates.Add(p.gameObject);
		//}
		//Debug.Log("Pirate cout: " + pirates.Count);
		//pirates = pirates.OrderBy(GameObject => GameObject.transform.GetComponent<CrewCard>().cardIndex).ToList<GameObject>();

		//foreach (Transform c in crewParent.transform) {
		//	//crew.Add(c.gameObject);
		//	//Debug.Log(c.transform.GetComponent<CrewCard>().cardIndex + "\t" + c.transform.GetComponent<CrewCard>().nameText.name + "\t" + c);
		//}

		//crew = playableSlots.ToList();

		crewSlots = playableSlots;
		

		//crewSlots = crewSlots.OrderBy(GameObject => GameObject.transform.GetComponent<CardDropZone>().dropIndex).ToList<GameObject>();
	}

	#region Negotiation
	private int moneyDemand = 0;
	private Resource[] playerInvo => Globals.GameVars.playerShipVariables.ship.cargo;
	int[] demandedAmounts;

	public void OpenNegotiations() 
	{
		demandedAmounts = new int[Globals.GameVars.playerShipVariables.ship.cargo.Length];

		if (!alreadyTriedNegotiating) 
		{
			//NEGOTIATION ALGORITHM GOES HERE
			//right now: completely random and uses random of % weight of an item for taking ---------------------------------------------------

			int currentPlayerMoney = Globals.GameVars.playerShipVariables.ship.currency;

			//Only consider stuff you have any actual quantity of
			List<Resource> availableInvo = new List<Resource>();
			foreach (Resource r in playerInvo) {
				if (r.amount_kg > 0) {
					availableInvo.Add(r);
				}
			}

			int typesOfCargo = UnityEngine.Random.Range(1, playerInvo.Length);
			typesOfCargo = Mathf.Min(availableInvo.Count, typesOfCargo);

			Resource[] demandedItems = new Resource[typesOfCargo];

			Vector2 amountMod = new Vector2(1, 1);
			if (rsp.CurrentPirates.difficulty > 3) {
				amountMod = new Vector2(.5f, .75f);
				Debug.Log("Hard pirates");
			}
			else if (rsp.CurrentPirates.difficulty < 3) {
				amountMod = new Vector2(.1f, .25f);
				Debug.Log("Easy pirates");
			}
			else {
				amountMod = new Vector2(.25f, .5f);
				Debug.Log("Medium pirates");
			}

			moneyDemand = UnityEngine.Random.Range((int)(currentPlayerMoney * amountMod.x), (int)(currentPlayerMoney * amountMod.y));
			string demandsText = "";

			for (int i = 0; i < typesOfCargo; i++) {
				Resource randomResource = availableInvo[UnityEngine.Random.Range(0, availableInvo.Count)];
				int randomResourceIndex = Globals.GameVars.masterResourceList.FindIndex(x => x.name == randomResource.name);

				float cargoMod = UnityEngine.Random.Range(amountMod.x, amountMod.y);
				int amountToTake = (int)(playerInvo[randomResourceIndex].amount_kg * cargoMod);

				demandedAmounts[randomResourceIndex] = amountToTake;
				demandedItems[i] = new Resource(playerInvo[randomResourceIndex].name, amountToTake);

				demandsText += $"\n{amountToTake}kg {randomResource.name} (You have {playerInvo[randomResourceIndex].amount_kg}kg)";

				availableInvo.Remove(randomResource);
			}
			

			//And put that into the button text
			acceptNegotiationButton.SetExplanationText($"{moneyDemand} Drachma (You have {currentPlayerMoney}){demandsText}");

			string deal = "";
			if (typesOfCargo > 0) {
				deal = $"The pirates make their offer: \n'We only want {moneyDemand} Drachma, and a percentage of {typesOfCargo} items from your ship's cargo. What we want is:\n{demandsText}" +
					$"\n\nYou may go freely after accepting our deal.'";
			}
			else {
				deal = $"The pirates make their offer: \n'We only want {moneyDemand} Drachma, as you are not carrying anything that we value at this time. \n\nYou may go freely after accepting our deal.'";
			}

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
					Globals.GameVars.pirateNegotiateText[UnityEngine.Random.Range(1, Globals.GameVars.pirateNegotiateText.Count)],
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

		Globals.GameVars.playerShipVariables.ship.currency -= moneyDemand;

		for (int i = 0; i < demandedAmounts.Length; i++) {
			Globals.GameVars.playerShipVariables.ship.cargo[i].amount_kg -= demandedAmounts[i];
		}

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
			bool check = runChance < UnityEngine.Random.Range(0.0f, 1.0f);

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
			Globals.GameVars.pirateRunSuccessText[0] + "\n\n" + cloutText + "\n\n" + Globals.GameVars.pirateRunSuccessText[UnityEngine.Random.Range(1, Globals.GameVars.pirateRunSuccessText.Count)],
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);

		//Globals.MiniGames.Exit();
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
			Globals.GameVars.pirateRunFailText[0] + "\n\n" + cloutText + "\n\n" + Globals.GameVars.pirateRunFailText[UnityEngine.Random.Range(1, Globals.GameVars.pirateRunFailText.Count)],
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

		//Debug.Log($"{1.0f / difficulty} * {crewMod} = {run}");
		
		return run;
	}

	#endregion

	#region Fighting
	

	public void Fight() 
	{
		Debug.Log("FIGHT BUTTON");
		CrewCard crewMember, pirate;

		//adding the crewmembers in the play area to the list of playable crew
		var minIndex = crewSlots.Min(c => c.dropIndex);
		var maxIndex = crewSlots.Max(c => c.dropIndex);

			for (var index = minIndex; index <= maxIndex; index++) {
			//int zeroBasedIndex = index - minIndex;

			//crewMember = crew[index].transform.GetComponentInChildren<CrewCard>();
			crewMember = crewParent.transform.GetComponentsInChildren<CrewCard>().FirstOrDefault(crewCard => crewCard.cardIndex == index);
			pirate = piratesParent.transform.GetComponentsInChildren<CrewCard>().FirstOrDefault(crewCard => crewCard.cardIndex == index);
			//pirates[index].transform.GetComponent<CrewCard>();

			//if there is an active game object for the crew list and for the pirates list that correspond, then they will fight
			if (crewMember != null && pirate != null && crewMember.gameObject.activeSelf && pirate.gameObject.activeSelf) {
				//if the pirate's power is greater than the crewmem's, the crewmem will "die"
				//pirate's power goes down by the amount of power the crewmem had
				//the crewmem MUST be removed from the crew list to keep the count updated
				if (crewMember.power < pirate.power) {
					pirate.UpdatePower(pirate.power -= crewMember.power); 
					crewMember.gameObject.SetActive(false);
					//crew.Remove(crewMember.gameObject);
					//crew[zeroBasedIndex] = null;
					Debug.Log("CREW MEMBER " + crewMember.nameText.text + " DIED with index = " + index + " and POWER = " + crewMember.power + "\n\n");
				}
				//if the crewmem's power is greater than the pirate's, the pirate will "die" 
				//crewmem's power goes down by the amount of power the pirate had
				//the pirate MUST be removed from the pirates list to keep the count updated
				else if (crewMember.power > pirate.power) {
					crewMember.UpdatePower(crewMember.power -= pirate.power);
					pirate.gameObject.SetActive(false);
					//pirates.Remove(pirate.gameObject);
					//pirates[zeroBasedIndex] = null;
					Debug.Log("PIRATE " + pirate.nameText.text + " DIED with index = " + index + " and POWER = " + crewMember.power + "\n\n");
				}
				//if the crewmem and the pirate have powers of equal value, they cancel out
				//the crewmem MUST be removed from the crew list to keep the count updated
				//the pirate MUST be removed from the pirates list to keep the count updated
				else {
					//this has not happened yet as of 07/02/2020
					crewMember.gameObject.SetActive(false);
					pirate.gameObject.SetActive(false);

					//crew[zeroBasedIndex] = null;
					//pirates[zeroBasedIndex] = null;

					Debug.Log("CREW MEMBER AND PIRATE DIED\n\n");
				}
			}
			else {
				Debug.Log("One of the above gameobject was not active or null.");
			}
			Debug.Log("\n\n");
			//no need for an else statement because of the updates within the above if/else if/else statements 
		}

		int pirateCounter = piratesParent.transform.GetComponentsInChildren<CrewCard>().Count();

		int crewCounter = crewParent.transform.GetComponentsInChildren<CrewCard>().Count();
		
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
			Globals.GameVars.pirateSuccessText[0] + "\n\n" + NetCloutText(clout) + "\n\n" + Globals.GameVars.pirateSuccessText[UnityEngine.Random.Range(1, Globals.GameVars.pirateSuccessText.Count)],
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
			Globals.GameVars.pirateFailureText[0] + "\n\n" + Globals.GameVars.pirateFailureText[UnityEngine.Random.Range(1, Globals.GameVars.pirateFailureText.Count)],
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	#endregion

	private void UnloadMinigame() {
		//UNLOAD MINIGAME CODE GOES HERE
		gameObject.SetActive(false);
		Globals.MiniGames.Exit();
	}
}
