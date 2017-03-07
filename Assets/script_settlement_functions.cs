using UnityEngine;
using System.Collections;

public class script_settlement_functions : MonoBehaviour {

	public Settlement thisSettlement;
	globalVariables MGV;
	// Use this for initialization
	void Start () {
	MGV = GameObject.FindGameObjectWithTag("global_variables").GetComponent<globalVariables>();
	
	}
	
	
	
	public void ActivateHighlightOnMouseOver(){
	//child selection ring to current settlement
	MGV.selection_ring.transform.SetParent(transform);
	//set the ring to the origin coordinates of the settlement
	MGV.selection_ring.transform.localPosition = new Vector3(0,2,0);
	//turn the ring on
	MGV.selection_ring.SetActive(true);
	
	MGV.currentSettlementGameObject = gameObject;
	MGV.currentSettlement = thisSettlement;
	
	}
}
