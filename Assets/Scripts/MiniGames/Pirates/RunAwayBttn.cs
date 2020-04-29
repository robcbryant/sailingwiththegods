using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunAwayBttn : MonoBehaviour
{
	public Button runAwayBttn;
    // Start is called before the first frame update
    void Start()
    {
		runAwayBttn.onClick.AddListener(RunAwayFromBaddyPirateShip);
    }

    public void RunAwayFromBaddyPirateShip() {
		print("testing running away");
	}
}
