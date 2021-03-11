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

	[SerializeField] ImageView PortIcon = null;

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
		Town.Interactable = model.allowTownAccess;

		Info?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => Globals.UI.Show<InfoScreen, InfoScreenModel>(new InfoScreenModel {
				Icon = model.PortCoin,
				Title = model.PortName,
				Subtitle = model.PortPopulationRank,
				Message = model.PortDescription
			})
		}));

		Loans?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Loans",
			OnClick = () => Globals.UI.Show<LoanView, LoanViewModel>(new LoanViewModel())
		}));

		Tavern?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Tavern",
			OnClick = () => Globals.MiniGames.EnterScene("TavernaMenu")
		}));

		Repairs?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Shipyard",
			OnClick = () => Globals.UI.Show<RepairsView, RepairsViewModel>(new RepairsViewModel())
		}));

		PortIcon?.Bind(new BoundModel<Sprite>(model, nameof(model.PortIcon)));

		PortName.Bind(ValueModel.New(Model.PortName));

		Capacity.Bind(Model.CrewManagement.CrewCapacity
			.AsString()
			.Select(Model.CrewManagement.CrewCount, (cap, count) => count + " / " + cap + " crew"));

		Money.Bind(ValueModel.Wrap(Model.CrewManagement.Money)
				.AsString()
				.Select(s => s + " dr")
		);
	}
}
