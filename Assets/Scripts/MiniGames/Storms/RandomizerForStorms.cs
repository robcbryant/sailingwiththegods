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
	public Vector2 rockPerSqM = new Vector2(0.125f, 0.14f);
	public GameObject[] stormRocks;
	public Transform cloudHolder;
	public Vector2 cloudScaleBounds = new Vector2(3, 5);
	public Vector2 cloudPerSqM = new Vector2(0.185f, 0.215f);
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
	public float timeLimit = 5f;
	public GameObject hintArrow;
	public float[] difficultyModifiers = new float[3];
	public float[] cloutRanges;
	public float[] cloutModifiers;
	public Transform arrowTarget;

	private Vector3 randomMGwaterSize;
	private GameObject ship;
	private Color baseLighting;
	private List<Vector3> gaps = new List<Vector3>();
	private int cloutBracket;
	private float damagePerSecond;
	private ShipHealth h;
	private bool countingDown;

	private void Start() 
	{
		baseLighting = RenderSettings.ambientLight;
		h = GetComponent<ShipHealth>();

		damagePerSecond = h.MaxHealth / (timeLimit * 60);
	}

	private void OnEnable() 
	{
		if (ship != null) 
		{
			Destroy(ship);
		}
		RenderSettings.ambientLight = stormLighting;

		cloutBracket = GetBracket(cloutRanges, Globals.GameVars.playerShipVariables.ship.playerClout);

		InitializeView();
		countingDown = false;
	}

	private void OnDisable() 
	{
		RenderSettings.ambientLight = baseLighting;
		DestroyAllChildren(rockHolder);
		DestroyAllChildren(cloudHolder);
		DestroyAllChildren(edgeHolder);
		countingDown = false;
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

		//Modify those random numbers via clout and difficulty
		float mod = cloutModifiers[cloutBracket] * difficultyModifiers[(int)difficulty];

		waterSize *= mod;
		float sqWater = waterSize * waterSize;
		int rockNum = Mathf.FloorToInt(Random.Range(rockPerSqM.x, rockPerSqM.y) * sqWater * mod);
		int cloudNum = Mathf.FloorToInt(Random.Range(cloudPerSqM.x, cloudPerSqM.y) * sqWater * mod);

		//Populate the area with rocks and clouds
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

	private void PopulateWithObstacles(int num, GameObject[] obstacle, Transform parent, Vector2 scaleRange, float edging) 
	{
		for (int i = 0; i < num; i++) 
		{
			int spawnIndex = Random.Range(0, obstacle.Length);
			GameObject spawn = Instantiate(obstacle[spawnIndex]);
			RandomizeScaleAndRotation(spawn.transform, scaleRange);
			spawn.transform.SetParent(parent);
			
			float dist = 0;
			do {
				float xPos = Random.Range(-(range + edging), range + edging);
				float zPos = Random.Range(-(range + edging), range + edging);
				spawn.transform.localPosition = new Vector3(xPos, 0, zPos);
				dist = Vector2.Distance(new Vector2(shipStartPoint.position.x, shipStartPoint.position.z), new Vector2(spawn.transform.position.x, spawn.transform.position.z));
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

	private Vector2Int ChooseTwoWithGap(int choiceRange, int gapWidth) 
	{
		int i, j;
		
		do {
			i = Random.Range(0, choiceRange);
			j = Random.Range(0, choiceRange);
		} while (Mathf.Abs(i - j) < gapWidth);
		return new Vector2Int(i, j);
	}

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

	private void RandomizeScaleAndRotation(Transform t, Vector2 scaleRange) {
		float scaleFactor = Random.Range(scaleRange.x, scaleRange.y);
		t.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		t.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
	}
}
