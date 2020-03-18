using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerForStorms : MonoBehaviour
{
	public GameObject miniGameWater;

	private Vector3 randomSpawner;
	private Vector3 randomMGwaterSize;

	public GameObject miniGameRock;
	public GameObject stormCloud; 

void Start() {
		//creates a randomly sized rectangle of water 
		//better shape needed in the future
		//maybe even an array of random shapes? 
		randomMGwaterSize = new Vector3(Random.Range(20, 60), 1, Random.Range(20, 60));
		miniGameWater.transform.localScale = randomMGwaterSize;

		int rndNum = Random.Range(50, 100); 

		for (int x = 0; x <= rndNum; x++) {
			PopulateNearbyArea();
		}
	}

	void Update() {
		
	}

	private void PopulateNearbyArea() {
		//rocks need to spawn within the water's area
		//spawning on x axis should occur between ((water.scale.x * 5)+ 1024) and ((water.scale.x * -5)+ 1024)  
		//spawning on x axis should occur between ((water.scale.z * -5)- 948) and ((water.scale.z * 5)- 948)  

		float minX = ((miniGameWater.transform.localScale.x * -5) + 1024);
		float maxX = ((miniGameWater.transform.localScale.x * 5) + 1024);

		float minZ = ((miniGameWater.transform.localScale.z * -5) - 948);
		float maxZ = ((miniGameWater.transform.localScale.z * 5) - 948);

		//Vector3 randomAreaForSpawningRocks = new Vector3(Random.Range(822, 1222), 783, Random.Range(-1150, -750));
		Vector3 randomAreaForSpawningRocks = new Vector3(Random.Range(minX, maxX), 783, Random.Range(minZ, maxZ));

		//cannot have rocks spawn too close to the ship start point 
		//ship start pnt = (1024, 782, -950)
		if ((randomAreaForSpawningRocks.x > 1040 || randomAreaForSpawningRocks.x < 1015) && (randomAreaForSpawningRocks.z > -935 || randomAreaForSpawningRocks.z < -969)) {
			Instantiate(miniGameRock, randomAreaForSpawningRocks, transform.rotation);
		}
	}

}
