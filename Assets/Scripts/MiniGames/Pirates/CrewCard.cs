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

	public int power;
	private bool overStart = true;
	private RandomSlotPopulator rsp;

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

	public void SetRSP(RandomSlotPopulator r) 
	{
		rsp = r;
	}

	public void Drag() 
	{
		transform.SetParent(rsp.crewParent);
		transform.SetAsLastSibling();
		isDragging = true;
	}

	public void Drop() 
	{
		isDragging = false;

		if (overSpot) 
		{
			startPos = dropPos;
			if (overStart) 
			{
				transform.SetParent(rsp.crewParentInOrigin);
			}
		}
		rect.position = startPos;
	}

	private void ToMousePos() 
	{
		rect.anchoredPosition = Input.mousePosition;
	}

	public void OverDropSpot(Vector2 pos, bool isStart) 
	{
		overSpot = true;
		dropPos = pos;
		overStart = isStart;
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
