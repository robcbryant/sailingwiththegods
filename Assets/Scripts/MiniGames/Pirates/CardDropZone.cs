using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDropZone : MonoBehaviour
{
	private bool occupied;

	private void OnTriggerEnter2D(Collider2D collision) 
	{
		if (collision.CompareTag("CrewCard") && !occupied) 
		{
			CrewCard cc = collision.GetComponent<CrewCard>();
			occupied = true;
			cc.OverDropSpot(GetComponent<RectTransform>().position);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) 
	{
		if (collision.CompareTag("CrewCard")) 
		{
			CrewCard cc = collision.GetComponent<CrewCard>();
			occupied = false;
			cc.LeaveDropSpot(GetComponent<RectTransform>().anchoredPosition);
		}
	}
}
