using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class script_hideLight : MonoBehaviour
{

	public Light lightComp;

	void OnPreRender() {

		lightComp.enabled = false;

	}

	void OnPostRender() {

		lightComp.enabled = true;

	}
}
