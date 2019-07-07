using UnityEngine;
using UnityEngine.UI;

public class ShrineOptionModel : Model
{
	GameVars GameVars => Globals.GameVars;

	public string Name;
	public int Cost;
	public int CloutGain;
	public string BenefitHint;

	public ShrineOptionModel(string name, int cost, int cloutGain, string benefitHint) {
		Name = name;
		Cost = cost;
		CloutGain = cloutGain;
		BenefitHint = benefitHint;
	}

	public void Buy() {

		if (GameVars.playerShipVariables.ship.currency > Cost) {
			GameVars.playerShipVariables.ship.currency -= Cost;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You built a " + Name + " for " + GameVars.currentSettlement.name + "! " + BenefitHint;
			GameVars.AdjustPlayerClout(1);
			GameVars.playerShipVariables.ship.builtMonuments += GameVars.currentSettlement.name + " -- " + Name + "\n";

		}
		else {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "You don't have enough money to build a " + Name + " for " + GameVars.currentSettlement.name;
		}

	}
}

public class ShrineOptionView : ViewBehaviour<ShrineOptionModel>
{
	[SerializeField] StringView Name = null;
	[SerializeField] StringView BenefitHint = null;
	[SerializeField] ButtonView Buy = null;

	public override void Bind(ShrineOptionModel model) {
		base.Bind(model);

		Name.Bind(ValueModel.New(model.Name));
		BenefitHint.Bind(ValueModel.New(model.BenefitHint));
		Buy.Bind(ValueModel.New(new ButtonViewModel {
			Label = model.Cost + " dr",
			OnClick = model.Buy
		}));
	}
}
