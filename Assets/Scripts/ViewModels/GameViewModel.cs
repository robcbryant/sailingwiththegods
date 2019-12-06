using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameViewModel : Model
{
	public enum Difficulty
	{
		Normal,
		Beginner
	}

	GameVars GameVars => Globals.GameVars;

	// happens once crew is selected and we're putting the player in game
	public void GUI_startMainGame() {
		GameVars.camera_titleScreen.SetActive(false);
		GameVars.bg_startScreen.SetActive(false);

		//Turn on the environment fog
		RenderSettings.fog = true;

		//Now turn on the main player controls camera
		GameVars.FPVCamera.SetActive(true);

		//Turn on the player distance fog wall
		GameVars.playerShipVariables.fogWall.SetActive(true);

		//Now change titleScreen to false
		GameVars.isTitleScreen = false;
		GameVars.isStartScreen = false;

		//Now enable the controls
		GameVars.controlsLocked = false;

		//Initiate the main questline
		GameVars.InitiateMainQuestLineForPlayer();

		//Reset Start Game Button
		GameVars.startGameButton_isPressed = false;

		// TODO: Crew select disabled for now
		//title_crew_select.SetActive(false);

		//Turn on the ship HUD
		Globals.UI.Show<Dashboard, DashboardViewModel>(new DashboardViewModel());
	}

	// TODO: This flow skips StartMainGame, so does some of the same stuff. Should merge if we can.
	public void GUI_loadGame(Difficulty difficulty) {
		if (GameVars.LoadSavedGame()) {
			GameVars.isLoadedGame = true;
			GameVars.isTitleScreen = false;
			GameVars.isStartScreen = false;

			Globals.UI.Hide<TitleScreen>();

			GameVars.camera_titleScreen.SetActive(false);



			//Turn on the environment fog
			RenderSettings.fog = true;
			//Now turn on the main player controls camera
			GameVars.FPVCamera.SetActive(true);
			//Turn on the player distance fog wall
			GameVars.playerShipVariables.fogWall.SetActive(true);
			//Now enable the controls
			GameVars.controlsLocked = false;
			//Set the player's initial position to the new position
			GameVars.playerShipVariables.lastPlayerShipPosition = GameVars.playerShip.transform.position;
			//Update Ghost Route
			GameVars.LoadSavedGhostRoute();


			//Setup Difficulty Level
			if (difficulty == Difficulty.Normal) GameVars.gameDifficulty_Beginner = false;
			else GameVars.gameDifficulty_Beginner = true;
			GameVars.SetupBeginnerGameDifficulty();

			//Turn on the ship HUD
			Globals.UI.Show<Dashboard, DashboardViewModel>(new DashboardViewModel());

			GameVars.controlsLocked = false;
			//Flag the main GUI scripts to turn on
			GameVars.runningMainGameGUI = true;
		}
	}

	// happens upon clicking the new game button on the title screen
	public void GUI_startNewGame(Difficulty difficulty) {
		GameVars.isTitleScreen = false;
		GameVars.isStartScreen = true;

		Globals.UI.Hide<TitleScreen>();

		GameVars.FillNewGameCrewRosterAvailability();

		if (difficulty == Difficulty.Normal) GameVars.gameDifficulty_Beginner = false;
		else GameVars.gameDifficulty_Beginner = true;
		GameVars.SetupBeginnerGameDifficulty();

		// since we're skipping crew select, force pick the first StartingCrewSize members
		for (var i = 0; i < Ship.StartingCrewSize; i++) {
			GameVars.newGameCrewSelectList[i] = true;
		}

		// TODO: For now, skip straight to starting the game since i turned off crew selection
		GUI_startMainGame();

		// TODO: Turned off crew selection because it's too overwhelming. Needs to be reworked.
		//title_crew_select.SetActive(true);
		//GUI_SetupStartScreenCrewSelection();

	}

	//-----------------------------------------------------
	//THIS IS THE SAVE DATA BUTTON
	public void GUI_saveGame() {
		GameVars.ShowANotificationMessage("Saved Data File 'player_save_game.txt' To: " + Application.persistentDataPath + "/");
		GameVars.SaveUserGameData(false);
	}

	//-----------------------------------------------------
	//THIS IS THE RESTART GAME BUTTON	
	public void GUI_restartGame() {
		GameVars.RestartGame();
	}
}
