using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetteiaColliderMover : MonoBehaviour
{
	public Vector2Int position;

	public bool destroy;
	//public PetteiaGameController p;
	public bool occupied;

	private GameObject currentPiece;

    void Start()
    {
		//p = GameObject.Find("board").GetComponent<PetteiaGameController>();
		destroy = false;
    }

	void OnTriggerEnter(Collider other) 
	{
		if (other.CompareTag("PetteiaB") || other.CompareTag("PetteiaW")) 
		{
			occupied = true;
			currentPiece = other.gameObject;
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
			Destroy(currentPiece);
			currentPiece = null;
			occupied = false;
		}
	}
}

