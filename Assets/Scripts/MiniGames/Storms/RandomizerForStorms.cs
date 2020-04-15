using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomizerForStorms : MonoBehaviour
{
	public enum StormDifficulty { Easy, Medium, Hard, Error }

	[Header("Water")]
	public GameObject miniGameWater;
	public Vector2 waterSizeBounds = new Vector2(20, 60);

	[Header("Ship")]
	public Transform shipStartPoint;
	public Vector3 shipSizeMod = new Vector3(.5f, .5f, .5f);
	public GameObject cam;
	public Vector3 camOffset = new Vector3(0f, 64f, 0f);
	public GameObject tempShip;

	[Header("Obstacles")]
	public float shipClearanceRange;
	public float range = 5;
	public Transform rockHolder;
	public Vector2 rockScaleBounds = new Vector2(1, 4);
	public GameObject[] stormRocks;
	public Transform cloudHolder;
	public Vector2 cloudScaleBounds = new Vector2(3, 5);
	public GameObject[] stormClouds;

	[Header("Difficulty Adjustment")]
	public GameObject hintArrow;
	public float[] difficultyModifiers = new float[3];
	public int[] cloutRanges;
	public float[] cloutModifiers;

	private Vector3 randomMGwaterSize;
	private GameObject ship;

void Start() {
		////easy, medium, and hard difficulties will be set here by the SetDifficulty method when it is operational 
		////current numbers in place will be for medium, and are good for someone who has played the game before, 
		////or someone who has been playing the game for a few days

		////creates a randomly sized rectangle of water 
		//randomMGwaterSize = new Vector3(Random.Range(waterSizeBounds.x, waterSizeBounds.y), 1, Random.Range(waterSizeBounds.x, waterSizeBounds.y));
		//miniGameWater.transform.localScale = randomMGwaterSize;

		////creates a random spawning sequence for rocks and clouds
		//int rndNumForRocks = Random.Range(50, 100);
		//int rndNumForClouds = Random.Range(400, 500);

		//for (int x = 0; x <= rndNumForRocks; x++) {
		//	PopulateNearbyAreaWithRocks();
		//}

		//for (int x = 0; x <= rndNumForClouds; x++) {
		//	PopulateNearbyAreaWithClouds();
		//}

		////SetDifficulty();
	}

	private void OnEnable() 
	{
		if (ship != null) 
		{
			Destroy(ship);
		}

		DestroyAllChildren(rockHolder);
		DestroyAllChildren(cloudHolder);

		InitializeView();
	}

	private void InitializeView() 
	{
		ship = Instantiate(tempShip);
		ship.tag = "StormShip";
		ship.transform.SetParent(transform);
		ship.transform.localScale = shipSizeMod;
		cam.transform.SetParent(ship.transform);
		cam.transform.localPosition = camOffset;
		hintArrow.transform.SetParent(ship.transform);
		ship.transform.position = shipStartPoint.position;

		GetComponent<StormMGmovement>().playerBoat = ship;
	}

	public void SetDifficulty(StormDifficulty difficulty) 
	{
		int rockNum, cloudNum;

		rockNum = 100;
		cloudNum = 450;

		PopulateWithObstacles(rockNum, stormRocks, rockHolder, rockScaleBounds);
		PopulateWithObstacles(cloudNum, stormClouds, cloudHolder, cloudScaleBounds);

		//Get random numbers for the size of the water

		//Modify the size according to clout and difficulty

		//Get random numbers for rocks and clouds

		//Modify those random numbers via clout and difficulty

		//Populate the area with rocks and clouds

		//Turn on/off the hint arrow
	}

	private void PopulateWithObstacles(int num, GameObject[] obstacle, Transform parent, Vector2 scaleRange) 
	{
		for (int i = 0; i < num; i++) 
		{
			float xPos = Random.Range(-range, range);
			float zPos = Random.Range(-range, range);
			Vector3 spawnPoint = new Vector3(xPos, 0, zPos);
			while (Vector3.Distance(shipStartPoint.position, spawnPoint) < shipClearanceRange) 
			{
				xPos = Random.Range(-range, range);
				zPos = Random.Range(-range, range);
				spawnPoint = new Vector3(xPos, 0, zPos);
			}

			int spawnIndex = Random.Range(0, obstacle.Length);
			Debug.Log($"trying index {spawnIndex} out of total length {obstacle.Length}");
			GameObject spawn = Instantiate(obstacle[spawnIndex]);
			float scaleFactor = Random.Range(scaleRange.x, scaleRange.y);
			spawn.transform.SetParent(parent);
			spawn.transform.localPosition = spawnPoint;
			spawn.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			spawn.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		}
	}

	private void DestroyAllChildren(Transform parent) 
	{
		Transform[] children = parent.GetComponentsInChildren<Transform>();
		for (int i = 0; i < children.Length; i++) 
		{
			Destroy(children[i].gameObject);
		}
	}

	private void PopulateNearbyAreaWithRocks(int rockNum) {
		////rocks need to spawn within the water's area
		////spawning on x axis should occur between ((water.scale.x * 5)+ 1024) and ((water.scale.x * -5)+ 1024)  
		////spawning on x axis should occur between ((water.scale.z * -5)- 948) and ((water.scale.z * 5)- 948)  

		//float minX = ((miniGameWater.transform.localScale.x * -5) + 1024);
		//float maxX = ((miniGameWater.transform.localScale.x * 5) + 1024);

		//float minZ = ((miniGameWater.transform.localScale.z * -5) - 948);
		//float maxZ = ((miniGameWater.transform.localScale.z * 5) - 948);

		////Vector3 randomAreaForSpawningRocks = new Vector3(Random.Range(822, 1222), 783, Random.Range(-1150, -750));
		//Vector3 randomAreaForSpawningRocks = new Vector3(Random.Range(minX, maxX), 783, Random.Range(minZ, maxZ));

		////cannot have rocks spawn too close to the ship start point 
		////ship start pnt = (1024, 782, -950)
		//if ((randomAreaForSpawningRocks.x > 1040 || randomAreaForSpawningRocks.x < 1015) && (randomAreaForSpawningRocks.z > -935 || randomAreaForSpawningRocks.z < -969)) {
		//	GameObject rock = Instantiate(miniGameRock, randomAreaForSpawningRocks, transform.rotation);
		//	rock.transform.SetParent(rockHolder);
		//}
	}

	//the below method works very similarly to the above
	//minor changes for spawning clouds have been implimented 
	private void PopulateNearbyAreaWithClouds() 
	{
		////Clouds need to spawn within the water's area
		////spawning on x axis should occur between ((water.scale.x * 5)+ 1024) and ((water.scale.x * -5)+ 1024)  
		////spawning on x axis should occur between ((water.scale.z * -5)- 948) and ((water.scale.z * 5)- 948)  

		//float minX = ((miniGameWater.transform.localScale.x * -5) + 1024);
		//float maxX = ((miniGameWater.transform.localScale.x * 5) + 1024);

		//float minZ = ((miniGameWater.transform.localScale.z * -5) - 948);
		//float maxZ = ((miniGameWater.transform.localScale.z * 5) - 948);

		////clouds will spawn in a similar matter, as they should only be within the minigame area
		////the Y value will have an additional 50 units added to it, so that clouds will act more so like they should 
		////Vector3 randomAreaForSpawningClouds = new Vector3(Random.Range(822, 1222), 808, Random.Range(-1150, -750));
		//Vector3 randomAreaForSpawningClouds = new Vector3(Random.Range(minX, maxX), 808, Random.Range(minZ, maxZ));

		////clouds will follow the Rock spawn requirements as to make starting the MG more user friendly
		////cannot have clouds spawn too close to the ship start point 
		////ship start pnt = (1024, 782, -950)
		//if ((randomAreaForSpawningClouds.x > 1040 || randomAreaForSpawningClouds.x < 1015) && (randomAreaForSpawningClouds.z > -935 || randomAreaForSpawningClouds.z < -969)) {
		//	int randCloud = Random.Range(0, 4);
		//	float randomRotation = Random.Range(0, 360);

		//	GameObject cloud = Instantiate(stormClouds[randCloud], randomAreaForSpawningClouds, transform.rotation);
		//	cloud.transform.rotation = Quaternion.Euler(0, randomRotation, 0);

		//	Vector3 rndCloudScale = new Vector3(Random.Range(1, 3), 1, Random.Range(1, 3));
		//	cloud.transform.localScale = rndCloudScale;

		//	cloud.transform.SetParent(cloudHolder);
		//}
	}

	//private void SetDifficulty() {
	//	//Difficulty changes:

	//	//EASY
	//	//clout range between 1 and 1499 (first three clout titles)
	//	//randomMGwaterSize = new Vector3(Random.Range(20, 40), 1, Random.Range(20, 40));
	//	//int rndNumForRocks = Random.Range(20,50);
	//	//int rndNumForClouds = Random.Range(200,400);

	//	//MEDIUM
	//	//clout range between 1500 and 3999 (next five clout titles)
	//	//randomMGwaterSize = new Vector3(Random.Range(20, 60), 1, Random.Range(20, 60));
	//	//int rndNumForRocks = Random.Range(50, 100);
	//	//int rndNumForClouds = Random.Range(400, 500);

	//	//HARD
	//	//clout range between 4000 and infinity (last three clout titles)
	//	//randomMGwaterSize = new Vector3(Random.Range(60, 100), 1, Random.Range(60, 100));
	//	//int rndNumForRocks = Random.Range(100, 200);
	//	//int rndNumForClouds = Random.Range(500, 1000);

	//  //NOTE: the below call for clout information on the player might need to be changed (04/06/2020)
	//	float testPlayerClout = Globals.GameVars.playerShipVariables.ship.playerClout;
	//	print(testPlayerClout);

	//	Vector3 randomMGwaterSize;
	//	int rndNumForRocks, rndNumForClouds;

	//	print("Current difficulty value is: " + testPlayerClout);
		
	//	if (testPlayerClout < 1500) {
	//		print("Difficulty setting is: EASY");

	//		randomMGwaterSize = new Vector3(Random.Range(20, 40), 1, Random.Range(20, 40));
	//		rndNumForRocks = Random.Range(20,50);
	//		rndNumForClouds = Random.Range(200,400);
	//	}
	//	else if (testPlayerClout > 3999) {
	//		print("Difficulty setting is: HARD");

	//		randomMGwaterSize = new Vector3(Random.Range(60, 100), 1, Random.Range(60, 100));
	//		rndNumForRocks = Random.Range(100, 200);
	//		rndNumForClouds = Random.Range(500, 1000);
	//	}
	//	else {
	//		print("Difficulty setting is: MEDIUM");

	//		randomMGwaterSize = new Vector3(Random.Range(20, 60), 1, Random.Range(20, 60));
	//		rndNumForRocks = Random.Range(50, 100);
	//		rndNumForClouds = Random.Range(400, 500);
	//	}

	//	//creates a randomly sized rectangle of water 
	//	miniGameWater.transform.localScale = randomMGwaterSize;

	//	//creates a random spawning sequence for rocks and clouds
	//	for (int x = 0; x <= rndNumForRocks; x++) {
	//		PopulateNearbyAreaWithRocks();
	//	}

	//	for (int x = 0; x <= rndNumForClouds; x++) {
	//		PopulateNearbyAreaWithClouds();
	//	}
	//}
}
