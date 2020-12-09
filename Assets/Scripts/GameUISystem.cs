using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameUISystem : UISystem
{
	// top tier
	[SerializeField] Dashboard Dashboard = null;
	[SerializeField] TitleScreen TitleScreen = null;
	[SerializeField] PortScreen PortScreen = null;
	[SerializeField] TownScreen TownScreen = null;

	// mid tier
	[SerializeField] CityView CityView = null;
	[SerializeField] CrewDetailsScreen CrewDetails = null;
	[SerializeField] MainMenuScreen MainMenu = null;

	// low tier
	[SerializeField] LoanView LoanView = null;
	[SerializeField] RepairsView RepairsView = null;
	[SerializeField] ShrinesView ShrinesView = null;
	[SerializeField] TavernView TavernView = null;
	[SerializeField] InfoScreen InfoScreen = null;
	[SerializeField] QuizScreen QuizScreen = null;
	[SerializeField] QuestScreen QuestScreen = null;
	[SerializeField] TimePassingView TimePassingView = null;
	[SerializeField] DialogScreen DialogScreen = null;

	void AddViews() {
		Add(Dashboard);
		Add(TitleScreen);
		Add(PortScreen);
		Add(TownScreen);
		Add(CityView);
		Add(CrewDetails);
		Add(MainMenu);
		Add(LoanView);
		Add(RepairsView);
		Add(ShrinesView);
		Add(TavernView);
		Add(InfoScreen);
		Add(TimePassingView);
		Add(QuizScreen);
		Add(QuestScreen);
		Add(DialogScreen);
	}

	void Start() {
		AddViews();
		Globals.Register(this);
	}
}
