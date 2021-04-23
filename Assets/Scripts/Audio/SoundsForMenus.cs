using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundsForMenus : MonoBehaviour
{
	[SerializeField] private AudioMixerGroup mixer;

	public Sound[] menuSounds;
	public static SoundsForMenus instance;

	// Start is called before the first frame update
	void Awake() {
		///////////////////////////////////////////////////////////////////////////
		///To make sure that the game doesn't create two audio managers by mistake.
		/*if (instance == null) {
			instance = this;
		}
		else {
			Destroy(gameObject);
			return;
		}*/
		/////////////////////////////////////////////////////////////////////////////////

		/*This funtion is here so that when the game loads to a new scene,
		 * the audio doesn't skip, repeat, or stop. The audio for, say music, is continuous.*/
		//DontDestroyOnLoad(gameObject);


		//This foreach loop is for adding all the variables to the audio source
		foreach (Sound s in menuSounds) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.outputAudioMixerGroup = mixer;

			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
			s.source.playOnAwake = s.onAwake;
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

	public bool GetSoundIsPlaying(string name) {
		Sound s = Array.Find(menuSounds, sound => sound.name == name);

		return s.source.isPlaying;
	}
}
