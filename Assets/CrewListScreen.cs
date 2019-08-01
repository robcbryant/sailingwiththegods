using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class CrewListScreen : ViewBehaviour<ICollectionModel<CrewManagementMemberViewModel>>
{
	[SerializeField] CrewManagementListView List = null;
	[SerializeField] ButtonView Close = null;

	public override void Bind(ICollectionModel<CrewManagementMemberViewModel> model) {
		base.Bind(model);

		List?.Bind(model
			.OrderBy(c => Globals.GameVars.Network.GetCrewMemberNetwork(c.Member).Count())
		);
		Close?.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Globals.UI.Hide(this) }));
	}
}
