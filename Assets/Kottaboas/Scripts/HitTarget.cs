using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//What happens when you land a shot on the kottboas stand targets or inside the cups
public class HitTarget : MonoBehaviour
{
    public KottaboasManager gm;

	/// <summary>
	/// Find if the collision is with the targets do something
	/// </summary>
	/// <param name="collision"></param>
	private void OnCollisionEnter(Collision collision) {
		if (collision.collider.gameObject.CompareTag("Target")) {
			if (collision.collider.gameObject.name == "TopTargetCol" || collision.collider.gameObject.name == "TopTargetCol2") {
				gm.SCORE_PER_HIT(3);
				//gameObject.SetActive(false);				
				collision.collider.gameObject.GetComponent<AudioSource>().Play();
			}
			else if (collision.collider.gameObject.name == "MidTarget") {
				gm.SCORE_PER_HIT(2);
				gameObject.SetActive(false);
				collision.collider.gameObject.GetComponent<AudioSource>().Play();
			}
			else if(collision.collider.gameObject.name.Contains("Floating_Bowl")) {
				gm.SCORE_PER_HIT();
				gameObject.SetActive(false);
				collision.collider.gameObject.GetComponent<AudioSource>().Play();
			}
			else 
			{
				gm.SCORE_PER_HIT();
				gameObject.SetActive(false);
				collision.collider.gameObject.GetComponent<AudioSource>().Play();
			}
			gm.Scored = true;
			gm.IsHit = true;
		}
		if (collision.collider.gameObject.CompareTag("Wine")) 
		{
			gameObject.SetActive(false);
			collision.collider.gameObject.GetComponent<AudioSource>().Play();
		}
		gameObject.SetActive(false);
		//Debug.Log(collision.collider.gameObject.GetComponent<AudioSource>());
		collision.collider.gameObject.GetComponent<AudioSource>().Play();
		gm.ContinueRound = true;
	}
}
