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
		rt = GetComponent<RectTransform>();
	}

	public void SetText(string s, RectTransform grandParent, float padding) 
	{
		if (rt == null) 
		{
			rt = GetComponent<RectTransform>();
		}
		text.text = s;
		rt.sizeDelta = new Vector2(grandParent.rect.width - padding, 1);
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, text.preferredHeight);

		RectTransform buttonRect = button.GetComponent<RectTransform>();
	}

	public void SetOnClick(UnityEngine.Events.UnityAction call) {
		button.onClick.AddListener(() => call());
	}
}
