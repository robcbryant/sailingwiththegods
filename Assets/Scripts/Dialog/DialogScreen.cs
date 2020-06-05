using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogScreen : MonoBehaviour
{
	public string partnerName = "Tax Collector Bob III";
	public TextMeshProUGUI conversationTitle;
	public Scrollbar conversationScroll;
	public Transform conversationHolder;
	public Transform choiceHolder;
	public DialogChoice choiceObject;
	public DialogPiece dialogObject;

	private CustomDialogUI yarnUI;

	private void OnValidate() 
	{
		yarnUI = GetComponent<CustomDialogUI>();
	}

	public void SetDialogUI(string title) 
	{
		Clear();
		conversationTitle.text = title;
	}

	public void AddToDialogText(string speaker, string text, TextAlignmentOptions align) {
		StartCoroutine(DoAddToDialogText(speaker, text, align));
	}
	
	private IEnumerator DoAddToDialogText(string speaker, string text, TextAlignmentOptions align) 
	{
		DialogPiece p = Instantiate(dialogObject);
		p.SetAlignment(align);
		p.SetText(speaker, text);
		yield return null;
		p.transform.SetParent(conversationHolder);
		yield return null;
		conversationScroll.value = 0;
		yield return null;
		conversationScroll.value = 0;
	}

	public void Testing() 
	{
		int choices = Random.Range(1, 6);
		for (int i = 0; i < choices; i++) 
		{
			AddChoice($"This is choice {i}!", AddRandomDialog);
		}
	}

	public void AddContinueOption() 
	{
		ClearOptions();
		if (!yarnUI.EndOfBlock) {
			AddChoice("Continue", yarnUI.MarkLineComplete);
		}
		else {
			StartCoroutine(WaitAndComplete());
		}
	}

	private IEnumerator WaitAndComplete() {
		yield return null;
		yarnUI.MarkLineComplete();
	}

	public void AddRandomDialog() 
	{
		int speaker = Random.Range(1, 3);
		string name = speaker % 2 == 0 ? "Jason" : partnerName;
		string conversation = $"This is being spoken by {name}. This is some dialog! Blah blah blah. Dialog dialog dialog.";
		TextAlignmentOptions align = TextAlignmentOptions.Left;

		if (speaker % 2 != 0) 
		{
			align = TextAlignmentOptions.Right;
		}
		
		StartCoroutine(DoAddToDialogText(name, conversation, align));
	}

	public void AddChoice(string text, UnityEngine.Events.UnityAction click) 
	{
		DialogChoice c = Instantiate(choiceObject);
		c.SetText(text);
		c.transform.SetParent(choiceHolder);
		c.SetOnClick(click);
	}

	public void Clear() 
	{
		ClearChildren(conversationHolder);
		ClearChildren(choiceHolder);
	}

	public void ClearOptions() 
	{
		ClearChildren(choiceHolder);
	}

	private void ClearChildren(Transform parent) 
	{
		Transform[] objs = parent.GetComponentsInChildren<Transform>();
		foreach (Transform t in objs) 
		{
			if (t != parent) 
			{
				Destroy(t.gameObject);
			}

		}
	}


}

