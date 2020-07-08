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
	public GameObject dialogSpacer;

	private CustomDialogUI yarnUI;
	private InMemoryVariableStorage storage;


	private string[] DialogInitializer(string prefix, int length) 
	{
		string[] dialog = new string[length];

		for (int i = 0; i < length; i++) 
		{
			dialog[i] = prefix + i;
		}

		return dialog;
	}

	private void OnValidate() 
	{
		yarnUI = GetComponent<CustomDialogUI>();
		storage = GetComponent<InMemoryVariableStorage>();
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
		p.transform.SetSiblingIndex(conversationHolder.childCount - 2);
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
		Instantiate(dialogSpacer).transform.SetParent(conversationHolder);
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

	[YarnCommand("reset")]
	public void ResetConversation() {
		storage.SetValue("$random_text", new Yarn.Value("Random text"));
		storage.SetValue("$random_bool", new Yarn.Value(false));
		storage.SetValue("$convo_title", new Yarn.Value("Convertation Title"));
		storage.SetValue("$emotion", new Yarn.Value("neutral"));
		Clear();
	}

	[YarnCommand("setconvotitle")]
	public void SetConversationTitle(string title) {
		string text = title.Replace('_', ' ');
		conversationTitle.text = text;
	}

	[YarnCommand("randomtext")]
	public void GenerateRandomText(string[] inputs) 
	{
		System.Enum.TryParse(inputs[0], out DialogText.Type t);
		DialogText.Emotion e = DialogText.Emotion.neutral;
		if (inputs[1] == "any") {
			e = DialogText.RandomEmotion();
		}
		else {
			System.Enum.TryParse(inputs[1], out e);
		}

		List<DialogText> matchingType = Globals.GameVars.portDialogText.FindAll(x => x.TextType == t);
		List<DialogText> matchingBoth = matchingType.FindAll(x => x.TextEmotion == e);

		if (matchingBoth.Count == 0) {
			Debug.Log($"Nothing found with both type {t.ToString()} and emotion {e.ToString()} ({matchingType.Count} matching just type)");
		}

		int i = Random.Range(0, matchingBoth.Count);
		
		Yarn.Value randText = new Yarn.Value(matchingBoth[i].Text);
		storage.SetValue("$random_text", randText);

		storage.SetValue("$emotion", new Yarn.Value(e.ToString()));
	}

	[YarnCommand("randombool")]
	public void TrueOrFalse(string threshold) {
		float limit = float.Parse(threshold);
		bool b = Random.Range(0f, 1f) > limit;
		Yarn.Value randBool = new Yarn.Value(b);
		storage.SetValue("$random_bool", randBool);
	}

	[YarnCommand("citynetworks")]
	public void NumberOfCityNetworks() {
		storage.SetValue("$city_networks", 0);
	}

	[YarnCommand("networkconnections")]
	public void NumberOfConnections() {
		storage.SetValue("$connections_number", 0);
	}

}

