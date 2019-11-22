using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizScreen : ViewBehaviour<InfoScreenModel>
{
	[SerializeField] ImageView Icon = null;
	[SerializeField] StringView Title = null;
	[SerializeField] StringView Subtitle = null;
	[SerializeField] StringView Message = null;
	[SerializeField] ButtonView[] Choices = null;

	public override void Bind(InfoScreenModel model) {
		base.Bind(model);

		if (model == null) {
			Debug.LogWarning("Tried to bind view to a null model on " + name);
			return;
		}

		if (model.Icon != null) {
			Icon?.Bind(new BoundModel<Sprite>(model, nameof(model.Icon)));
		}

		Subtitle?.Bind(new BoundModel<string>(model, nameof(Model.Subtitle)));
		Title?.Bind(new BoundModel<string>(Model, nameof(Model.Title)));
		Message?.Bind(new BoundModel<string>(Model, nameof(Model.Message)));

		Choices[0].Bind(ValueModel.New(new ButtonViewModel { Label = "Pray to Athena", OnClick = () => { } }));
		Choices[1].Bind(ValueModel.New(new ButtonViewModel { Label = "Apologize to Zeus", OnClick = () => { } }));
		Choices[2].Bind(ValueModel.New(new ButtonViewModel { Label = "Release a trembling dove", OnClick = () => { } }));
		Choices[3].Bind(ValueModel.New(new ButtonViewModel { Label = "Row as if your lives and fame and ship depended on it", OnClick = () => { } }));
		Choices[4].Bind(ValueModel.New(new ButtonViewModel { Label = "Ask the ship’s seer to perform a sacrifice to Poseidon", OnClick = () => { } }));
	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		if (Icon != null) {
			Icon.transform.localScale = Vector3.one * Model.IconScale;
		}
	}
}
