using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

public class DashboardViewModel : ViewModel
{
	public string CaptainsLog => Globals.GameVars.currentCaptainsLog;
	public readonly CargoInventoryViewModel WaterInventory;
	public readonly CargoInventoryViewModel FoodInventory;
	public readonly ObservableCollection<CargoInventoryViewModel> CargoList;

	public DashboardViewModel() {

		var water = Globals.GameVars.playerShipVariables.ship.cargo.FirstOrDefault(r => r.name == Resource.Water);
		WaterInventory = new CargoInventoryViewModel(water);

		var food = Globals.GameVars.playerShipVariables.ship.cargo.FirstOrDefault(r => r.name == Resource.Provisions);
		FoodInventory = new CargoInventoryViewModel(food);

		CargoList = new ObservableCollection<CargoInventoryViewModel>(Globals.GameVars.playerShipVariables.ship.cargo.Select(c => new CargoInventoryViewModel(c)));

	}
}
