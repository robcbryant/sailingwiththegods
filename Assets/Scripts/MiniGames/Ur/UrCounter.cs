using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrCounter : MonoBehaviour
{
	public bool onBoard = false;
	public bool goingHome = false;
	public bool enemyTile = false;
	public UrGameTile currentTile;
	public GameObject ikTarget;
	public float increment;
	public bool onTheMove = false;
	public UrArmIKHandler armIK;
	public UrGameController UR;
	public Vector3 initPosit;
	public bool pointScored = false;

	private void Start() {
		initPosit = transform.position;
	}

	private void Update() {
		if (onTheMove && GetComponent<MeshRenderer>().enabled == false) {
			if ((Vector3.Distance(ikTarget.transform.position, currentTile.transform.position) > 0.001f))
				ikTarget.transform.position = Vector3.MoveTowards(ikTarget.transform.position, currentTile.transform.position, (increment * Time.deltaTime));
			else {
				armIK.gameObject.GetComponent<Animator>().SetTrigger("MovementIsDone");
				transform.position = currentTile.transform.position;
				//GetComponent<MeshRenderer>().enabled = true; transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
				onBoard = true;
				onTheMove = false;
				if(!enemyTile) {
					StartCoroutine(EnemyTurnDelay());
				}
				//GetComponent<MeshRenderer>().enabled = true; transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}
	public void PlaceOnBoard(UrGameTile tile, bool flip, bool enemyT, bool ps) {
		//GetComponent<MeshRenderer>().enabled = false;
		//transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		pointScored = ps;
		goingHome = flip;
		enemyTile = enemyT;
		armIK.counterOnTheMove = this;
		currentTile = tile;
		onTheMove = true;
		armIK.gameObject.GetComponent<Animator>().SetTrigger("BeginMovement");
		
		ikTarget.transform.position = transform.position;

		
		
	}

	public void TileMT() {
		GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
		transform.GetChild(0).GetComponent<MeshRenderer>().enabled = !transform.GetChild(0).GetComponent<MeshRenderer>().enabled;
		if(goingHome) {
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}

	IEnumerator EnemyTurnDelay() {
		yield return new WaitForSeconds(2.0f);
		if(pointScored) {
			gameObject.SetActive(false);
		}
		UR.EnemyTurn();
	}
}
