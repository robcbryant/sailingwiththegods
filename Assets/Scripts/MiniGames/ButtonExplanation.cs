using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonExplanation : MonoBehaviour
{
	public GameObject explanation;
	public TMPro.TextMeshProUGUI text;
	public float edging = 25;

	private RectTransform explanationRect;

	private void Start() 
	{
		explanationRect = explanation.GetComponent<RectTransform>();
	}

	public void DisplayText() 
	{
		//TESTING
		SetExplanationText("TESTING TESTING 1 LINE\nTESTING TESTING 2 LINES\nTESTING TESTING REALLY LONG 3 LINE");

		explanation.SetActive(true);
	}

	public void HideText() 
	{
		explanation.SetActive(false);
	}

	public void SetExplanationText(string toDisplay) 
	{
		text.text = toDisplay;

		float boxWidth = text.preferredWidth + (2 * edging);
		float boxHeight = text.preferredHeight + (2 * edging);

		explanationRect.sizeDelta = new Vector2(boxWidth, boxHeight);
	}
}
