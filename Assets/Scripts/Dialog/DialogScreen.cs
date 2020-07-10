using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogScreen : MonoBehaviour
{
	public script_GUI gui;

	public TextMeshProUGUI conversationTitle;
	public Scrollbar conversationScroll;
	public Transform conversationHolder;
	public Transform choiceHolder;
	public DialogChoice choiceObject;
	public DialogPiece dialogObject;
	public GameObject dialogSpacer;

	private CustomDialogUI yarnUI;
	private InMemoryVariableStorage storage;
	private DialogueRunner runner;
	private Settlement city;

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
		runner = GetComponent<DialogueRunner>();
	}

	public void AddToDialogText(string speaker, string text, TextAlignmentOptions align) {
		StartCoroutine(DoAddToDialogText(speaker, text, align));
	}
	
	private void SetCity(Settlement s) 
	{
		city = s;
		Debug.Log("Current settlement: " + city.name);
		storage.SetValue("$city_name", new Yarn.Value(city.name));
		storage.SetValue("$city_description", new Yarn.Value(city.description));
	}

	public void StartDialog(Settlement s) {
		SetCity(s);
		StartCoroutine(StartDialog());
	}

	private IEnumerator StartDialog() {
		yield return null;
		yield return null;
		runner.StartDialogue();
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

	private IEnumerator DeactivateSelf() {
		yield return null;
		gameObject.SetActive(false);
	}

	public void ExitConversation() {
		bool city = storage.GetValue("$entering_city").AsBool;
		Debug.Log($"Exiting the conversation. Entering the city {city}");

		if (city) {
			gui.GUI_EnterPort();
		}
		else {
			gui.GUI_ExitPortNotification();
		}

		StartCoroutine(DeactivateSelf());
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

	[YarnCommand("cityinfo")]
	public void SetCityInfo() {
		storage.SetValue("$city_name", new Yarn.Value(city.name));
		storage.SetValue("$city_description", new Yarn.Value(city.description));
	}

}

