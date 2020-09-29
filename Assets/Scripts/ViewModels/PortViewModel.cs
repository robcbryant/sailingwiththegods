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
	protected Sprite heraldIcon;
	protected float heraldEffect;

	public PortViewModel(bool townAccess = true, Sprite herald = null, float heraldModifier = 1) : base(Globals.GameVars.currentSettlement, null){
		CrewManagement = new CrewManagementViewModel(City);
		allowTownAccess = townAccess;
		heraldIcon = herald;
		heraldEffect = heraldModifier;
	}

	public void GoToTown() {
		Globals.UI.Hide<PortScreen>();
		Globals.UI.Show<TownScreen, TradeViewModel>(new TradeViewModel(heraldIcon, false, true, heraldEffect));
	}
}
