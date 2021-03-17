//David Herrod
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrGameController : MonoBehaviour
{
	public List<UrGameTile> boardPositions;
	public List<UrGameTile> eBoardPositions;
	public List<UrCounter> counters;
	public List<UrCounter> eCounters;
	public DiceRoller dice;
	public Text dvText;
	private int diceValue = 0;
	public int countersOffBoard = 7;
	public int countersOnBoard = 0;
	public int enemyCountersOnBoard;
	public Camera cam;
	public bool selectingObject = false;
	public bool isPlayerTurn;
	public bool selectingBoardPosition = false;
	public UrCounter selectedCounter;
	public GameObject rollDiceButton;
	public Animator aiAnim;
	private Animator playerArms;
	public int playerScore = 0;
	public int enemyScore = 0;
	public Text playerScoreText;
	public Text enemyScoreText;

	public void Awake() {
		playerArms = dice.playerAnimator;
	}

	public void Update() {
		if(Input.GetKeyDown("p")) {
			EnemyTurn();
		}
		if(!isPlaying(playerArms, "RollDiceLoop")) { 
			if (selectingObject) {
				if (Input.GetMouseButtonDown(0) && diceValue > 0) {
					foreach (UrCounter c in counters) {
						c.GetComponent<CapsuleCollider>().enabled = true;
					}
					Ray ray;
					RaycastHit hit;
					ray = cam.ScreenPointToRay(Input.mousePosition);

					if (Physics.Raycast(ray, out hit, 50)) {
						if (hit.collider.GetComponent<UrCounter>() != null && hit.collider.tag == "PlayerTile") {
							//if(hit.)
							CounterSelected(hit.collider.GetComponent<UrCounter>());
						}
					}
				}

			}
			//Does this need to be in update or can it be called less frequently?
			if (selectingBoardPosition) {
				Debug.Log("Selecting Board Position");
				if (Input.GetMouseButtonDown(0)) {
					//foreach (Counter c in counters) {
					//	c.GetComponent<CapsuleCollider>().enabled = false;
					//}
					Ray ray;
					RaycastHit hit;
					ray = cam.ScreenPointToRay(Input.mousePosition);

					if (Physics.Raycast(ray, out hit, 50)) {
						if (hit.collider.tag == "GameBoard") {
							if (!selectedCounter.onBoard) {
								countersOffBoard--;
								countersOnBoard++;
							}
							Debug.Log("Gameboard Hit");
							CounterSelected(selectedCounter);
							if (boardPositions.IndexOf(hit.collider.transform.parent.GetComponent<UrGameTile>()) == 19) { PointScored(true); selectedCounter.PlaceOnBoard(hit.collider.transform.parent.GetComponent<UrGameTile>(), true, false, true); /*selectedCounter.enabled = false;*/ }
							else {
								if (boardPositions.IndexOf(hit.collider.transform.parent.GetComponent<UrGameTile>()) >= 13) { selectedCounter.PlaceOnBoard(hit.collider.transform.parent.GetComponent<UrGameTile>(), true, false, false); if (IsEnemySpaceOccupied(hit.collider.transform.parent.GetComponent<UrGameTile>())){ IsEnemySpaceOccupiedCounter(hit.collider.transform.parent.GetComponent<UrGameTile>()).transform.position = IsEnemySpaceOccupiedCounter(hit.collider.transform.parent.GetComponent<UrGameTile>()).initPosit; IsEnemySpaceOccupiedCounter(hit.collider.transform.parent.GetComponent<UrGameTile>()).currentTile = null; enemyCountersOnBoard--; } }
								else { selectedCounter.PlaceOnBoard(hit.collider.transform.parent.GetComponent<UrGameTile>(), false, false, false); }
							}
							selectingBoardPosition = false;
							diceValue = 0;
							//EnemyTurn();

						}
					}
				}
				//if(Input.GetMouseButtonDown(1)) {
				//	selectingBoardPosition = false;
				//	selectingObject = true;
				//	CounterSelected(selectedCounter);
				}
			}
		
	}
	public void RollDice() {
		dice.StartDiceRoll();
	}

	public void CounterSelected(UrCounter c) {
		if (c.onBoard) {
			selectedCounter = c;
			int bIndex = boardPositions.IndexOf(c.currentTile);
			//for (int i = 0; i < diceValue; i++) {
			//need to figure this out - what if bIndex + diceValue is out of range?
			if (bIndex + diceValue < boardPositions.Count) {
				if (!IsSpaceOccupied(boardPositions[bIndex + diceValue])) 
				{
					boardPositions[bIndex + diceValue].ShowAvailable();
					selectingBoardPosition = !selectingBoardPosition;
				}
				else 
				{
					Debug.Log("This tile cannot move.");
				}
			}
			else 
			{
				Debug.Log("This tile cannot move.");
			}

			//bIndex++;
			//}

			//selectingObject = !selectingObject;
		}
		else {
			selectedCounter = c;
			if (diceValue == 1 && !IsSpaceOccupied(boardPositions[0])) {
				boardPositions[0].ShowAvailable();
				selectingBoardPosition = !selectingBoardPosition;
			}
			else if(diceValue == 5 && !IsSpaceOccupied(boardPositions[4]) && !IsSpaceOccupied(boardPositions[16])) { boardPositions[4].ShowAvailable(); selectingBoardPosition = !selectingBoardPosition; }
		}
	 
	}

	public bool IsSpaceOccupied(UrGameTile gt) {
		foreach(UrCounter c in counters) {
			if(boardPositions.IndexOf(c.currentTile) == boardPositions.IndexOf(gt)) { Debug.Log("space is occupied");  return true; }
		}
		Debug.Log("space is free");
		return false;
	}

	public UrCounter IsSpaceOccupiedCounter(UrGameTile gt) {
		foreach (UrCounter c in counters) {
			if (boardPositions.IndexOf(c.currentTile) == boardPositions.IndexOf(gt)) { Debug.Log("space is occupied"); return c; }
		}
		Debug.Log("space is free");
		return null;
	}

	public bool IsEnemySpaceOccupied(UrGameTile gt) {
		foreach (UrCounter c in eCounters) {
			if (eBoardPositions.IndexOf(c.currentTile) == eBoardPositions.IndexOf(gt)) { Debug.Log("space is occupied"); return true; }
		}
		Debug.Log("space is free");
		return false;
	}

	public UrCounter IsEnemySpaceOccupiedCounter(UrGameTile gt) {
		foreach (UrCounter c in eCounters) {
			if (eBoardPositions.IndexOf(c.currentTile) == eBoardPositions.IndexOf(gt)) { Debug.Log("space is occupied"); return c; }
		}
		Debug.Log("space is free");
		return null;
	}

	public bool CanPlayerMove(int val) {
		foreach(UrCounter c in counters) {
			if(c.currentTile != null && (boardPositions.IndexOf(c.currentTile)+ val) < 19) { return true; }
		}
		return false;
	}
	public bool CanEnemyMove(int val, UrCounter c) {

		if (c.currentTile != null && (eBoardPositions.IndexOf(c.currentTile) + val) < 19) { return true; }
		else {
			return false;
		}
	}

	public void SetDiceValue(int val) {
		diceValue = dice.DiceResult(val);
		rollDiceButton.SetActive(false);
		Debug.Log(diceValue);
		dvText.text = "" + diceValue;
		if(diceValue == 0) { EnemyTurn(); }
		if (diceValue == 4 && !CanPlayerMove(diceValue)) { EnemyTurn(); }
		if (countersOnBoard > 0) {
			selectingObject = true;
		}
		else {


			if (diceValue == 1 && countersOffBoard > 0 && countersOnBoard == 0) {
				counters[countersOffBoard - 1].PlaceOnBoard(boardPositions[0], false, false, false);
				countersOffBoard--;
				countersOnBoard++;
				aiAnim.SetTrigger("Angry");
				//EnemyTurn();
			}
			else if (diceValue == 5 && countersOffBoard > 0 && countersOnBoard == 0) {
				counters[countersOffBoard - 1].PlaceOnBoard(boardPositions[4], false, false, false);
				countersOffBoard--;
				countersOnBoard++;
				aiAnim.SetTrigger("Angry");
				//EnemyTurn();
			}
			else {
				EnemyTurn();
			}
		}
		
	}

	bool isPlaying(Animator anim, string stateName) {
		if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
			return true;
		else
			return false;
	}

	public void PointScored(bool player) {
		if (player) {
			playerScore++;
			playerScoreText.text = "" + playerScore;
		}
		else {
			enemyScore++;
			enemyScoreText.text = "" + enemyScore;
		}
		if(playerScore == 7) {
			Debug.Log("You win.");
		}
		if (enemyScore == 7) {
			Debug.Log("You lose.");
		}
	}


	public void EnemyTurn() {
		int[] i = { 0, 1, 4, 5 };
		int sel = Random.Range(0, 4);
		int ediceValue = i[sel];
		dvText.text = "" + ediceValue;
		if(ediceValue == 1 && enemyCountersOnBoard == 0) {
			eCounters[0].PlaceOnBoard(eBoardPositions[0], false, true, false); enemyCountersOnBoard++;
		}
		else if(ediceValue == 5 && enemyCountersOnBoard == 0) {
				eCounters[0].PlaceOnBoard(eBoardPositions[4], false, true, false); enemyCountersOnBoard++;
			}
		else if (ediceValue == 1 && enemyCountersOnBoard > 0 && !IsEnemySpaceOccupied(eBoardPositions[0])) {
			eCounters[enemyCountersOnBoard].PlaceOnBoard(eBoardPositions[0], false, true, false); enemyCountersOnBoard++;
		}
		else if (ediceValue == 5 && enemyCountersOnBoard > 0 && !IsEnemySpaceOccupied(eBoardPositions[4])) {
			eCounters[enemyCountersOnBoard].PlaceOnBoard(eBoardPositions[4], false, true, false); enemyCountersOnBoard++;
		}
		else if(enemyCountersOnBoard > 0) {
			if(ediceValue == 1) {
				foreach(UrCounter c in eCounters) {
					if (c.onBoard && CanEnemyMove(ediceValue, c)) {
						Debug.Log("i get here");
						int index = eBoardPositions.IndexOf(c.currentTile);
						if (!IsEnemySpaceOccupied(eBoardPositions[index + 1]) && ((index + 1) < 13)) {
							c.PlaceOnBoard(eBoardPositions[index + 1], false, true, false);
							if (IsSpaceOccupied(boardPositions[index + 1])) {
								Debug.Log("haha rekt"); IsSpaceOccupiedCounter(boardPositions[index + 1]).onBoard = false; IsSpaceOccupiedCounter(boardPositions[index + 1]).transform.position = IsSpaceOccupiedCounter(boardPositions[index + 1]).initPosit; IsSpaceOccupiedCounter(boardPositions[index + 1]).currentTile = null; break;
							}
						}
						else if (!IsEnemySpaceOccupied(eBoardPositions[index + 1]) && ((index + 1) >= 13)) {
							if(index + 1 == 19) { PointScored(false); }
							c.PlaceOnBoard(eBoardPositions[index + 1], true, true, false);
							if (IsSpaceOccupied(boardPositions[index + 1])) {
								Debug.Log("haha rekt"); IsSpaceOccupiedCounter(boardPositions[index + 1]).onBoard = false; IsSpaceOccupiedCounter(boardPositions[index + 1]).transform.position = IsSpaceOccupiedCounter(boardPositions[index + 1]).initPosit; IsSpaceOccupiedCounter(boardPositions[index + 1]).currentTile = null; break;
							}

						}
						
					}
					
				}
			}
			else if (ediceValue == 4) {
				foreach (UrCounter c in eCounters) {
					if (c.onBoard && CanEnemyMove(ediceValue, c)) {
						Debug.Log("i get here");
						int index = eBoardPositions.IndexOf(c.currentTile);
						if (!IsEnemySpaceOccupied(eBoardPositions[index + 4]) && ((index + 4) < 13)) {
							c.PlaceOnBoard(eBoardPositions[index + 4], false, true, false);
							if (IsSpaceOccupied(boardPositions[index + 4])) {
								Debug.Log("haha rekt"); IsSpaceOccupiedCounter(boardPositions[index + 4]).onBoard = false; IsSpaceOccupiedCounter(boardPositions[index + 4]).transform.position = IsSpaceOccupiedCounter(boardPositions[index + 4]).initPosit; IsSpaceOccupiedCounter(boardPositions[index + 4]).currentTile = null; break;
							}
						}
						else if (!IsEnemySpaceOccupied(eBoardPositions[index + 4]) && ((index + 4) >= 13)) {
							if (index + 4 == 19) { PointScored(false); }
							c.PlaceOnBoard(eBoardPositions[index + 4], true, true, false);
							if (IsSpaceOccupied(boardPositions[index + 4])) {
								Debug.Log("haha rekt"); IsSpaceOccupiedCounter(boardPositions[index + 4]).onBoard = false; IsSpaceOccupiedCounter(boardPositions[index + 4]).transform.position = IsSpaceOccupiedCounter(boardPositions[index + 4]).initPosit; IsSpaceOccupiedCounter(boardPositions[index + 4]).currentTile = null; break;
							}

						}

					}
				}
			}
			else if (ediceValue == 5) {
				foreach (UrCounter c in eCounters) {
					if (c.onBoard && CanEnemyMove(ediceValue, c)) {
						Debug.Log("i get here");
						int index = eBoardPositions.IndexOf(c.currentTile);
						if (!IsEnemySpaceOccupied(eBoardPositions[index + 5]) && ((index + 5) < 13)) {
							c.PlaceOnBoard(eBoardPositions[index + 5], false, true, false);
							if (IsSpaceOccupied(boardPositions[index + 5])) {
								Debug.Log("haha rekt"); IsSpaceOccupiedCounter(boardPositions[index + 5]).onBoard = false; IsSpaceOccupiedCounter(boardPositions[index + 5]).transform.position = IsSpaceOccupiedCounter(boardPositions[index + 5]).initPosit; IsSpaceOccupiedCounter(boardPositions[index + 5]).currentTile = null; break;
							}
						}
						else if (!IsEnemySpaceOccupied(eBoardPositions[index + 5]) && ((index + 5) >= 13)) {
							if (index + 5 == 19) { PointScored(false); }
							c.PlaceOnBoard(eBoardPositions[index + 5], true, true, false);
							if (IsSpaceOccupied(boardPositions[index + 5])) {
								Debug.Log("haha rekt"); IsSpaceOccupiedCounter(boardPositions[index + 5]).onBoard = false; IsSpaceOccupiedCounter(boardPositions[index + 5]).transform.position = IsSpaceOccupiedCounter(boardPositions[index + 5]).initPosit; IsSpaceOccupiedCounter(boardPositions[index + 5]).currentTile = null; break;
							}

						}

					}
				}
			}
		}
		rollDiceButton.SetActive(true);
	}
}
