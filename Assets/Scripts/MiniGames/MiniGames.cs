using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGames : MonoBehaviour
{
	Scene? Scene;

	private void Awake() {
		Globals.Register(this);
	}

	/// <summary>
	/// True if any minigame is active, whether it's an additive scene or a child game object
	/// </summary>
	public bool IsMiniGameActive { get; private set; }

	/// <summary>
	/// Start a minigame that is in a separate scene which will be additively loaded on top of the current scene.
	/// The scene should have its own Camera since the main camera will be disabled.
	/// Remember to add the scene to BuildSettings
	/// Calling Exit will unload the additive scene.
	/// </summary>
	//public void Enter(string additiveSceneName) {
	//	EnterInternal();

	//	SceneManager.LoadScene(additiveSceneName, LoadSceneMode.Additive);
	//	Scene = SceneManager.GetSceneByName(additiveSceneName);
	//}

	/// <summary>
	/// Start a minigame that is a component on a game object that's a child of this MiniGames parent object.
	/// The child object can act as the origin of the mini-game coordinate space so the mini-game can live anywhere in the world.
	/// The scene should have its own Camera since the main camera will be disabled.
	/// Calling Exit will disable the child game object.
	/// </summary>
	public void Enter(String miniGame){
		EnterInternal();
		Instantiate<GameObject>(Resources.Load<GameObject>(miniGame)).transform.SetParent(transform);
	}

	/// <summary>
	/// End any currently active minigame, whether it's an additive scene or a child game object 
	/// </summary>
	public void Exit() {

		// shut off all minigames
		for (var i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject, .1f);
		}

		StartCoroutine(ExitInternal());
	}

	void EnterInternal() {
		CutsceneMode.Enter();
		IsMiniGameActive = true;

		Globals.GameVars.camera_Mapview.SetActive(false);
		//Globals.GameVars.FPVCamera.SetActive(false);
	}

	IEnumerator ExitInternal() {

		// unload all additive minigame scenes. don't leave cutscene mode until its done to avoid weirdness
		if (Scene.HasValue) {
			yield return SceneManager.UnloadSceneAsync(Scene.Value);
			Scene = null;
		}

		CutsceneMode.Exit();
		IsMiniGameActive = false;

		Globals.GameVars.camera_Mapview.SetActive(true);
		Globals.GameVars.FPVCamera.SetActive(true);

	}
}
