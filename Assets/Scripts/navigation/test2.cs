using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nav;
public class test2 : MonoBehaviour
{
	Navigation navigation;
	private city cities;
	public Transform player;
	// Start is called before the first frame update
	void Start() {
		navigation = GetComponent<Navigation>();
		//GridClass grid = new GridClass(width,high,size,100);

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Q)) {
			//Debug.Log(navigation.player.transform.position);
			navigation.Setdestination("Iolcus",2);
			//cities = new city();
		}
		navigation.Setdestination("Tisaia",2);
		//navigation.Setdestination("Iolcus", 2);
		//navigation.Setdestination("Iolcus");
	}
	
}
