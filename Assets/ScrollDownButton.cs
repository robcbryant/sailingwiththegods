using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDownButton : MonoBehaviour
{
	public GameObject DownButtonOn;
	public GameObject DownButtonOff;

	public GameObject[] crewMemberSlots;

	// Start is called before the first frame update
	void Start()
    {
		DownButtonOn.SetActive(true);
		DownButtonOff.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
