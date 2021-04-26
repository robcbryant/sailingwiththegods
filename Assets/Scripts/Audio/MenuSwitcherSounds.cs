using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitcherSounds : MonoBehaviour
{
	[SerializeField] private SoundsForMenus sounds;

    // Update is called once per frame
    void Update()
    {

    }

	//THIS ENTIRE CLASS IS TO START AND STOP SOUNDS FROM PLAYING WHEN CERTIAN PANELS ARE OPEN


	public void PlayDashboardSound() {
		print("Playing dashboard sfx");
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


