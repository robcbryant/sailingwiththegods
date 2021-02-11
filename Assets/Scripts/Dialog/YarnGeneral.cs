using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class YarnGeneral : MonoBehaviour
{
	private DialogScreen ds;

	void Awake()
	{
		ds = GetComponent<DialogScreen>();
	}

	#region Yarn Funtions - Start
	[YarnCommand("setconvotitle")]
	public void SetConversationTitle(string title) 
	{
		string text = title.Replace('_', ' ');
		ds.conversationTitle.text = text;
	}

	[YarnCommand("reset")]
	public void ResetConversation() 
	{
		ds.Storage.SetValue("$random_text", "Random text");
		ds.Storage.SetValue("$random_bool", false);
		ds.Storage.SetValue("$convo_title", "Conversation Title");
		ds.Storage.SetValue("$emotion", "neutral");
		ds.Clear();
	}

	[YarnCommand("setpartner")]
	public void SetConversationPartner(string name) 
	{
		foreach (Yarn.Unity.Example.SpriteSwitcher p in ds.convoPartners) {
			if (p.name == name) {
				p.gameObject.SetActive(true);
			}
			else {
				p.gameObject.SetActive(false);
			}
		}
	}

	[YarnCommand("setbg")]
	public void SetBackgroundObject(string name) {
		foreach (Yarn.Unity.Example.SpriteSwitcher p in ds.backgrounds) {
			if (p.name == name) {
				p.gameObject.SetActive(true);
			}
			else {
				p.gameObject.SetActive(false);
			}
		}
	}
	#endregion

	#region Yarn Functions - Random

	//Create a random amount between inputs
	[YarnCommand("randomcost")]
	public void GenerateRandomAmount(string[] inputs) 
	{
		int amount = Random.Range(int.Parse(inputs[0]), int.Parse(inputs[1]));
		Debug.Log("Ran Randomcost. Cost is " + amount);
		ds.Storage.SetValue("$generated_cost", amount);
	}

	[YarnCommand("randomtext")]
	public void GenerateRandomText(string[] inputs) 
	{
		//Get the parameters for the text
		System.Enum.TryParse(inputs[0], out DialogText.Type t);
		DialogText.Emotion e = DialogText.Emotion.neutral;
		if (inputs[1] == "any") {
			e = DialogText.RandomEmotion();
		}
		else {
			System.Enum.TryParse(inputs[1], out e);
		}

		//Gets a list of text that matches just the type and then the type and emotion of the desired random text
		List<DialogText> matchingType = Globals.GameVars.portDialogText.FindAll(x => x.TextType == t);
		List<DialogText> matchingBoth = matchingType.FindAll(x => x.TextEmotion == e);

		if (matchingBoth.Count == 0) {
			Debug.Log($"Nothing found with both type {t.ToString()} and emotion {e.ToString()} ({matchingType.Count} matching just type)");
		}

		int i = Random.Range(0, matchingBoth.Count);

		ds.Storage.SetValue("$random_text", matchingBoth[i].Text);

		ds.Storage.SetValue("$emotion", e.ToString());
	}

	

	[YarnCommand("randombool")]
	public void TrueOrFalse(string threshold) 
	{
		float limit = float.Parse(threshold);
		bool b = Random.Range(0f, 1f) < limit;
		ds.Storage.SetValue("$random_bool", b);
	}
	#endregion

	[YarnCommand("checkafford")]
	public void CheckAffordability(string cost) {
		int itemCost = 0;
		if (cost[0] == '$') {
			itemCost = YarnGeneral.IntFromVariableName(cost, ds.Storage);
		}
		else {
			itemCost = Mathf.CeilToInt(float.Parse(cost));
		}
		ds.Storage.SetValue("$can_afford", Globals.GameVars.playerShipVariables.ship.currency >= itemCost);
	}

	[YarnCommand("roundup")]
	public void RoundToInt(string cost) {
		int itemCost = 0;
		if (cost[0] == '$') {
			itemCost = YarnGeneral.IntFromVariableName(cost, ds.Storage);
		}
		else {
			itemCost = Mathf.CeilToInt(float.Parse(cost));
		}
		ds.Storage.SetValue("$rounded_num", itemCost);
	}


	[YarnCommand("pay")]
	public void PayAmount(string cost) {
		int itemCost = 0;
		if (cost[0] == '$') {
			itemCost = YarnGeneral.IntFromVariableName(cost, ds.Storage);
		}
		else {
			itemCost = Mathf.RoundToInt(float.Parse(cost));
		}

		Globals.GameVars.playerShipVariables.ship.currency -= itemCost;
		ds.UpdateMoney();
	}

	#region Yarn Helpers
	public static int IntFromVariableName(string name, InMemoryVariableStorage storage) 
	{
		return Mathf.CeilToInt(storage.GetValue(name).AsNumber);
	}

	public static float Truncate(float num, int places) 
	{
		int factor = (int)Mathf.Pow(10, places);

		return Mathf.Round(num * factor) / factor;
	}

	public static string FormatList(List<Resource> resources) 
	{
		string formatted = $"{resources[0].amount_kg}kg of {resources[0].name}";
		formatted += resources.Count > 2 ? ", " : " ";
		for (int i = 1; i < resources.Count - 1; i++) {
			formatted += $"{resources[i].amount_kg}kg of {resources[i].name}, ";
		}
		if (resources.Count > 1) {
			formatted += $"and {resources[resources.Count - 1].amount_kg}kg of {resources[resources.Count - 1].name}";
		}

		return formatted;
	}
	#endregion


}
