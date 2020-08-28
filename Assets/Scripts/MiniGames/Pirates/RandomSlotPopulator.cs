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
	#region Obsolete Variables
	//public GameObject[] enemySlotsEven;
	//public GameObject[] enemySlotsOdd;
	//public GameObject[] playableSlotsEven;
	//public GameObject[] playableSlotsOdd;
	#endregion
	[Header("Crew Slots")]
	public GameObject crewOriginSlot;
	public Transform crewOriginParent;
	public int padding = 50;

	[Header("Crew Cards")]
	public Pirate pirateCard;
	public Transform pirateParent;
	public CrewCard crewCard;
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
		crewGrid = crewOriginParent.GetComponent<GridLayoutGroup>();
		crewPerRow = crewGrid.constraintCount;
		slotsPerRow = Mathf.CeilToInt(pirateRange.y / 2f);
	}

	private void OnDisable() {
		RandomizerForStorms.DestroyAllChildren(pirateParent);
		RandomizerForStorms.DestroyAllChildren(crewParent);
		RandomizerForStorms.DestroyAllChildren(crewParentInOrigin);
		RandomizerForStorms.DestroyAllChildren(crewOriginParent);
	}

	/// <summary>
	/// Begins the pirate mini game by loading in all the crew and pirates
	/// </summary>
	public void StartMinigame() 
	{
		PopulateScreen();
		ActivateCrewRow(0);
		loaded = true;
	}

	public void SetPirateType(PirateType t) 
	{
		typeToSpawn = t;
	}

	/// <summary>
	/// Generates the pirates and the pirate slots behind them, and the crew slots and scrolling crew list
	/// </summary>
	public void PopulateScreen() 
	{
		/*---------------------------------------------------------------------
		PIRATE SPAWNING
		---------------------------------------------------------------------*/

		//random number of enemy priates within range
		//the number spawned will never exceed the number of crew you have
		int count = Random.Range(pirateRange.x, Mathf.Min(pirateRange.y, crewNum) + 1);
		List<CrewMember> possiblePirates = Globals.GameVars.Pirates.Where(x => x.pirateType.Equals(typeToSpawn)).ToList();

		List<CardDropZone> crewSlots = new List<CardDropZone>();

		//spawns pirate and crew drop zones at the same time, since there will always be the same number
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

		GetComponent<MiniGameManager>().InitializeCrewSlots(crewSlots);

		//once the slots are in place, they won't be moving, so you can add the pirates
		//you can't add the pirates at the same time because the slots are in a horizontal layout group, so they'll be moving as new slots are added
		//it looks like you can just child the pirates to the slots and just leave it, but because of how the fighting works they have to be children of the pirate parent
		//if you don't change the parent, you'll automatically win every fight, which is obviously wrong
		//so we set the parent to the pirate slots now that they're in place, then switch parent so we get the right coordinates without having to do math
		for (int i = 0; i < count; i++) {
			Pirate p = Instantiate(pirateCard);
			CrewCard pCard = p.GetComponent<CrewCard>();
			switch (typeToSpawn.difficulty) {
				case (1):
					pCard.Power /= 5;
					break;
				case (2):
					pCard.Power /= 3;
					break;
				case (4):
					pCard.Power = (int)(pCard.Power * 1.5f);
					break;
			}

			CrewMember randomPirate = possiblePirates.RandomElement();
			pCard.SetCrew(randomPirate);
			p.Bind();
			//after you've used a pirate, remove it from the list of possibilities to avoid duplicates
			possiblePirates.Remove(randomPirate);
			Transform row = i < slotsPerRow ? enemyZones[0] : enemyZones[1];

			p.transform.SetParent(row.transform.GetChild(i % slotsPerRow));
			p.transform.localScale = Vector3.one;

			StartCoroutine(WaitAndChangeParent(p.transform, pirateParent));
			
			pCard.CardIndex = i;
		}

		#region Old Spawning Method
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
		#endregion

		/*----------------------------------------------------------------------
		SCALING
		----------------------------------------------------------------------*/

		//Everything that's instantiated will need to have its scale set
		//When you change an object's parent, its *actual* size doesn't change, but its *relative* size changes
		//The canvas scales with the screen resolution, so let's pretend it gets scaled to .75
		//When you instantiate a new crew card, it has no parent and its scale is just 1
		//But when you set its parent to something that's been scaled with the canvas to .75, the crew card doesn't change size on the screen
		//Instead, it changes scale to 1.25, which we don't want - we *want* it to get smaller so it fits the screen
		//The solution is that once you've changed the parent, you need to tell it to set its scale back down to 1
		//This line here doesn't actually have to do with any of that - this section had more in it but I figured out it wasn't necessary
		//So instead, I repurposed it into an explanation!
		float width = crewCard.GetComponent<RectTransform>().rect.width;


		/*----------------------------------------------------------------------
		ORIGIN SLOT SPAWNING
		----------------------------------------------------------------------*/

		//We need an int here, but we want the *right* int, so we do float division and then cast it up
		//The reason we use ceil and not just round is that we only want full rows. If there's 5 crew per row and you have 3 crew, we want you to still show all 5 slots for aesthetic reasons
		int totalCrewRows = Mathf.CeilToInt((crewNum * 1.0f) / crewPerRow);
		int spawnedSlots = 0;
		spawnedCrewSlots = new CardDropZone[totalCrewRows, crewPerRow];

		for (int r = 0; r < totalCrewRows; r++) 
		{
			for (int c = 0; c < crewPerRow; c++) 
			{
				//Spawns a new crew slot
				GameObject slot = Instantiate(crewOriginSlot);
				//scaling crew card slots
				slot.transform.SetParent(crewOriginParent);
				slot.transform.localScale = Vector3.one;
				
				CardDropZone cdz = slot.GetComponent<CardDropZone>();
				spawnedCrewSlots[r, c] = cdz;
			}
		}

		RectTransform crewParentRect = crewOriginParent.GetComponent<RectTransform>();
		//We're using a grid layout group, which means we might have expanded vertically, so we need to set the grid so the top row is properly centered
		crewParentRect.anchoredPosition = new Vector2(crewParentRect.anchoredPosition.x, CenterGrid(totalCrewRows, padding, width));

		/*----------------------------------------------------------------------
		CREW SPAWNING
		----------------------------------------------------------------------*/

		//Figure out where the center of the first crew card is
		//You'd think you can just get the position of the first crew origin slot, but for some reason that doesn't quite work
		float startX = width / 2;
		float startY = crewOriginParent.GetComponent<RectTransform>().rect.height / 2;

		float xPos = startX;
		float yPos = startY;

		for (int r = 0; r < totalCrewRows; r++) 
		{
			for (int c = 0; c < crewPerRow; c++) 
			{
				//Spawns a new crew member if there's another one to spawn
				//We need to check this because if you don't have an even number of crew members compared to slots per row, there'll be empty slots
				if (spawnedSlots < crewNum) {
					CrewCard newCrew = Instantiate(crewCard);
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
					spawnedSlots++;
					xPos += width;
				}
			}

			xPos = startX;
			yPos -= padding + width;
		}
	}

	/// <summary>
	/// Makes sure only the active row of the scrolling crew list is interactable
	/// </summary>
	/// <param name="rowNum">Row to activate</param>
	public void ActivateCrewRow(int rowNum) 
	{
		int index = 0;
		for (int r = 0; r < spawnedCrewSlots.GetLength(0); r++) 
		{
			for (int c = 0; c < spawnedCrewSlots.GetLength(1); c++) 
			{
				//Makes it so you can't accidentally drop onto invisible slots
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
		//The total size of the object is the number of rows times the size of each row, plus the number of padding sections times the size of the padding
		float total = (size * rows) + (padding * (rows - 1));

		//The offset is half the size of the object, made negative because you'll be moving it down
		//Naturally the grid is centered, so you need to move it half of its height so the top of the grid is visible, not the center
		float offset = total / -2.0f;
		//Then add on half the size of each row because the rows are centered, not bottom aligned
		offset += size / 2.0f;
		
		return offset;
	}

	private IEnumerator WaitAndChangeParent(Transform child, Transform newParent) {
		yield return null;
		child.SetParent(newParent);
	}

}
