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

		//currentShipHealth = maxShipHealth;
		currentShipHealth = Globals.GameVars.playerShipVariables.ship.health;

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
		if (currentShipHealth <= 0) 
		{
			currentShipHealth = 0;
			GetComponent<RitualController>().LoseGame();
		}
		SetHealth(currentShipHealth);
		UpdateHealthBar();
	}

	private void UpdateHealthBar() 
	{
		leftSlider.value = currentShipHealth / 2f;
		rightSlider.value = currentShipHealth / 2f;
	}
}
