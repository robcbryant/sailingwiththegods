using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDropZone : MonoBehaviour
{
	public bool startingPoint;
	private bool occupied;

	private void OnTriggerEnter2D(Collider2D collision) 
	{
		if (collision.CompareTag("CrewCard") && !occupied) 
		{
			CrewCard cc = collision.GetComponent<CrewCard>();
			SetOccupied(true);
			cc.OverDropSpot(GetComponent<RectTransform>().position, startingPoint);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) 
	{
		if (collision.CompareTag("CrewCard")) 
		{
			CrewCard cc = collision.GetComponent<CrewCard>();
			SetOccupied(false);
			cc.LeaveDropSpot(GetComponent<RectTransform>().position);
		}
	}

	public void SetOccupied(bool occupy) 
	{
		occupied = occupy;
	}
}
