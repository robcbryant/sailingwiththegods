using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PortViewModel : CityViewModel
{
	public readonly CrewManagementViewModel CrewManagement;

	public bool allowTownAccess;

	public PortViewModel(bool town = true) : base(Globals.GameVars.currentSettlement, null){
		CrewManagement = new CrewManagementViewModel(City);
		allowTownAccess = town;
	}

	public void GoToTown() {
		Globals.UI.Hide<PortScreen>();
		Globals.UI.Show<TownScreen, TradeViewModel>(new TradeViewModel());
	}
}
