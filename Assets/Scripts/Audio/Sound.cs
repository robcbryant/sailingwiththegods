using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sound
{
	public string name; 

	public AudioClip clip;

	[Range(0,1)]
	public float volume, pitch;

	public bool loop;

	[HideInInspector]
	public AudioSource source;
}
