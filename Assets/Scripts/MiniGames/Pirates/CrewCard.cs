using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewCard : MonoBehaviour
{
	private const string ResourcePath = "crew_portraits";
	private const string DefaultPortrait = "crew_portraits/phoenician_sailor";

	public bool dragable = true;
	public TMPro.TextMeshProUGUI nameText;
	public TMPro.TextMeshProUGUI powerText;
	public Image crewImage;

	private RectTransform rect;
	private bool isDragging = false;
	private Vector2 startPos;
	private bool overSpot;
	private Vector2 dropPos;
	private CrewMember crew;

	private int power;

	private void Start() 
	{
		rect = GetComponent<RectTransform>();
		startPos = rect.anchoredPosition;
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

	public void SetCrew(CrewMember c) 
	{
		crew = c;
		SetPower();
		ShowCrewData();
	}

	private void SetPower() 
	{
		//Will eventually do some fun calculations in here to get the actual number
		power = crew.clout;
	}

	private void ShowCrewData() 
	{
		nameText.text = crew.name;
		powerText.text = power.ToString();
		crewImage.sprite = Resources.Load<Sprite>(ResourcePath + "/" + crew.ID) ?? Resources.Load<Sprite>(DefaultPortrait);
	}
}
