using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollider : MonoBehaviour
{
	public RitualController rc;

	private void OnCollisionExit(Collision collision) 
	{
		if (collision.gameObject.CompareTag("StormShip")) 
		{
			rc.WinGame();
		}
	}
}
