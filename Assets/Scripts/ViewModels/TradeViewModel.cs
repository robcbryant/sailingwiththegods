using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum TradeAction
{
	Buy,
	Sell
}

public class TradeViewModel : CityViewModel
{
	public Ship Ship => GameVars.playerShipVariables.ship;

	public readonly ICollectionModel<CargoItemTradeViewModel> Available;
	public readonly ICollectionModel<CargoItemTradeViewModel> Mine;

	private TradeAction _TradeAction;
	public TradeAction TradeAction { get => _TradeAction; set { _TradeAction = value; Notify(); } }

	private CargoItemTradeViewModel _Selected;
	public CargoItemTradeViewModel Selected { get => _Selected; set { _Selected = value; Notify(); } }

	public string Capacity => Mathf.RoundToInt(GameVars.playerShipVariables.ship.CurrentCargoKg) + " / " + Mathf.RoundToInt(GameVars.playerShipVariables.ship.cargo_capicity_kg) + " kg";

	public BoundModel<int> Money;

	public bool allowPortAccess;
	public bool monuments;
	private Sprite noHeraldIcon;
	private float heraldEffect;
	private int heraldUses;
	private CargoItemTradeViewModel heraldTarget;

	public TradeViewModel(Sprite herald = null, Sprite noHerald = null, bool justWater = false, bool portAccess = true, float heraldMod = 1.0f) : base(Globals.GameVars.currentSettlement, null) 
	{
		noHeraldIcon = noHerald;
		heraldEffect = heraldMod;
		portAccess = justWater ? false : portAccess;

		Money = new BoundModel<int>(GameVars.playerShipVariables.ship, nameof(GameVars.playerShipVariables.ship.currency));

		// just wrap these non-observable lists as the resource list is static. only the contents change
		Available = ValueModel.Wrap(new ObservableCollection<CargoItemTradeViewModel>(GameVars.currentSettlement.cargo
			.Where(r => r.amount_kg > 0)
			.Select(r => new CargoItemTradeViewModel(TradeAction.Buy, r, this))
		));
		Mine = ValueModel.Wrap(new ObservableCollection<CargoItemTradeViewModel>(GameVars.playerShipVariables.ship.cargo
			.Where(r => r.amount_kg > 0)
			.Select(r => new CargoItemTradeViewModel(TradeAction.Sell, r, this))
		));

		foreach (CargoItemTradeViewModel c in Available) {
			c.HeraldIcon = noHeraldIcon;
		}
		foreach (CargoItemTradeViewModel c in Mine) {
			c.HeraldIcon = noHeraldIcon;
		}

		if (justWater) {
			foreach (CargoItemTradeViewModel item in Available.Value) {
				if (item.Name != "Water" && item.Name != "Provisions") {
					item.AllowSelection = false;
				}
			}

			foreach (CargoItemTradeViewModel item in Mine.Value) {
				item.AllowSelection = false;
			}
		}

		if (heraldMod > 1.0f) {
			Debug.Log($"TradeViewModel is setting herald to {(heraldMod - 1) * 100}%");

			CargoItemTradeViewModel cargo = Mine.RandomElement();
			Debug.Log($"Random cargo to boost: {cargo.Name}");

			cargo.PriceMod = heraldMod;

			cargo.HeraldIcon = herald;

			heraldUses = cargo.AmountKg;

			heraldTarget = cargo;
		}
		else {
			heraldTarget = null;
		}

		allowPortAccess = portAccess;
		monuments = !justWater;
	}

	public void BackToPort() {
		Globals.UI.Hide<TownScreen>();
		Globals.UI.Show<PortScreen, PortViewModel>(Globals.GameVars.MasterGUISystem.Port);
	}

	public void SmallTxn() {
		if (TradeAction == TradeAction.Buy) {
			GUI_Buy_Resources(Selected, Mathf.Min(1, Selected.AmountKg));
		}
		else {
			GUI_Sell_Resources(Selected, Mathf.Min(1, Selected.AmountKg));
		}
	}

	public void LargeTxn() {
		if (TradeAction == TradeAction.Buy) {
			GUI_Buy_Resources(Selected, Mathf.Min(10, Selected.AmountKg));
		}
		else {
			GUI_Sell_Resources(Selected, Mathf.Min(10, Selected.AmountKg));
		}
	}

	public void AllTxn() {
		if(TradeAction == TradeAction.Buy) {
			GUI_Buy_Resources(Selected, Selected.AmountKg);
		}
		else {
			GUI_Sell_Resources(Selected, Selected.AmountKg);
		}
	}

	void ChangeSettlementCargo(string resourceName, float changeAmount) {
		GameVars.currentSettlement.GetCargoByName(resourceName).amount_kg += changeAmount;
	}

	void ChangeShipCargo(string resourceName, int changeAmount, float priceMod) 
	{
		float price = 0.0f;

		int unitPrice = GameVars.Trade.GetPriceOfResource(resourceName, GameVars.currentSettlement);

		//if you're selling and there's a herald in play and this is what's being boosted, check the price
		if (changeAmount < 0 && heraldTarget != null && heraldTarget.Name == resourceName) 
		{
			Debug.Log($"Calculating herald price for {changeAmount} units");
			//We do it one by one in case you run out of herald uses partway through selling multiples

			if (heraldUses >= Mathf.Abs(changeAmount)) 
			{
				price = unitPrice * priceMod * changeAmount;
				heraldUses -= Mathf.Abs(changeAmount);
			}
			else 
			{
				int numWithoutMod = Mathf.Abs(changeAmount) - heraldUses;
				price = unitPrice * priceMod * heraldUses;
				price += unitPrice * numWithoutMod;
				price *= -1.0f;
				heraldUses = 0;
			}

			if (heraldUses <= 0) {
				Debug.Log("Out of herald uses");
				heraldTarget.PriceMod = 1.0f;
				heraldTarget.HeraldIcon = noHeraldIcon;
				heraldTarget.NotifyAny();
				heraldTarget = null;
			}
			Debug.Log("Price with herald mod figured in: " + price);
		}
		else 
		{
			price = unitPrice * changeAmount;
			Debug.Log("Price without herald: " + price);
		}

		
		Debug.Log(resourceName + "  :  " + GameVars.playerShipVariables.ship.GetCargoByName(resourceName).amount_kg + "  :  " + changeAmount);
		GameVars.playerShipVariables.ship.GetCargoByName(resourceName).amount_kg += changeAmount;
		//we use a (-) change amount here because the changeAmount reflects the direction of the goods
		//e.g. if the player is selling--they are negative in cargo---but their currency is positive and vice versa.
		GameVars.playerShipVariables.ship.currency += Mathf.FloorToInt(-price);
	}

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_Buy_Resources(CargoItemTradeViewModel item, int amount) {
		Debug.Log(item.Name + " : " + amount);

		int amountToBuy = GameVars.Trade.AdjustBuy(amount, item.Name);
		if (amountToBuy > 0) {

			// these change the values in our model too, just need to notify
			ChangeShipCargo(item.Name, amountToBuy, item.PriceMod);
			ChangeSettlementCargo(item.Name, -amountToBuy);

			// update the list so the new row appears
			// probably need to write some sort of wrapper that watches for amount == 0 and does this automatically
			var mine = Mine.FirstOrDefault(n => n.Name == item.Name);
			if (mine == null) {
				mine = new CargoItemTradeViewModel(TradeAction.Sell, GameVars.playerShipVariables.ship.GetCargoByName(item.Name), this);
				Mine.Add(mine);
			}

			if(item.AmountKg <= 0) {
				Available.Remove(item);
			}

			item.Notify(nameof(item.AmountKg));
			mine.Notify(nameof(mine.AmountKg));
			Notify(nameof(Capacity));
			Notify(nameof(Money));
		}
	}

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_Sell_Resources(CargoItemTradeViewModel item, int amount) {
		Debug.Log(item.Name + " : " + amount);

		int amountToSell = GameVars.Trade.AdjustSell(amount, item.Name);
		if (amountToSell > 0) {

			// these change the values in our model too, just need to notify
			ChangeShipCargo(item.Name, -amountToSell, item.PriceMod);
			ChangeSettlementCargo(item.Name, amountToSell);

			// update the list so the new row appears
			// probably need to write some sort of wrapper that watches for amount == 0 and does this automatically
			var available = Available.FirstOrDefault(n => n.Name == item.Name);
			if (available == null) {
				available = new CargoItemTradeViewModel(TradeAction.Buy, GameVars.currentSettlement.GetCargoByName(item.Name), this);
				Available.Add(available);
			}

			item.Notify(nameof(item.AmountKg));
			available.Notify(nameof(available.AmountKg));
			Notify(nameof(Capacity));
			Notify(nameof(Money));
		}
	}
}
