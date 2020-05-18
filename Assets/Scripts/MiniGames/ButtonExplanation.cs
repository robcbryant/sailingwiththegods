using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonExplanation : MonoBehaviour
{
	public Image explanation;
	public TMPro.TextMeshProUGUI text;
	public float edging = 25;
	public float maxWidth;

	private RectTransform explanationRect;
	private bool sizeSet = false;
	private Color explanationCol = Color.white;
	private Color textCol = Color.white;

	private void Start() {
		
	}

	private void OnEnable() 
	{
		if (explanationRect == null) 
		{
			explanationRect = explanation.GetComponent<RectTransform>();
			explanationCol = explanation.color;
			textCol = text.color;
		}


		HideText();
	}

	public void DisplayText() 
	{
		if (!sizeSet) {
			StartCoroutine(SetSize());
		}
		else {
			explanation.gameObject.SetActive(true);
		}
	}

	public void HideText() 
	{
		explanation.gameObject.SetActive(false);
	}

	public void SetExplanationText(string toDisplay) 
	{
		text.text = toDisplay;
		sizeSet = false;

		if (explanationRect == null) {
			explanationRect = explanation.GetComponent<RectTransform>();
		}
	}

	private IEnumerator SetSize() 
	{
		explanation.gameObject.SetActive(true);
		InvisibleExplanation();
		yield return null;

		float boxWidth = Mathf.Min(text.preferredWidth, maxWidth) + (2 * edging);
		explanationRect.sizeDelta = new Vector2(boxWidth, text.fontSize);

		float boxHeight = text.preferredHeight + (2 * edging);
		explanationRect.sizeDelta = new Vector2(boxWidth, boxHeight);
		VisibleExplanation();
		sizeSet = true;
	}

	private void InvisibleExplanation() 
	{
		if (explanationCol == Color.white && textCol == Color.white) 
		{
			explanationCol = explanation.color;
			textCol = text.color;
		}

		explanation.color = Color.clear;
		text.color = Color.clear;
	}

	private void VisibleExplanation() 
	{
		explanation.color = explanationCol;
		text.color = textCol;
	}
}
