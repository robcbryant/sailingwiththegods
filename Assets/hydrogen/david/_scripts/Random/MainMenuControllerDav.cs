using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// used for tavern minigame menu
// TODO: Rename
public class MainMenuControllerDav : MonoBehaviour
{
	public GameObject menuUI;
	public void PlayPauScene() {
		StartCoroutine(LoadGame("Petteia"));
	}

	public void PlayDaveScene() {
		StartCoroutine(LoadGame("Ur"));
	}

	public void PlayMatScene() {
		StartCoroutine(LoadGame("SongCompMainMenu"));
	}

	public void PlayMyloScene() {
		Leave();
		Globals.UI.Show<DialogScreen>().StartDialog("Start_Taverna");
	}

	// can't use static methods in unityevent inspector
	public void MainMenuScene() {
		BackToMainMenu();
	}

	public static void BackToMainMenu() {
		// TODO: This needs refactoring. This script is sometimes added on minigames too and used to unload the game, but we're disabling it in the original scene and need to turn it back on...
		// we can find the real one by looking for the one with a canvas above it
		var dav = GameObject.FindObjectsOfType<MainMenuControllerDav>().FirstOrDefault(d => d.GetComponentInParent<Canvas>() != null);
		if(dav != null) {
			dav.GetComponentInParent<Canvas>().enabled = true;
			dav.StartCoroutine(UnloadTavernGame());
		}
	}

	static IEnumerator UnloadTavernGame() {
		yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		var scene = SceneManager.GetSceneByName("MiniGameMainMenu");
		SceneManager.SetActiveScene(scene);
	}

	// can just call minigames.exit because MiniGames system kept track of minigamemainmenu and will unload it for us
	public void Leave() {
		Globals.MiniGames.Exit();
	}

	// load individual minigame scenes on top of the main menu, leaving main menu open
	IEnumerator LoadGame(string sceneName) {
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		var scene = SceneManager.GetSceneByName(sceneName);
		SceneManager.SetActiveScene(scene);

		GetComponentInParent<Canvas>().enabled = false;
	}
}
