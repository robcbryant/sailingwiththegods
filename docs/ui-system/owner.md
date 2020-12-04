# Owner

It can be difficult to ensure that objects created by a class are cleaned up, it's easy to forget something. C# provides a mechanism for ensuring objects are cleaned up within a block:

```cs
public class Test : MonoBehavior
{
	void Start()
	{
		// the file is opened, and then automatically Disposed at the end of the block
		using var streamReader = new StreamReader("file1.txt");
	}
}
```

SteamReader implmements the IDisposable interface, and the using keyword automatically calls IDisposable.Dispose() for you.

Although this is great for within a block, C# doesn't provide a way to automatically clean something up when an object is destroyed or goes out of scope, which means you have to remember to call Dispose on everything you create, or in Unity you might need to call Destroy for every game object that a class creates internally.

The Owner system is a kind of port of the C++ [RAII (Resource Aquisition is Initialization)](https://en.wikipedia.org/wiki/Resource_acquisition_is_initialization) pattern to C#. Owner tracks references to objects it has ownership of cleaning up, and no other object should be responsible for cleaning up those objects. When an Owner is Disposed with a call to .Dispose(), it disposes all of its owned objects as well. Objects can also be transferred to another owner with the Move() function, which returns the object and removes it from the Owner's tracking.

There's also an OwnerBehaviour which automatically hooks up Owner.Dispose to run on Destroy.

## Example use cases:

```cs
public class Test : OwnerBehaviour
{
	void Start()
	{
		// because you passed this into Own, OwnerBehaviour will dispose it automatically for you in OnDestroy.
		Own(new StreamReader("file1.txt"));

		// this will automatically call Destroy(gameObject) for you on the bullet in OnDestroy
		Own(new GameObject("bullet"));
	}
}
```

## Events

The other major use for the Owner system is to automatically unsubscribe event listeners

```cs
public class Test : OwnerBehaviour
{
	public Button Button;

	void Start()
	{
		// because you used Subscribe, OwnerBehavior will Enable/Disable the listener in OnEnable/OnDisable for you, and will unsubcribe completely in OnDestroy
		Subscribe(Button.onClick, () => Debug.Log("clicked button"));
	}
}
```
