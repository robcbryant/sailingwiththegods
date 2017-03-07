using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class script_hideLight : MonoBehaviour {

	public Light light;
	
	void OnPreRender()
	{

			light.enabled = false;

	}
	
	void OnPostRender()
	{

			light.enabled = true;

	}
}
