using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerForStorms : MonoBehaviour
{
	public GameObject miniGameWater;

	private Vector3 randomMGwaterSize;

	public Transform rockHolder;
	public GameObject miniGameRock;
	public GameObject[] stormClouds;

void Start() {
		//creates a randomly sized rectangle of water 
		randomMGwaterSize = new Vector3(Random.Range(20, 60), 1, Random.Range(20, 60));
		miniGameWater.transform.localScale = randomMGwaterSize;

		//far fewer rocks will need to be included than clouds at this current time (03/21/2020)
		int rndNumForRocks = Random.Range(50, 100);
		int rndNumForClouds = Random.Range(400, 500);

		for (int x = 0; x <= rndNumForRocks; x++) {
			PopulateNearbyAreaWithRocks();
		}

		for (int x = 0; x <= rndNumForClouds; x++) {
			PopulateNearbyAreaWithClouds();
		}
	}

	void Update() {
		
	}

	private void PopulateNearbyAreaWithRocks() {
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
			GameObject rock = Instantiate(miniGameRock, randomAreaForSpawningRocks, transform.rotation);
			rock.transform.SetParent(rockHolder);
		}
	}

	//the below method works very similarly to the above
	//minor changes for spawning clouds have been implimented 
	private void PopulateNearbyAreaWithClouds() {
		//Clouds need to spawn within the water's area
		//spawning on x axis should occur between ((water.scale.x * 5)+ 1024) and ((water.scale.x * -5)+ 1024)  
		//spawning on x axis should occur between ((water.scale.z * -5)- 948) and ((water.scale.z * 5)- 948)  

		float minX = ((miniGameWater.transform.localScale.x * -5) + 1024);
		float maxX = ((miniGameWater.transform.localScale.x * 5) + 1024);

		float minZ = ((miniGameWater.transform.localScale.z * -5) - 948);
		float maxZ = ((miniGameWater.transform.localScale.z * 5) - 948);

		//clouds will spawn in a similar matter, as they should only be within the minigame area
		//the Y value will have an additional 50 units added to it, so that clouds will act more so like they should 
		//Vector3 randomAreaForSpawningClouds = new Vector3(Random.Range(822, 1222), 808, Random.Range(-1150, -750));
		Vector3 randomAreaForSpawningClouds = new Vector3(Random.Range(minX, maxX), 808, Random.Range(minZ, maxZ));

		//clouds will follow the Rock spawn requirements as to make starting the MG more user friendly
		//cannot have clouds spawn too close to the ship start point 
		//ship start pnt = (1024, 782, -950)
		if ((randomAreaForSpawningClouds.x > 1040 || randomAreaForSpawningClouds.x < 1015) && (randomAreaForSpawningClouds.z > -935 || randomAreaForSpawningClouds.z < -969)) {
			int randCloud = Random.Range(0, 4);
			Instantiate(stormClouds[randCloud], randomAreaForSpawningClouds, transform.rotation);
		}
	}

}
