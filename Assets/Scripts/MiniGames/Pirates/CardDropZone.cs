using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDropZone : MonoBehaviour
{
	public bool startingPoint;
	private bool occupied;
	public int dropIndex;

	private bool allowDropping = true;
	private RectTransform rect;

	private void Start() {
		rect = GetComponent<RectTransform>();
	}

	private void OnTriggerEnter2D(Collider2D collision) 
	{
		if (collision.CompareTag("CrewCard") && !occupied && allowDropping) 
		{
			CrewCard cc = collision.GetComponent<CrewCard>();
			SetOccupied(true);
			cc.cardIndex = this.dropIndex;
			//dropIndex++;
			cc.OverDropSpot(rect.position, startingPoint);
			Debug.Log($"Card is over a spot {rect.position.x} {rect.position.y}");
		}
		//dropIndex++;
	}

	private void OnTriggerExit2D(Collider2D collision) 
	{
		if (collision.CompareTag("CrewCard")) 
		{
			CrewCard cc = collision.GetComponent<CrewCard>();
			SetOccupied(false);
			cc.LeaveDropSpot(rect.position);
			//dropIndex++; //<-- changes the dropIndex value one time ONLY
		}
		//dropIndex++;
	}

	public void SetOccupied(bool occupy) 
	{
		occupied = occupy;
		//dropIndex++;
	}

	public void ToggleDropping(bool drop) 
	{
		allowDropping = drop;
		//dropIndex++;
	}
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Utils.drawString(dropIndex.ToString(), transform.position);
	}
#endif
}
