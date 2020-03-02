using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewCard : MonoBehaviour
{
	private RectTransform rect;
	private bool isDragging = false;
	private Vector2 startPos;
	private bool overSpot;
	private Vector2 dropPos;

	private void Start() 
	{
		rect = GetComponent<RectTransform>();
		startPos = rect.anchoredPosition;
	}

	private void Update() 
	{
		if (isDragging) 
		{
			ToMousePos();
		}
	}

	public void Drag() 
	{
		Debug.Log("Dragging...");
		isDragging = true;
	}

	public void Drop() 
	{
		Debug.Log("Drop");
		isDragging = false;

		if (overSpot) 
		{
			startPos = dropPos;
		}
		rect.anchoredPosition = startPos;
	}

	private void ToMousePos() 
	{
		rect.anchoredPosition = Input.mousePosition;
	}

	public void OverDropSpot(Vector2 pos) 
	{
		overSpot = true;
		dropPos = pos;
	}

	public void LeaveDropSpot(Vector2 pos) 
	{
		if (pos.Equals(dropPos)) 
		{
			overSpot = false;
		}
	}
}
