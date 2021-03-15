using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class TavernCityView : ViewBehaviour<CityViewModel>
{
	[SerializeField] StringView Name = null;
	[SerializeField] ButtonView Ask = null;

	private DialogScreen ds;
	// [SerializeField] ButtonView Hire = null;

	// convenience so we don't have to make a separate CityListView just for taverns
	TavernCityViewModel CityModel => Model as TavernCityViewModel;


	public override void Bind(CityViewModel model) {
		base.Bind(model);
		ds = CityModel.GetDS;
		Name?.Bind(new BoundModel<string>(Model, nameof(Model.PortName)));

		Ask?.Bind(ValueModel.New(new ButtonViewModel {
			Label = "Select",
			OnClick = () => 
			{
				//CityModel.GUI_BuyHint
				ds.Storage.SetValue("$known_city", Model.PortName);
				ds.Storage.SetValue("$known_city_ID", Model.City.settlementID);
				Debug.Log("We have clicked on button.");
				ds.yarnOnComplete();
				Globals.UI.Hide<TavernView>();
			}
		})) ;

		/*Hire?.Bind(ValueModel.New(new ButtonViewModel {
			Label = CityModel.CostToHire + " dr",
			OnClick = CityModel.GUI_HireANavigator
		}));*/
	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		// setup a hint button if it's a city. If it's not a city, then there is no trading and nothign to ask about
		Ask.gameObject.SetActive(Model.City.typeOfSettlement == 1);
	}
}

