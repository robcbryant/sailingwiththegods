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
	public int slotsPerRow;
	public float paddingX;
	public float paddingY;
	public Vector2 startPos;
	public int crewNum = 25;

	[Header("Spawning")]
	public Pirate pirate;
	public Transform pirateParent;

	public CrewCard crew;
	public Transform crewParent;
	public Transform crewParentInOrigin;

	public Vector2Int pirateRange = new Vector2Int(1, 12);

	private PirateType typeToSpawn;

	// Start is called before the first frame update
	void Start()
    {
		SetPirateType(Globals.GameVars.PirateTypes.RandomElement());
		crewNum = Globals.GameVars.playerShipVariables.ship.crew;
		Debug.Log($"Crew num {crewNum}");
		//call currently being used for testing purposes 
		populateScreen();
	}


	public void SetPirateType(PirateType t) 
	{
		typeToSpawn = t;
		Debug.Log($"Spawning only pirates of type {typeToSpawn.name}");
	}

	public void populateScreen() 
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
			//CrewMember randomPirate = Globals.GameVars.Pirates.RandomElement();
			CrewMember randomPirate = possiblePirates.RandomElement();
			g.SetCrew(randomPirate);
			possiblePirates.Remove(randomPirate);
			g.GetComponent<RectTransform>().anchoredPosition = pirateSlots[x].GetComponent<RectTransform>().anchoredPosition;
			g.transform.SetParent(pirateParent);
			playerSlots[x].SetActive(true);
		}

		int totalCrewRows = Mathf.CeilToInt((crewNum * 1.0f) / slotsPerRow);
		float xPos = startPos.x;
		float yPos = startPos.y;
		int spawnedSlots = 0;

		for (int r = 0; r < totalCrewRows; r++) 
		{
			for (int c = 0; c < slotsPerRow; c++) 
			{
				//Spawns a new crew slot
				GameObject slot = Instantiate(crewMemberSlot);
				slot.transform.SetParent(crewSlotParent);
				RectTransform slotRect = slot.GetComponent<RectTransform>();
				slotRect.anchoredPosition = new Vector2(xPos, yPos);
				slot.GetComponent<CardDropZone>().SetOccupied(true);

				//Spawns a new crew member
				CrewCard newCrew = Instantiate(crew);
				newCrew.SetRSP(this);
				newCrew.GetComponent<RectTransform>().anchoredPosition = slotRect.position;
				newCrew.transform.SetParent(crewParentInOrigin);
				newCrew.SetCrew(Globals.GameVars.playerShipVariables.ship.crewRoster[spawnedSlots]);

				//Moves to the next point
				xPos += paddingX;
				spawnedSlots++;
				if (spawnedSlots == crewNum) 
				{
					break;
				}
			}
			xPos = startPos.x;
			yPos += paddingY;
			if (spawnedSlots == crewNum) 
			{
				break;
			}
		}

		//for (int i = 0; i < crewMemberSlots.Length; i++) {
		//	CrewCard c = Instantiate(crew);
		//	c.GetComponent<RectTransform>().anchoredPosition = crewMemberSlots[i].GetComponent<RectTransform>().position;
		//	c.transform.SetParent(crewParent);
		//	c.gameObject.SetActive(crewMemberSlots[i].activeSelf);
		//}
	}
}
