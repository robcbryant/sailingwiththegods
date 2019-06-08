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
	[SerializeField] CrewManagementListView Hire;
	[SerializeField] CrewManagementListView Fire;

	[SerializeField] StringView PortName;

	[SerializeField] StringView Capacity;
	[SerializeField] StringView Money;

	[SerializeField] ButtonView Info;
	[SerializeField] ButtonView Sail;
	[SerializeField] ButtonView Town;

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

		PortName.Bind(ValueModel.New(Model.PortName));
		Capacity.Bind(new BoundModel<string>(Model.CrewManagement, nameof(Model.CrewManagement.Capacity)));
		Money.Bind(new BoundModel<string>(Model.CrewManagement, nameof(Model.CrewManagement.Money)));
	}
}
