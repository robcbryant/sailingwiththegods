using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class TownScreen : ViewBehaviour<TradeViewModel>
{
	[SerializeField] CargoTradeListView Available = null;
	[SerializeField] CargoTradeListView Mine = null;

	[SerializeField] StringView PortName = null;

	[SerializeField] StringView Capacity = null;
	[SerializeField] StringView Money = null;

	[SerializeField] ButtonView Info = null;
	[SerializeField] ButtonView Port = null;

	[SerializeField] ButtonView SmallTxn = null;
	[SerializeField] ButtonView LargeTxn = null;
	[SerializeField] ButtonView AllTxn = null;

	public override void Bind(TradeViewModel model) {
		base.Bind(model);

		Available?.Bind(model.Available);
		Mine?.Bind(model.Mine);

		Port?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Port",
			OnClick = model.BackToPort
		}));

		Info?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => Debug.Log("Info clicked for " + model.PortName)
		}));
	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		PortName.Bind(ValueModel.New(Model.PortName));
		Capacity.Bind(ValueModel.New(Model.Capacity));
		Money.Bind(ValueModel.New(Model.Money));

		SmallTxn.Bind(ValueModel.New(new ButtonViewModel {
			Label = Model.TradeAction == TradeAction.Buy ? ">" : "<",
			OnClick = Model.SmallTxn
		}));

		LargeTxn.Bind(ValueModel.New(new ButtonViewModel {
			Label = Model.TradeAction == TradeAction.Buy ? ">>" : "<<",
			OnClick = Model.LargeTxn
		}));

		AllTxn.Bind(ValueModel.New(new ButtonViewModel {
			Label = Model.TradeAction == TradeAction.Buy ? "All>" : "<All",
			OnClick = Model.AllTxn
		}));

	}
}
