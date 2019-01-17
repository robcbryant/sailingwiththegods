using UnityEngine;
using System.Collections;

public class script_cameraHideFog : MonoBehaviour {

	[SerializeField] bool enableFog = true;
	public bool revertFogState = false;
	
	
	void OnPreRender(){
		revertFogState = RenderSettings.fog;
		RenderSettings.fog = enableFog;
	}
	
	void OnPostRender(){
		RenderSettings.fog = revertFogState;
	}
	
}
