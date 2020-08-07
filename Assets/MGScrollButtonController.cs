using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MGScrollButtonController : MonoBehaviour
{
	public float scrollDistance = 250;
	public RectTransform[] scrollObjects;
	public GameObject originCardHolder;
	public RandomSlotPopulator rsp;
	public Button up;
	public Button down;

	private int totalRows;
	private int currentRow = 0;

	private void Start() 
	{
		//-1 here so it goes from 0 to n-1 instead of 1 to n
		totalRows = Mathf.CeilToInt((Globals.GameVars.playerShipVariables.ship.crew * 1.0f) / rsp.CrewPerRow) - 1;
		CheckButtons();
	}

	public void MoveDown() 
	{
		if (currentRow < totalRows) 
		{
			MoveArrayObjects(scrollObjects, new Vector2(0, scrollDistance));
			UpdateCards(scrollDistance);
			currentRow++;
			rsp.ActivateCrewRow(currentRow);
		}
	}

	public void MoveUp() 
	{
		if (currentRow > 0) 
		{
			MoveArrayObjects(scrollObjects, new Vector2(0, -scrollDistance));
			UpdateCards(-scrollDistance);
			currentRow--;
			rsp.ActivateCrewRow(currentRow);
		}
	}

	public void UpdateCards(float scroll) {
		CrewCard[] cards = originCardHolder.GetComponentsInChildren<CrewCard>();
		foreach (CrewCard c in cards) {
			c.UpdateScroll(scroll);
		}
	}

	public void CheckButtons() 
	{
		if (currentRow == totalRows) 
		{
			down.interactable = false;
		}
		else 
		{
			down.interactable = true;
		}

		if (currentRow == 0) 
		{
			up.interactable = false;
		}
		else 
		{
			up.interactable = true;
		}
	}

	private void MoveArrayObjects(RectTransform[] rt, Vector2 moveBy) 
	{
		foreach (RectTransform r in rt) 
		{
			r.anchoredPosition += moveBy;
		}
	}
}