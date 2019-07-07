using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class PortScreen : ViewBehaviour<PortViewModel>
{
	[SerializeField] CrewManagementListView Hire = null;
	[SerializeField] CrewManagementListView Fire = null;

	[SerializeField] StringView PortName = null;

	[SerializeField] StringView Capacity = null;
	[SerializeField] StringView Money = null;

	[SerializeField] ButtonView Info = null;
	[SerializeField] ButtonView Sail = null;
	[SerializeField] ButtonView Town = null;

	[SerializeField] ButtonView Tavern = null;
	[SerializeField] ButtonView Loans = null;
	[SerializeField] ButtonView Repairs = null;

	public override void Bind(PortViewModel model) {
		base.Bind(model);

		Hire?.Bind(model.CrewManagement.AvailableCrew);
		Fire?.Bind(model.CrewManagement.MyCrew);

		Sail?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Sail",
			OnClick = model.GUI_Button_TryToLeavePort
		}));

		Town?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Town",
			OnClick = model.GoToTown
		}));

		Info?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => Debug.Log("Info clicked for " + model.PortName)
		}));

		Loans?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Loans",
			OnClick = () => Globals.UI.Show<LoanView, LoanViewModel>(new LoanViewModel())
		}));

		Tavern?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Tavern",
			OnClick = () => Globals.UI.Show<TavernView, TavernViewModel>(new TavernViewModel())
		}));

		Repairs?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Repairs",
			OnClick = () => Globals.UI.Show<RepairsView, RepairsViewModel>(new RepairsViewModel())
		}));

		PortName.Bind(ValueModel.New(Model.PortName));
		Capacity.Bind(new BoundModel<string>(Model.CrewManagement, nameof(Model.CrewManagement.Capacity)));
		Money.Bind(ValueModel.Wrap(Model.CrewManagement.Money)
				.AsString()
				.Select(s => s + " dr")
		);
	}
}
