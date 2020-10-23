using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomizerForStorms : MonoBehaviour
{
	public enum StormDifficulty { Easy, Medium, Hard, Error }
	
	public Light stormLight;
	[Header("Water")]
	public GameObject miniGameWater;
	public Vector2 waterSizeBounds = new Vector2(20, 60);

	[Header("Ship")]
	public GameObject[] shipModels;
	public Transform shipStartPoint;
	public GameObject cam;
	public Vector3 camOffset = new Vector3(0f, 64f, 0f);

	[Header("Rocks")]
	public GameObject[] stormRocks;
	public float shipClearanceRange;
	public float range = 5;
	public Transform rockHolder;
	public Vector2 rockScaleBounds = new Vector2(1, 4);
	public Vector2 rockPerSqM = new Vector2(0.125f, 0.14f);

	[Header("Edging")]
	public Transform edgeHolder;
	[Min(0.01f)]
	public float edgeRockSpacing;
	[Tooltip("Because the spacing is based on local position, the same number won't always work. This is your 'base' that you got the spacing on.")]
	public float spacingBase = 40;
	public int gapsPerSide = 2;
	public int standardGapWidth = 4;
	public int distBtwnGaps = 2;

	[Header("Clouds")]
	public GameObject[] stormClouds;
	public Transform cloudHolder;
	public Vector2 cloudScaleBounds = new Vector2(3, 5);
	public Vector2 cloudPerSqM = new Vector2(0.185f, 0.215f);

	[Header("Difficulty Adjustment")]
	public float timeLimit = 5f;
	public GameObject hintArrow;
	public float[] difficultyModifiers = new float[3];
	public float[] cloutRanges;
	public float[] cloutModifiers;
	public Transform arrowTarget;

	private Vector3 randomMGwaterSize;
	private GameObject ship;
	private List<Vector3> gaps = new List<Vector3>();
	private int cloutBracket;
	private float damagePerSecond;
	private ShipHealth h;
	private bool countingDown;

	private GameObject sunLight;

	private void Start() 
	{
		h = GetComponent<ShipHealth>();

		damagePerSecond = h.MaxHealth / (timeLimit * 60);

		sunLight = Globals.GameVars.skybox_sun;
	}

	private void OnEnable() 
	{
		if (ship != null) 
		{
			Destroy(ship);
		}

		if (sunLight == null) {
			sunLight = Globals.GameVars.skybox_sun.GetComponentInChildren<Light>().gameObject;
		}

		sunLight.SetActive(false);
		Globals.GameVars.FPVCamera.gameObject.SetActive(false);

		stormLight.gameObject.SetActive(true);

		cloutBracket = GetBracket(cloutRanges, Globals.GameVars.playerShipVariables.ship.playerClout);

		InitializeView();
		countingDown = false;
	}

	private void OnDisable() 
	{
		sunLight.SetActive(true);
		Globals.GameVars.FPVCamera.gameObject.SetActive(true);
		stormLight.gameObject.SetActive(false);

		DestroyAllChildren(rockHolder);
		DestroyAllChildren(cloudHolder);
		DestroyAllChildren(edgeHolder);
		countingDown = false;
	}

	/// <summary>
	/// Initializes everything for the minigame, like setting positions
	/// </summary>
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

	/// <summary>
	/// Based on the difficulty, changes water size and spawns obstacles
	/// </summary>
	/// <param name="difficulty"></param>
	public void SetDifficulty(StormDifficulty difficulty) 
	{
		//Get random numbers for stuff
		float waterSize = Random.Range(waterSizeBounds.x, waterSizeBounds.y);

		//Modify those random numbers via clout and difficulty
		float mod = cloutModifiers[cloutBracket] * difficultyModifiers[(int)difficulty];

		waterSize *= mod;
		//Calculates the size of the water in square units, then uses that along with the obstacle density to get the number to spawn
		float sqWater = waterSize * waterSize;
		int rockNum = Mathf.FloorToInt(Random.Range(rockPerSqM.x, rockPerSqM.y) * sqWater * mod);
		int cloudNum = Mathf.FloorToInt(Random.Range(cloudPerSqM.x, cloudPerSqM.y) * sqWater * mod);

		//Populate the area with rocks and clouds now that you have the amount of each
		miniGameWater.transform.localScale = new Vector3(waterSize, 1, waterSize);
		float currentSpacing = (spacingBase / waterSize) * edgeRockSpacing;

		PopulateWithObstacles(rockNum, stormRocks, rockHolder, rockScaleBounds, currentSpacing);
		PopulateWithObstacles(cloudNum, stormClouds, cloudHolder, cloudScaleBounds, currentSpacing);

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

	public void StartDamageTimer() 
	{
		countingDown = true;
		StartCoroutine(DamageOverTime());
	}

	public void StopDamageTimer() 
	{
		countingDown = false;
		StopCoroutine(DamageOverTime());
	}

	private IEnumerator DamageOverTime() 
	{
		while (countingDown) 
		{
			yield return new WaitForSeconds(1f);
			h.TakeDamage(damagePerSecond);
		}

	}

	/// <summary>
	/// Fills the area with obstacles
	/// </summary>
	/// <param name="num">How many obstacles to spawn</param>
	/// <param name="obstacle">The list of obstacles to choose from</param>
	/// <param name="parent">What to parent the obstacles to</param>
	/// <param name="scaleRange">The minimum and maximum scale of the obstacles</param>
	/// <param name="edging">How far from the edge to stop spawning</param>
	private void PopulateWithObstacles(int num, GameObject[] obstacle, Transform parent, Vector2 scaleRange, float edging) 
	{
		for (int i = 0; i < num; i++) 
		{
			//Choose a random obstacle to spawn and randomize its size and rotation
			int spawnIndex = Random.Range(0, obstacle.Length);
			GameObject spawn = Instantiate(obstacle[spawnIndex]);
			RandomizeScaleAndRotation(spawn.transform, scaleRange);
			spawn.transform.SetParent(parent);
			
			float dist = 0;
			do {
				//Choose a random position in the given range
				float xPos = Random.Range(-(range + edging), range + edging);
				float zPos = Random.Range(-(range + edging), range + edging);
				spawn.transform.localPosition = new Vector3(xPos, 0, zPos);
				//Check if the position is too close to the starting point
				dist = Vector2.Distance(new Vector2(shipStartPoint.position.x, shipStartPoint.position.z), new Vector2(spawn.transform.position.x, spawn.transform.position.z));
			} while (dist < shipClearanceRange);
			
		}
	}

	private void LineEdges(float spacing) 
	{
		FillOneSide(Vector3.forward, new Vector3(-range, 0, 0), spacing);
		FillOneSide(Vector3.forward, new Vector3(range, 0, 0), spacing);
		FillOneSide(Vector3.right, new Vector3(0, 0, -range), spacing);
		FillOneSide(Vector3.right, new Vector3(0, 0, range), spacing);
	}

	/// <summary>
	/// Fills one side of a square with obstacles
	/// </summary>
	/// <param name="side">Either Vector3.forward or Vector3.right, depending on what axis this side is on</param>
	/// <param name="bound">How far the side goes</param>
	/// <param name="spacing">How far apart each rock is</param>
	private void FillOneSide(Vector3 side, Vector3 bound, float spacing) {
		//I'm sure this method is a bit of a mess and could be made much more efficient, but it functions

		int rocksPerLine = Mathf.FloorToInt((range * 2) / spacing);
		
		bool counting = false;
		int gapCount = 0;
		int rockCount = 0;
		Vector2Int gapPos = ChooseTwoWithGap(rocksPerLine, standardGapWidth + distBtwnGaps);

		//Starts at the far end of the range, then goes through the center to the other far end
		for (float i = -range; i <= range; i += spacing) {

			//If it's time to make a gap
			if (rockCount == gapPos.x || rockCount == gapPos.y) {
				//Mark that you're counting gap positions, then add its center position to a list
				counting = true;
				gaps.Add((i * side) + bound);
			}

			if (counting) {
				//If you're counting the gap, check if you're done, and if so, stop counting
				gapCount++;
				if (gapCount > standardGapWidth) {
					counting = false;
					gapCount = 0;
				}
			}
			else {
				//Otherwise, if there's no gap right here, make a rock and position it
				GameObject rock = Instantiate(stormRocks[Random.Range(0, stormRocks.Length)]);
				RandomizeScaleAndRotation(rock.transform, rockScaleBounds);
				rock.transform.SetParent(edgeHolder);
				rock.transform.localPosition = (i * side) + bound;
			}
			rockCount++;
		}
	}

	/// <summary>
	/// Destroys all children of the given transform
	/// </summary>
	/// <param name="parent"></param>
	public static void DestroyAllChildren(Transform parent) 
	{
		Transform[] children = parent.GetComponentsInChildren<Transform>();
		for (int i = 0; i < children.Length; i++) 
		{
			if (children[i] != parent) {
				Destroy(children[i].gameObject);
			}
		}
	}

	/// <summary>
	/// Picks two positions separated by a gap
	/// </summary>
	/// <param name="choiceRange"></param>
	/// <param name="gapWidth"></param>
	/// <returns></returns>
	private Vector2Int ChooseTwoWithGap(int choiceRange, int gapWidth) 
	{
		int i, j;
		
		//Keep picking two random numbers until they're at least gapWidth apart
		//I'm 100% sure there's a more efficient way to do this that can be abstracted out to n numbers rather than just 2
		//But it took me a LONG time to even get this much, and it's functional, so here it is, inefficiency and all
		do {
			i = Random.Range(0, choiceRange);
			j = Random.Range(0, choiceRange);
		} while (Mathf.Abs(i - j) < gapWidth);
		return new Vector2Int(i, j);
	}

	/// <summary>
	/// Determines the index i of an index where a given value is larger than array[i] but smaller than array[i+1]
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="brackets">The array to search through</param>
	/// <param name="find">The value to find</param>
	/// <returns>The index of brackets that the value is larger than but smaller than the following index, or -1 if the value is not found</returns>
	public static int GetBracket<T>(T[] brackets, T find) where T : System.IComparable
	{
		for (int i = brackets.Length - 1; i >= 0; i--) {
			if (find.CompareTo(brackets[i]) >= 0) {
				string s = i + 1 < brackets.Length ? brackets[i + 1].ToString() : "maximum";
				Debug.Log($"{find} is larger than or equal to {brackets[i]} but smaller than {s}");
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Randomizes the scale and rotation of a transform within range
	/// </summary>
	/// <param name="t"></param>
	/// <param name="scaleRange"></param>
	private void RandomizeScaleAndRotation(Transform t, Vector2 scaleRange) {
		float scaleFactor = Random.Range(scaleRange.x, scaleRange.y);
		t.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		t.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
	}
}
