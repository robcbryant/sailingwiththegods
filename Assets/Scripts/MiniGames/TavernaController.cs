using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// used for tavern minigame menu
public class TavernaController : MonoBehaviour
{
	public AudioListener tavernaListener;
	public Light tavernaLight;

	private Scene thisScene;

	private void Awake() {
		thisScene = SceneManager.GetSceneByName("TavernaMenu");
	}

	public void StartPetteia() {
		StartCoroutine(LoadTavernaGame("Petteia"));
	}

	public void StartUr() {
		StartCoroutine(LoadTavernaGame("Ur"));
	}

	public void StartSong() {
		StartCoroutine(LoadTavernaGame("SongCompMainMenu"));
	}
	
	public void StartKottaboas() {
		StartCoroutine(LoadTavernaGame("Kottaboas_Game"));
	}

	public void StartTavernaConvo() {
		LeaveTavernaGame();
		Globals.UI.Show<DialogScreen>().StartDialog("Start_Taverna", "taverna");
	}

	private void Update() {
		if (SceneManager.GetActiveScene().Equals(thisScene) && !tavernaListener.enabled) 
		{
			ToggleTavernaObjects(true);
		}
	}

	// can't use static methods in unityevent inspector
	public void TavernaMenu() {
		BackToTavernaMenu();
	}

	public static void BackToTavernaMenu() {

		TavernaController controller = GetTavernaControllerInstance();
		
		if(controller != null) {
			controller.GetComponentInParent<Canvas>().enabled = true;
			controller.StartCoroutine(UnloadTavernaGame());
		}
	}

	public static void ReloadTavernaGame(string sceneName) 
	{
		TavernaController controller = GetTavernaControllerInstance();

		if (controller != null) {
			Debug.Log("Found controller, starting coroutine");
			controller.StartCoroutine(TavernaReload(sceneName, controller));
		}
		else {
			Debug.Log("Couldn't find the controller");
		}
	}

	// Moved to its own function so it can be used multiple places and then when we refactor it we only change it the once
	// TODO: This needs refactoring. This script is sometimes added on minigames too and used to unload the game, but we're disabling it in the original scene and need to turn it back on...
	// we can find the real one by looking for the one with a canvas above it
	private static TavernaController GetTavernaControllerInstance() {
		return GameObject.FindObjectsOfType<TavernaController>().FirstOrDefault(d => d.GetComponentInParent<Canvas>() != null);
	}

	private static IEnumerator TavernaReload(string sceneName, TavernaController c) 
	{
		yield return SceneManager.UnloadSceneAsync(sceneName);
		SceneManager.SetActiveScene(SceneManager.GetSceneByName("TavernaMenu"));
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		yield return null;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
		c.ToggleTavernaObjects(false);
	}

	static IEnumerator UnloadTavernaGame() {
		yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		Scene scene = SceneManager.GetSceneByName("TavernaMenu");
		SceneManager.SetActiveScene(scene);
	}

	// can just call minigames.exit because MiniGames system kept track of minigamemainmenu and will unload it for us
	public void LeaveTavernaGame() {
		Globals.MiniGames.Exit();
	}

	// load individual minigame scenes on top of the main menu, leaving main menu open
	IEnumerator LoadTavernaGame(string sceneName) {
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		ToggleTavernaObjects(false);
		Scene scene = SceneManager.GetSceneByName(sceneName);
		SceneManager.SetActiveScene(scene);

		GetComponentInParent<Canvas>().enabled = false;
	}

	private void ToggleTavernaObjects(bool toggle) {
		Debug.Log($"Toggling {name}: {toggle}");
		tavernaListener.enabled = toggle;
		tavernaLight.enabled = toggle;
	}

	
}
