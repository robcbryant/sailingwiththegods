using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MGScrollButtonController : MonoBehaviour
{
	//buttons needed for actual scrolling 
	public Button DownButton;
	public Button UpButton;

	//used for updating experience of scrolling as scrolling occurs 
	public GameObject DownButtonOn;
	public GameObject DownButtonOff;
	public GameObject UpButtonOn;
	public GameObject UpButtonOff;

	//needs to be the same as the "crewMemberSlots" array in RandomSlotPopulator.cs
	public GameObject[] crewMemberSlots;

	//used for having the correct number of scrolls
	private int rowCounter;
	private int maxCounter;

	//current row number of crew members listed on screen 
	private int currentRowNumber; 

	// Start is called before the first frame update
	void Start() {
		//starts with up being inactive and down being active in players eyes
		DownButtonOn.SetActive(true);
		DownButtonOff.SetActive(false);
		UpButtonOff.SetActive(true);
		UpButtonOn.SetActive(false);

		//counter needs to be changeble to increase to maxCounter and decrease back to zero
		rowCounter = 0;

		//this will eventually need to be the value of current crew memebers on the ship / 5 to make the number of rows that can be scrolled through 
		//example: if you have 30 crew members, maxCounter should equal 6 so that all crew members may be seen at some point by the player
		maxCounter = 6;

		//always start off with your first row
		currentRowNumber = 1; 

		//button function controllers 
		Button btn = UpButton.GetComponent<Button>();
		btn.onClick.AddListener(ClickingandCountingForUp);

		Button btn2 = DownButton.GetComponent<Button>();
		btn2.onClick.AddListener(ClickingandCountingForDown);
	}

	//turns the buttons "on" and "off" 
	public void UpdateUpAndDownButtons() {
		if (rowCounter <= 0) {
			//if player is at the beginning of the list, they can only scroll down

			rowCounter = 0;

			DownButtonOn.SetActive(true);
			DownButtonOff.SetActive(false);
			UpButtonOff.SetActive(true);
			UpButtonOn.SetActive(false);
		}
		else if (rowCounter == maxCounter-1) {
			//if player is at the bottom of the list, they can only scroll up

			DownButtonOff.SetActive(true);
			DownButtonOn.SetActive(false);
			UpButtonOff.SetActive(false);
			UpButtonOn.SetActive(true);
		}
		else {
			//if player is at the middle of the list, they can scroll down or up

			DownButtonOn.SetActive(true);
			DownButtonOff.SetActive(false);
			UpButtonOff.SetActive(false);
			UpButtonOn.SetActive(true);
		}
	}

	//shows current row of current crew members on screen 
	public void UpdateCurrentCrewSlots() {
		//for development testing 
		print("This is line " + (rowCounter+1) + " out of " + maxCounter);

		//below is the current set of crew members and how they are laid out 

		//row = 1		x	x	x	x	x		counter = 0
		//row = 2		x	x	x	x	x		counter = 1
		//row = 3		x	x	x	x	x		counter = 2
		//row = 4		x	x	x	x	x		counter = 3
		//row = 5		x	x	x	x	x		counter = 4
		//row = 6		x	x	x	x	x		counter = 5

		//---------------------------------------------------

		//row = 1		0	1	2	3	4		counter = 0
		//row = 2		5	6	7	8	9		counter = 1
		//row = 3		10	11	12	13	14		counter = 2
		//row = 4		15	16	17	18	19		counter = 3
		//row = 5		20	21	22	23	24		counter = 4
		//row = 6		25	26	27	28	29		counter = 5

		//---------------------------------------------------

		//row = 1		0	1	2	3	4		counter = 0
		if (currentRowNumber == 1) {

			for (int x = 0; x < 5; x++) {
				crewMemberSlots[x].SetActive(true);
			}
			for (int x = 5; x < maxCounter*5; x++) {
				crewMemberSlots[x].SetActive(false);
			}
		}

		//row = 2		5	6	7	8	9		counter = 1
		else if (currentRowNumber == 2) {

			for (int x = 5; x < 10; x++) {
				crewMemberSlots[x].SetActive(true);
				print(crewMemberSlots[x]);
			}
			for (int x = 10; x < maxCounter * 5; x++) {
				crewMemberSlots[x].SetActive(false);
			}
			for (int x = 0; x < 10; x++) {
				crewMemberSlots[x].SetActive(false);
			}
		}

		//row = 3		10	11	12	13	14		counter = 2
		else if (currentRowNumber == 3) {

			for (int x = 10; x < 15; x++) {
				crewMemberSlots[x].SetActive(true);
			}
			for (int x = 15; x < maxCounter * 5; x++) {
				crewMemberSlots[x].SetActive(false);
			}
			for (int x = 0; x < 15; x++) {
				crewMemberSlots[x].SetActive(false);
			}
		}

		//row = 4		15	16	17	18	19		counter = 3
		else if (currentRowNumber == 4) {

			for (int x = 15; x < 20; x++) {
				crewMemberSlots[x].SetActive(true);
			}
			for (int x = 20; x < maxCounter * 5; x++) {
				crewMemberSlots[x].SetActive(false);
			}
			for (int x = 0; x < 20; x++) {
				crewMemberSlots[x].SetActive(false);
			}
		}

		//row = 5		20	21	22	23	24		counter = 4
		else if (currentRowNumber == 5) {

			for (int x = 20; x < 25; x++) {
				crewMemberSlots[x].SetActive(true);
			}
			for (int x = 25; x < maxCounter * 5; x++) {
				crewMemberSlots[x].SetActive(false);
			}
			for (int x = 0; x < 25; x++) {
				crewMemberSlots[x].SetActive(false);
			}
		}
		//row = 6		25	26	27	28	29		counter = 5
		else if (currentRowNumber == 6) {

			for (int x = 25; x < 30; x++) {
				crewMemberSlots[x].SetActive(true);
			}
			for (int x = 0; x < 25; x++) {
				crewMemberSlots[x].SetActive(false);
			}
		}
	}


	public void ClickingandCountingForDown() {
		//for development testing 
		print("you pressed down");

		//counts to be the next row down from the current one
		rowCounter++;
		currentRowNumber++;

		//for development testing
		print(rowCounter);
		print(currentRowNumber);

		//updates scroll options
		UpdateUpAndDownButtons();

		//updates which crew members to show 
		UpdateCurrentCrewSlots();
	}

	public void ClickingandCountingForUp() {
		//for development testing 
		print("you pressed up");

		//counts to be the next row down from the current one
		rowCounter--;
		currentRowNumber--;

		//for development testing
		print(rowCounter);
		print(currentRowNumber);

		//updates scroll options
		UpdateUpAndDownButtons();

		//updates which crew members to show 
		UpdateCurrentCrewSlots();
	}
}