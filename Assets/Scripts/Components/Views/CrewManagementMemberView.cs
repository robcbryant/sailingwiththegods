using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CrewManagementMemberViewModel : ViewModel
{
	private const string ResourcePath = "crew_portraits";
	private const string DefaultPortrait = "crew_portraits/phoenician_sailor";

	private CrewMember Member;
	
	public Sprite Portrait { get; private set; }
	public string Name => Member.name;
	public string City => Globals.GameVars.GetSettlementFromID(Member.originCity).name;
	public string Job => Globals.GameVars.GetJobClassEquivalency(Member.typeOfCrew);

	public string Role => Job + "\n" + Skills;

	public string Skills {
		get {

		}
	}

	//TODO Temporary solution--need to add a clout check modifier
	public int Cost => Member.clout * 2;


	public CrewManagementMemberViewModel(CrewMember member) {
		Member = member;

		Portrait = Resources.Load<Sprite>(ResourcePath + "/" + member.ID) ?? Resources.Load<Sprite>(DefaultPortrait);
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
	[SerializeField] StringView Salary;

	private void Start() {
		if (InfoButton != null) {
			Subscribe(InfoButton.onClick, ShowInfo);
		}
	}

	public override void Bind(CargoInventoryViewModel model) {
		base.Bind(model);

		Amount?.Bind(new BoundModel<int>(Model, nameof(Model.AmountKg)).AsString());
		Name?.Bind(new BoundModel<string>(Model, nameof(Model.Name)));
		Icon?.Bind(new BoundModel<Sprite>(Model, nameof(Model.Icon)));
	}

	void ShowInfo() {
		Debug.Log("Cargo - " + Model.Name + " - " + Model.AmountKg + " kg");
	}
}
