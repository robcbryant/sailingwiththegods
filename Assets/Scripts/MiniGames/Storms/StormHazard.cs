using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormHazard : MonoBehaviour
{
	public float damage;

	private void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.CompareTag("StormShip")) 
		{
			collision.gameObject.GetComponentInParent<ShipHealth>().TakeDamage(damage);
		}
	}
}
