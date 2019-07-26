using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RepairsViewModel : Model
{
	GameVars GameVars => Globals.GameVars;

	public int costToRepair { get; private set; }
	public int costToBuyUpgrade => 10000;			// TODO: Drive with something.

	public BoundModel<float> shipHealth { get; private set; }
	public BoundModel<int> shipLevel { get; private set; }

	public RepairsViewModel() {

		//We need to do a clout check as well as a network checks
		int baseModifier = Mathf.CeilToInt(2 - GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID));
		if (GameVars.Network.CheckIfCityIDIsPartOfNetwork(GameVars.currentSettlement.settlementID)) {
			costToRepair = Mathf.CeilToInt(GameVars.currentSettlement.tax_network * baseModifier * 1);
		}
		else {
			costToRepair = Mathf.CeilToInt(GameVars.currentSettlement.tax_neutral * baseModifier * 1);
		}

		shipHealth = new BoundModel<float>(GameVars.playerShipVariables.ship, nameof(GameVars.playerShipVariables.ship.health));
		shipLevel = new BoundModel<int>(GameVars.playerShipVariables.ship, nameof(GameVars.playerShipVariables.ship.upgradeLevel));

	}

	public void GUI_RepairShipByOneHP() {
		GameVars.playerShipVariables.ship.health += 1f;
		//make sure the hp can't go above 100
		if (GameVars.playerShipVariables.ship.health > 100) {
			GameVars.playerShipVariables.ship.health = 100;
			GameVars.showNotification = true;
			GameVars.notificationMessage = "Your ship is already fully repaired";
		}
		else {
			GameVars.playerShipVariables.ship.currency -= costToRepair;
		}

		NotifyAny();
	}

	public void GUI_RepairShipByAllHP() {
		if (Mathf.CeilToInt(GameVars.playerShipVariables.ship.health) >= 100) {
			GameVars.showNotification = true;
			GameVars.notificationMessage = "Your ship is already fully repaired";
		}
		else {
			GameVars.playerShipVariables.ship.currency -= (int)(costToRepair * Mathf.CeilToInt(100 - GameVars.playerShipVariables.ship.health));
			GameVars.playerShipVariables.ship.health = 100f;
		}

		NotifyAny();
	}

	public void GUI_BuyNewShip() {
		if(GameVars.playerShipVariables.ship.currency > costToBuyUpgrade) {
			GameVars.playerShipVariables.ship.upgradeLevel = 1;
			GameVars.playerShipVariables.ship.currency -= costToBuyUpgrade;

			// TODO: These should be defined per uprade level, but until we have a better idea how upgrades will work long term, just hard here
			GameVars.playerShipVariables.ship.crewCapacity = 30;
			GameVars.playerShipVariables.ship.cargo_capicity_kg = 1200;

			// add all the non-fireable story crew members now that you have your big boy ship
			GameVars.FillBeginStoryCrew();

			Globals.UI.Show<InfoScreen, InfoScreenModel>(new InfoScreenModel {
				Title = "Welcome to the Argonautica!",
				Message = "Find your way through the dangerous seas to complete your quest! You have found yourself at Pagasae, where King Pelias has given you the task of sailing across " +						"the Aegean and the Black Sea to retrieve the Golden Fleece. This is the hide of the flying ram that brought Phrixus from Boetia to Aea. The hide now hangs on a tree on the other side of the Black" +
						" Sea in the city of Aea. The great lord Aeetes prizes the fleece, and a very large dragon guards it.\n\n" +
						"The task seems impossible!But you do not sail alone, Jason.You have assembled a group of the most powerful warriors, sailors, and prophets in Greece to help you in your quest. " +
						"Most are the sons of royal families, and each one has a unique skill.Once the heroes have all arrived, your crew stocks the ships and the people of Pagasae greet you all."
			});
		}
		else { 
			GameVars.showNotification = true;
			GameVars.notificationMessage = "Earn more drachma through trade to upgrade your ship!";
		}
	}

}

public class RepairsView : ViewBehaviour<RepairsViewModel>
{
	[SerializeField] StringView ShipHealth = null;
	[SerializeField] StringView CostOneHp = null;
	[SerializeField] StringView CostAllHp = null;
	[SerializeField] ButtonView RepairOneButton = null;
	[SerializeField] ButtonView RepairAllButton = null;
	[SerializeField] ButtonView UpgradeButton = null;

	public override void Bind(RepairsViewModel model) {
		base.Bind(model);

		ShipHealth.Bind(ValueModel.Wrap(model.shipHealth)
			.Select(h => Mathf.CeilToInt(Globals.GameVars.playerShipVariables.ship.health))
			.AsString()
		);

		RepairOneButton.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Repair",
			OnClick = Model.GUI_RepairShipByOneHP
		}));

		RepairAllButton.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Repair",
			OnClick = Model.GUI_RepairShipByAllHP
		}));

		UpgradeButton.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Buy",
			OnClick = model.GUI_BuyNewShip
		}));

	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		//If the ship is at 100HP already, then let's not worry about giving the player the costs--we'll replace the costs by an X
		//	--and disable the repair buttons
		if (Mathf.CeilToInt(Model.shipHealth.Value) ==  100) {
			CostOneHp.Bind(ValueModel.New("X"));
			CostAllHp.Bind(ValueModel.New("X"));

			RepairOneButton.GetComponent<Button>().interactable = false;
			RepairAllButton.GetComponent<Button>().interactable = false;
		}
		else {

			CostOneHp.Bind(ValueModel.New(Model.costToRepair)
				.Select(cost => Mathf.CeilToInt(cost))
				.AsString());

			CostAllHp.Bind(ValueModel.New(Model.costToRepair)
				.Select(cost => (Mathf.CeilToInt(100 - Mathf.CeilToInt(Globals.GameVars.playerShipVariables.ship.health)) * cost))
				.AsString());

			RepairOneButton.GetComponent<Button>().interactable = true;
			RepairAllButton.GetComponent<Button>().interactable = true;
		}

		// TODO: Flesh out upgrade system? For now, you can only upgrade once and it just gives you the main ship. You start out with a smaller one.
		UpgradeButton.GetComponent<Button>().interactable = Model.shipLevel.Value == 0;
	}
}
