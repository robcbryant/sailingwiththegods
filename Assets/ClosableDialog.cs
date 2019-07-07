using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosableDialog : OwnerBehaviour
{
	[SerializeField] Button Button;

	private void Start() {
		Subscribe(Button.onClick, () => 
			Globals.UI.Hide(GetComponent<ViewBehaviour>())
		);
	}
}
