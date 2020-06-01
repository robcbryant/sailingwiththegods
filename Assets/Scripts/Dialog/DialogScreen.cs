using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogScreen : MonoBehaviour
{
	public enum Emotion
	{
		Neutral, Welcoming, Superstitious, Grumpy, Angry, Happy, Pirate, Foreigner, WellBred, HatesSea, Patriotic, Swindler
	}
	public Image jasonIcon;
	public Image otherIcon;
	public Sprite[] emotionFaces = new Sprite[12];
	public TextMeshProUGUI conversationTitle;
	public Scrollbar conversationScroll;
	public Transform conversationHolder;
	public Transform choiceHolder;
	public DialogChoice choiceObject;
	public DialogPiece dialogObject;

	private void Start() 
	{
		SetDialogUI(emotionFaces[0], "Test Conversation");
		Testing();
	}

	public void SetDialogUI(Sprite other, string title) 
	{
		Clear();
		otherIcon.sprite = other;
		conversationTitle.text = title;
	}

	public void ChangeOtherEmotion(Emotion mood) 
	{
		otherIcon.sprite = emotionFaces[(int)mood];
	}

	public void ChangeOtherEmotion(string mood) 
	{
		System.Enum.TryParse(mood, out Emotion e);
		ChangeOtherEmotion(e);
	}

	public void SetJasonSprite(Sprite img) 
	{
		jasonIcon.sprite = img;
	}

	public IEnumerator AddToDialogText(string speaker, string text, TextAlignmentOptions align) 
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
			AddChoice($"This is choice {i}!");
		}
	}

	public void AddRandomDialog() 
	{
		int speaker = Random.Range(1, 3);
		string name = speaker % 2 == 0 ? "Jason" : "Tax Collector Bob the III";
		string conversation = $"This is being spoken by {name}. This is some dialog! Blah blah blah. Dialog dialog dialog.";
		TextAlignmentOptions align = TextAlignmentOptions.Left;

		if (speaker % 2 != 0) 
		{
			align = TextAlignmentOptions.Right;
			int moodIndex = Random.Range(0, System.Enum.GetNames(typeof(Emotion)).Length);
			if (otherIcon.sprite != emotionFaces[moodIndex]) 
			{
				conversation += $"\n\nSomething you said changed my mood, and now I'm {(Emotion)moodIndex}";
				ChangeOtherEmotion((Emotion)moodIndex);
			}
		}
		
		StartCoroutine(AddToDialogText(name, conversation, align));
	}

	public void AddChoice(string text) 
	{
		DialogChoice c = Instantiate(choiceObject);
		c.ds = this;
		c.SetText(text);
		c.transform.SetParent(choiceHolder);
	}

	public void Clear() 
	{
		ClearChildren(conversationHolder);
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

