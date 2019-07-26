using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreenModel : Model
{
	private string _Title;
	public string Title { get => _Title; set { _Title = value; Notify(); } }

	private string _Subtitle;
	public string Subtitle { get => _Subtitle; set { _Subtitle = value; Notify(); } }

	private string _Message;
	public string Message { get => _Message; set { _Message = value; Notify(); } }

	private Sprite _Icon;
	public Sprite Icon { get => _Icon; set { _Icon = value; Notify(); } }

	private float _IconScale = 1;
	public float IconScale { get => _IconScale; set { _IconScale = value; Notify(); } }
}

public class InfoScreen : ViewBehaviour<InfoScreenModel>
{
	[SerializeField] ImageView Icon = null;
	[SerializeField] StringView Title = null;
	[SerializeField] StringView Subtitle = null;
	[SerializeField] StringView Message = null;

	public override void Bind(InfoScreenModel model) {
		base.Bind(model);

		if (model == null) {
			Debug.LogWarning("Tried to bind view to a null model on " + name);
			return;
		}

		Icon?.Bind(new BoundModel<Sprite>(model, nameof(model.Icon)));
		Subtitle?.Bind(new BoundModel<string>(model, nameof(Model.Subtitle)));
		Title?.Bind(new BoundModel<string>(Model, nameof(Model.Title)));
		Message?.Bind(new BoundModel<string>(Model, nameof(Model.Message)));
	}

	protected override void Refresh(object sender, string propertyChanged) {
		base.Refresh(sender, propertyChanged);

		if(Icon != null) {
			Icon.transform.localScale = Vector3.one * Model.IconScale;
		}
	}
}
