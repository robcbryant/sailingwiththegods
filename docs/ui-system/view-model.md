# ViewModel-View Binding

As an example, say you want to have a CargoItem view that displays the cargo's name and quantity. The game's representation of cargo is the [Resource class](/Assets/Scripts/DataObjects.cs#L586). We'd need to create a View class and a ViewModel class that drives it.

```cs
public class CargoItemViewModel : Model
{
	Resource Cargo;

	// The Name is not expected to change, so we can do a one-way pull from the Model layer
	public string Name => Cargo.name;

	// The Quantity may change externally at any time, and the amount_kg variable has the ability to notify listeners when it changes
	// IValueModel<int> is a wrapper around the Model layer's value that will let us watch for changes and update the UI automatically
	public IValueModel<int> Quantity;

	// The ViewModel will take in the cargo item that we want the View to display (eg. lumber, food, water)
	public CargoItemViewModel(Resource cargo) {
		Cargo = cargo;

		// and now we set up our Quantity to listen to the Model layer's amount_kg value
		Quantity = ValueModel.Wrap(Cargo.amount_kg);
	}
}
```

And then we need to make a View class to bind the UI to the ViewModel data that drives it:

```cs
public class CargoItemView : ViewBehaviour<CargoItemViewModel>
{
	// These are the nested views for the UGUI Text objects, which are assigned in the Unity inspector
	[SerializeField] StringView Name = null;
	[SerializeField] StringView Quantity = null;

	public override void Bind(CargoItemViewModel model) {
		base.Bind(model);

		// The Name text will pull its data from the Name property of our ViewModel
		Name?.Bind(new BoundModel<string>(Model, nameof(Model.Name)));

		// The Quantity text will pull from the Quantity property, but that's an int, so we transform the BoundModel<int> into a BoundModel<string> 
		Quantity?.Bind(new BoundModel<int>(Model, nameof(Model.Quantity)).AsString());
	}
}
```

Then drop that component onto the GameObject and hook up the inspector references.