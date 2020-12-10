using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormMGmovement : MonoBehaviour
{
	public float speed;
	public float reverseSpeed;
	public float rotSpeed;
	public float sizeMod = 0.16f;

	private bool move;
	private ParticleSystem[] ps;

	private void Start() {
		ps = GetComponentsInChildren<ParticleSystem>();
	}

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
		float movement = Input.GetAxis("Vertical");
		float rotation = Input.GetAxis("Horizontal");

		transform.position += transform.right.normalized * sizeMod * movement * (movement > 0 ? speed : reverseSpeed);
		transform.localEulerAngles += Vector3.up * rotSpeed * rotation;

		if (Mathf.Approximately(0.0f, movement)) {
			foreach (ParticleSystem p in ps) {
				if (p.isPlaying) {
					p.Stop();
				}
			}
		}
		else {
			foreach (ParticleSystem p in ps) {
				if (p.isStopped) {
					p.Play();
				}
			}
		}

	}

	public void ToggleMovement(bool toggle) 
	{
		move = toggle;
	}
}