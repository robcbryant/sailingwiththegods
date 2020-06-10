using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	[Header("Resolutions Settings Texts")]
	[SerializeField] Text default_text = null;
	[SerializeField] Text higher_text = null;
	[SerializeField] Text highest_text = null;
	[SerializeField] Text lower_text = null;
	[SerializeField] Text lowest_text = null;

	Text[] information_texts;
	int green_text_pos;
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

	private void Start() {
		Subscribe(title_newgame_button.onClick, () => Model.GUI_startNewGame(GameViewModel.Difficulty.Normal));
		Subscribe(title_loadgame_button.onClick, () => Model.GUI_loadGame(GameViewModel.Difficulty.Normal));
		Subscribe(title_quitgame_button.onClick, Application.Quit);

		Subscribe(title_credits_button.onClick, () => GUI_showCredits());
		Subscribe(title_credits_exit.onClick, () => GUI_hideCredits());

		Subscribe(resolutions_settings_button.onClick, () => GUI_ShowReolutionsSettings());
		Subscribe(resolutions_settings_exit.onClick, () => GUI_HideResolutionsSettings());

		Subscribe(default_full_resolution_button.onClick, () => GUI_DefaultGameResolution_FullScreen());
		Subscribe(higher_full_resolution_button.onClick, () => GUI_HigherGameResolution_FullScreen());
		Subscribe(highest_full_resolution_button.onClick, () => GUI_HighestGameResolution_Fullscreen());
		Subscribe(lower_full_resolution_button.onClick, () => GUI_LowerGameResolution_FullScreen());
		Subscribe(lowest_full_resolution_button.onClick, () => GUI_LowestGameResolution_Fullscreen());

		Subscribe(default_windowed_resolution_button.onClick, () => GUI_DefaultGameResolution_Windowed());
		Subscribe(higher_windowed_resolution_button.onClick, () => GUI_HigherGameResolution_Windowed());
		Subscribe(highest_windowed_resolution_button.onClick, () => GUI_HighestGameResolution_Windowed());
		Subscribe(lower_windowed_resolution_button.onClick, () => GUI_LowerGameResolution_Windowed());
		Subscribe(lowest_windowed_resolution_button.onClick, () => GUI_LowestGameResolution_Windowed());

		information_texts = new Text[5] { default_text, higher_text, highest_text, lower_text, lowest_text };
		regional_zones = new GameObject[] { Aetolian_Region_Zone, Cretan_Region_Zone, Etruscan_Pirate_Region_Zone, Illyrian_Region_Zone };

		Make_Zones_Invisible_On_Play_Start();
	}

	#region Changing Text Colors for Current Resolution
	private void FixedUpdate() {
		if (resolutions_settings_screen) {
			Changing_Text_Colors();
		}
	}

	void Changing_Text_Colors() {
		if (Screen.currentResolution.height == 1200) {
			green_text_pos = 0;
		}
		else if (Screen.currentResolution.height == 1440) {
			green_text_pos = 1;
		}
		else if(Screen.currentResolution.height == 1600) {
			green_text_pos = 2;
		}
		else if(Screen.currentResolution.height == 1080) {
			green_text_pos = 3;
		}
		else if(Screen.currentResolution.height == 720) {
			green_text_pos = 4;
		}

		for (int x = 0; 0 < information_texts.Length; x++) {
			if (x != green_text_pos && x < 5) {
				information_texts[x].color = Color.black;
			}
			else if (x == green_text_pos && x < 5) {
				information_texts[x].color = green_text_color;
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

	#region Resolution Button Options (Fullscreen) Methods

	//what the game build generally starts off in
	public void GUI_DefaultGameResolution_FullScreen() {
		Screen.SetResolution(1920, 1200, true);
		green_text_pos = 0;
	}

	public void GUI_HigherGameResolution_FullScreen() {
		Screen.SetResolution(2560, 1440, true);
		green_text_pos = 1;
	}

	public void GUI_HighestGameResolution_Fullscreen() {
		Screen.SetResolution(2560, 1600, true);
		green_text_pos = 2;
	}

	public void GUI_LowerGameResolution_FullScreen() {
		Screen.SetResolution(1920, 1080, true);
		green_text_pos = 3;
	}

	public void GUI_LowestGameResolution_Fullscreen() {
		Screen.SetResolution(1280, 720, true);
		green_text_pos = 4;
	}
	#endregion

	#region Resolution Button Options (Windowed) Methods
	public void GUI_DefaultGameResolution_Windowed() {
		Screen.SetResolution(1920, 1200, false);
		green_text_pos = 0;
	}

	public void GUI_HigherGameResolution_Windowed() {
		Screen.SetResolution(2560, 1440, false);
		green_text_pos = 1;
	}

	public void GUI_HighestGameResolution_Windowed() {
		Screen.SetResolution(2560, 1600, false);
		green_text_pos = 2;
	}

	public void GUI_LowerGameResolution_Windowed() {
		Screen.SetResolution(1920, 1080, false);
		green_text_pos = 3;
	}

	public void GUI_LowestGameResolution_Windowed() {
		Screen.SetResolution(1280, 720, false);
		green_text_pos = 4;
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
