using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetteiaColliderMover : MonoBehaviour
{
	public Vector2Int position;
	public GameObject highlight;
	public PetteiaGameController pController;

	//public bool destroy;
	//public PetteiaGameController p;
	public bool occupied;

	private PetteiaMovePiece currentPiece;

    void Start()
    {
		//p = GameObject.Find("board").GetComponent<PetteiaGameController>();
		//destroy = false;
		highlight.SetActive(false);
    }

	void OnTriggerEnter(Collider other) 
	{
		if (other.CompareTag("PetteiaB") || other.CompareTag("PetteiaW")) 
		{
			occupied = true;
			currentPiece = other.GetComponent<PetteiaMovePiece>();
		}
	}

	private void OnTriggerExit(Collider other) 
	{
		if (other.CompareTag("PetteiaB") || other.CompareTag("PetteiaW")) 
		{
			occupied = false;
			currentPiece = null;
		}
	}

	public void DestroyPiece() {
		if (currentPiece != null) {
			if (pController.playerPieces.Contains(currentPiece)) {
				Debug.Log("Player piece being destroyed, removing it from the list...");
				pController.playerPieces.Remove(currentPiece);
			}
			else {
				Debug.Log("Not a player piece");
			}
			Destroy(currentPiece.gameObject);
			currentPiece = null;
			occupied = false;
		}
		else {
			Debug.Log("CurrentPiece null");
		}
	}

	public void HighlightSpace(bool toggle) 
	{
		highlight.SetActive(toggle);
	}
}

