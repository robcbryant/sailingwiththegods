using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MiniGameManager : MonoBehaviour
{
	public GameObject piratesParent, crewParent;
	private Color tempColor;
	public float timer;

	private void Start() {
		timer = .2f;
		tempColor = piratesParent.transform.GetChild(0).gameObject.GetComponent<Image>().color;
	}

	private void FixedUpdate() {
		if(timer >= 0) {
			timer -= Time.deltaTime;
		}
		else {
			foreach (Transform pirate in piratesParent.transform) {
				pirate.gameObject.GetComponent<Image>().color = tempColor;
			}
			timer = .2f;
		}
	}

	public void Attack() {

		foreach(Transform pirate in piratesParent.transform) {

			AnimateAttack(pirate.gameObject);
		}
	}

	public void AnimateAttack(GameObject pirate) {
		pirate.gameObject.GetComponent<Image>().color = Color.red;
	}
}
