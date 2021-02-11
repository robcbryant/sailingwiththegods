using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// used for the tavern singing minigaame. TODO: Rename
public class mainMenuController : MonoBehaviour
{
    public void PlayWisdomSong()
    {
        StartCoroutine(LoadSong("WisdomSongScene"));
    }

    public void PlayMilitarySong()
    {
        StartCoroutine(LoadSong("WarSongScene"));
    }

    public void PlayMournfulSong()
    {
        StartCoroutine(LoadSong("SorrowSongScene"));
    }

    public void PlayPartySong()
    {
        StartCoroutine(LoadSong("PartySongScene"));
    }

	public void MainMenuScene()
    {
		StartCoroutine(UnloadSong());
	}

	public void BackToTavern() {
		MainMenuControllerDav.BackToMainMenu();
	}

	// set the menu active before unloading the song scene since that will cancel the coroutine
	// TODO: Needs a refactor so that the context doesn't get lost on unloading the song (same issue with tavern menu)
	IEnumerator UnloadSong() {
		var songScene = SceneManager.GetActiveScene();
		var songMenuScene = SceneManager.GetSceneByName("SongCompMainMenu");
		SceneManager.SetActiveScene(songMenuScene);
		yield return SceneManager.UnloadSceneAsync(songScene);
	}

	IEnumerator LoadSong(string sceneName) {
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}
