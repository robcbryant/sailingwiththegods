using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundEffects : MonoBehaviour
{
	//THIS CLASS IS FOR EXECUTING THE SOUND OF THE UI

	public void CoinPurseSound() 
	{
		FindObjectOfType<AudioManager>().PlaySound("Coin Purse");
	}

	public void TradeSwapSound() 
	{
		FindObjectOfType<AudioManager>().PlaySound("Trade Swap");
	}

	public void ShipRepairSound()
	{
		FindObjectOfType<AudioManager>().PlaySound("Ship Repair");
	}
}
