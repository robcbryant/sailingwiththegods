using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ritual
{
	public bool HasSeer { get; set; }
	public string RitualText { get; set; }
	public float SuccessChance { get; set; }
	public int CloutGain { get; set; }
	public int CloutLoss { get; set; }
	public int[] ResourceTypes { get; set; }
	public int[] ResourceAmounts { get; set; }

	public Ritual(bool seer, string text, float success, int gain, int loss, int[] resources, int[] resourceQtys) 
	{
		HasSeer = seer;
		RitualText = text;
		SuccessChance = success;
		CloutGain = gain;
		CloutLoss = loss;
		ResourceTypes = resources;
		ResourceAmounts = resourceQtys;
	}

	public override string ToString() 
	{
		string shortened = RitualText.Substring(0, 25) + "...";

		return $"Seer: {HasSeer} | Text: {shortened} | Success: {SuccessChance} | Clout Gain: {CloutGain} | Clout Loss {CloutLoss} | Requires {ResourceTypes.Length} types of resource";
	}
}
