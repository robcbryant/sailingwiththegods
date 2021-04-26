using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
	//Game objects
	[SerializeField] private GameObject audioSettings;
	[SerializeField] private SettingsPanel settingsPanel;

	//Sliders
	[SerializeField] private Slider masterSlider, soundEffectsSlider, backgroundAudioSlider, musicSlider;

	//Audio Mixer Groups
	[SerializeField] private AudioMixer masterMixer;

	// Start is called before the first frame update
	void Awake()
    {
		audioSettings.SetActive(false);
    }

	public void OpenSettings() {
		//opens the settings for the audio setitngs
		audioSettings.SetActive(true);
		settingsPanel.CloseSettings();
	}

	public void CloseSettings() {
		//closes the audio setitngs
		audioSettings.SetActive(false);
		settingsPanel.OpenSettings();
	}

	public void MasterSldier(float vol) {
		vol = masterSlider.value;
		masterMixer.SetFloat("masterVolume", vol);
		//controls for the master slider
	}

	public void SoundEffectsSldier(float vol) {
		vol = soundEffectsSlider.value;
		masterMixer.SetFloat("soundEffectsVolume", vol);
		//controls for the sound effects slider
	}

	public void BackgroundSldier(float vol) {
		vol = backgroundAudioSlider.value;
		masterMixer.SetFloat("backgroundVolume", vol);
		//controls for the backgorund audio slider
	}

	public void MusicSldier(float vol) {
		vol = musicSlider.value;
		masterMixer.SetFloat("musicVolume", vol);
		//controls for the music slider
	}
}
