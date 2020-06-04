using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogChoice : MonoBehaviour
{
	private TextMeshProUGUI text;
	private Button b;

	private void Start() 
	{
		text = GetComponent<TextMeshProUGUI>();
		b = GetComponentInChildren<Button>();
	}

	public void SetText(string s) 
	{
		if (text == null) 
		{
			text = GetComponent<TextMeshProUGUI>();
			b = GetComponentInChildren<Button>();
		}
		text.text = s;
	}

	public void SetOnClick(UnityEngine.Events.UnityAction call) {
		b.onClick.AddListener(() => call());
	}
}
