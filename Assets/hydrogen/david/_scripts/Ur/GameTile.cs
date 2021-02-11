//David Herrod
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
	public Transform nextTile;
	public Transform prevTile;

	public Transform nextTileAL;
	public Transform prevTileAL;

	public bool rosette = false;
	public int timesLandedOn = 0;

	public GameObject isAvailable;

	private void Awake() {
		isAvailable = transform.GetChild(0).gameObject;
	}
	//public void ShowAvailablePositions(int drv) {
	//	List<GameTile> aTiles = new List<GameTile>();
	//	aTiles.Add(nextTile.GetComponent<GameTile>());
	//	for (int i = 0; i< drv; i++) {
	//		aTiles[i].available.SetActive(true);
	//	}
		
	//}

	public void ShowAvailable() {
		isAvailable.SetActive(!isAvailable.activeSelf);
	}
}
