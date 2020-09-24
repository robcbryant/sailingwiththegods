using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

public class YarnTaxes : MonoBehaviour
{
	[Range(0f, 1f)]
	public float heraldChance = 0.1f;
	[Range(0f, 1f)]
	public float heraldEffect = 0.01f;
	public Sprite heraldIcon;

	private DialogScreen ds;
	private Settlement city;
	private List<Resource> owedResources = new List<Resource>();

	void OnValidate()
    {
		ds = GetComponent<DialogScreen>();
    }

	public void ExitPortConversation() 
	{
		bool city = ds.Storage.GetValue("$entering_city").AsBool;
		Debug.Log($"Exiting the conversation. Entering the city {city}");

		string intentText = ds.Storage.GetValue("$intent").AsString;
		script_GUI.Intention intent;
		switch (intentText) {
			case ("water"):
				intent = script_GUI.Intention.Water;
				break;
			case ("trading"):
				intent = script_GUI.Intention.Trading;
				break;
			case ("tavern"):
				intent = script_GUI.Intention.Tavern;
				break;
			case ("all"):
				intent = script_GUI.Intention.All;
				break;
			default:
				intent = script_GUI.Intention.Water;
				break;
		}

		if (city) {
			float heraldMod = 1.0f;
			if (ds.Storage.GetValue("$have_herald").AsBool) {
				float chance = Random.Range(0f, 1f);
				if (chance < heraldChance) {
					Debug.Log("Herald in effect");
					heraldMod += heraldEffect;
				}
			}

			ds.gui.GUI_EnterPort(heraldIcon, intent, heraldMod);
		}
		else {
			ds.gui.GUI_ExitPortNotification();
		}

		StartCoroutine(ds.DeactivateSelf());
	}

	public void SetPortInfo(Settlement s) 
	{
		city = s;
		Debug.Log("Current settlement: " + city.name);
		ds.Storage.SetValue("$city_name", city.name);
		ds.Storage.SetValue("$city_description", city.description);
		ds.Storage.SetValue("$jason_connected", false);
		ds.Storage.SetValue("$crew_name", "Bob IV");

		ds.YarnUI.onDialogueEnd.RemoveAllListeners();
		ds.YarnUI.onDialogueEnd.AddListener(ExitPortConversation);
	}

	#region Yarn Functions - Set Variables
	[YarnCommand("citynetworks")]
	public void NumberOfCityNetworks() 
	{
		ds.Storage.SetValue("$city_networks", city.networks.Count);
	}

	[YarnCommand("networkconnections")]
	public void NumberOfConnections() 
	{
		IEnumerable<CrewMember> connected = Globals.GameVars.Network.CrewMembersWithNetwork(city, true);
		int connectedNum = Enumerable.Count(connected);
		ds.Storage.SetValue("$connections_number", connectedNum);
	}

	[YarnCommand("cityinfo")]
	public void SetCityInfo() 
	{
		ds.Storage.SetValue("$city_name", city.name);
		ds.Storage.SetValue("$city_description", city.description);
	}

	[YarnCommand("checkcitytaxes")]
	public void CheckCityTaxes() 
	{
		ds.Storage.SetValue("$god_tax", city.godTax);
		ds.Storage.SetValue("$god_tax_amount", city.godTaxAmount);
		ds.Storage.SetValue("$transit_tax", city.transitTax);
		ds.Storage.SetValue("$transit_tax_amount", city.transitTaxPercent);
		ds.Storage.SetValue("$foreigner_tax", city.foreignerFee);
		ds.Storage.SetValue("$foreigner_tax_amount", city.foreignerFeePercent);
		ds.Storage.SetValue("$wealth_tax", CargoValue() >= 1000000);
		ds.Storage.SetValue("$wealth_tax_amount", .10f);

		ds.Storage.SetValue("$no_taxes", !city.godTax && !city.transitTax && !city.foreignerFee);
		ds.Storage.SetValue("$cargo_value", Mathf.CeilToInt(CargoValue()));
	}

	[YarnCommand("connectedcrew")]
	public void ConnectedCrewName() 
	{
		if (ds.Storage.GetValue("$connections_number").AsNumber > 0) {
			if (Globals.GameVars.Network.GetCrewMemberNetwork(Globals.GameVars.Jason).Contains(city)) {
				ds.Storage.SetValue("$jason_connected", true);
				ds.Storage.SetValue("$crew_name_1", "me");
				ds.Storage.SetValue("$crew_name_2", "I");
				ds.Storage.SetValue("$crew_name_3", "You");
				ds.Storage.SetValue("$crew_description", Globals.GameVars.Jason.backgroundInfo);
				ds.Storage.SetValue("$crew_home", Globals.GameVars.GetSettlementFromID(Globals.GameVars.Jason.originCity).name);
			}
			else {
				IEnumerable<CrewMember> connected = Globals.GameVars.Network.CrewMembersWithNetwork(city);
				CrewMember crew = connected.RandomElement();
				ds.Storage.SetValue("$crew_name_1", crew.name);
				ds.Storage.SetValue("$crew_name_2", crew.name);
				ds.Storage.SetValue("$crew_name_3", crew.name);
				ds.Storage.SetValue("$crew_description", crew.backgroundInfo);
				ds.Storage.SetValue("$crew_home", Globals.GameVars.GetSettlementFromID(crew.originCity).name);
			}
		}
		else {
			ds.Storage.SetValue("$jason_connected", false);
			ds.Storage.SetValue("$crew_name_1", "ERROR");
			ds.Storage.SetValue("$crew_name_2", "ERROR");
			ds.Storage.SetValue("$crew_name_3", "ERROR");
			ds.Storage.SetValue("$crew_description", "ERROR");
			ds.Storage.SetValue("$crew_home", "ERROR");
		}
	}

	[YarnCommand("cargovalue")]
	public void StoreCargoValue() 
	{
		ds.Storage.SetValue("$cargo_value", Mathf.CeilToInt(CargoValue()));
	}
	#endregion

	#region Yarn functions - Tax Calculations
	[YarnCommand("calculatetaxes")]
	public void CalculateTaxCharges() 
	{
		float subtotal = 0;

		float cargo = CargoValue();

		if (ds.Storage.GetValue("$god_tax").AsBool) {
			//God Tax is a flat number
			subtotal += ds.Storage.GetValue("$god_tax_amount").AsNumber;
		}
		if (ds.Storage.GetValue("$transit_tax").AsBool) {
			//Transit tax is a percent
			subtotal += ds.Storage.GetValue("$transit_tax_amount").AsNumber * cargo;
		}
		if (ds.Storage.GetValue("$foreigner_tax").AsBool) {
			//Foreigner tax is a percent
			subtotal += ds.Storage.GetValue("$foreigner_tax_amount").AsNumber * cargo;
		}
		if (ds.Storage.GetValue("$wealth_tax").AsBool) {
			//Wealth tax is a percent
			subtotal += ds.Storage.GetValue("$wealth_tax_amount").AsNumber * cargo;
		}

		cargo = CargoValue();
		ds.Storage.SetValue("$tax_subtotal", Mathf.CeilToInt(subtotal));
		ds.Storage.SetValue("$cargo_value", cargo);
		ds.Storage.SetValue("$ellimenion_percent", .05f);
	}

	[YarnCommand("calculatepercents")]
	public void CalculateIntentPercent() 
	{
		float subtotal = ds.Storage.GetValue("$tax_subtotal").AsNumber;
		float cargo = CargoValue();

		float percent = 0.01f;
		ds.Storage.SetValue("$water_intent", Mathf.CeilToInt(percent * cargo));

		percent = 0.02f;
		ds.Storage.SetValue("$trade_intent", Mathf.CeilToInt(percent * cargo));

		percent = 0.03f;
		ds.Storage.SetValue("$tavern_intent", Mathf.CeilToInt(percent * cargo));

		percent = 0.05f;
		ds.Storage.SetValue("$all_intent", Mathf.CeilToInt(percent * cargo));
	}

	[YarnCommand("cargopay")]
	public void PayAmountResources(string cost) 
	{
		Globals.GameVars.playerShipVariables.ship.currency = 0;
		ds.UpdateMoney();
		for (int i = 0; i < owedResources.Count; i++) {
			System.Array.Find(Globals.GameVars.playerShipVariables.ship.cargo, x => x.name == owedResources[i].name).amount_kg -= owedResources[i].amount_kg;
			Debug.Log($"Paying {owedResources[i].amount_kg}kg of {owedResources[i].name}");
		}
	}

	[YarnCommand("getresources")]
	public void CalculateNeededResources() 
	{
		owedResources.Clear();
		float currentDr = Globals.GameVars.playerShipVariables.ship.currency;
		ds.Storage.SetValue("$drachma", currentDr);
		float cost = ds.Storage.GetValue("$final_cost").AsNumber;
		float owedDr = cost - currentDr;
		Debug.Log($"Taxes remaining: {owedDr}dr");

		MetaResource[] sortedResources = Globals.GameVars.masterResourceList.OrderBy(x => x.trading_priority).ToArray();
		Resource[] playerResources = Globals.GameVars.playerShipVariables.ship.cargo;

		for (int i = 0; i < sortedResources.Length; i++) {
			//If it's something that can be demanded (ie not water or food)...
			if (sortedResources[i].trading_priority != 100) {
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
						Debug.Log($"Paying {amt}kg of {r.name}: value {value}dr");
					}
					else {
						//Otherwise, you'll need to add all of it and keep going
						r = new Resource(playerResources[id].name, playerResources[id].amount_kg);
						Debug.Log($"Paying {r.amount_kg}kg of {r.name}: value {value}dr");
					}

					owedDr -= value;
					Debug.Log($"Taxes remaining: {owedDr}dr");
					owedResources.Add(r);

					//If you've got enough value, end the loop
					if (owedDr <= 0) {
						break;
					}
				}
			}

		}

		ds.Storage.SetValue("$demanded_resources_value", cost - owedDr);
		ds.Storage.SetValue("$demanded_resources", YarnGeneral.FormatList(owedResources));
	}
	#endregion

	private float CargoValue() 
	{
		return Globals.GameVars.Trade.GetTotalPriceOfGoods() + Globals.GameVars.playerShipVariables.ship.currency;
	}

	private float OneCargoValue(Resource r, float qty) 
	{
		return Globals.GameVars.Trade.GetPriceOfResource(r.name, city) * qty;
	}
}
