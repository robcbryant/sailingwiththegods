using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuController : MonoBehaviour
{

    public void PlayWisdomSong()
    {
        //SceneManager.LoadScene("WisdomSongScene");
        StartCoroutine(LoadSong("WisdomSongScene"));
    }

    public void PlayMilitarySong()
    {
        //SceneManager.LoadScene("WarSongScene");
        StartCoroutine(LoadSong("WarSongScene"));
    }

    public void PlayMournfulSong()
    {
        //SceneManager.LoadScene("SorrowSongScene");
        StartCoroutine(LoadSong("SorrowSongScene"));
    }

    public void PlayPartySong()
    {
        //SceneManager.LoadScene("PartySongScene");
        StartCoroutine(LoadSong("PartySongScene"));
    }

	public void MainMenuScene()
    {
        //SceneManager.LoadScene("SongCompMainMenu");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        var scene = SceneManager.GetSceneByName("SongCompMainMenu");
        SceneManager.SetActiveScene(scene);
    }

    IEnumerator LoadSong(string sceneName)
    {
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}
