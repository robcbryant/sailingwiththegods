using UnityEngine;
using System.Collections;

// TODO: Replace this with the new tooltip system.
public class script_settlement_functions : MonoBehaviour
{

	public Settlement thisSettlement;
	GameVars GameVars;


	void Start() {
		GameVars = Globals.GameVars;
	}

	public void ActivateHighlightOnMouseOver() {
		//child selection ring to current settlement
		GameVars.selection_ring.transform.SetParent(transform);
		//set the ring to the origin coordinates of the settlement
		GameVars.selection_ring.transform.localPosition = new Vector3(0, 2, 0);
		//turn the ring on
		GameVars.selection_ring.SetActive(true);

		GameVars.currentSettlementGameObject = gameObject;
		GameVars.currentSettlement = thisSettlement;

	}
}
