using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormHazard : MonoBehaviour
{
	[HideInInspector] public ShipHealth health;

	public float damage;

	private void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.CompareTag("StormShip")) 
		{
			health.TakeDamage(damage);
		}
	}
}
