using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class RandomSlotPopulator : MonoBehaviour
{
	//Arrays of the drops zones (names respectively to their roles) are public so they may be edited in the future
	[Header("Drop Zones")]
	public Transform[] enemyZones = new Transform[2];
	public Transform[] crewZones = new Transform[2];
	public CardDropZone enemySlot;
	public CardDropZone crewSlot;
	//public GameObject[] enemySlotsEven;
	//public GameObject[] enemySlotsOdd;
	//public GameObject[] playableSlotsEven;
	//public GameObject[] playableSlotsOdd;
	[Header("Crew Slots")]
	public GameObject crewMemberSlot;
	public Transform crewSlotParent;
	public int padding = 50;

	[Header("Spawning")]
	public Pirate pirate;
	public Transform pirateParent;

	public CrewCard crew;
	public Transform crewParent;
	public Transform crewParentInOrigin;

	public Vector2Int pirateRange = new Vector2Int(1, 12);

	private PirateType typeToSpawn;

	private CardDropZone[,] spawnedCrewSlots;

	private Canvas canvas;
	private int crewNum;
	private GridLayoutGroup crewGrid;

	private int crewPerRow;
	private int slotsPerRow;
	private bool loaded = false;

	//GameObject[] pirateSlots, playerSlots;

	void OnEnable()
    {
		loaded = false;
		canvas = GetComponent<Canvas>();
		crewNum = Globals.GameVars.playerShipVariables.ship.crew;
		crewGrid = crewSlotParent.GetComponent<GridLayoutGroup>();
		crewPerRow = crewGrid.constraintCount;
		slotsPerRow = Mathf.CeilToInt(pirateRange.y / 2f);
	}

	private void OnDisable() {
		RandomizerForStorms.DestroyAllChildren(pirateParent);
		RandomizerForStorms.DestroyAllChildren(crewParent);
		RandomizerForStorms.DestroyAllChildren(crewParentInOrigin);
		RandomizerForStorms.DestroyAllChildren(crewSlotParent);
	}

	public void StartMinigame() 
	{
		PopulateScreen();
		ActivateCrewRow(0);
		loaded = true;
		//MiniGameManager.InitilizePirates();
	}

	public void SetPirateType(PirateType t) 
	{
		typeToSpawn = t;
		//Debug.Log($"Spawning only pirates of type {typeToSpawn.name}"); //<-- that type gets changed again later when the actual pirate cardds are loaded in (based on the zone(s) the player is currently in
	}

	public void PopulateScreen() 
	{
		/*---------------------------------------------------------------------
		PIRATE SPAWNING
		---------------------------------------------------------------------*/

		//random number of enemy priates created (1-12) 
		//different ranging numbers of pirates will be added later
		pirateRange.y = Mathf.Min(pirateRange.y, crewNum);
		int count = Random.Range(pirateRange.x, pirateRange.y+1);
		List<CrewMember> possiblePirates = Globals.GameVars.Pirates.Where(x => x.pirateType.Equals(typeToSpawn)).ToList();

		List<CardDropZone> crewSlots = new List<CardDropZone>();

		for (int i = 0; i < count; i++) {
			CardDropZone newEnemySlot = Instantiate(enemySlot);
			CardDropZone newCrewSlot = Instantiate(crewSlot);

			newEnemySlot.transform.SetParent(enemyZones[i < slotsPerRow ? 0 : 1]);
			newCrewSlot.transform.SetParent(crewZones[i < slotsPerRow ? 0 : 1]);

			newEnemySlot.transform.localScale = Vector3.one;
			newCrewSlot.transform.localScale = Vector3.one;

			newEnemySlot.dropIndex = i;
			newCrewSlot.dropIndex = i;

			crewSlots.Add(newCrewSlot);
		}

		GetComponent<MiniGameManager>().InitilizePirates(crewSlots);

		for (int i = 0; i < count; i++) {
			Pirate p = Instantiate(pirate);
			CrewCard pCard = p.GetComponent<CrewCard>();
			switch (typeToSpawn.difficulty) {
				case (1):
					pCard.power /= 5;
					break;
				case (2):
					pCard.power /= 3;
					break;
				case (4):
					pCard.power = (int)(pCard.power * 1.5f);
					break;
			}

			CrewMember randomPirate = possiblePirates.RandomElement();
			pCard.SetCrew(randomPirate);
			p.Bind();
			possiblePirates.Remove(randomPirate);
			Transform row = i < slotsPerRow ? enemyZones[0] : enemyZones[1];

			p.transform.SetParent(row.transform.GetChild(i % slotsPerRow));
			p.transform.localScale = Vector3.one;

			StartCoroutine(WaitAndChangeParent(p.transform, pirateParent));
			
			pCard.cardIndex = i;
		}


		//for (int x = 0; x < count; x++) 
		//{
		//	pirateSlots[x].SetActive(true);
		//	//pirateSlots[x].GetComponent<RectTransform>().localScale = canvas.transform.localScale;
		//	Pirate g = Instantiate(pirate);
		//	CrewCard gCard = g.GetComponent<CrewCard>();
		//	//Debug.Log("pirate local scale = " + pirate.transform.localScale);
		//	//Debug.Log("pirate lossy scale = " + pirate.transform.lossyScale);
		//	if (typeToSpawn.difficulty == 1) {
		//		gCard.power = (gCard.power / 5);
		//		gCard.powerText.text = gCard.power.ToString();
		//	}
		//	else if (typeToSpawn.difficulty == 2) {
		//		gCard.power = (gCard.power / 3);
		//		gCard.powerText.text = gCard.power.ToString();
		//	}
		//	else if (typeToSpawn.difficulty == 4) {
		//		gCard.power = (int)(gCard.power * 1.5f);
		//		gCard.powerText.text = gCard.power.ToString();

		//	}

		//	//CrewMember randomPirate = Globals.GameVars.Pirates.RandomElement();
		//	CrewMember randomPirate = possiblePirates.RandomElement();
		//	g.SetCrew(randomPirate);
		//	g.Bind();
		//	possiblePirates.Remove(randomPirate);
		//	g.GetComponent<RectTransform>().position = pirateSlots[x].GetComponent<RectTransform>().position;
		//	g.transform.SetParent(pirateParent);
		//	//no idea why this is necessary, but all cards need to be scaled to the local scale of the canvas of the minigame 
		//	//same thing below is done for the crew cards
		//	g.transform.localScale = Vector3.one;
		//	playerSlots[x].SetActive(true);
		//	//playerSlots[x].GetComponent<RectTransform>().localScale = canvas.transform.localScale;

		//	g.GetComponent<CrewCard>().cardIndex = pirateSlots[x].GetComponent<CardDropZone>().dropIndex;
		//}


		/*----------------------------------------------------------------------
		SCALING
		----------------------------------------------------------------------*/
		float width = crew.GetComponent<RectTransform>().rect.width;
		//crewSlotParent.GetComponent<RectTransform>().localScale = canvas.transform.localScale;
		//crewParentInOrigin.GetComponent<RectTransform>().localScale = canvas.transform.localScale;


		/*----------------------------------------------------------------------
		ORIGIN SLOT SPAWNING
		----------------------------------------------------------------------*/

		int totalCrewRows = Mathf.CeilToInt((crewNum * 1.0f) / crewPerRow);
		int spawnedSlots = 0;
		spawnedCrewSlots = new CardDropZone[totalCrewRows, crewPerRow];

		for (int r = 0; r < totalCrewRows; r++) 
		{
			for (int c = 0; c < crewPerRow; c++) 
			{
				//Spawns a new crew slot
				GameObject slot = Instantiate(crewMemberSlot);
				//scaling crew card slots
				slot.transform.SetParent(crewSlotParent);
				slot.transform.localScale = Vector3.one;
				
				RectTransform slotRect = slot.GetComponent<RectTransform>();
				CardDropZone cdz = slot.GetComponent<CardDropZone>();
				spawnedCrewSlots[r, c] = cdz;
			}
		}

		RectTransform crewParentRect = crewSlotParent.GetComponent<RectTransform>();
		crewParentRect.anchoredPosition = new Vector2(crewParentRect.anchoredPosition.x, CenterGrid(totalCrewRows, padding, width));

		/*----------------------------------------------------------------------
		CREW SPAWNING
		----------------------------------------------------------------------*/

		float startX = width / 2;
		float startY = crewSlotParent.GetComponent<RectTransform>().rect.height / 2;
		Debug.Log("Height: " + startY);

		float xPos = startX;
		float yPos = startY;

		for (int r = 0; r < totalCrewRows; r++) 
		{
			for (int c = 0; c < crewPerRow; c++) 
			{
				//Spawns a new crew member
				if (spawnedSlots < crewNum) 
				{
					CrewCard newCrew = Instantiate(crew);
					newCrew.SetRSP(this);
					newCrew.Bind();
					newCrew.transform.SetParent(crewParentInOrigin);
					newCrew.name = "Card " + r + ", " + c;
					CardDropZone cdz = spawnedCrewSlots[r, c];
					//scaling crew cards
					newCrew.transform.localScale = Vector3.one;
					newCrew.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
					newCrew.SetCrew(Globals.GameVars.playerShipVariables.ship.crewRoster[spawnedSlots]);
					cdz.SetOccupied(true);
					//Debug.Log("crewmember scale = " + crew.transform.localScale);
					//Debug.Log("crewmember lossy scale = " + crew.transform.lossyScale);
					spawnedSlots++;
					xPos += width;
				}
			}

			xPos = startX;
			yPos -= padding + width;
		}
	}

	public void ActivateCrewRow(int rowNum) 
	{
		int index = 0;
		for (int r = 0; r < spawnedCrewSlots.GetLength(0); r++) 
		{
			for (int c = 0; c < spawnedCrewSlots.GetLength(1); c++) 
			{
				spawnedCrewSlots[r, c].ToggleDropping(r == rowNum);
				spawnedCrewSlots[r, c].dropIndex = index;
				index++;
			}
		}
	}

	public bool Loaded 
	{
		get { return loaded; }
	}

	public PirateType CurrentPirates 
	{
		get { return typeToSpawn; }
	}

	public int CrewPerRow {
		get {
			return crewPerRow;
		}
	}

	private float CenterGrid(int rows, float padding, float size) {
		float total = (size * rows) + (padding * (rows - 1));

		float offset = total / -2.0f;
		offset += size / 2.0f;

		return offset;
	}

	private IEnumerator WaitAndChangeParent(Transform child, Transform newParent) {
		yield return null;
		child.SetParent(newParent);
	}

}
