using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class CityView : ViewBehaviour<CityViewModel>
{
	[SerializeField] CrewManagementListView Crew = null;

	[SerializeField] StringView PortName = null;

	[SerializeField] CargoListView Buy = null;
	[SerializeField] CargoListView Sell = null;

	[SerializeField] ButtonView ActionButton = null;

	public override void Bind(CityViewModel model) {
		base.Bind(model);

		Buy?.Bind(model.Buy);
		Sell?.Bind(model.Sell);
		Crew?.Bind(model.Crew);

		PortName?.Bind(ValueModel.New(model.PortName));

		ActionButton?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => model.OnClick?.Invoke(Model)
		}));
	}
}
