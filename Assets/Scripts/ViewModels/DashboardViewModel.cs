using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

public class DashboardViewModel : Model
{
	public string CaptainsLog => Globals.GameVars.currentCaptainsLog;
	public readonly CargoInventoryViewModel WaterInventory;
	public readonly CargoInventoryViewModel FoodInventory;
	public readonly ObservableCollection<CargoInventoryViewModel> CargoList;
	public readonly ObservableCollection<CrewManagementMemberViewModel> CrewList;

	public DashboardViewModel() {

		var water = Globals.GameVars.playerShipVariables.ship.cargo.FirstOrDefault(r => r.name == Resource.Water);
		WaterInventory = new CargoInventoryViewModel(water);

		var food = Globals.GameVars.playerShipVariables.ship.cargo.FirstOrDefault(r => r.name == Resource.Provisions);
		FoodInventory = new CargoInventoryViewModel(food);

		CargoList = new ObservableCollection<CargoInventoryViewModel>(Globals.GameVars.playerShipVariables.ship.cargo.Select(c => new CargoInventoryViewModel(c)));

		// can pass in null for the parent model since we aren't hiring/firing. should make an interface or something though
		CrewList = new ObservableCollection<CrewManagementMemberViewModel>(Globals.GameVars.playerShipVariables.ship.crewRoster
			.Select(c => new CrewManagementMemberViewModel(c, null))
		);

	}
}
