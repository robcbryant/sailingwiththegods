using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogScreen : ViewBehaviour
{
	private const string ResourcePath = "dialog_images";

	public script_GUI gui;

	[Header("Conversation")]
	public TextMeshProUGUI moneyText;
	public TextMeshProUGUI conversationTitle;
	public Scrollbar conversationScroll;
	public Transform conversationHolder;
	public Yarn.Unity.Example.SpriteSwitcher[] convoPartners;
	public Yarn.Unity.Example.SpriteSwitcher backgrounds;

	[Header("Choices")]
	public RectTransform choiceScroll;
	public Transform choiceHolder;
	public RectTransform choiceGrandParent;

	[Header("Prefabs")]
	public DialogChoice choiceObject;
	public DialogPiece dialogObject;
	public Image dialogImage;
	public GameObject dialogSpacer;

	private CustomDialogUI yarnUI;
	private InMemoryVariableStorage storage;
	private DialogueRunner runner;
	private Canvas canvas;

	private YarnTaxes taxes;
	bool set = false;

	private void Awake() 
	{
		yarnUI = GetComponent<CustomDialogUI>();
		storage = GetComponent<InMemoryVariableStorage>();
		runner = GetComponent<DialogueRunner>();
		canvas = GetComponentInParent<Canvas>();

		taxes = GetComponent<YarnTaxes>();
		set = true;

	}

	protected override void OnEnable() 
	{
		UpdateMoney();
	}

	public void UpdateMoney() 
	{
		moneyText.text = Globals.GameVars.playerShipVariables.ship.currency + " dr";
	}

	public void AddToDialogText(string speaker, string text, TextAlignmentOptions align) 
	{
		StartCoroutine(DoAddToDialogText(speaker, text, align));
	}

	public void AddImage(string imgName) 
	{
		StartCoroutine(DoAddImage(imgName));
	}

	public void StartDialog(Settlement s, string startNode) 
	{
		Debug.Log("StartDialog: settlement " + (s == null ? "null" : s.name));
		if (!set) {
			yarnUI = GetComponent<CustomDialogUI>();
			storage = GetComponent<InMemoryVariableStorage>();
			runner = GetComponent<DialogueRunner>();
			canvas = GetComponentInParent<Canvas>();

			taxes = GetComponent<YarnTaxes>();
			set = true;
		}
		taxes.SetPortInfo(s);
		Clear();
		runner.startNode = startNode;
		StartCoroutine(DoStartDialog());
	}

	public void StartDialog(string startNode) 
	{
		Clear();
		runner.startNode = startNode;
		StartCoroutine(DoStartDialog());
	}

	private IEnumerator DoStartDialog() 
	{
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
		//Set this as almost but not quite the last element, since we always want the spacer to be the last
		//Without the spacer, the text winds up too close to the bottom of the screen and is hard to read
		p.transform.SetSiblingIndex(conversationHolder.childCount - 2);
		p.transform.localScale = Vector3.one;

		//I don't know why I need to do this twice, but it seems to work better this way?
		yield return null;
		conversationScroll.value = 0;
		yield return null;
		conversationScroll.value = 0;
	}

	private IEnumerator DoAddImage(string imgName) 
	{
		Sprite s = Resources.Load<Sprite>(ResourcePath + "/" + imgName);

		if (s != null) {
			Image i = Instantiate(dialogImage);
			i.sprite = s;
			yield return null;
			i.transform.SetParent(conversationHolder);
			i.transform.SetSiblingIndex(conversationHolder.childCount - 2);
			i.transform.localScale = Vector3.one;
			yield return null;
			conversationScroll.value = 0;
			yield return null;
			conversationScroll.value = 0;
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

	private IEnumerator WaitAndComplete() 
	{
		yield return null;
		yarnUI.MarkLineComplete();
	}

	public void AddChoice(string text, UnityEngine.Events.UnityAction click) 
	{
		DialogChoice c = Instantiate(choiceObject);
		c.transform.SetParent(choiceHolder);

		VerticalLayoutGroup choiceLayout = choiceHolder.GetComponent<VerticalLayoutGroup>();

		c.SetText(text, choiceGrandParent);
		c.transform.localScale = Vector3.one;
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

	public void ExitConversation() 
	{
		StartCoroutine(DeactivateSelf());
	}

	public IEnumerator DeactivateSelf() 
	{
		Clear();
		yield return null;
		gameObject.SetActive(false);
	}
	
	public InMemoryVariableStorage Storage {
		get {
			return storage;
		}
	}

	public DialogueRunner Runner {
		get {
			return runner;
		}
	}

	public CustomDialogUI YarnUI {
		get {
			return yarnUI;
		}
	}

}

