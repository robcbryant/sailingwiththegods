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
			//I know this looks like it shouldn't need the absolute value, because it shouldn't, but trust me, it does
			//For whatever reason, the object in the scene flips the sign of the offset
			//This should *always* evaluate to a negative number, so in the scene it's always positive
			//But if this is *positive*, it's negative in the scene like it needs to be
			//I completely don't understand what's going on here, but this way works
			buttonRect.offsetMax = new Vector2(Mathf.Abs(text.preferredWidth - parentWidth), buttonRect.offsetMax.y);
		}
	}

	public void SetOnClick(UnityEngine.Events.UnityAction call) {
		button.onClick.AddListener(() => call());
	}
}
