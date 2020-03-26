using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ritual
{
	private bool hasSeer;
	private string ritualText;
	private float successChance;
	private int cloutGain;
	private int cloutLoss;
	private int[] resourceTypes;
	private int[] resourceAmounts;

	public Ritual(bool seer, string text, float success, int gain, int loss, int[] resources, int[] resourceQtys) 
	{
		hasSeer = seer;
		ritualText = text;
		successChance = success;
		cloutGain = gain;
		cloutLoss = loss;
		resourceTypes = resources;
		resourceAmounts = resourceQtys;
	}

	public override string ToString() {
		string shortened = ritualText.Substring(0, 25) + "...";

		return $"Seer: {hasSeer} | Text: {shortened} | Success: {successChance} | Clout Gain: {cloutGain} | Clout Loss {cloutLoss} | Requires {resourceTypes.Length} types of resource";
	}
}
