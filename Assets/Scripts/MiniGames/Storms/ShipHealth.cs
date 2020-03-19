using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHealth : MonoBehaviour
{
	public Slider leftSlider;
	public Slider rightSlider;

	private float maxShipHealth;
	private float currentShipHealth;

	private void Start() 
	{
		//for now, we set this to a constant because it's constant everywhere else
		//eventually ship will have its own max health variable and we'll pull from that
		maxShipHealth = 100f;

		leftSlider.maxValue = maxShipHealth / 2f;
		rightSlider.maxValue = maxShipHealth / 2f;

		currentShipHealth = maxShipHealth;
		//currentShipHealth = Globals.GameVars.playerShipVariables.ship.health;

		UpdateHealthBar();
	}

	//using this to avoid typing the whole long this every time
	private void SetHealth(float h) 
	{
		Globals.GameVars.playerShipVariables.ship.health = h;
	}

	public void TakeDamage(float damage) 
	{
		currentShipHealth -= damage;
		if (damage <= 0) 
		{
			damage = 0;
			LoseGame();
		}
		//SetHealth(currentShipHealth);
		UpdateHealthBar();
	}

	private void LoseGame() 
	{

	}

	private void UpdateHealthBar() 
	{
		leftSlider.value = currentShipHealth / 2f;
		rightSlider.value = currentShipHealth / 2f;
	}

	private void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.CompareTag("MGrock")) 
		{
			StormHazard s = collision.gameObject.GetComponent<StormHazard>();
			TakeDamage(s.damage);
		}
	}

}
