using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SkillModifiers
{
	public int BattlePercentChance;
	public int Navigation;
	public int PositiveEvent;
	//public int ShipSpeed;		// this one has a very complicated formula. doesn't seem intuitive enough to expose to the player
	public int CitiesInNetwork;

	public override string ToString() =>
		MakeLine(BattleStr) +
		MakeLine(NavigationStr) +
		MakeLine(PositiveEventStr) +
		//MakeLine(ShipSpeedStr) +
		MakeLine(CitiesInNetworkStr);

	private string MakeLine(string str) => string.IsNullOrEmpty(str) ? "" : str + "\n";

	public string BattleStr => MakeChange(BattlePercentChance + "% in Battle", BattlePercentChance);
	public string NavigationStr => MakeChange(Navigation + "% Navigation", Navigation);
	public string PositiveEventStr => MakeChange(PositiveEvent + "% Forsight", PositiveEvent);
	//public string ShipSpeedStr => MakeChange(ShipSpeed + "% Ship Speed", ShipSpeed);
	public string CitiesInNetworkStr => MakeChange(CitiesInNetwork + " Cities in Network", CitiesInNetwork);

	public string MakeChange(string str, int val) {
		if (val > 0) return MakeGreen("+" + str);
		else if (val < 0) return MakeRed(str);
		else return "";
	}

	public string MakeGreen(string str) => "<#008800>" + str + "</color>";
	public string MakeRed(string str) => "<#880000>" + str + "</color>";


}
