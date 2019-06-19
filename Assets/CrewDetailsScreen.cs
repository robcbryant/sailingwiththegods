using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewDetailsScreen : ViewBehaviour<CrewManagementMemberViewModel>
{
	[SerializeField] CrewManagementMemberView CrewMember;
	[SerializeField] StringView FlavorText;
	[SerializeField] ButtonView CloseButton;
	[SerializeField] CityListView Cities;

	public override void Bind(CrewManagementMemberViewModel model) {
		base.Bind(model);

		CrewMember?.Bind(model);
		Cities?.Bind(model.CitiesInNetwork);
		FlavorText?.Bind(ValueModel.New(model.BackgroundInfo));

		CloseButton?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = Globals.UI.Hide<CrewDetailsScreen>
		}));
	}
}
