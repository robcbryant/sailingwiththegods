// Mylo Gonzalez

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

public class YarnTavern : MonoBehaviour
{

	private DialogScreen ds;

	void Awake() 
	{
		ds = GetComponent<DialogScreen>();
	}

	[YarnCommand("displayKnownSettlements")]
	public void GenerateKnownSettlementUI() 
	{
		Globals.UI.Show<TavernView, TavernViewModel>(new TavernViewModel(ds));
		Debug.Log("POPPING KNOWN SETLLEMTNS");
	}

	[YarnCommand("randomGuide")]
	public void GenerateGuideDialogue() 
	{
		List<DialogText> guideText = Globals.GameVars.guideDialogText;

		int i = Random.Range(1, guideText.Count);

		if(guideText[i].TextQA[0].Equals("")) 
		{
			guideText[i].TextQA = guideText[1].TextQA;
			Debug.Log("WAS EMPT E");

		}
		if (guideText[i].TextQA[1].Equals("")) {
			guideText[i].TextQA = guideText[1].TextQA;
			Debug.Log("WAS EMPTY");
		}

		Debug.Log("TEXT: " + guideText[i].Text);
		Debug.Log("TEXT1: " + guideText[i].TextQA[0]);
		Debug.Log("TEXT2: " + guideText[i].TextQA[1]);


		ds.Storage.SetValue("$flavor_text1", guideText[i].CityType); // Wrongfully added in CityType.
		ds.Storage.SetValue("$flavor_text2", guideText[i].TextQA[0]);
		ds.Storage.SetValue("$flavor_text3", guideText[i].TextQA[1]);

	}

	[YarnCommand("setbeacon")]
	public void SetSettlementWaypoint()
	{
		int cityID = (int)ds.Storage.GetValue("$known_city_ID").AsNumber;
		Vector3 location = Vector3.zero;
		for (int x = 0; x < Globals.GameVars.settlement_masterList_parent.transform.childCount; x++)
			if (Globals.GameVars.settlement_masterList_parent.transform.GetChild(x).GetComponent<script_settlement_functions>().thisSettlement.settlementID == cityID)
				location = Globals.GameVars.settlement_masterList_parent.transform.GetChild(x).position;
		Globals.GameVars.ActivateNavigatorBeacon(Globals.GameVars.navigatorBeacon, location);
		Globals.GameVars.playerShipVariables.ship.currentNavigatorTarget = cityID;
		//Globals.GameVars.ShowANotificationMessage("You hired a navigator to " + City.name + " for " + CostToHire + " drachma.");
		
	}

	[YarnCommand("randomFoodDialogue")]
	public void GenerateRandomFoodDialogue() {
		// Begin pulling random food item.
		List<FoodText> foodList = Globals.GameVars.foodDialogueText;

		int i = Random.Range(1, foodList.Count);

		if (foodList[i].FoodCost == 0) {
			foodList[i].FoodCost = (int)ds.Storage.GetValue("$dracma_cost").AsNumber;
		}

		ds.Storage.SetValue("$food_dialogue_item", foodList[i].Item);
		ds.Storage.SetValue("$food_dialogue_quote", foodList[i].GetQuote);
	}

	[YarnCommand("randomWine")]
	public void GenerateRandomWineInfo() {
		// Begin pulling random food item.
		List<FoodText> foodList = Globals.GameVars.wineInfoText;

		int i = Random.Range(1, foodList.Count);

		if (foodList[i].FoodCost == 0) {
			foodList[i].FoodCost = (int)ds.Storage.GetValue("$dracma_cost").AsNumber;
		}

		ds.Storage.SetValue("$drachma_cost", foodList[i].FoodCost);
		ds.Storage.SetValue("$random_wine", foodList[i].Item);
		ds.Storage.SetValue("$wine_quote", foodList[i].GetQuote);
	}


	[YarnCommand("randomFood")]
	public void GenerateRandomFoodItem() 
	{
		// Begin pulling random food item.
		List<FoodText> foodList =  Globals.GameVars.foodItemText;

		int i = Random.Range(1, foodList.Count);
		Debug.Log("COUNT THE FOOD " + foodList.Count);

		if (foodList[i].FoodCost == 0) {
			foodList[i].FoodCost = (int)ds.Storage.GetValue("$generated_cost").AsNumber;
			Debug.Log("Cost of this item: " + foodList[i].FoodCost + "while i is " + i + " Item should be " + foodList[i].Item);
		}

		ds.Storage.SetValue("$drachma_cost", foodList[i].FoodCost);
		ds.Storage.SetValue("$random_food", foodList[i].Item);
		ds.Storage.SetValue("$food_quote", foodList[i].GetQuote);
	}

	[YarnCommand("randomQA")]
	public void GenerateRandomQAText(string input) 
	{
		// Get the city we know of
		string e = ds.Storage.GetValue("$known_city").AsString;
		Debug.Log("WE ARE ASKING ABOUT " + e);
		List<DialogText> matchingType = new List<DialogText>();

		// Obtain the known settlements we can talk about! (NOTE: will change to display known settlements and we'll search our info based on selection)
		Settlement[] settlementList = Globals.GameVars.settlement_masterList;
		Settlement targetSettlement = settlementList[0]; // Simple Assignment to ease compile errors.

		// Finding the currentSettlement
		foreach (Settlement a in settlementList) 
		{
			if (a.name == e)
				targetSettlement = a;
		}

		switch (input) 
		{
			case "network":
				e = Globals.GameVars.networkDialogText.Exists(x => x.CityType == e) ? e : "ALLOTHERS";
				matchingType = Globals.GameVars.networkDialogText.FindAll(x => x.CityType == e);
				break;
			case "pirate":
				e = Globals.GameVars.pirateDialogText.Exists(x => x.CityType == e) ? e : "ALLOTHERS";
				matchingType = Globals.GameVars.pirateDialogText.FindAll(x => x.CityType == e);
				break;
			case "myth":
				if (!e.Equals(ds.Storage.GetValue("$current_myth_city").AsString)) 
				{
					ds.Storage.SetValue("$current_myth_count", 0);
					ds.Storage.SetValue("$current_myth_city", e);
				}
				else
					ds.Storage.SetValue("$current_myth_count", ds.Storage.GetValue("$current_myth_count").AsNumber + 1);
				e = Globals.GameVars.mythDialogText.Exists(x => x.CityType == e) ? e : "ALLOTHERS";
				matchingType = Globals.GameVars.mythDialogText.FindAll(x => x.CityType == e);
				break;
			default:
				Debug.Log("Nice going, doofus. We will now crash because one of your nodes is probably mispelling your \"input\" and matchingType is empty. Good job.");
				break;
		}

		int i = Random.Range(0, matchingType.Count);

		ds.Storage.SetValue("$question", matchingType[i].TextQA[0]);
		ds.Storage.SetValue("$check_myth", matchingType.Count > ds.Storage.GetValue("$current_myth_count").AsNumber);

		if (e != "ALLOTHERS") 
		{
			if(e.Equals(ds.Storage.GetValue("$current_myth_city").AsString)) 
			{
				if(ds.Storage.GetValue("$check_myth").AsBool) 
				{
					// Clean this up for readability.
					ds.Storage.SetValue("$response", matchingType[(int)ds.Storage.GetValue("$current_myth_count").AsNumber].TextQA[1]);
					Globals.GameVars.AddToCaptainsLog("Myth of " + e + ":\n" + ds.Storage.GetValue("$response").AsString);
				}
				else
					ds.Storage.SetValue("$response", "There is nothing more for me to say!");
			}
			else
				ds.Storage.SetValue("$response", matchingType[i].TextQA[1]);
		}
		else
			ds.Storage.SetValue("$response", targetSettlement.description);

		// For wanting to learn more. May consider changing conditional to check if input == myth instead
		if (input == "myth" && matchingType[i].TextQA.Length > 2)
			ds.Storage.SetValue("$response2", matchingType[i].TextQA[2]);

		//Special condition for home town?

	}

}
