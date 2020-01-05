using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class QuizScreenModel : Model
{
	private string _Title;
	public string Title { get => _Title; set { _Title = value; Notify(); } }

	private string _Message;
	public string Message { get => _Message; set { _Message = value; Notify(); } }

	private Sprite _Icon;
	public Sprite Icon { get => _Icon; set { _Icon = value; Notify(); } }

	public readonly ICollectionModel<ButtonViewModel> Choices;

	public QuizScreenModel(string title, string message, Sprite icon, ObservableCollection<ButtonViewModel> choices) {
		Title = title;
		Message = message;
		Icon = icon;
		Choices = ValueModel.Wrap(choices);
	}
}

public class QuizScreen : ViewBehaviour<QuizScreenModel>
{
	[SerializeField] ImageView Icon = null;
	[SerializeField] StringView Title = null;
	[SerializeField] StringView Message = null;
	[SerializeField] ButtonView[] Choices = null;

	public override void Bind(QuizScreenModel model) {
		base.Bind(model);

		if (model == null) {
			Debug.LogWarning("Tried to bind view to a null model on " + name);
			return;
		}

		if (model.Icon != null) {
			Icon?.Bind(new BoundModel<Sprite>(model, nameof(model.Icon)));
		}

		Title?.Bind(new BoundModel<string>(Model, nameof(Model.Title)));
		Message?.Bind(new BoundModel<string>(Model, nameof(Model.Message)));

		// TODO: populate from the collection directly
		Choices[0]?.Bind(ValueModel.New(model.Choices.ElementAtOrDefault(0)));
		Choices[1]?.Bind(ValueModel.New(model.Choices.ElementAtOrDefault(1)));
		Choices[2]?.Bind(ValueModel.New(model.Choices.ElementAtOrDefault(2)));
		Choices[3]?.Bind(ValueModel.New(model.Choices.ElementAtOrDefault(3)));
		Choices[4]?.Bind(ValueModel.New(model.Choices.ElementAtOrDefault(4)));
	}
}
