using UnityEngine;
using System.Collections;

public class script_cameraHideFog : MonoBehaviour
{
	public static bool FogActive => !Globals.MiniGames.IsMiniGameSceneActive;

	[SerializeField] bool enableFog = true;
	public bool revertFogState = false;


	void OnPreRender() {
		revertFogState = RenderSettings.fog;
		RenderSettings.fog = enableFog && FogActive;
	}

	void OnPostRender() {
		RenderSettings.fog = revertFogState;
	}

}
