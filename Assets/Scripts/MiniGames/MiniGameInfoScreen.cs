using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameInfoScreen : MonoBehaviour
{
	public enum MiniGame { Pirates, StormStart, Storm, Start, Finish, Negotiation }

	public TextMeshProUGUI titleText;
	public TextMeshProUGUI subtitleText;
	public TextMeshProUGUI contentText;
	public Scrollbar vertScroll;
	public Image iconIMG;
	public GameObject[] buttons;

	public void DisplayText(string title, string subtitle, string content, Sprite icon, MiniGame type) 
	{
		StartCoroutine(TextDisplay(title, subtitle, content, icon, type));
	}

	private IEnumerator TextDisplay(string title, string subtitle, string content, Sprite icon, MiniGame type) 
	{
		titleText.text = title;
		subtitleText.text = subtitle;
		contentText.text = content;
		iconIMG.sprite = icon;
		ChangeButtons(type);
		yield return null;
		yield return null;
		vertScroll.value = 1;
	}

	private void ChangeButtons(MiniGame type) 
	{
		for (int i = 0; i < buttons.Length; i++) 
		{
			if (i == (int)type) 
			{
				buttons[i].SetActive(true);
			}
			else 
			{
				buttons[i].SetActive(false);
			}
		}
	}

	public void AddToText(string add) {
		contentText.text += add;
	}

	public void CloseDialog() 
	{
		gameObject.SetActive(false);
	}
}
