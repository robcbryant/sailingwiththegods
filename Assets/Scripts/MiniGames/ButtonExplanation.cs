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
		//string[] lines = toDisplay.Split('\n');
		//float lineHeight = text.fontSize;

		//float boxHeight = lines.Length * lineHeight + (2 * edging);
		//float boxWidth = lines[0].Length * lineHeight + (2 * edging);

		//Debug.Log($"{lines.Length} lines at {lineHeight} size plus 2 * {edging} = {boxHeight}");
		//Debug.Log($"line 1 is {lines[0].Length} characters at {lineHeight} size plus 2 * {edging} = {boxWidth}");

		text.text = toDisplay;

		float boxWidth = text.preferredWidth + (2 * edging);
		float boxHeight = text.preferredHeight + (2 * edging);

		explanationRect.sizeDelta = new Vector2(boxWidth, boxHeight);
	}
}
