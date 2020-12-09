# Registry

Registry is an implementation of the service locator pattern and is really just a replacement for static singleton ClassName.Instance style access for global things. It keeps everything scoped so that for ex. a scene load will wipe out and recreate the registry. Static singletons tend to need to be cleaned up manually in between scenes.

## Example usage

```cs
public class StartupInitialization : OwnerBehaviour
{
	public Registry Managers;

	void Start()
	{
		Managers.Register(new InputManager());
		Managers.Register(new EffectsManager());
	}
}

public class GameSystem : OwnerBehaviour
{
	void Start()
	{
		var input = StartupInitialization.Registry.Get<InputManager>();
		Debug.Log("input manager located: " + input.name);
	}
}
```