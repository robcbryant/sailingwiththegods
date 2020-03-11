using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerForStorms : MonoBehaviour
{
	public GameObject MiniGameWater;

	private Vector3 randomSpawner;
	private Vector3 randomMGwaterSize;

	public GameObject miniGameRock;
	public GameObject stormCloud; 

void Start() {
		//creates a randomly sized rectangle of water 
		//better shape needed in the future
		//maybe even an array of random shapes? 
		randomMGwaterSize = new Vector3(Random.Range(20, 40), 1, Random.Range(20, 40));
		MiniGameWater.transform.localScale = randomMGwaterSize;

		int rndNum = Random.Range(50, 100); 

		for (int x = 0; x <= rndNum; x++) {
			PopulateNearbyArea();
		}

		
	}

	void Update() {
		
	}

	//transform.position = Random.insideUnitSphere* 5;

	private void PopulateNearbyArea() {
		Vector3 randomAreaForSpawningRocks = new Vector3(Random.Range(822, 1222), 783, Random.Range(-1150, -750));

		//cannot have rocks spawn too close to the ship start point 
		//ship start pnt = (1024, 782, -950)
		if ((randomAreaForSpawningRocks.x > 1040 || randomAreaForSpawningRocks.x < 1015) && (randomAreaForSpawningRocks.z > -935 || randomAreaForSpawningRocks.z < -969)) {
			Instantiate(miniGameRock, randomAreaForSpawningRocks, transform.rotation);
		}
	}

}
