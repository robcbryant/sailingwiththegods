using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogChoice : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private Button button;
#pragma warning restore 0649
	private RectTransform rt;

	private void Start() 
	{
		//text = GetComponent<TextMeshProUGUI>();
		//b = GetComponentInChildren<Button>();
		rt = GetComponent<RectTransform>();
	}

	public void SetText(string s, RectTransform grandParent) 
	{
		if (rt == null) 
		{
			//text = GetComponent<TextMeshProUGUI>();
			//b = GetComponentInChildren<Button>();
			rt = GetComponent<RectTransform>();
		}
		text.text = s;
		rt.sizeDelta = new Vector2(text.preferredWidth, rt.sizeDelta.y);

		RectTransform buttonRect = button.GetComponent<RectTransform>();

		float parentWidth = grandParent.rect.width;
		if (parentWidth > text.preferredWidth) {
			buttonRect.offsetMax = new Vector2(Mathf.Abs(text.preferredWidth - parentWidth), buttonRect.offsetMax.y);
		}

		Debug.Log($"{text.preferredWidth} minus {parentWidth} is {text.preferredWidth - parentWidth}; current width is {buttonRect.offsetMax.x}");
	}

	public void SetOnClick(UnityEngine.Events.UnityAction call) {
		button.onClick.AddListener(() => call());
	}
}
