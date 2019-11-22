using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : ViewBehaviour<GameViewModel>
{
	[SerializeField] Button ResumeButton = null;
	[SerializeField] Button HelpButton = null;
	[SerializeField] Button SaveButton = null;
	[SerializeField] Button QuitButton = null;

	[SerializeField] GameObject hud_button_helpwindow = null;
	[SerializeField] Button HelpExitButton = null;

	private void Start() {
		Subscribe(ResumeButton.onClick, () => Globals.UI.Hide<MainMenuScreen>());
		Subscribe(HelpButton.onClick, GUI_showHelpMenu);
		Subscribe(HelpExitButton.onClick, GUI_closeHelpMenu);
		Subscribe(SaveButton.onClick, () => DoAndClose(Model.GUI_saveGame));
		Subscribe(QuitButton.onClick, () => DoAndClose(() => Model.GUI_restartGame()));
	}

	void DoAndClose(Action action) {
		action();
		Globals.UI.Hide<MainMenuScreen>();
	}

	//-----------------------------------------------------
	//THIS IS THE HELP BUTTON	
	public void GUI_showHelpMenu() {
		hud_button_helpwindow.SetActive(true);
	}

	//-----------------------------------------------------
	//THIS IS THE CLOSE HELP BUTTON	
	public void GUI_closeHelpMenu() {
		hud_button_helpwindow.SetActive(false);
	}
}
