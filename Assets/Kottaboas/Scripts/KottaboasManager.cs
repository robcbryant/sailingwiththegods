using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KottaboasManager : MonoBehaviour
{
	GameVars GameVars => Globals.GameVars;

	public MiniGameInfoScreen mgscreen;

	public GameObject playerPos;
	private Rigidbody playerRb;
	private Vector3 playerStartPos;

	//Used to reset the targets on kottaboas stand
	public GameObject randomPlacement;
	public Transform[] childPos;

	//Used to reset the top target on kottaboas stand because of rigidbody attachment
	private Vector3 topTargetStartPos;
	private Quaternion topTargetStartRot;
	private Throw tr;

	private static int score = 0;
	private static int tries = 5;

	public bool ContinueRound { get; set; } = false;
	public bool Scored { get; set; } = false;
	public bool IsHit { get; set; } = false;

	/// <summary>
	/// Reset Kotaboas varibles after a game is played
	/// </summary>
	private void KottabosReset() {
		score = 0;
		tries = 5;

		ContinueRound = false;
		Scored = false;
		IsHit = false;

		Time.timeScale = 1;
	}

	// Start is called before the first frame update
	void Start() {
		
		mgscreen.DisplayText("Kottabos", "Wine throwing game", "Try and hit targets with the doplet of wine.", null, MiniGameInfoScreen.MiniGame.TavernaStart);

		playerStartPos = playerPos.transform.position;
		playerRb = playerPos.GetComponent<Rigidbody>();

		topTargetStartPos = childPos[5].position;
		topTargetStartRot = childPos[5].rotation;

		tr = playerPos.GetComponent<Throw>();
	}

	// Update is called once per frame
	void Update() {

		KottabosPauseAndUnPause();

		if (ContinueRound) {
			Debug.Log("C or B");
			Debug.Log(score);
			//Debug.Log(tries);
			if (Input.GetKeyDown(KeyCode.C)) {
				tr.animate.SetBool("isFlinged", false);
				playerPos.SetActive(true);
				//Reset
				ResetRound();
				//Debug.Log("reset");
				ContinueRound = false;
			}
			else if (Input.GetKeyDown(KeyCode.B) || tries == 0) {
				//Start = 15;
				//tries = 0;
				mgscreen.gameObject.SetActive(true);

				if (score >= 15) {
					//Here's your reward end game
					//KottabosReset();
					//GameVars.AdjustPlayerClout(15 * score, false);
					mgscreen.DisplayText("Perfect", "Perfection absolute – desired but dangerous!", "You have reached it, but now beware\n Lest Envy drive the god of War\n To take aim at you as you have at these cups!", null, MiniGameInfoScreen.MiniGame.TavernaEnd);
				}
				else if (score >= 10) {
					//KottabosReset();
					//GameVars.AdjustPlayerClout(Random.Range(10, 14) * score, false);
					mgscreen.DisplayText("Great", "Zeus himself could not have thrown better!", "Your hand was neither too stiff , nor too crooked; a master of the javelin, a god of the sling, a hero of missiles must you be on  the battlefield!", null, MiniGameInfoScreen.MiniGame.TavernaEnd);
				}
				else if (score >= 5) {
					//KottabosReset();
					//GameVars.AdjustPlayerClout(Random.Range(5, 9) * score, false);
					mgscreen.DisplayText("Good", "A winner in this game is a winner in love!", "A Sophokles says, The golden-colored drop of Aphrodite descends on all the houses! (Athenaeus Deipnosophistae 668)", null, MiniGameInfoScreen.MiniGame.TavernaEnd);
				}
				else 
				{
					//KottabosReset();
					mgscreen.DisplayText("You Lose", "You have lost!", "Your clout is like a tiny mouse who must hide from the cat, the silvery fish who flee from great whales, or warriors who run away on skinny legs from ravaging birds of prey.", null, MiniGameInfoScreen.MiniGame.TavernaEnd);
				}
			}
		}
	}

	public void SCORE_PER_HIT() {
		score += 1;
	}
	public void SCORE_PER_HIT(int num) {
		score += num;
	}

	/// <summary>
	/// Currently ends game when you hit c after 5 misses
	/// </summary>
	public void SubtractTries() {
		tries -= 1;
	}

	private void ResetBallPosition() {
		playerPos.transform.position = playerStartPos;
		playerPos.transform.rotation = Quaternion.Euler(Vector3.zero);

		playerRb.useGravity = false;
		playerRb.velocity = Vector3.zero;
		playerRb.angularVelocity = Vector3.zero;

		for (int i = 0; i < playerPos.GetComponent<Transform>().childCount; i++) {
			playerPos.transform.GetChild(i).gameObject.SetActive(true);
		}
		tr.Launch = false;
	}

	private void ResetTargetPosition() {
		randomPlacement.GetComponent<RandomPlacement>().PlaceRandomPosition();
		//Stop velocity angular velocity
		childPos[5].localPosition = topTargetStartPos;
		childPos[5].rotation = topTargetStartRot;
		childPos[5].GetComponent<Rigidbody>().velocity = Vector3.zero;
		childPos[5].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		IsHit = false;
	}

	private void ResetRound() {
		//ResetBallPosition
		ResetBallPosition();
		//ResetTargetPosition
		if (IsHit) {
			ResetTargetPosition();
		}
		SubtractTries();
	}

	public void LeaveKottaboas() {
		KottabosReset();
		TavernaController.BackToTavernaMenu();
	}

	public void KottabosPauseMenu() {
		mgscreen.gameObject.SetActive(true);
		Time.timeScale = 0;
		mgscreen.DisplayText("Kottabos", "Taverna Game", "Kottaboas is paused, here's where the controls will go", null, MiniGameInfoScreen.MiniGame.TavernaPause);
	}

	public void KottabosUnPauseMenu() {
		mgscreen.gameObject.SetActive(false);
		Time.timeScale = 1;
		mgscreen.CloseDialog();
	}

	private void KottabosPauseAndUnPause() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Time.timeScale == 1) {
				KottabosPauseMenu();
			}
			else {
				KottabosUnPauseMenu();
			}
		}
	}
}
