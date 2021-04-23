using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcherSounds : MonoBehaviour
{
	[SerializeField] private GameObject playerHUD, agoraScene;

	[SerializeField] private SoundsForMenus sounds;

    // Update is called once per frame
    void Update()
    {

    }

	public void PlayDashboardSound() {
		sounds.PlaySound("Dashboard");
	}

	public void StopDoashboardSounnd() {
		sounds.StopSound("Dashboard");
	}

	public void PlayAgora() {
		sounds.PlaySound("Agora");
	}

	public void StopAgora() {
		sounds.StopSound("Agora");
	}
}


