using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControllerDav : MonoBehaviour
{
	public GameObject menuUI;
	public void PlayPauScene() {
		//SceneManager.LoadScene("WisdomSongScene");
		StartCoroutine(LoadSong("Petteia"));
	}

	public void PlayDaveScene() {
		//SceneManager.LoadScene("WarSongScene");
		StartCoroutine(LoadSong("Ur"));
	}

	public void PlayMatScene() {
		//SceneManager.LoadScene("SorrowSongScene");
		StartCoroutine(LoadSong("SongCompMainMenu"));
	}

	public void PlayMyloScene() {
		//SceneManager.LoadScene("PartySongScene");
		StartCoroutine(LoadSong("PartySongScene"));
	}

	public void MainMenuScene() {
		//SceneManager.LoadScene("SongCompMainMenu");
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		var scene = SceneManager.GetSceneByName("MiniGameMainMenu");
		SceneManager.SetActiveScene(scene);
	}

	IEnumerator LoadSong(string sceneName) {
		//SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		var scene = SceneManager.GetSceneByName(sceneName);
		SceneManager.SetActiveScene(scene);
	}
}
