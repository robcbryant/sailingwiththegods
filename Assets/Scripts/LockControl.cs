using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CutsceneMode
{
	public static void Enter() {
		Globals.GameVars.IsCutsceneMode = true;
		Globals.UI.HideAll();
	}

	public static void Exit() {
		Globals.GameVars.IsCutsceneMode = false;
		Globals.UI.Show<Dashboard, DashboardViewModel>(new DashboardViewModel());
	}
}
