using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDropZone : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision) 
	{
		if (collision.CompareTag("CrewCard")) 
		{
			CrewCard cc = collision.GetComponent<CrewCard>();
			cc.OverDropSpot(GetComponent<RectTransform>().anchoredPosition);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("CrewCard")) {
			CrewCard cc = collision.GetComponent<CrewCard>();
			cc.LeaveDropSpot(GetComponent<RectTransform>().anchoredPosition);
		}
	}
}
