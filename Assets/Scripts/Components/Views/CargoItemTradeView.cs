using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CargoItemTradeView : ViewBehaviour<CargoItemTradeViewModel>
{
	InteractableBehaviour Interactable;

	[SerializeField] StringView Name;
	[SerializeField] StringView Price;
	[SerializeField] StringView Amount;
	[SerializeField] StringView Hint;
	[SerializeField] ImageView Icon;

	private void Awake() {
		Interactable = GetComponent<InteractableBehaviour>();
	}

	private void Start() {
		if (Interactable != null) {
			Subscribe(Interactable.PointerClick, Clicked);
		}
	}

	public override void Bind(CargoItemTradeViewModel model) {
		base.Bind(model);

		Amount?.Bind(new BoundModel<int>(Model, nameof(Model.AmountKg)).AsString());
		Name?.Bind(new BoundModel<string>(Model, nameof(Model.Name)));
		Icon?.Bind(new BoundModel<Sprite>(Model, nameof(Model.Icon)));
		Price?.Bind(new BoundModel<string>(Model, nameof(Model.PriceStr)));
		Hint?.Bind(new BoundModel<string>(Model, nameof(Model.HintStr)));
	}

	void Clicked() {
		Model.Select();
	}
}
