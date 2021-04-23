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
	[SerializeField] private AudioMixer masterMixer, soundEffectsMixer, backgroundMixer, musicMixer;

	// Start is called before the first frame update
	void Awake()
    {
		audioSettings.SetActive(false);
    }

	public void OpenSettings() {
		audioSettings.SetActive(true);
		settingsPanel.CloseSettings();
	}

	public void CloseSettings() {
		audioSettings.SetActive(false);
		settingsPanel.OpenSettings();
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
