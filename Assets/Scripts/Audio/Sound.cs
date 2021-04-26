using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//This class is here for the devs to edit or add settings to any of the other sound scripts that are available
[System.Serializable]
public class Sound
{
	public string name; 

	public AudioClip clip;

	[Range(0,1)]
	public float volume, pitch;

	public bool loop, soundIsPlaying, onAwake;

	[HideInInspector]
	public AudioSource source;
}
