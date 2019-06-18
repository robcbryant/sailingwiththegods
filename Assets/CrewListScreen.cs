using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class CrewListScreen : ViewBehaviour<ObservableCollection<CrewManagementMemberViewModel>>
{
	[SerializeField] CrewManagementListView List;
	[SerializeField] ButtonView Close;

	public override void Bind(ObservableCollection<CrewManagementMemberViewModel> model) {
		base.Bind(model);

		List?.Bind(model);
		Close?.Bind(ValueModel.New(new ButtonViewModel { OnClick = () => Globals.UI.Hide(this) }));
	}
}
