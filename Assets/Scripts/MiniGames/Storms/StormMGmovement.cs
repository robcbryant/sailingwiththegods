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
	private bool particlesPlaying = false;

	private void Start() {
		ps = GetComponentsInChildren<ParticleSystem>();
		StartCoroutine(TurnOnParticles());
	}

	//This looks completely useless, but it's actually necessary
	//I have no idea why, but I noticed that the game lagged pretty severely if the particles were turned on in the start
	//If I manually turned them off and back on, however, the lag completely went away
	//So this is needed to prevent the storm game from lagging for whatever reason
	private IEnumerator TurnOnParticles() {
		yield return null;
		foreach (ParticleSystem p in ps) {
			p.gameObject.SetActive(false);
		}
		yield return null;
		foreach (ParticleSystem p in ps) {
			p.gameObject.SetActive(true);
		}
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
			if (particlesPlaying) 
			{
				foreach (ParticleSystem p in ps) 
				{
					p.Stop();
				}
				particlesPlaying = false;
			}

		}
		else {
			if (!particlesPlaying) 
			{
				foreach (ParticleSystem p in ps) 
				{
					p.Play();
				}
				particlesPlaying = true;
			}
		}

	}

	public void ToggleMovement(bool toggle) 
	{
		move = toggle;
	}
}