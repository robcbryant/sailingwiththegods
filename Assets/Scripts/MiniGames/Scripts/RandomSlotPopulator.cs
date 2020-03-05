using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSlotPopulator : MonoBehaviour
{
	//Arrays of the drops zones (names respectively to their roles) are public so they may be edited in the future
	public GameObject[] enemySlotsEven;
	public GameObject[] enemySlotsOdd;
	public GameObject[] playableSlotsEven;
	public GameObject[] playableSlotsOdd;
	public GameObject[] crewMemberSlots;

	public GameObject[] enemyBackgrounds; 

	// Start is called before the first frame update
	void Start()
    {
		//call currently being used for testing purposes 
		populateScreen();
	}

    // Update is called once per frame
    void Update()
    {
				
    }

	public void populateScreen() 
	{
		//random number of enemy priates created (1-12) 
		//different ranging numbers of prirates will be added later
		int enAndPlayCnt = Random.Range(1, 12);

		//print to the console for development team to check and make sure the call is going correctly
		print(enAndPlayCnt);

		//shaded background for enemy slots (to show difference) 
		//one for each row of enemy slots
		if(enAndPlayCnt<= 6) {
			enemyBackgrounds[0].SetActive(true);
		}
		else {
			enemyBackgrounds[0].SetActive(true);
			enemyBackgrounds[1].SetActive(true);
		}

		//if the number is even, the even array objects will be called 
		if (enAndPlayCnt % 2 == 0) {
			for (int x = 0; x < enAndPlayCnt; x++) {
				enemySlotsEven[x].SetActive(true);
				playableSlotsEven[x].SetActive(true);
			}
		}
		else {
			//the odd array objects are called here 
			for (int x = 0; x < enAndPlayCnt; x++) {
				enemySlotsOdd[x].SetActive(true);
				playableSlotsOdd[x].SetActive(true);
			}
		}
	}
}
