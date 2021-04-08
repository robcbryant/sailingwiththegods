using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
	[SerializeField] private GameObject audioSettings;

	//Sliders
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider soundEffectsSlider;
	[SerializeField] private Slider backgroundAudioSlider;
	[SerializeField] private Slider musicSlider;

	//Audio Mixer Groups
	[SerializeField] private AudioMixer masterMixer;
	[SerializeField] private AudioMixer soundEffectsMixer;
	[SerializeField] private AudioMixer backgroundMixer;
	[SerializeField] private AudioMixer musicMixer;

	// Start is called before the first frame update
	void Awake()
    {
		audioSettings.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		//print("Master slider value is: " + masterSlider.value);
    }

	public void OpenSettings() {
		audioSettings.SetActive(true);
	}

	public void CloseSettings() {
		audioSettings.SetActive(false);
	}

	public void MasterSldier(float vol) {
		vol = masterSlider.value;
		masterMixer.SetFloat("masterVolume", vol);
	}

	public void SoundEffectsSldier(float vol) {
		vol = soundEffectsSlider.value;
		soundEffectsMixer.SetFloat("soundEffectsVolume", vol);
	}

	public void BackgroundSldier(float vol) {
		vol = backgroundAudioSlider.value;
		backgroundMixer.SetFloat("backgroundVolume", vol);
	}

	public void MusicSldier(float vol) {
		vol = musicSlider.value;
		musicMixer.SetFloat("musicVolume", vol);
	}
}
