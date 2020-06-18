using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogText
{
	public enum Type
	{
		greeting,
		response,
		farewell
	}

	public enum Emotion
	{
		neutral,
		happy,
		angry
	}

	private Type textType;
	private Emotion textEmotion;
	private string text;

	public DialogText(Type t, Emotion e, string s) 
	{
		textType = t;
		textEmotion = e;
		text = s;
	}

	public DialogText(string t, string e, string s) 
	{
		textType = (Type)System.Enum.Parse(typeof(Type), t);
		textEmotion = (Emotion)System.Enum.Parse(typeof(Emotion), e);
		text = s;
	}

	public Type TextType 
	{
		get { return textType; }
	}

	public Emotion TextEmotion 
	{
		get { return textEmotion; }
	}

	public string Text 
	{
		get { return text; }
	}
}
