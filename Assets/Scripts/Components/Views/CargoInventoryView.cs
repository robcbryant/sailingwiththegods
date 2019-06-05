using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CargoInventoryViewModel : ViewModel
{
	private Resource Resource;

	private int _AmountKg;
	public int AmountKg {
		get => _AmountKg;
		set {
			_AmountKg = value;
			Resource.amount_kg = value;
			Notify();
		}
	}
	
	public string Name => Resource.name;
	public float ProbabilityOfAvailability => Resource.probabilityOfAvailability;       // TODO: This is only set/relevant for resources on a settlement obj, not in the player's ship
	public Sprite Icon => Resource.icon;

	public CargoInventoryViewModel(Resource resource) {
		Resource = resource;
	}
}

public class CargoInventoryView : ViewBehaviour<CargoInventoryViewModel>
{
	[SerializeField] Button InfoButton;
	[SerializeField] ImageView Icon;					// TODO: Should this be removed? Never set in code i think, it's baked into the row.
	[SerializeField] StringView Name;
	[SerializeField] StringView Amount;

	private void Start() {
		if(InfoButton != null) {
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
