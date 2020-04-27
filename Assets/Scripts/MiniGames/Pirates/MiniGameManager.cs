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
	public GameObject piratesParent, crewParent;
	public List<GameObject> pirates, crew;
	public Transform[] pirateSpaces, crewSpaces;

	private float runChance;
	private bool alreadyTriedRunning;
	private bool alreadyTriedNegotiating;
	private RandomSlotPopulator rsp;

	private void OnEnable() 
	{
		if (rsp == null) {
			rsp = GetComponent<RandomSlotPopulator>();
		}

		//CALCULATE RUN CHANCE HERE
		runChance = 0.5f;
		alreadyTriedRunning = false;
		alreadyTriedNegotiating = false;
		foreach (ButtonExplanation button in runButtons) 
		{
			button.SetExplanationText($"{runChance * 100}% success chance");
			button.GetComponentInChildren<Button>().interactable = true;
		}
		foreach (ButtonExplanation button in negotiateButtons) {
			button.SetExplanationText("The pirates may let you go if you give them what they want");
			button.GetComponentInChildren<Button>().interactable = true;
		}

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[0], 
			Globals.GameVars.pirateSubtitles[0], 
			Globals.GameVars.pirateStartText[0] + "\n\n" + pirateInstructions + "\n\n" + Globals.GameVars.pirateStartText[Random.Range(1, Globals.GameVars.pirateStartText.Count)], 
			pirateIcon, 
			MiniGameInfoScreen.MiniGame.Pirates);
	}

	private void Update() {
		//TESTING
		if (Input.GetKeyDown(KeyCode.F)) {
			WinGame();
		}
		else if (Input.GetKeyDown(KeyCode.R)) {
			LoseGame();
		}
	}

	public void OpenNegotiations() 
	{
		if (!alreadyTriedNegotiating) 
		{

			//NEGOTIATION ALGORITHM GOES HERE
			acceptNegotiationButton.SetExplanationText("Cost\nCost\nCost");

			string deal = "This is what the pirates are offering";

			rejectNegotiationButton.onClick.RemoveAllListeners();
			rejectNegotiationButton.onClick.AddListener(mgInfo.CloseDialog);
			if (!rsp.Loaded) {
				rejectNegotiationButton.onClick.AddListener(rsp.StartMinigame);
			}

			mgInfo.gameObject.SetActive(true);
			mgInfo.DisplayText(
				Globals.GameVars.pirateTitles[1],
				Globals.GameVars.pirateSubtitles[1],
				Globals.GameVars.pirateNegotiateText[0] + "\n\n" + deal + "\n\n" + Globals.GameVars.pirateNegotiateText[Random.Range(1, Globals.GameVars.pirateNegotiateText.Count)],
				pirateIcon,
				MiniGameInfoScreen.MiniGame.Negotiation);

				foreach (ButtonExplanation button in negotiateButtons) {
				button.SetExplanationText("You already rejected the pirates' deal!");
				button.GetComponentInChildren<Button>().interactable = false;
			}
		}

	}

	public void TryRunning() 
	{
		if (!alreadyTriedRunning) {
			//RUNNING CALCULATION GOES HERE
			bool check = runChance < Random.Range(0.0f, 1.0f);
			List<string> flavorText;
			string buttonText;
			closeButton.onClick.RemoveAllListeners();

			if (check) {
				flavorText = Globals.GameVars.pirateRunSuccessText;
				buttonText = successRunClose;
				closeButton.onClick.AddListener(WinGame);
			}
			else {
				flavorText = Globals.GameVars.pirateRunFailText;
				buttonText = failedRunClose;
				alreadyTriedRunning = true;
				foreach (ButtonExplanation button in runButtons) {
					button.SetExplanationText($"There's no escape!");
					button.GetComponentInChildren<Button>().interactable = false;
				}
				closeButton.onClick.AddListener(mgInfo.CloseDialog);
				if (!rsp.Loaded) {
					closeButton.onClick.AddListener(rsp.StartMinigame);
				}
			}

			closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buttonText;

			mgInfo.gameObject.SetActive(true);
			mgInfo.DisplayText(
				Globals.GameVars.pirateTitles[2],
				Globals.GameVars.pirateSubtitles[2],
				flavorText[0] + "\n\n" + flavorText[Random.Range(1, flavorText.Count)],
				pirateIcon,
				MiniGameInfoScreen.MiniGame.Finish);
		}
	}

	public void WinGame() {
		Debug.Log("Successfully escaped the pirates alive");

		//FIGURE OUT YOUR CLOUT CHANGES

		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = wonGameClose;
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(UnloadMinigame);

		mgInfo.gameObject.SetActive(true);
		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[3],
			Globals.GameVars.pirateSubtitles[3],
			Globals.GameVars.pirateSuccessText[0] + "\n\n" + NetCloutText() + "\n\n" + Globals.GameVars.pirateSuccessText[Random.Range(1, Globals.GameVars.pirateSuccessText.Count)],
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	public void LoseGame() {
		Debug.Log("Lost the pirate game");

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

	private void UnloadMinigame() {
		gameObject.SetActive(false);
	}

	public void GameOver() {
		Globals.GameVars.isGameOver = true;
	}

	public void AcceptDeal() {
		//Subtract out resources

		closeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = acceptedNegotiationClose;
		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(UnloadMinigame);

		mgInfo.DisplayText(
			Globals.GameVars.pirateTitles[1],
			Globals.GameVars.pirateSubtitles[1],
			"You accepted the trade deal. You hand over what the pirates asked for and sail away.\n\n" + NetCloutText(),
			pirateIcon,
			MiniGameInfoScreen.MiniGame.Finish);
	}

	private string NetCloutText() {
		int cloutIncrease = 1;
		int previousCloutChange = 2;
		string previousChange = "";

		if (previousCloutChange > 0) {
			previousChange = $"Combined with the earlier {previousCloutChange}, that is a net clout change of {cloutIncrease + previousCloutChange}.";
		}

		return $"For sailing away with your lives, your clout has increased by {cloutIncrease}. {previousChange}";
	}

	public void Fight() {
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
		
	}
	
}
