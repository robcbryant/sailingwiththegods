using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// control beacon visibity in one spot since there's a couple factors that go into deciding if it's visible or not
public class Beacon : MonoBehaviour
{
	bool _isBeaconActive = false;
	public bool IsBeaconActive {
		get => _isBeaconActive;
		set {
			_isBeaconActive = value;
			if(value == false) {
				Target = null;
			}
			UpdateBeaconVisibility();
		}
	}

	bool _isTemporarilyHidden = false;
	public bool IsTemporarilyHidden {
		get => _isTemporarilyHidden;
		set {
			_isTemporarilyHidden = value;
			UpdateBeaconVisibility();
		}
	}

	public Settlement Target { get; set; }

	private void Start() {
		UpdateBeaconVisibility();
	}

	void UpdateBeaconVisibility() {
		GetComponent<Renderer>().enabled = _isBeaconActive && !_isTemporarilyHidden;
	}
}
