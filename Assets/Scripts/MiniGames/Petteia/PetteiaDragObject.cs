using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class PetteiaDragObject : MonoBehaviour

{
	GameObject g;
	public Camera c;
	private Vector3 screenPoint;
	//private Vector3 offset;
	void Start() {
		g = GameObject.FindGameObjectWithTag("MainCamera");
		c = g.GetComponent<Camera>();
	}
	void OnMouseDown() {
		screenPoint = c.WorldToScreenPoint(gameObject.transform.position);
		//offset = gameObject.transform.position - c.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}

	void OnMouseDrag() {
		Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 cursorPosition = c.ScreenToWorldPoint(cursorPoint);
		cursorPosition.y = 1;
		transform.position = cursorPosition;
	}


}