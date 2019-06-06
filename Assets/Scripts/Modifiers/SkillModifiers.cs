using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SkillModifiers
{
	public int Battle;
	public int Navigation;
	public int PositiveEvent;
	public int ShipSpeed;
	public int CitiesInNetwork;

	public override string ToString() =>
		MakeLine(BattleStr) +
		MakeLine(NavigationStr) +
		MakeLine(PositiveEventStr) +
		MakeLine(ShipSpeedStr) +
		MakeLine(CitiesInNetworkStr);

	private string MakeLine(string str) => string.IsNullOrEmpty(str) ? "" : str + "\n";

	public string BattleStr => MakeChange(Battle + "% in Battle", Battle);
	public string NavigationStr => MakeChange(Battle + "% Navigation", Battle);
	public string PositiveEventStr => MakeChange(Battle + "% Forsight", Battle);
	public string ShipSpeedStr => MakeChange(ShipSpeed + "% Ship Speed", ShipSpeed);
	public string CitiesInNetworkStr => MakeChange(CitiesInNetwork + " Cities in Network", CitiesInNetwork);

	public string MakeChange(string str, int val) {
		if (val > 0) return MakeGreen("+" + str);
		else if (val < 0) return MakeRed("-" + str);
		else return "";
	}

	public string MakeGreen(string str) => "<#008800>" + str + "</color>";
	public string MakeRed(string str) => "<#880000>" + str + "</color>";


}
