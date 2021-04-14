using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the placement and chances of placement for the kottabos stand and table
public class RandomPlacement : MonoBehaviour
{
	public KottaboasManager gm;

	public Transform[] pos;
	private Vector3 startPosLekane, startPosKottaboaStand;

	//Limit the how far the table and kottaboas stand will be displaced
	private float xMaxRange = 4f;
	private float xMinRange = -4.0f;
	private float zMaxRange = 4f;
	private float zMinRange = -1.0f;

	private static float percent = 50.0f;
	// Start is called before the first frame update
	void Start() {
		startPosLekane = new Vector3(pos[0].position.x - 1.0f, pos[0].position.y, pos[0].position.z);
		startPosKottaboaStand = new Vector3(pos[1].position.x + 1.0f, pos[1].position.y, pos[1].position.z);

		PlacementChances();

		PlaceRandomPosition();
	}

	private void Update() {
		//As score goes up diffculty goes up
		if (gm.Scored) {
			percent -= 10;
		}
		gm.Scored = false;
		PlacementChances();

		//Thinking of adding boolean here
		if (Vector3.Distance(pos[0].transform.position, pos[1].transform.position) < 3.0f) {
			PlaceRandomPosition();
		}
		else {
			//Debug.Log(placmentCount);
			placmentCount = 0;
		}
		/* See method DefaultPositioning
		if(placmentCount > 10) {
			DefaultPositioning();
		}
		*/
	}


	/// <summary>
	/// Adds chance for more diffcult placements after a target hit
	/// </summary>
	public void PlacementChances() {
		int rangeInt = Random.Range(1, 100);

		if (percent >= 50 && percent < rangeInt) {
			zMaxRange = 0;
		}
		else if (percent >= 40 && percent < rangeInt) {
			zMaxRange = 1.0f;
		}
		else if (percent >= 30 && percent < rangeInt) {
			zMaxRange = 2.0f;
			zMinRange = -3.0f;
		}
		else if (percent >= 20 && percent < rangeInt) {
			zMaxRange = 3.0f;
			zMinRange = -1.5f;
		}
		else if (percent >= 10 && percent < rangeInt) {
			zMaxRange = 4.5f;
			zMinRange = 0.0f;
		}
	}

	//Tracks number of times PlaceRandomPosition called
	private int placmentCount = 0;
	/// <summary>
	/// Add after 15 tries of random places, set default position
	/// </summary>
	public void PlaceRandomPosition() {
		placmentCount++;
		for (int i = 0; i < pos.Length; i++) {
			pos[i].transform.position = new Vector3(Random.Range(xMinRange, xMaxRange), pos[i].position.y, Random.Range(zMinRange, zMaxRange));
			/*if (i < pos.Length - 1 && Vector3.Distance(pos[i].transform.position, pos[i + 1].transform.position) < 3.0f)
			if (Vector3.Distance(pos[0].transform.position, pos[1].transform.position) < 3.0f)
            {
				placmentCount++;
				Debug.Log(placmentCount);
				if (pos[i].name == "Lekane")
                {
                    PlaceRandomPosition();
                }
            }
			*/
		}
	}

	/// <summary>
	/// If PlaceRandomPosition's placementCount is to high the objects will just be placed in there default positions
	/// </summary>
	public void DefaultPositioning() {
		pos[0].position = startPosKottaboaStand;
		pos[1].position = startPosLekane;
		placmentCount = 0;
	}

	/// <summary>
	/// Used for setting resetting difficulty when going to different ports
	/// </summary>
	public void DifficultyReset() 
	{
		percent = 50.0f;
	}
}
