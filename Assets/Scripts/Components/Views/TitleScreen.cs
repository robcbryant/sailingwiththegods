using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : ViewBehaviour<GameViewModel>
{
	[SerializeField] Button title_newgame_button;
	[SerializeField] Button title_newgame_beginner_button;
	[SerializeField] Button title_loadgame_button;
	[SerializeField] Button title_loadgame_beginner_button;

	[SerializeField] Button title_credits_button;
	[SerializeField] Button title_credits_exit;
	[SerializeField] Text title_credits_text;
	[SerializeField] GameObject title_credits_screen;

	// killing this screen. or at least redoing it
	/*
	[SerializeField] GameObject title_crew_select;
	[SerializeField] GameObject title_crew_select_crew_list;
	[SerializeField] GameObject title_crew_select_entry_template;
	[SerializeField] GameObject title_crew_select_crew_count;
	[SerializeField] GameObject title_crew_select_start_game;
	*/

	private void Start() {
		Subscribe(title_newgame_button.onClick, () => Model.GUI_startNewGame(GameViewModel.Difficulty.Normal));
		Subscribe(title_newgame_beginner_button.onClick, () => Model.GUI_startNewGame(GameViewModel.Difficulty.Beginner));
		Subscribe(title_loadgame_button.onClick, () => Model.GUI_loadGame(GameViewModel.Difficulty.Normal));
		Subscribe(title_loadgame_beginner_button.onClick, () => Model.GUI_loadGame(GameViewModel.Difficulty.Beginner));

		Subscribe(title_credits_button.onClick, () => GUI_showCredits());
		Subscribe(title_credits_exit.onClick, () => GUI_hideCredits());
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
}
