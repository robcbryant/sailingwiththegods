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

public class TradeViewModel : Model
{
	private GameVars GameVars;

	public readonly ObservableCollection<CargoItemTradeViewModel> Available;
	public readonly ObservableCollection<CargoItemTradeViewModel> Mine;

	private TradeAction _TradeAction;
	public TradeAction TradeAction { get => _TradeAction; set { _TradeAction = value; Notify(); } }

	private CargoItemTradeViewModel _Selected;
	public CargoItemTradeViewModel Selected { get => _Selected; set { _Selected = value; Notify(); } }

	public string PortName => GameVars.currentSettlement.name;
	public string Capacity => Mathf.RoundToInt(GameVars.playerShipVariables.ship.CurrentCargoKg) + " / " + Mathf.RoundToInt(GameVars.playerShipVariables.ship.cargo_capicity_kg) + " kg";
	public string Money => GameVars.playerShipVariables.ship.currency + " dr";

	public TradeViewModel() {
		GameVars = Globals.GameVars;

		Available = new ObservableCollection<CargoItemTradeViewModel>(GameVars.currentSettlement.cargo
			.Where(r => r.amount_kg > 0)
			.Select(r => new CargoItemTradeViewModel(TradeAction.Buy, r, this))
		);
		Mine = new ObservableCollection<CargoItemTradeViewModel>(GameVars.playerShipVariables.ship.cargo
			.Where(r => r.amount_kg > 0)
			.Select(r => new CargoItemTradeViewModel(TradeAction.Sell, r, this))
		);
	}

	public void BackToPort() {
		Globals.UI.Hide<TownScreen>();
		Globals.UI.Show<PortScreen, PortViewModel>(new PortViewModel());
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

	void ChangeShipCargo(string resourceName, float changeAmount) {
		float price = GameVars.Trade.GetPriceOfResource(resourceName, GameVars.currentSettlement);
		Debug.Log(resourceName + "  :  " + GameVars.playerShipVariables.ship.GetCargoByName(resourceName).amount_kg + "  :  " + changeAmount);
		GameVars.playerShipVariables.ship.GetCargoByName(resourceName).amount_kg += changeAmount;
		//we use a (-) change amount here because the changeAmount reflects the direction of the goods
		//e.g. if the player is selling--they are negative in cargo---but their currency is positive and vice versa.
		GameVars.playerShipVariables.ship.currency += (int)(price * -changeAmount);
	}

	// REFERENCED IN BUTTON CLICK UNITYEVENT
	public void GUI_Buy_Resources(CargoItemTradeViewModel item, int amount) {
		Debug.Log(item.Name + " : " + amount);

		var amountToBuy = GameVars.Trade.AdjustBuy(amount, item.Name);
		if (amountToBuy > 0) {

			// these change the values in our model too, just need to notify
			ChangeShipCargo(item.Name, amountToBuy);
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

		var amountToSell = GameVars.Trade.AdjustSell(amount, item.Name);
		if (amountToSell > 0) {

			// these change the values in our model too, just need to notify
			ChangeShipCargo(item.Name, -amountToSell);
			ChangeSettlementCargo(item.Name, amountToSell);

			// update the list so the new row appears
			// probably need to write some sort of wrapper that watches for amount == 0 and does this automatically
			var available = Available.FirstOrDefault(n => n.Name == item.Name);
			if (available == null) {
				available = new CargoItemTradeViewModel(TradeAction.Sell, GameVars.currentSettlement.GetCargoByName(item.Name), this);
				Available.Add(available);
			}

			item.Notify(nameof(item.AmountKg));
			available.Notify(nameof(available.AmountKg));
			Notify(nameof(Capacity));
			Notify(nameof(Money));
		}
	}
}
