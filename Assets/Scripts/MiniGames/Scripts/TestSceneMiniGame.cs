using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Test of a minigame that lives in an additively loaded scene
/// </summary>
public class TestSceneMiniGame : MonoBehaviour
{
	[SerializeField] Camera Camera = null;
	[SerializeField] GameObject TestObj = null;

	private void Start() {
		Debug.Log("Scene Mini game started");
		TestObj.transform.position = Camera.transform.position + new Vector3(0, 0, 2f);
	}

	private void OnDestroy() {
		Debug.Log("Scene Mini game ended");
	}

	private void Update() {
		TestObj.transform.position += Vector3.one * Time.deltaTime;
	}
}
