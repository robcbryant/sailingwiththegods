using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomSlotPopulator : MonoBehaviour
{
	//Arrays of the drops zones (names respectively to their roles) are public so they may be edited in the future
	[Header("Slots and Drop Zones")]
	public GameObject[] enemySlotsEven;
	public GameObject[] enemySlotsOdd;
	public GameObject[] playableSlotsEven;
	public GameObject[] playableSlotsOdd;
	public GameObject[] crewMemberSlots;

	[Header("Spawning")]
	public Pirate pirate;
	public Transform pirateParent;

	public CrewCard crew;
	public Transform crewParent;

	public Vector2Int pirateRange = new Vector2Int(1, 12);

	private PirateType typeToSpawn;

	// Start is called before the first frame update
	void Start()
    {
		SetPirateType(Globals.GameVars.PirateTypes.RandomElement());
		//call currently being used for testing purposes 
		populateScreen();
	}

    // Update is called once per frame
    void Update()
    {
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

		for (int i = 0; i < crewMemberSlots.Length; i++) 
		{
			CrewCard c = Instantiate(crew);
			c.GetComponent<RectTransform>().anchoredPosition = crewMemberSlots[i].GetComponent<RectTransform>().anchoredPosition;
			c.transform.SetParent(crewParent);
			c.gameObject.SetActive(crewMemberSlots[i].activeSelf);
		}
	}
}
