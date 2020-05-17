using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
		text.text = s;
	}

	public void AdvanceConversation() 
	{
		//whatever we do goes in here
	}
}
