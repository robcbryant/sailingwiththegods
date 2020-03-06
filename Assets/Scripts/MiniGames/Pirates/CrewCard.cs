using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewCard : MonoBehaviour
{
	public bool dragable = true;

	private RectTransform rect;
	private bool isDragging = false;
	private Vector2 startPos;
	private bool overSpot;
	private Vector2 dropPos;

	private void Start() 
	{
		rect = GetComponent<RectTransform>();
		startPos = rect.anchoredPosition;
		//Scale();
	}

	private void Update() 
	{
		if (dragable && isDragging) 
		{
			ToMousePos();
		}
	}

	public void Drag() 
	{
		transform.SetAsLastSibling();
		isDragging = true;
	}

	public void Drop() 
	{
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

	private void Scale() 
	{
		CanvasScaler cs = transform.GetComponentInParent<CanvasScaler>();
		Vector2 defaultResolution = cs.referenceResolution;
		Vector2 currentResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		Vector2 scaleFactor = new Vector2(defaultResolution.x / currentResolution.x, defaultResolution.y / currentResolution.y);
		Debug.Log($"Scale: {defaultResolution.x}/{currentResolution.x}:{scaleFactor.x} x {defaultResolution.y}/{currentResolution.y}:{scaleFactor.y}");
		rect.localScale = new Vector3(scaleFactor.x, scaleFactor.y, 1);
	}
}
