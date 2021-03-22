using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// used for tavern minigame menu
public class TavernaController : MonoBehaviour
{
	public GameObject menuUI;
	public void StartPetteia() {
		StartCoroutine(LoadTavernaGame("Petteia"));
	}

	public void StartUr() {
		StartCoroutine(LoadTavernaGame("Ur"));
	}

	public void StartSong() {
		StartCoroutine(LoadTavernaGame("SongCompMainMenu"));
	}

	public void StartTavernaConvo() {
		LeaveTavernaGame();
		Globals.UI.Show<DialogScreen>().StartDialog("Start_Taverna", "taverna");
	}

	// can't use static methods in unityevent inspector
	public void TavernaMenu() {
		BackToTavernaMenu();
	}

	public static void BackToTavernaMenu() {
		// TODO: This needs refactoring. This script is sometimes added on minigames too and used to unload the game, but we're disabling it in the original scene and need to turn it back on...
		// we can find the real one by looking for the one with a canvas above it
		TavernaController controller = GameObject.FindObjectsOfType<TavernaController>().FirstOrDefault(d => d.GetComponentInParent<Canvas>() != null);
		if(controller != null) {
			controller.GetComponentInParent<Canvas>().enabled = true;
			controller.StartCoroutine(UnloadTavernaGame());
		}
	}

	static IEnumerator UnloadTavernaGame() {
		yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		Scene scene = SceneManager.GetSceneByName("MiniGameMainMenu");
		SceneManager.SetActiveScene(scene);

	}

	// can just call minigames.exit because MiniGames system kept track of minigamemainmenu and will unload it for us
	public void LeaveTavernaGame() {
		Globals.MiniGames.Exit();
	}

	// load individual minigame scenes on top of the main menu, leaving main menu open
	IEnumerator LoadTavernaGame(string sceneName) {
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		Scene scene = SceneManager.GetSceneByName(sceneName);
		SceneManager.SetActiveScene(scene);

		GetComponentInParent<Canvas>().enabled = false;
	}
}
