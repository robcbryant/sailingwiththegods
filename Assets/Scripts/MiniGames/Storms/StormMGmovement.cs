using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormMGmovement : MonoBehaviour
{
	//the bpat the player identifies as in the MG
	[HideInInspector]
	public GameObject playerBoat;
	public float speed;

	private bool move;

	void Update() 
	{
		if (move) 
		{
			MoveBoat();
		}
		
	}

	//simple movement
	//up and down moves the boat 
	//left and right turns the boat 
	private void MoveBoat() 
	{
		float movement = Input.GetAxisRaw("Vertical");
		float rotation = Input.GetAxisRaw("Horizontal");

		playerBoat.transform.Translate(speed * movement, 0, 0);
		playerBoat.transform.Rotate(0, speed * 2 * rotation, 0);
	}

	public void ToggleMovement(bool toggle) 
	{
		move = toggle;
	}
}