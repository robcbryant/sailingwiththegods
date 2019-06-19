using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewDetailsScreen : ViewBehaviour<CrewManagementMemberViewModel>
{
	[SerializeField] CrewManagementMemberView CrewMember = null;
	[SerializeField] StringView FlavorText = null;
	[SerializeField] ButtonView CloseButton = null;
	[SerializeField] CityListView Cities = null;

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
