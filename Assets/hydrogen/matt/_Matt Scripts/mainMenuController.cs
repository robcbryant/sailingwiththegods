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

	IEnumerator UnloadSong() {
		yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		var scene = SceneManager.GetSceneByName("SongCompMainMenu");
		SceneManager.SetActiveScene(scene);
	}

	IEnumerator LoadSong(string sceneName) {
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}
