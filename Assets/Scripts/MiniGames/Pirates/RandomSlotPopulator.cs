using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
	}

	public void SetPirateType(PirateType t) 
	{
		typeToSpawn = t;
		Debug.Log($"Spawning only pirates of type {typeToSpawn.name}");
	}

	public void PopulateScreen() 
	{
		//random number of enemy priates created (1-12) 
		//different ranging numbers of prirates will be added later
		pirateRange.y = Mathf.Min(pirateRange.y, crewNum);
		int enAndPlayCnt = Random.Range(pirateRange.x, pirateRange.y+1);

		//print to the console for development team to check and make sure the call is going correctly
		print(enAndPlayCnt);

		//if the number is even, the even array objects will be called
		GameObject[] pirateSlots = enAndPlayCnt % 2 == 0 ? enemySlotsEven : enemySlotsOdd;
		GameObject[] playerSlots = enAndPlayCnt % 2 == 0 ? playableSlotsEven : playableSlotsOdd;

		List<CrewMember> possiblePirates = Globals.GameVars.Pirates.Where(x => x.pirateType.Equals(typeToSpawn)).ToList();
		for (int x = 0; x < enAndPlayCnt; x++) 
		{
			pirateSlots[x].SetActive(true);
			Pirate g = Instantiate(pirate);
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
			g.GetComponent<RectTransform>().anchoredPosition = pirateSlots[x].GetComponent<RectTransform>().anchoredPosition;
			g.transform.SetParent(pirateParent);
			playerSlots[x].SetActive(true);
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
					newCrew.SetCrew(Globals.GameVars.playerShipVariables.ship.crewRoster[spawnedSlots]);
					cdz.SetOccupied(true);
				}
				
				//Moves to the next point
				xPos += paddingX;
				spawnedSlots++;
			}
			xPos = startPos.x;
			yPos += paddingY;
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
