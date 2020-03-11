using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FightState { PLAYERTURN, ENEMYTURN, WON, LOSS}


public class MiniGameManager : MonoBehaviour
{
	public FightState state;
	public GameObject piratesParent, crewParent;
	private Color tempColor;

	private void Start() {
		state = FightState.PLAYERTURN;
		tempColor = piratesParent.transform.GetChild(0).gameObject.GetComponent<Image>().color;
	}

	public void Fight() {
		state = FightState.ENEMYTURN;
		CrewCard crewMember, pirate;
		for (int index = 0; index <= crewParent.transform.childCount - 1; index++) {
			crewMember = crewParent.transform.GetChild(index).GetComponent<CrewCard>();
			pirate = piratesParent.transform.GetChild(index).GetComponent<CrewCard>();

			if (crewMember.power < pirate.power) {
				crewParent.transform.GetChild(index).gameObject.SetActive(false);
			}
			else if(crewMember.power > pirate.power) {
				piratesParent.transform.GetChild(index).gameObject.SetActive(false);
			}
			else {
				crewParent.transform.GetChild(index).gameObject.SetActive(false);
				piratesParent.transform.GetChild(index).gameObject.SetActive(false);
			}
		}
		
	}

	public void AnimateAttack(GameObject pirate) {
	}
}
