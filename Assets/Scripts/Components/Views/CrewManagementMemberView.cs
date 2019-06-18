using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CrewManagementMemberViewModel : Model
{
	private const string ResourcePath = "crew_portraits";
	private const string DefaultPortrait = "crew_portraits/phoenician_sailor";

	public CrewMember Member { get; private set; }
	public CrewManagementViewModel Parent { get; private set; }
	
	public Sprite Portrait { get; private set; }
	public string Name => Member.name;
	public string City => Globals.GameVars.GetSettlementFromID(Member.originCity).name;
	public string Job => Globals.GameVars.GetJobClassEquivalency(Member.typeOfCrew);
	public string BackgroundInfo => Member.backgroundInfo;

	public string Role => "<#000000>" + Job + "</color>" + "\n" + Skills;

	public bool IsInCrew => Globals.GameVars.playerShipVariables.ship.crewRoster.Contains(Member);
	public string Skills => IsInCrew ? Member.changeOnFire.ToString() : Member.changeOnHire.ToString();

	public string CitiesInNetwork => Member.currentContribution.CitiesInNetworkStr;

	//TODO Temporary solution--need to add a clout check modifier
	public int CostToHire => Member.clout * 2;


	public CrewManagementMemberViewModel(CrewMember member, CrewManagementViewModel parent) {
		Member = member;
		Parent = parent;

		Portrait = Resources.Load<Sprite>(ResourcePath + "/" + member.ID) ?? Resources.Load<Sprite>(DefaultPortrait);
	}

	public void DoAction() {
		if(IsInCrew) {
			Parent.GUI_FireCrewMember(this);
		}
		else {
			Parent.GUI_HireCrewMember(this);
		}
	}
}

public class CrewManagementMemberView : ViewBehaviour<CrewManagementMemberViewModel>
{ 
	[SerializeField] ButtonView InfoButton;
	[SerializeField] ButtonView ActionButton;
	[SerializeField] ImageView Portrait;
	[SerializeField] StringView Name;
	[SerializeField] StringView City;
	[SerializeField] StringView Skills;
	[SerializeField] StringView Cost;
	[SerializeField] StringView CitiesContributed;

	public override void Bind(CrewManagementMemberViewModel model) {
		base.Bind(model);

		InfoButton?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => Debug.Log("Clicked info button for " + Model.Name)
		}));

		ActionButton?.Bind(ValueModel.New(new ButtonViewModel {
			Label = model.IsInCrew ? "Fire" : "Hire",
			OnClick = model.DoAction
		}));

		Name?.Bind(new BoundModel<string>(Model, nameof(Model.Name)));
		City?.Bind(new BoundModel<string>(Model, nameof(Model.City)));
		Skills?.Bind(new BoundModel<string>(Model, nameof(Model.Role)));
		Cost?.Bind(new BoundModel<int>(Model, nameof(Model.CostToHire)).AsString());
		Portrait?.Bind(new BoundModel<Sprite>(Model, nameof(Model.Portrait)));
		CitiesContributed?.Bind(new BoundModel<string>(Model, nameof(Model.CitiesInNetwork)));
	}
}
