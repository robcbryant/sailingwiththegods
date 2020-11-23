using UnityEngine;

/// <summary>
/// At runtime, swap out with a fallback if the proprietary assets are missing
/// We want the game to be runnable and buildable if you don't have the _Proprietary submodule 
/// so that the open source verison of the game can be fully functional.
/// Can be used to swap between two active game objects, or you can use FallbackSwap.HasProprietaryAssets anywhere in the code.
/// </summary>
public class FallbackSwap : MonoBehaviour
{
	[SerializeField] GameObject _proprietary = null;
	[SerializeField] GameObject _fallback = null;

	public static bool HasProprietaryAssets {
		get {
			if(_hasProprietaryAssets == null) {
				_hasProprietaryAssets = new Cache();
			}
			return _hasProprietaryAssets.Value;
		}
	}

	static Cache _hasProprietaryAssets;
	class Cache
	{
		public bool Value { get; private set; }
		public Cache() {
			var marker = Resources.Load<TextAsset>("proprietary");
			Value = marker != null;
		}
	}

	void Start() {

		if(_proprietary != null)
		{
			_proprietary.SetActive(HasProprietaryAssets);
		}

		if(_fallback != null)
		{
			_fallback.SetActive(!HasProprietaryAssets);
		}
	}
}