using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogScreen : MonoBehaviour
{
	public Image icon;
	public TextMeshProUGUI conversationTitle;
	public TextMeshProUGUI partnerName;
	public TextMeshProUGUI conversationText;
	public Transform choiceHolder;
	public DialogChoice choiceObject;

	public void SetDialogUI(Sprite img, string title, string name, string text) 
	{
		icon.sprite = img;
		conversationTitle.text = title;
		partnerName.text = name;
		conversationText.text = text;
	}

	public void SetDialogIcon(Sprite img) 
	{
		icon.sprite = img;
	}

	public void SetDialogText(string text) 
	{
		conversationText.text = text;
	}

	public void AddToDialogText(string text) 
	{
		conversationText.text += text;
	}

	public void AddChoice(string text) 
	{
		DialogChoice c = Instantiate(choiceObject);
		c.SetText(text);
		c.transform.SetParent(choiceHolder);
	}

	public void Clear() 
	{
		conversationText.text = "";
		ClearChoices();
	}

	private void ClearChoices() 
	{
		Transform[] objs = choiceHolder.GetComponentsInChildren<Transform>();
		foreach (Transform t in objs) 
		{
			Destroy(t.gameObject);
		}
	}


}

