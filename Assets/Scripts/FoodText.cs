using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodText
{
	public enum Type {
		Food,
		Quote,
		Wine
	}
	private string source, chapter, item, quote, speaker, scenario = "This is EMPTY what are you DOING I am going to LOSE IT";
	private int foodCost = 0;
	private Type textType;


	public FoodText() { }

	public string GetQuote 
	{
		get { return string.Format("{0} ({1} {2})", quote, source, chapter); }
	}

	public int FoodCost 
	{
		get { return foodCost; }
		set { foodCost = value; }
	}
	public Type TextType 
	{
		get { return textType; }
		set { textType = value; }
	}
	public string Source
	{
		get { return source; }
		set { source = value; }
	}

	public string Chapter 
	{
		get { return chapter; }
		set { chapter = value; }
	}

	public string Item 
	{
		get { return item; }
		set { item = value; }
	}

	public string Quote 
	{
		get { return quote; }
		set { quote = value; }
	}

	public string Speaker 
	{
		get { return speaker; }
		set { speaker = value; }
	}

	public string Scenario 
	{
		get { return scenario; }
		set { scenario = value; }
	}

}
