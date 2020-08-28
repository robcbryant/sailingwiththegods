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
	[SerializeField] StringView Region = null;
	[SerializeField] StringView Distance = null;

	[SerializeField] CargoListView Buy = null;
	[SerializeField] CargoListView Sell = null;

	[SerializeField] ButtonView ActionButton = null;

	public override void Bind(CityViewModel model) {
		base.Bind(model);

		if(model is CityDetailsViewModel) {
			var details = model as CityDetailsViewModel;
			Buy?.Bind(details.Buy);
			Sell?.Bind(details.Sell);
			Crew?.Bind(details.Crew);
		}

		PortName?.Bind(ValueModel.New(model.PortName));
		Region?.Bind(ValueModel.New(model.RegionName));
		Distance?.Bind(ValueModel.New(model.Distance)
			.Select(d => string.Format("{0} km away", Mathf.RoundToInt(d)))
		);

		ActionButton?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => model.OnClick?.Invoke(Model)
		}));
	}
}
