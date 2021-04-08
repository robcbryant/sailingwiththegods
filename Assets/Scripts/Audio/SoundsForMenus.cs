using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundsForMenus : MonoBehaviour
{
	[SerializeField]
	private GameObject playerHUD, titleScreen, agoroScene, tavernScene, loanScene, dialogScene, pauseMenu, portScene;

	[SerializeField]
	private AudioSource ambientSound;

	public Sound[] menuSounds;
	public static SoundsForMenus instance;

	// Start is called before the first frame update
	void Awake() {
		///////////////////////////////////////////////////////////////////////////
		///To make sure that the game doesn't create two audio managers by mistake.
		if (instance == null) {
			instance = this;
		}
		else {
			Destroy(gameObject);
			return;
		}
		/////////////////////////////////////////////////////////////////////////////////

		/*This funtion is here so that when the game loads to a new scene,
		 * the audio doesn't skip, repeat, or stop. The audio for, say music, is continuous.*/
		DontDestroyOnLoad(gameObject);


		//This foreach loop is for adding all the variables to the audio source
		foreach (Sound s in menuSounds) {
			s.source = gameObject.AddComponent<AudioSource>();

			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}
	}


	//This function plays a sound
	public void PlaySound(string name) {
		Sound s = Array.Find(menuSounds, sound => sound.name == name);

		s.source.Play();
	}


	//this function is to stop a sound from playing
	public void StopSound(string name) {
		Sound s = Array.Find(menuSounds, sound => sound.name == name);

		if (s == null) {
			Debug.LogWarning("Sound:  " + name + " not found!");
			return;
		}

		s.source.Stop();
	}

	private  void ScreenSwitcher() {
		if (playerHUD.activeSelf) {
			PlaySound("Dashboard");
			StopSound("Agora Scene");
			StopSound("Title Screen");
		}
		else if (agoroScene.activeSelf) {
			PlaySound("Agora Scene");
		}
		else if (titleScreen.activeSelf) {
			PlaySound("Title Screen");
		}
	}
}
