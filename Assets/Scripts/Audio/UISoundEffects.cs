using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundEffects : MonoBehaviour
{
	[SerializeField]
	private AudioSource coinPurse;

	[SerializeField]
	private AudioSource tradeSwap;

	[SerializeField]
	private AudioSource shipRepair;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void CoinPurseSound() 
	{
		coinPurse.Play();
	}

	public void TradeSwapSound() 
	{
		tradeSwap.Play();
	}

	public void ShipRepairSound()
	{
		shipRepair.Play();
	}
}
