using UnityEngine;
using System.Linq;

public class Trade
{
	GameVars GameVars;

	public Trade(GameVars gameVars) {
		GameVars = gameVars;
	}

	// given the amount of this cargo in the current settlment, returns the price of it at this settlement based on scarcity
	public int GetPriceOfResource(string resource, Settlement port) {
		var amount = port.GetCargoByName(resource).initial_amount_kg;

		//Price = 1000 * 1.2^(-.1*x)
		int price = (int)Mathf.Floor(1000 * Mathf.Pow(1.2f, (-.1f * amount)));
		if (price < 1) price = 1;
		return price;

	}

	// finds the average price of the resource across all settlements, so you can tell whether you have a good price or not
	public int GetAvgPriceOfResource(string resourceName, float amount) {
		return Mathf.RoundToInt((float)GameVars.settlement_masterList
			.Select(s => GetPriceOfResource(resourceName, s))
			.Average()
		);
	}

	public bool CheckIfPlayerCanAffordToPayPortTaxes() {
		if (GameVars.playerShipVariables.ship.currency >= GameVars.currentPortTax) return true; else return false;
	}

	public int GetTaxRateOnCurrentShipManifest() {
		//We need to get the total price of all cargo on the ship
		float totalPriceOfGoods = 0f;

		//Loop through each resource in the settlement's cargo and figure out the price of that resource 
		for (int setIndex = 2; setIndex < GameVars.currentSettlement.cargo.Length; setIndex++) {
			float currentResourcePrice = GetPriceOfResource(GameVars.currentSettlement.cargo[setIndex].name, GameVars.currentSettlement);
			//with this price, let's check the ships cargo at the same index position and calculate its worth and add it to the total
			totalPriceOfGoods += (currentResourcePrice * GameVars.playerShipVariables.ship.cargo[setIndex].amount_kg);
			//Debug.Log (MGV.currentSettlement.cargo[setIndex].name + totalPriceOfGoods);


		}

		float taxRateToApply = 0f;
		//Now we need to figure out the tax on the total price of the cargo--which is based on the settlements in/out of network tax
		// total price / 100 * tax rate = amount player owes to settlement for docking
		if (GameVars.isInNetwork)
			taxRateToApply = GameVars.currentSettlement.tax_network;
		else
			taxRateToApply = GameVars.currentSettlement.tax_neutral;

		//Add the players clout modifier. It will be a 0-100 percent reduction of the current tax rate

		float taxReductionAmount = taxRateToApply * (-1 * GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID));
		float newTaxRate = taxRateToApply + taxReductionAmount;
		GameVars.currentPortTax = (int)newTaxRate;

		return (int)((totalPriceOfGoods / 100) * taxRateToApply);
	}

	public int AdjustBuy(int amountToCheck, string resourceName) {
		//This function checks 3 thing(s)
		//	1 Does the city have the resource for the player to buy?
		//	2 Does the player have the currency to buy the resource?
		//	3 Does the player have the cargo hold space to buy the resource?
		float resourceAmount = GameVars.currentSettlement.GetCargoByName(resourceName).amount_kg;
		float price = GetPriceOfResource(resourceName, GameVars.currentSettlement);
		float remainingSpace = GameVars.playerShipVariables.ship.cargo_capicity_kg - GameVars.playerShipVariables.ship.GetTotalCargoAmount();

		if (resourceAmount < amountToCheck) {
			amountToCheck = Mathf.RoundToInt(resourceAmount);
		}
		if ((price * amountToCheck) > GameVars.playerShipVariables.ship.currency) {
			amountToCheck = Mathf.FloorToInt(GameVars.playerShipVariables.ship.currency / price);
		}
		if(remainingSpace < amountToCheck) {
			amountToCheck = Mathf.FloorToInt(remainingSpace);
		}

		return amountToCheck;
	}

	public int AdjustSell(int amountToCheck, string resourceName) {
		//This function checks 1 thing(s):
		//	1 Does the player have cargo to sell?
		float resourceAmount = GameVars.playerShipVariables.ship.GetCargoByName(resourceName).amount_kg;

		if (resourceAmount < amountToCheck) { 
			amountToCheck = Mathf.RoundToInt(resourceAmount);
		}

		return amountToCheck;
	}
}
