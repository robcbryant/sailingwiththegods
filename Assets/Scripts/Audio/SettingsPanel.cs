using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
	[SerializeField] private GameObject settingsPanel;
	[SerializeField] private GameObject audioSettings;
	[SerializeField] private GameObject videoSettings;

	private void Awake() {
		CloseSettings();
	}

	public void OpenSettings() {
		settingsPanel.SetActive(true);
	}

	public void CloseSettings() {
		settingsPanel.SetActive(false);
	}

	public void Open_VideoSettings() {
		settingsPanel.SetActive(false) ;
		videoSettings.SetActive(true);
	}

	public void Close_VideoSettings() {
		settingsPanel.SetActive(true);
		videoSettings.SetActive(false);
	}

	public void Open_AudioSettings() {
		settingsPanel.SetActive(false);
		audioSettings.SetActive(true);
	}

	public void Close_AudioSettings() {
		settingsPanel.SetActive(true);
		audioSettings.SetActive(false);
	}
}
