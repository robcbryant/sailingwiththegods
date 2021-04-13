using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewCard : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] ButtonView infoButton;
#pragma warning restore 0649

	public bool draggable = true;
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI powerText;
	public Image crewImage;

	private RectTransform rect;
	private bool isDragging = false;
	private Vector2 startPos;
	private bool overSpot;
	private Vector2 dropPos;
	private CrewMember crew;

	private int power;
	private bool overStart = true;
	private RandomSlotPopulator rsp;
	private int cardIndex;

	private void Start() 
	{
		rect = GetComponent<RectTransform>();
		startPos = rect.position;
	}

	private void Update() 
	{
		if (draggable && isDragging) 
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
		//The origin crew parent has a mask on it for the scrolling crew list, so if you dragged the card it'd be invisible
		//That's why we move it to a different parent, so it's not affected by the mask
		//And if you drag it back to an origin slot, it goes back to that parent so it *will* properly be masked
		//UI render order is controlled by the order, with the highest in the inspector rendering first and therefore behind everything else
		//We want the card to display above every other card while you're dragging it, so we put it as the last child
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
				//Need to set the parent here because of the mask and the scrolling
				transform.SetParent(rsp.crewParentInOrigin);
			}
		}
		rect.position = startPos;
	}

	private void ToMousePos() 
	{
		//Use .position here instead of .anchoredPosition or .localPosition so it works regardless of parents and anchors and such
		rect.position = Input.mousePosition;
	}

	public void OverDropSpot(Vector2 pos, bool isStart) 
	{
		overSpot = true;
		dropPos = pos;
		overStart = isStart;
	}

	public void LeaveDropSpot(Vector2 pos) 
	{
		//This check is if the card is hovering over two spots at once or goes immediately from one to another
		//Without it, you just can't drop cards at all
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
		//Setting power for the beginning of the game
		power = crew.clout;
		powerText.text = power.ToString();
	}

	public void UpdatePower(int tempPower) {
		power = tempPower;
		powerText.text = power.ToString();
		powerText.color = Color.red;
	}

	private void ShowCrewData() 
	{
		nameText.text = crew.name;
		powerText.text = power.ToString();
		crewImage.sprite = crew.PortraitSprite();
	}

	public void Bind() 
	{
		infoButton?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () =>
			Globals.UI.Show<InfoScreen, InfoScreenModel>(new InfoScreenModel {
				Icon = crewImage.sprite,
				Title = crew.name,
				Subtitle = Globals.GameVars.GetJobClassEquivalency(crew.typeOfCrew),
				Message = crew.backgroundInfo
			})
		}));
	}

	public void UpdateScroll(float scrollAmount) {
		startPos += Vector2.up * scrollAmount;
	}

	public int Power {
		get {
			return power;
		}
		set {
			power = value;
		}
	}

	public int CardIndex {
		get {
			return cardIndex;
		}
		set {
			cardIndex = value;
		}
	}

#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Utils.drawString(cardIndex.ToString(), transform.position, Color.red);
	}
#endif
}
