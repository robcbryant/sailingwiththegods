using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormMGmovement : MonoBehaviour
{
	//the bpat the player identifies as in the MG
	public GameObject playerBoat;
	public float boatHealth;
	public float currentClout;
	public float speed;
	
	void Update() {
		MoveBoat();
	}

	//simple movement
	//up and down moves the boat 
	//left and right turns the boat 
	private void MoveBoat() {
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
			transform.Translate(speed, 0, 0);
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			transform.Translate(-speed, 0, 0);
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
			transform.Rotate(0, -speed*2, 0);
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			transform.Rotate(0, speed*2, 0);
		}
	}
}