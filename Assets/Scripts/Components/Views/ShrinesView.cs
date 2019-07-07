using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShrinesViewModel : Model
{
	GameVars GameVars => Globals.GameVars;

	int BaseCost {
		get {
			int baseCost = 0;
			//We need to do a clout check as well as a network checks
			int baseModifier = Mathf.CeilToInt(1000 - (200 * GameVars.GetOverallCloutModifier(GameVars.currentSettlement.settlementID)));
			if (GameVars.Network.CheckIfCityIDIsPartOfNetwork(GameVars.currentSettlement.settlementID)) {
				baseCost = Mathf.CeilToInt(GameVars.currentSettlement.tax_network * baseModifier * 1);
			}
			else {
				baseCost = Mathf.CeilToInt(GameVars.currentSettlement.tax_neutral * baseModifier * 1);
			}
			return baseCost;
		}
	}

	public ObservableCollection<ShrineOptionModel> Options { get; private set; }

	public ShrinesViewModel() {
		Options = new ObservableCollection<ShrineOptionModel>(new[]
		{
			new ShrineOptionModel("Votive", BaseCost / 200, 1, "+1 Clout"),
			new ShrineOptionModel("Feast", BaseCost / 10, 10, "+10 Clout"),
			new ShrineOptionModel("Statue", BaseCost / 3, 30, "+30 Clout"),
			new ShrineOptionModel("Shrine", BaseCost / 3 * 50, 50, "+50 Clout"),
			new ShrineOptionModel("Temple", BaseCost / 3 * 50 * 20, 100, "+100 Clout")
		});
	}
}

public class ShrinesView : ViewBehaviour<ShrinesViewModel>
{
	[SerializeField] ShrineListView Options = null;

	public override void Bind(ShrinesViewModel model) {
		base.Bind(model);

		Options.Bind(model.Options);
	}
}
