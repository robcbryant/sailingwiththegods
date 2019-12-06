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
		Cities?.Bind(model.CitiesInNetwork);			// TODO: This is null because we passed the model down from the tooltip which doesn't have this...
		FlavorText?.Bind(ValueModel.New(model.BackgroundInfo));

		CloseButton?.Bind(ValueModel.New(new ButtonViewModel {
			OnClick = () => {

				// the city tooltip might be up from clicking a city in a crew member's network
				if (Globals.UI.IsShown<CityView>()) {
					Globals.UI.Hide<CityView>();
				}

				Globals.UI.Hide<CrewDetailsScreen>();

			}
		}));
	}
}
