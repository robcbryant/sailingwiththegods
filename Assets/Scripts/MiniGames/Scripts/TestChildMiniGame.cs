using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Test of a minigame that lives in a disabled child object underneath the MiniGames obj in the main scene
/// </summary>
public class TestChildMiniGame : MonoBehaviour
{
	[SerializeField] Camera Camera = null;
	[SerializeField] GameObject TestObj = null;

	private void OnEnable() {
		Debug.Log("Child Mini game started");
		// you can use this.transform.position to get the origin of the minigame (ie. you can position the entire root minigame object somewhere on the map)
		TestObj.transform.position = Camera.transform.position + new Vector3(0, 0, 2f);
	}

	private void OnDisable() {
		Debug.Log("Child Mini game ended");
	}

	private void Update() {
		TestObj.transform.position += Vector3.one * Time.deltaTime;
	}
}
