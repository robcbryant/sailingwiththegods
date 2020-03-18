using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum FightState { PLAYERTURN, ENEMYTURN, WON, LOSS}


public class MiniGameManager : MonoBehaviour
{
	public FightState state;
	public GameObject piratesParent, crewParent;
	public List<GameObject> pirates;
	private Color tempColor;

	private void Start() 
	{
		state = FightState.PLAYERTURN;
		//tempColor = piratesParent.transform.GetChild(0).gameObject.GetComponent<Image>().color;
	}

	public void Fight() {
		state = FightState.ENEMYTURN;
		CrewCard crewMember, pirate;
		foreach(Transform p in piratesParent.transform) {
			pirates.Add(p.gameObject);
		}
		pirates = pirates.OrderBy(GameObject => GameObject.transform.position.x).ToList<GameObject>();
		foreach (GameObject p in pirates) {
			Debug.Log(p.transform.GetComponent<CrewCard>().nameText.text);
		}
		for (int index = 0; index <= crewParent.transform.childCount - 1; index++) {
			crewMember = crewParent.transform.GetChild(index).GetComponent<CrewCard>();
			pirate = pirates[index].transform.GetComponent<CrewCard>();

			if (crewMember.gameObject.activeSelf && pirate.gameObject.activeSelf) {
				if (crewMember.power < pirate.power) {
					crewMember.gameObject.SetActive(false);
				}
				else if (crewMember.power > pirate.power) {
					pirate.gameObject.SetActive(false);
				}
				else {
					crewMember.gameObject.SetActive(false);
					pirate.gameObject.SetActive(false);
				}
			}
		}
		
	}

	public void AnimateAttack(GameObject pirate) {
	}
}
