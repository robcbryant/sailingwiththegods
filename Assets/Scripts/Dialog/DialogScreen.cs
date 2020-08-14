using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogScreen : MonoBehaviour
{
	private const string ResourcePath = "dialog_images";

	public script_GUI gui;

	public TextMeshProUGUI moneyText;
	public TextMeshProUGUI conversationTitle;
	public Scrollbar conversationScroll;
	public Transform conversationHolder;
	public Transform choiceHolder;
	public RectTransform choiceGrandParent;
	public DialogChoice choiceObject;
	public DialogPiece dialogObject;
	public Image dialogImage;
	public GameObject dialogSpacer;

	private CustomDialogUI yarnUI;
	private InMemoryVariableStorage storage;
	private DialogueRunner runner;
	private Settlement city;
	private Canvas canvas;
	private List<Resource> owedResources = new List<Resource>();

	private void OnValidate() 
	{
		yarnUI = GetComponent<CustomDialogUI>();
		storage = GetComponent<InMemoryVariableStorage>();
		runner = GetComponent<DialogueRunner>();
		canvas = GetComponentInParent<Canvas>();
	}

	private void OnEnable() 
	{
		UpdateMoney();
	}

	public void AddToDialogText(string speaker, string text, TextAlignmentOptions align) {
		StartCoroutine(DoAddToDialogText(speaker, text, align));
	}

	public void AddImage(string imgName) {
		StartCoroutine(DoAddImage(imgName));
	}
	
	private void SetCity(Settlement s) 
	{
		city = s;
		Debug.Log("Current settlement: " + city.name);
		storage.SetValue("$city_name", new Yarn.Value(city.name));
		storage.SetValue("$city_description", new Yarn.Value(city.description));
	}

	public void StartDialog(Settlement s) {
		SetCity(s);
		Clear();
		StartCoroutine(StartDialog());
	}

	private IEnumerator StartDialog() {
		yield return null;
		yield return null;
		runner.StartDialogue();
	}

	private IEnumerator DoAddToDialogText(string speaker, string text, TextAlignmentOptions align) 
	{
		DialogPiece p = Instantiate(dialogObject);
		p.SetAlignment(align);
		p.SetText(speaker, text);
		yield return null;
		p.transform.SetParent(conversationHolder);
		p.transform.SetSiblingIndex(conversationHolder.childCount - 2);
		p.transform.localScale = Vector3.one;
		yield return null;
		conversationScroll.value = 0;
		yield return null;
		conversationScroll.value = 0;
	}

	public IEnumerator DoAddImage(string imgName) 
	{
		Sprite s = Resources.Load<Sprite>(ResourcePath + "/" + imgName);

		if (s != null) {
			Image i = Instantiate(dialogImage);
			i.sprite = s;
			yield return null;
			i.transform.SetParent(conversationHolder);
			i.transform.SetSiblingIndex(conversationHolder.childCount - 2);
			i.transform.localScale = Vector3.one;
			yield return null;
			conversationScroll.value = 0;
			yield return null;
			conversationScroll.value = 0;
		}
	}

	public void AddContinueOption() 
	{
		ClearOptions();
		if (!yarnUI.EndOfBlock) {
			AddChoice("Continue", yarnUI.MarkLineComplete);
		}
		else {
			StartCoroutine(WaitAndComplete());
		}
	}

	private IEnumerator WaitAndComplete() {
		yield return null;
		yarnUI.MarkLineComplete();
	}

	public void AddChoice(string text, UnityEngine.Events.UnityAction click) 
	{
		DialogChoice c = Instantiate(choiceObject);
		c.transform.SetParent(choiceHolder);
		c.SetText(text, choiceGrandParent);
		c.transform.localScale = Vector3.one;
		c.SetOnClick(click);
	}


	public void Clear() 
	{
		ClearChildren(conversationHolder);
		Instantiate(dialogSpacer).transform.SetParent(conversationHolder);
		ClearChildren(choiceHolder);
	}

	public void ClearOptions() 
	{
		ClearChildren(choiceHolder);
	}

	private void ClearChildren(Transform parent) 
	{
		Transform[] objs = parent.GetComponentsInChildren<Transform>();
		foreach (Transform t in objs) 
		{
			if (t != parent) 
			{
				Destroy(t.gameObject);
			}

		}
	}

	private IEnumerator DeactivateSelf() {
		Clear();
		yield return null;
		gameObject.SetActive(false);
	}

	public void ExitConversation() {
		bool city = storage.GetValue("$entering_city").AsBool;
		Debug.Log($"Exiting the conversation. Entering the city {city}");

		if (city) {
			gui.GUI_EnterPort();
		}
		else {
			gui.GUI_ExitPortNotification();
		}

		StartCoroutine(DeactivateSelf());
	}

	#region Yarn Funtions - Start
	[YarnCommand("setconvotitle")]
	public void SetConversationTitle(string title) {
		string text = title.Replace('_', ' ');
		conversationTitle.text = text;
	}

	[YarnCommand("reset")]
	public void ResetConversation() {
		storage.SetValue("$random_text", "Random text");
		storage.SetValue("$random_bool", false);
		storage.SetValue("$convo_title", "Convertation Title");
		storage.SetValue("$emotion", "neutral");
		storage.SetValue("$jason_connected", false);
		storage.SetValue("$crew_name", "Bob IV");
		Clear();
	}
	#endregion

	#region Yarn Functions - Set Variables
	[YarnCommand("citynetworks")]
	public void NumberOfCityNetworks() {
		storage.SetValue("$city_networks", city.networks.Count);
	}

	[YarnCommand("networkconnections")]
	public void NumberOfConnections() {
		IEnumerable<CrewMember> connected = Globals.GameVars.Network.CrewMembersWithNetwork(city, true);
		int connectedNum = Enumerable.Count(connected);
		storage.SetValue("$connections_number", connectedNum);
	}

	[YarnCommand("cityinfo")]
	public void SetCityInfo() {
		storage.SetValue("$city_name", city.name);
		storage.SetValue("$city_description", city.description);
	}

	[YarnCommand("checkcitytaxes")]
	public void CheckCityTaxes() {

		storage.SetValue("$god_tax", city.godTax);
		storage.SetValue("$god_tax_amount", city.godTaxAmount);
		storage.SetValue("$transit_tax", city.transitTax);
		storage.SetValue("$transit_tax_amount", city.transitTaxPercent);
		storage.SetValue("$foreigner_tax", city.foreignerFee);
		storage.SetValue("$foreigner_tax_amount", city.foreignerFeePercent);
		storage.SetValue("$wealth_tax", CargoValue() >= 1000000);
		storage.SetValue("$wealth_tax_amount", .10f);

		storage.SetValue("$no_taxes", !city.godTax && !city.transitTax && !city.foreignerFee);
		storage.SetValue("$cargo_value", Mathf.CeilToInt(CargoValue()));
	}

	[YarnCommand("connectedcrew")]
	public void ConnectedCrewName() {
		if (storage.GetValue("$connections_number").AsNumber > 0) {
			if (Globals.GameVars.Network.GetCrewMemberNetwork(Globals.GameVars.Jason).Contains(city)) {
				storage.SetValue("$jason_connected", true);
				storage.SetValue("$crew_name_1", "me");
				storage.SetValue("$crew_name_2", "I");
				storage.SetValue("$crew_name_3", "You");
				storage.SetValue("$crew_description", Globals.GameVars.Jason.backgroundInfo);
				storage.SetValue("$crew_home", Globals.GameVars.GetSettlementFromID(Globals.GameVars.Jason.originCity).name);
			}
			else {
				IEnumerable<CrewMember> connected = Globals.GameVars.Network.CrewMembersWithNetwork(city);
				CrewMember crew = connected.RandomElement();
				storage.SetValue("$crew_name_1", crew.name);
				storage.SetValue("$crew_name_2", crew.name);
				storage.SetValue("$crew_name_3", crew.name);
				storage.SetValue("$crew_description", crew.backgroundInfo);
				storage.SetValue("$crew_home", Globals.GameVars.GetSettlementFromID(crew.originCity).name);
			}
		}
		else {
			storage.SetValue("$jason_connected", false);
			storage.SetValue("$crew_name_1", "ERROR");
			storage.SetValue("$crew_name_2", "ERROR");
			storage.SetValue("$crew_name_3", "ERROR");
			storage.SetValue("$crew_description", "ERROR");
			storage.SetValue("$crew_home", "ERROR");
		}
	}

	[YarnCommand("cargovalue")]
	public void StoreCargoValue() {
		storage.SetValue("$cargo_value", Mathf.CeilToInt(CargoValue()));
	}
	#endregion

	#region Yarn Functions - Random
	[YarnCommand("randomtext")]
	public void GenerateRandomText(string[] inputs) 
	{
		System.Enum.TryParse(inputs[0], out DialogText.Type t);
		DialogText.Emotion e = DialogText.Emotion.neutral;
		if (inputs[1] == "any") {
			e = DialogText.RandomEmotion();
		}
		else {
			System.Enum.TryParse(inputs[1], out e);
		}

		List<DialogText> matchingType = Globals.GameVars.portDialogText.FindAll(x => x.TextType == t);
		List<DialogText> matchingBoth = matchingType.FindAll(x => x.TextEmotion == e);

		if (matchingBoth.Count == 0) {
			Debug.Log($"Nothing found with both type {t.ToString()} and emotion {e.ToString()} ({matchingType.Count} matching just type)");
		}

		int i = Random.Range(0, matchingBoth.Count);
		
		storage.SetValue("$random_text", matchingBoth[i].Text);

		storage.SetValue("$emotion", e.ToString());
	}

	[YarnCommand("randombool")]
	public void TrueOrFalse(string threshold) {
		float limit = float.Parse(threshold);
		bool b = Random.Range(0f, 1f) < limit;
		storage.SetValue("$random_bool", b);
	}
	#endregion

	#region Yarn functions - Tax Calculations
	[YarnCommand("calculatetaxes")]
	public void CalculateTaxCharges() {
		float subtotal = 0;
		//TESTING
		float cargo = 1000000;

		if (storage.GetValue("$god_tax").AsBool) {
			//God Tax is a flat number
			subtotal += storage.GetValue("$god_tax_amount").AsNumber;
		}
		if (storage.GetValue("$transit_tax").AsBool) {
			//Transit tax is a percent
			subtotal += storage.GetValue("$transit_tax_amount").AsNumber * cargo * 100;
		}
		if (storage.GetValue("$foreigner_tax").AsBool) {
			//Foreigner tax is a percent
			subtotal += storage.GetValue("$foreigner_tax_amount").AsNumber * cargo * 100;
		}
		if (storage.GetValue("$wealth_tax").AsBool) {
			//Wealth tax is a percent
			subtotal += storage.GetValue("$wealth_tax_amount").AsNumber * cargo * 100;
		}

		cargo = CargoValue();
		storage.SetValue("$tax_subtotal", Mathf.CeilToInt(subtotal));
		storage.SetValue("$cargo_value", cargo);
		storage.SetValue("$ellimenion_percent", .05f);
	}

	[YarnCommand("calculatepercents")]
	public void CalculateIntentPercent() {
		float subtotal = storage.GetValue("$tax_subtotal").AsNumber;
		float cargo = CargoValue();

		float percent = 0.01f;
		storage.SetValue("$water_intent", Mathf.CeilToInt(percent * cargo));

		percent = 0.02f;
		storage.SetValue("$trade_intent", Mathf.CeilToInt(percent * cargo));

		percent = 0.03f;
		storage.SetValue("$tavern_intent", Mathf.CeilToInt(percent * cargo));

		percent = 0.05f;
		storage.SetValue("$all_intent", Mathf.CeilToInt(percent * cargo));
	}

	[YarnCommand("checkafford")]
	public void CheckAffordability(string cost) {
		int itemCost = 0;
		if (cost[0] == '$') {
			itemCost = IntFromVariableName(cost);
		}
		else {
			itemCost = Mathf.CeilToInt(float.Parse(cost));
		}
		storage.SetValue("$can_afford", Globals.GameVars.playerShipVariables.ship.currency >= itemCost);
	}

	[YarnCommand("roundup")]
	public void RoundToInt(string cost) {
		int itemCost = 0;
		if (cost[0] == '$') {
			itemCost = IntFromVariableName(cost);
		}
		else {
			itemCost = Mathf.CeilToInt(float.Parse(cost));
		}
		storage.SetValue("$rounded_num", itemCost);
	}


	[YarnCommand("pay")]
	public void PayAmount(string cost) {
		int itemCost = 0;
		if (cost[0] == '$') {
			itemCost = IntFromVariableName(cost);
		}
		else {
			itemCost = Mathf.RoundToInt(float.Parse(cost));
		}
		Globals.GameVars.playerShipVariables.ship.currency -= itemCost;
		UpdateMoney();
	}

	[YarnCommand("payresources")]
	public void PayAmountResources(string cost) {
		Globals.GameVars.playerShipVariables.ship.currency = 0;
		for (int i = 0; i < owedResources.Count; i++) {
			Resource r = System.Array.Find(Globals.GameVars.playerShipVariables.ship.cargo, x => x.name == owedResources[i].name);
			r.amount_kg -= owedResources[i].amount_kg;
		}
	}

	[YarnCommand("getresources")]
	public void CalculateNeededResources() {
		owedResources.Clear();
		float currentDr = Globals.GameVars.playerShipVariables.ship.currency;
		storage.SetValue("$drachma", currentDr);
		float cost = storage.GetValue("$final_cost").AsNumber;
		float owedDr = cost - currentDr;

		MetaResource[] sortedResources = Globals.GameVars.masterResourceList.OrderBy(x => x.trading_priority).ToArray();
		Resource[] playerResources = Globals.GameVars.playerShipVariables.ship.cargo;

		for (int i = 0; i < sortedResources.Length; i++) {
			//If you have any of it...
			int id = sortedResources[i].id;
			if (playerResources[id].amount_kg > 0) {
				//Do you have enough to completely cover your costs?
				float value = OneCargoValue(playerResources[id], playerResources[id].amount_kg);
				Resource r;

				if (value >= owedDr) {
					//If you do have more than enough, check how much is enough
					int amt;
					for (amt = 1; amt < Mathf.FloorToInt(playerResources[id].amount_kg); amt++) {
						float currentCost = OneCargoValue(playerResources[id], amt);
						if (currentCost >= owedDr) {
							break;
						}
					}
					value = OneCargoValue(playerResources[id], amt);
					r = new Resource(playerResources[id].name, amt);
				}
				else {
					//Otherwise, you'll need to add all of it and keep going
					r = new Resource(playerResources[id].name, playerResources[id].amount_kg);
				}

				owedDr -= value;
				owedResources.Add(r);

				//If you've got enough value, end the loop
				if (owedDr <= 0) {
					break;
				}
			}
		}

		storage.SetValue("$demanded_resources", FormatList(owedResources));
	}
	#endregion

	#region Yarn Helpers
	public void UpdateMoney() {
		moneyText.text = Globals.GameVars.playerShipVariables.ship.currency + " dr";
	}

	private int IntFromVariableName(string name) {
		return Mathf.CeilToInt(storage.GetValue(name).AsNumber);
	}

	private float CargoValue() {
		return Globals.GameVars.Trade.GetTotalPriceOfGoods() + Globals.GameVars.playerShipVariables.ship.currency;
	}

	private float OneCargoValue(Resource r, float qty) {
		return Globals.GameVars.Trade.GetPriceOfResource(r.name, city) * qty;
	}

	private float Truncate(float num, int places) {
		int factor = (int)Mathf.Pow(10, places);

		return Mathf.Round(num * factor) / factor;
	}

	private string FormatList(List<Resource> resources) 
	{
		string formatted = $"{resources[0].amount_kg}kg of {resources[0].name}";
		if (resources.Count >= 2) {
			formatted += ", ";
		}
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

