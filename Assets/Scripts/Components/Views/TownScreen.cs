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
	[SerializeField] CargoTradeListView Available;
	[SerializeField] CargoTradeListView Mine;

	[SerializeField] StringView PortName;

	[SerializeField] StringView Capacity;
	[SerializeField] StringView Money;

	[SerializeField] ButtonView Info;
	[SerializeField] ButtonView Port;

	[SerializeField] ButtonView SmallTxn;
	[SerializeField] ButtonView LargeTxn;
	[SerializeField] ButtonView AllTxn;

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

		PortName.Bind(ValueModel.New(model.PortName));
		Capacity.Bind(ValueModel.New(model.Capacity));
		Money.Bind(ValueModel.New(model.Money));

		SmallTxn.Bind(ValueModel.New(new ButtonViewModel {
			Label = model.TradeAction == TradeAction.Buy ? ">" : "<",
			OnClick = model.SmallTxn
		}));

		LargeTxn.Bind(ValueModel.New(new ButtonViewModel {
			Label = model.TradeAction == TradeAction.Buy ? ">>" : "<<",
			OnClick = model.LargeTxn
		}));

		AllTxn.Bind(ValueModel.New(new ButtonViewModel {
			Label = model.TradeAction == TradeAction.Buy ? "All>" : "<All",
			OnClick = model.AllTxn
		}));
	}
}
