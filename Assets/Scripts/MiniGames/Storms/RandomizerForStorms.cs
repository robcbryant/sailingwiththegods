using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomizerForStorms : MonoBehaviour
{
	public enum StormDifficulty { Easy, Medium, Hard, Error }

	[ColorUsage(true, true)]
	public Color stormLighting;
	[Header("Water")]
	public GameObject miniGameWater;
	public Vector2 waterSizeBounds = new Vector2(20, 60);

	[Header("Ship")]
	public GameObject[] shipModels;
	public Transform shipStartPoint;
	public GameObject cam;
	public Vector3 camOffset = new Vector3(0f, 64f, 0f);

	[Header("Obstacles")]
	public float shipClearanceRange;
	public float range = 5;
	public Transform rockHolder;
	public Vector2 rockScaleBounds = new Vector2(1, 4);
	public Vector2Int rockNumbers;
	public GameObject[] stormRocks;
	public Transform cloudHolder;
	public Vector2 cloudScaleBounds = new Vector2(3, 5);
	public Vector2Int cloudNumbers;
	public GameObject[] stormClouds;
	public Transform edgeHolder;
	[Min(0.01f)]
	public float edgeRockSpacing;
	[Tooltip("Because the spacing is based on local position, the same number won't always work. This is your 'base' that you got the spacing on.")]
	public float spacingBase = 40;
	public int gapsPerSide = 2;
	public int standardGapWidth = 4;
	public int distBtwnGaps = 2;

	[Header("Difficulty Adjustment")]
	public GameObject hintArrow;
	public float[] difficultyModifiers = new float[3];
	public int[] cloutRanges;
	public float[] cloutModifiers;
	public Transform arrowTarget;

	private Vector3 randomMGwaterSize;
	private GameObject ship;
	private Color baseLighting;
	private List<Vector3> gaps = new List<Vector3>();
	private int cloutBracket;

	private void Start() 
	{
		baseLighting = RenderSettings.ambientLight;
	}

	private void OnEnable() 
	{
		if (ship != null) 
		{
			Destroy(ship);
		}

		DestroyAllChildren(rockHolder);
		DestroyAllChildren(cloudHolder);
		DestroyAllChildren(edgeHolder);

		RenderSettings.ambientLight = stormLighting;

		cloutBracket = GetCloutBracket();

		InitializeView();
	}

	private void OnDisable() 
	{
		RenderSettings.ambientLight = baseLighting;
	}

	private void InitializeView() 
	{
		ship = Instantiate(shipModels[Globals.GameVars.playerShipVariables.ship.upgradeLevel]);
		ship.tag = "StormShip";
		ship.transform.SetParent(transform);
		cam.transform.SetParent(ship.transform);
		cam.transform.position = ship.transform.position + camOffset;
		ship.transform.position = shipStartPoint.position;
		hintArrow.transform.SetParent(ship.transform);
		
		GetComponent<StormMGmovement>().playerBoat = ship;
	}

	public void SetDifficulty(StormDifficulty difficulty) 
	{
		//Get random numbers for stuff
		float waterSize = Random.Range(waterSizeBounds.x, waterSizeBounds.y);
		int rockNum = Random.Range(rockNumbers.x, rockNumbers.y);
		int cloudNum = Random.Range(cloudNumbers.x, cloudNumbers.y);

		//Modify those random numbers via clout and difficulty
		float mod = cloutModifiers[cloutBracket] * difficultyModifiers[(int)difficulty];

		waterSize *= mod;
		rockNum = Mathf.FloorToInt(rockNum * mod);
		cloudNum = Mathf.FloorToInt(cloudNum * mod);

		Debug.Log($"SqM: {waterSize * waterSize}, rocks {rockNum}, clouds {cloudNum}");

		//Populate the area with rocks and clouds
		miniGameWater.transform.localScale = new Vector3(waterSize, 1, waterSize);
		PopulateWithObstacles(rockNum, stormRocks, rockHolder, rockScaleBounds);
		PopulateWithObstacles(cloudNum, stormClouds, cloudHolder, cloudScaleBounds);

		float currentSpacing = (spacingBase / waterSize) * edgeRockSpacing;

		LineEdges(currentSpacing);

		//Turn on/off the hint arrow
		if (difficulty == StormDifficulty.Easy) {
			hintArrow.SetActive(true);
			arrowTarget.transform.position = gaps[Random.Range(0, gaps.Count)];
		}
		else {
			hintArrow.SetActive(false);
		}
	}

	private void PopulateWithObstacles(int num, GameObject[] obstacle, Transform parent, Vector2 scaleRange) 
	{
		for (int i = 0; i < num; i++) 
		{
			int spawnIndex = Random.Range(0, obstacle.Length);
			GameObject spawn = Instantiate(obstacle[spawnIndex]);
			RandomizeScaleAndRotation(spawn.transform, scaleRange);
			spawn.transform.SetParent(parent);
			
			float dist = 0;
			do {
				float xPos = Random.Range(-range, range);
				float zPos = Random.Range(-range, range);
				spawn.transform.localPosition = new Vector3(xPos, 0, zPos);
				dist = Vector2.Distance(new Vector2(shipStartPoint.position.x, shipStartPoint.position.y), new Vector2(spawn.transform.position.x, spawn.transform.position.z));
			} while (dist < shipClearanceRange);
			
		}
	}

	private void LineEdges(float spacing) 
	{
		FillOneSide(new Vector3(0, 0, 1), new Vector3(-range, 0, 0), spacing);
		FillOneSide(new Vector3(0, 0, 1), new Vector3(range, 0, 0), spacing);
		FillOneSide(new Vector3(1, 0, 0), new Vector3(0, 0, -range), spacing);
		FillOneSide(new Vector3(1, 0, 0), new Vector3(0, 0, range), spacing);
	}

	private void FillOneSide(Vector3 side, Vector3 bound, float spacing) {
		int rocksPerLine = Mathf.FloorToInt((range * 2) / spacing);
		
		bool counting = false;
		int gapCount = 0;
		int rockCount = 0;
		Vector2Int gapPos = ChooseTwoWithGap(rocksPerLine, standardGapWidth + distBtwnGaps);
		for (float i = -range; i <= range; i += spacing) {

			if (rockCount == gapPos.x || rockCount == gapPos.y) {
				counting = true;
				gaps.Add((i * side) + bound);
			}

			if (counting) {
				gapCount++;
				if (gapCount > standardGapWidth) {
					counting = false;
					gapCount = 0;
				}
			}
			else {
				GameObject rock = Instantiate(stormRocks[Random.Range(0, stormRocks.Length)]);
				RandomizeScaleAndRotation(rock.transform, rockScaleBounds);
				rock.transform.SetParent(edgeHolder);
				rock.transform.localPosition = (i * side) + bound;
			}
			rockCount++;
		}
	}

	private void DestroyAllChildren(Transform parent) 
	{
		Transform[] children = parent.GetComponentsInChildren<Transform>();
		for (int i = 0; i < children.Length; i++) 
		{
			if (children[i] != parent) {
				Destroy(children[i].gameObject);
			}
		}
	}

	private Vector2Int ChooseTwoWithGap(int choiceRange, int gapWidth) 
	{
		int i, j;
		
		do {
			i = Random.Range(0, choiceRange);
			j = Random.Range(0, choiceRange);
		} while (Mathf.Abs(i - j) < gapWidth);
		return new Vector2Int(i, j);
	}

	private int GetCloutBracket() 
	{
		for (int i = cloutRanges.Length - 1; i >= 0; i--) {
			if (Globals.GameVars.playerShipVariables.ship.playerClout > cloutRanges[i]) {
				string s = i + 1 < cloutRanges.Length ? cloutRanges[i+1].ToString() : "maximum";
				Debug.Log($"Player clout {Globals.GameVars.playerShipVariables.ship.playerClout} is larger than {cloutRanges[i]} but smaller than {s}");
				return i;
			}
		}

		return -1;
	}

	private void RandomizeScaleAndRotation(Transform t, Vector2 scaleRange) {
		float scaleFactor = Random.Range(scaleRange.x, scaleRange.y);
		t.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		t.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
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
