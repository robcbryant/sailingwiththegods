using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogChoice : MonoBehaviour
{
	private TextMeshProUGUI text;
	private Button b;
	public DialogScreen ds;

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

	public void AdvanceConversation() 
	{
		ds.AddRandomDialog();
	}
}
