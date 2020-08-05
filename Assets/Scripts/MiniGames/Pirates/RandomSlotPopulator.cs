using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class RandomSlotPopulator : MonoBehaviour
{
	//Arrays of the drops zones (names respectively to their roles) are public so they may be edited in the future
	[Header("Drop Zones")]
	public GameObject[] enemySlotsEven;
	public GameObject[] enemySlotsOdd;
	public GameObject[] playableSlotsEven;
	public GameObject[] playableSlotsOdd;
	[Header("Crew Slots")]
	public GameObject crewMemberSlot;
	public Transform crewSlotParent;
	public int crewPerRow = 5;
	public float paddingX = 250;
	public float paddingY = 250;
	public Vector2 startPos;

	[Header("Spawning")]
	public Pirate pirate;
	public Transform pirateParent;

	public CrewCard crew;
	public Transform crewParent;
	public Transform crewParentInOrigin;

	public Vector2Int pirateRange = new Vector2Int(1, 12);

	private PirateType typeToSpawn;

	private CardDropZone[,] spawnedCrewSlots;

	private int crewNum;
	private bool loaded = false;

	GameObject[] pirateSlots, playerSlots;
	
	void OnEnable()
    {
		loaded = false;
		crewNum = Globals.GameVars.playerShipVariables.ship.crew;
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
		GameObject.FindObjectOfType<MiniGameManager>().InitilizePirates(playerSlots);
		//MiniGameManager.InitilizePirates();
	}

	public void SetPirateType(PirateType t) 
	{
		typeToSpawn = t;
		//Debug.Log($"Spawning only pirates of type {typeToSpawn.name}"); //<-- that type gets changed again later when the actual pirate cardds are loaded in (based on the zone(s) the player is currently in
	}

	public void PopulateScreen() 
	{
		//random number of enemy priates created (1-12) 
		//different ranging numbers of pirates will be added later
		pirateRange.y = Mathf.Min(pirateRange.y, crewNum);
		int enAndPlayCnt = Random.Range(pirateRange.x, pirateRange.y+1);

		//if the number is even, the even array objects will be called
		pirateSlots = enAndPlayCnt % 2 == 0 ? enemySlotsEven : enemySlotsOdd;
		playerSlots = enAndPlayCnt % 2 == 0 ? playableSlotsEven : playableSlotsOdd;

		List<CrewMember> possiblePirates = Globals.GameVars.Pirates.Where(x => x.pirateType.Equals(typeToSpawn)).ToList();
		for (int x = 0; x < enAndPlayCnt; x++) 
		{
			pirateSlots[x].SetActive(true);
			Pirate g = Instantiate(pirate);
			Debug.Log("pirate local scale = " + pirate.transform.localScale);
			Debug.Log("pirate lossy scale = " + pirate.transform.lossyScale);
			if (typeToSpawn.difficulty == 1) {
				g.GetComponent<CrewCard>().power = (g.GetComponent<CrewCard>().power / 5);
				g.GetComponent<CrewCard>().powerText.text = g.GetComponent<CrewCard>().power.ToString();
			}
			else if (typeToSpawn.difficulty == 2) {
				g.GetComponent<CrewCard>().power = (g.GetComponent<CrewCard>().power / 3);
				g.GetComponent<CrewCard>().powerText.text = g.GetComponent<CrewCard>().power.ToString();
			}
			else if (typeToSpawn.difficulty == 4) {
				g.GetComponent<CrewCard>().power = (int)(g.GetComponent<CrewCard>().power * 1.5f);
				g.GetComponent<CrewCard>().powerText.text = g.GetComponent<CrewCard>().power.ToString();

			}

			//CrewMember randomPirate = Globals.GameVars.Pirates.RandomElement();
			CrewMember randomPirate = possiblePirates.RandomElement();
			g.SetCrew(randomPirate);
			g.Bind();
			possiblePirates.Remove(randomPirate);
			g.GetComponent<RectTransform>().position = pirateSlots[x].GetComponent<RectTransform>().position;
			g.transform.SetParent(pirateParent);
			//no idea why this is necessary, but all cards need to be scaled to the local scale of the canvas of the minigame 
			//same thing below is done for the crew cards
			g.transform.localScale = GetComponent<Canvas>().transform.localScale;
			playerSlots[x].SetActive(true);

			g.GetComponent<CrewCard>().cardIndex = pirateSlots[x].GetComponent<CardDropZone>().dropIndex;
		}

		int totalCrewRows = Mathf.CeilToInt((crewNum * 1.0f) / crewPerRow);
		float xPos = startPos.x;
		float yPos = startPos.y;
		int spawnedSlots = 0;
		spawnedCrewSlots = new CardDropZone[totalCrewRows, crewPerRow];

		for (int r = 0; r < totalCrewRows; r++) 
		{
			for (int c = 0; c < crewPerRow; c++) 
			{
				//Spawns a new crew slot
				GameObject slot = Instantiate(crewMemberSlot);
				slot.transform.SetParent(crewSlotParent);
				//scaling crew card slots
				slot.transform.localScale = GetComponent<Canvas>().transform.localScale;
				RectTransform slotRect = slot.GetComponent<RectTransform>();
				slotRect.anchoredPosition = new Vector2(xPos, yPos);
				CardDropZone cdz = slot.GetComponent<CardDropZone>();
				spawnedCrewSlots[r, c] = cdz;

				//Spawns a new crew member
				if (spawnedSlots < crewNum) 
				{
					CrewCard newCrew = Instantiate(crew);
					newCrew.SetRSP(this);
					newCrew.Bind();
					newCrew.GetComponent<RectTransform>().anchoredPosition = slotRect.position;
					newCrew.transform.SetParent(crewParentInOrigin);
					//scaling crew cards
					newCrew.transform.localScale = GetComponent<Canvas>().transform.localScale;
					newCrew.SetCrew(Globals.GameVars.playerShipVariables.ship.crewRoster[spawnedSlots]);
					cdz.SetOccupied(true);
					Debug.Log("crewmember scale = " + crew.transform.localScale);
					Debug.Log("crewmember lossy scale = " + crew.transform.lossyScale);
				}
				
				//Moves to the next point
				xPos += paddingX * (GetComponent<Canvas>().transform.localScale.x);
				spawnedSlots++;
			}
			xPos = startPos.x;
			yPos += paddingY * (GetComponent<Canvas>().transform.localScale.y);
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
}
