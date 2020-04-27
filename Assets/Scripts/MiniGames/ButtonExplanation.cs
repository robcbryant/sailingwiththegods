using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonExplanation : MonoBehaviour
{
	public GameObject explanation;
	public TMPro.TextMeshProUGUI text;
	public float edging = 25;
	public float maxWidth;

	private RectTransform explanationRect;

	private void OnEnable() 
	{
		explanationRect = explanation.GetComponent<RectTransform>();
		HideText();
	}

	public void DisplayText() 
	{
		explanation.SetActive(true);
		StartCoroutine(SetSize());
	}

	public void HideText() 
	{
		explanation.SetActive(false);
	}

	public void SetExplanationText(string toDisplay) 
	{
		text.text = toDisplay;

		if (explanationRect == null) {
			explanationRect = explanation.GetComponent<RectTransform>();
		}
	}

	private IEnumerator SetSize() 
	{
		yield return null;

		float boxWidth = Mathf.Min(text.preferredWidth, maxWidth) + (2 * edging);
		explanationRect.sizeDelta = new Vector2(boxWidth, text.fontSize);

		float boxHeight = text.preferredHeight + (2 * edging);
		explanationRect.sizeDelta = new Vector2(boxWidth, boxHeight);
	}
}
