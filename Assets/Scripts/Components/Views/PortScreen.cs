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
	[SerializeField] ButtonView Info;
	[SerializeField] ButtonView Sail;

	public override void Bind(PortViewModel model) {
		base.Bind(model);

		Hire?.Bind(model.CrewManagement.AvailableCrew);
		Fire?.Bind(model.CrewManagement.MyCrew);

		Sail?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Sail",
			OnClick = () => Globals.UI.Hide<PortScreen>()	// TODO: There's more to do on hide. Got to set all the state stuff in gamevars
		}));

		Info?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => Debug.Log("Info clicked for " + model.PortName)
		}));

		PortName.Bind(ValueModel.New(model.PortName));
	}
}
