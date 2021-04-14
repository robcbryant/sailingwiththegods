using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KottaboasManager : MonoBehaviour
{
	GameVars GameVars => Globals.GameVars;

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

	// Start is called before the first frame update
	void Start() {
		playerStartPos = playerPos.transform.position;
		playerRb = playerPos.GetComponent<Rigidbody>();

		topTargetStartPos = childPos[5].position;
		topTargetStartRot = childPos[5].rotation;

		tr = playerPos.GetComponent<Throw>();
	}

	// Update is called once per frame
	void Update() {
		if (ContinueRound) {
			Debug.Log("C or B");
			//Debug.Log(score);
			if (Input.GetKey(KeyCode.C)) {
				tr.animate.SetBool("isFlinged", false);
				playerPos.SetActive(true);
				//Reset
				ResetRound();
				//Debug.Log("reset");
				ContinueRound = false;
			}
			else if (Input.GetKey(KeyCode.B) || score >= 7 || tries == 0) {
				//Thinking if you reached number of tries and have low amount of points you lose and get an insult				

				//if the scored atleast 8 then you get something
				if (score >= 10) {
					//Here's your reward end game
					//GameVars.AdjustPlayerClout(10, false);
				}
				else if (score >= 5) {
					//GameVars.AdjustPlayerClout(5, false);
				}

				//Get Reward and return to tavern
				if (tries == 0) {
					Debug.Log("Insult");
					ContinueRound = false;
					//SceneManager.LoadScene(8);
				}

				//Quit to tavern
				if (Input.GetKey(KeyCode.B)) 
				{
					//SceneManager.LoadScene(8);
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
		else {
			SubtractTries();
		}
	}
}
