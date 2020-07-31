using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TitleScreen : ViewBehaviour<GameViewModel>
{
	[Header("Title Screen Buttons")]
	[SerializeField] Button title_newgame_button = null;
	[SerializeField] Button title_loadgame_button = null;
	[SerializeField] Button title_quitgame_button = null;

	[SerializeField] Button title_credits_button = null;
	[SerializeField] Button title_credits_exit = null;
	[SerializeField] Text title_credits_text = null;
	[SerializeField] GameObject title_credits_screen = null;

	[Header("Resolutions Settings Buttons")]
	[SerializeField] Button resolutions_settings_button = null;
	[SerializeField] Button resolutions_settings_exit = null;
	[SerializeField] GameObject resolutions_settings_screen = null;

	[SerializeField] Button default_full_resolution_button = null;
	[SerializeField] Button higher_full_resolution_button = null;
	[SerializeField] Button highest_full_resolution_button = null;
	[SerializeField] Button lower_full_resolution_button = null;
	[SerializeField] Button lowest_full_resolution_button = null;

	[SerializeField] Button default_windowed_resolution_button = null;
	[SerializeField] Button higher_windowed_resolution_button = null;
	[SerializeField] Button highest_windowed_resolution_button = null;
	[SerializeField] Button lower_windowed_resolution_button = null;
	[SerializeField] Button lowest_windowed_resolution_button = null;

	[SerializeField] Button[] fullResolutionButtons;
	[SerializeField] Button[] windowedResolutionButtons;

	[Header("Resolutions Settings Texts")]
	[SerializeField] Text default_text = null;
	[SerializeField] Text higher_text = null;
	[SerializeField] Text highest_text = null;
	[SerializeField] Text lower_text = null;
	[SerializeField] Text lowest_text = null;

	Text[] information_texts;
	int green_text_pos = 0;
	Color green_text_color = new Color32(00, 150, 00, 255);

	[Header("Regional Zones")]
	//any time a new regional zone is added to this list or to the IDE, 
	//the regional_zones array will need to be hard-code edited in this script's start method
	//AND the game object within the IDE needs to be inactive to start off with 
	[SerializeField] GameObject Aetolian_Region_Zone = null;
	[SerializeField] GameObject Cretan_Region_Zone = null;
	[SerializeField] GameObject Etruscan_Pirate_Region_Zone = null;
	[SerializeField] GameObject Illyrian_Region_Zone = null;

	GameObject[] regional_zones;

	Vector2Int[] supportedResolutions;

	private void Start() {
		Subscribe(title_newgame_button.onClick, () => Model.GUI_startNewGame(GameViewModel.Difficulty.Normal));
		Subscribe(title_loadgame_button.onClick, () => Model.GUI_loadGame(GameViewModel.Difficulty.Normal));
		Subscribe(title_quitgame_button.onClick, Application.Quit);

		Subscribe(title_credits_button.onClick, () => GUI_showCredits());
		Subscribe(title_credits_exit.onClick, () => GUI_hideCredits());

		Subscribe(resolutions_settings_button.onClick, () => GUI_ShowReolutionsSettings());
		Subscribe(resolutions_settings_exit.onClick, () => GUI_HideResolutionsSettings());


		information_texts = new Text[5] { highest_text, higher_text, default_text, lower_text, lowest_text };
		regional_zones = new GameObject[] { Aetolian_Region_Zone, Cretan_Region_Zone, Etruscan_Pirate_Region_Zone, Illyrian_Region_Zone };

		Make_Zones_Invisible_On_Play_Start();

		fullResolutionButtons = new Button[5] { highest_full_resolution_button, higher_full_resolution_button, 
			default_full_resolution_button, lower_full_resolution_button, lowest_full_resolution_button };
		
		windowedResolutionButtons = new Button[5] { highest_windowed_resolution_button, higher_windowed_resolution_button,
			default_windowed_resolution_button, lower_windowed_resolution_button, lowest_windowed_resolution_button };

		AddSupportedReolutionsToArray();

		//sets game to start in highest available resolution
		var highestRes = supportedResolutions.First();
		Screen.SetResolution(highestRes.x, highestRes.y, true);
		SetGreenText();

		SetResolutionsAndGreenTexts();
	}

	#region Resolutions Settings
	//make sure only one copy of resolution in list
	public void AddSupportedReolutionsToArray() {
		var distinctResoultionsList = Screen.resolutions
			.Select(res => new Vector2Int(res.width, res.height))
			.Distinct();

		supportedResolutions = distinctResoultionsList.OrderByDescending(size => size.magnitude)
			.Take(5)
			.ToArray();
	}

	public void SetResolutionsAndGreenTexts() {
		for (int x = 0; x < 5; x++) {
			if (x < supportedResolutions.Length) {
				fullResolutionButtons[x].GetComponentInChildren<Text>().text = supportedResolutions[x].x + " x " + supportedResolutions[x].y;
				windowedResolutionButtons[x].GetComponentInChildren<Text>().text = supportedResolutions[x].x + " x " + supportedResolutions[x].y;

				var index = x;

				Subscribe(fullResolutionButtons[x]
					.onClick, () => {
						Screen.SetResolution(supportedResolutions[index].x, supportedResolutions[index].y, true);
						green_text_pos = index;

						SetGreenText();
					});

				Subscribe(windowedResolutionButtons[x]
					.onClick, () => {
						Screen.SetResolution(supportedResolutions[index].x, supportedResolutions[index].y, false);
						green_text_pos = index;

						SetGreenText();
					});

			}
			else {
				fullResolutionButtons[x].gameObject.SetActive(false);
				windowedResolutionButtons[x].gameObject.SetActive(false);
				information_texts[x].gameObject.SetActive(false);
			}
		}
	}

	public void SetGreenText() {
		for (int i = 0; 0 < information_texts.Length; i++) {
			if (i != green_text_pos && i < 5) {
				information_texts[i].color = Color.black;
			}
			else if (i == green_text_pos && i < 5) {
				information_texts[i].color = green_text_color;
			}
			else {
				break;
			}
		}
	}
	#endregion

	#region GUI Show and Hide
	override protected void OnEnable() {
		base.OnEnable();

		title_credits_text.text = (Resources.Load("game_credits_message") as TextAsset).text;
	}

	public void GUI_showCredits() {
		title_credits_screen.SetActive(true);
	}
	public void GUI_hideCredits() {
		title_credits_screen.SetActive(false);
	}

	public void GUI_ShowReolutionsSettings() {
		resolutions_settings_screen.SetActive(true);
	}

	public void GUI_HideResolutionsSettings() {
		resolutions_settings_screen.SetActive(false);
	}
	#endregion

	#region Making Regional Zones Invisible in Game

	public void Make_Zones_Invisible_On_Play_Start() {
		foreach( GameObject zone in regional_zones) {
			zone.SetActive(true);
			zone.GetComponentInChildren<MeshRenderer>().enabled = false;
		}
	}
	#endregion
}
