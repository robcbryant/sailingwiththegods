using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : ViewBehaviour<GameViewModel>
{
	[SerializeField] Button title_newgame_button = null;
	[SerializeField] Button title_loadgame_button = null;
	[SerializeField] Button title_quitgame_button = null;

	[SerializeField] Button title_credits_button = null;
	[SerializeField] Button title_credits_exit = null;
	[SerializeField] Text title_credits_text = null;
	[SerializeField] GameObject title_credits_screen = null;

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
	}

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

	#region Resolution Button Options (Fullscreen) Methods

	//what the game build generally starts off in
	public void GUI_DefaultGameResolution_FullScreen() {
		Screen.SetResolution(2048, 1536, true);
	}

	//25% increase from the default resolution
	public void GUI_HigherGameResolution_FullScreen() {
		Screen.SetResolution(2560, 1920, true);
	}

	//50% increase from the default resolution
	public void GUI_HighestGameResolution_Fullscreen() {
		Screen.SetResolution(3072, 2304, true);
	}

	//75% of the original default resolution 
	public void GUI_LowerGameResolution_FullScreen() {
		Screen.SetResolution(1536, 1152, true);
	}

	//50% of the original default resolution
	public void GUI_LowestGameResolution_Fullscreen() {
		Screen.SetResolution(1024, 768, true);
	}
	#endregion

	#region Resolution Button Options (Windowed) Methods
	//resolution the game build generally starts off in -- windowed
	public void GUI_DefaultGameResolution_Windowed() {
		Screen.SetResolution(2048, 1536, false);
	}

	//25% increase from the default resolution          -- windowed
	public void GUI_HigherGameResolution_Windowed() {
		Screen.SetResolution(2560, 1920, false);
	}

	//50% increase from the default resolution          -- windowed
	public void GUI_HighestGameResolution_Windowed() {
		Screen.SetResolution(3072, 2304, false);
	}

	//75% of the original default resolution            -- windowed
	public void GUI_LowerGameResolution_Windowed() {
		Screen.SetResolution(1536, 1152, false);
	}

	//50% of the original default resolution            -- windowed
	public void GUI_LowestGameResolution_Windowed() {
		Screen.SetResolution(1024, 768, false);
	}
	#endregion
}
