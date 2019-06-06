using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

public class PortViewModel : ViewModel
{
	Settlement Settlement => Globals.GameVars.currentSettlement;

	public string PortName => Settlement.name;
	public readonly CrewManagementViewModel CrewManagement;

	public PortViewModel() {
		CrewManagement = new CrewManagementViewModel();
	}
}
