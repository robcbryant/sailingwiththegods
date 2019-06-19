using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

// TODO: Rename to SettlementComponent or something
public class script_settlement_functions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	bool IsTooltipPendingHide;

	public Settlement thisSettlement;
	GameVars GameVars;


	void Start() {
		GameVars = Globals.GameVars;
	}

	// TODO: Remove this once we're sure we don't want any of this selection_ring code
	/*
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
	*/

	public void OnPointerEnter(PointerEventData eventData) {
		TryShowTooltip();
	}

	public void OnPointerExit(PointerEventData eventData) {
		IsTooltipPendingHide = !TryHideTooltip(false);
	}

	void TryShowTooltip() {
		if (!Globals.UI.IsShown<CityView>()) {
			var ui = Globals.UI.Show<CityView, CityViewModel>(new CityViewModel(thisSettlement));
			ui.transform.position = Globals.UI.WorldToUI(GameVars.FPVCamera.GetComponent<Camera>(), transform.position);
		}
	}

	bool TryHideTooltip(bool requirePendingHide) {

		// allow he pointer to hover over the tooltip without closing it so we can make it clickable and not flicker on the edge
		if (Globals.UI.IsShown<CityView>() && UISystem.IsMouseOverUI(Globals.UI.Get<CityView>().GetComponent<Graphic>())) {
			return false;
		}
		else if(IsTooltipPendingHide || !requirePendingHide) {
			Globals.UI.Hide<CityView>();
			IsTooltipPendingHide = false;
			return true;
		}
		else {
			return false;
		}

	}

	void Update() {

		// hide the tooltip after mousing over it and mousing off (so we missed settlement.OnPointerExit)
		TryHideTooltip(true);

	}
}
