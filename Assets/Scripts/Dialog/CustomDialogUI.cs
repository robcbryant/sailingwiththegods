using System;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class CustomDialogUI : Yarn.Unity.DialogueUIBehaviour
{
	private bool userRequestedNextLine = false;

	private System.Action<int> currentOptionSelectionHandler;

	private bool waitingForOptionSelection = false;

	/// <summary>
	/// A <see cref="DialogueRunner.StringUnityEvent"/> that is called
	/// when a <see cref="Command"/> is received.
	/// </summary>
	/// <remarks>
	/// Use this method to dispatch a command to other parts of your game.
	/// 
	/// This method is only called if the <see cref="Command"/> has not
	/// been handled by a command handler that has been added to the
	/// <see cref="DialogueRunner"/>, or by a method on a <see
	/// cref="MonoBehaviour"/> in the scene with the attribute <see
	/// cref="YarnCommandAttribute"/>.
	/// 
	/// {{|note|}}
	/// When a command is delivered in this way, the <see cref="DialogueRunner"/> will not pause execution. If you want a command to make the DialogueRunner pause execution, see <see cref="DialogueRunner.AddCommandHandler(string,
	/// DialogueRunner.BlockingCommandHandler)"/>.
	/// {{|/note|}}
	///
	/// This method receives the full text of the command, as it appears between
	/// the `<![CDATA[<<]]>` and `<![CDATA[>>]]>` markers.
	/// </remarks>
	/// <seealso cref="DialogueRunner.AddCommandHandler(string,
	/// DialogueRunner.CommandHandler)"/> 
	/// <seealso cref="DialogueRunner.AddCommandHandler(string,
	/// DialogueRunner.BlockingCommandHandler)"/> 
	/// <seealso cref="YarnCommandAttribute"/>
	public DialogueRunner.StringUnityEvent onCommand;

	private DialogScreen ds;
	private string otherName = "Tax Collector Bob III";
	private string currentSpeakerName;
	private TMPro.TextAlignmentOptions textAlign = TMPro.TextAlignmentOptions.Left;
	private bool end = false;

	#region Events
	/// <summary>
	/// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
	/// when the dialogue starts.
	/// </summary>
	/// <remarks>
	/// Use this event to enable any dialogue-related UI and gameplay
	/// elements, and disable any non-dialogue UI and gameplay
	/// elements.
	/// </remarks>
	public UnityEngine.Events.UnityEvent onDialogueStart;

	/// <summary>
	/// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
	/// when the dialogue ends.
	/// </summary>
	/// <remarks>
	/// Use this event to disable any dialogue-related UI and gameplay
	/// elements, and enable any non-dialogue UI and gameplay elements.
	/// </remarks>
	public UnityEngine.Events.UnityEvent onDialogueEnd;

	/// <summary>
	/// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
	/// when a <see cref="Line"/> has been delivered.
	/// </summary>
	/// <remarks>
	/// This method is called before <see cref="onLineUpdate"/> is
	/// called. Use this event to prepare the scene to deliver a line.
	/// </remarks>
	public UnityEngine.Events.UnityEvent onLineStart;

	/// <summary>
	/// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
	/// when a line has finished being delivered.
	/// </summary>
	/// <remarks>
	/// This method is called after <see cref="onLineUpdate"/>. Use
	/// this method to display UI elements like a "continue" button.
	///
	/// When this method has been called, the Dialogue UI will wait for
	/// the <see cref="MarkLineComplete"/> method to be called, which
	/// signals that the line should be dismissed.
	/// </remarks>
	/// <seealso cref="onLineUpdate"/>
	/// <seealso cref="MarkLineComplete"/>
	public UnityEngine.Events.UnityEvent onLineFinishDisplaying;

	/// <summary>
	/// A <see cref="DialogueRunner.StringUnityEvent"/> that is called
	/// when the visible part of the line's localised text changes.
	/// </summary>
	/// <remarks>
	/// The <see cref="string"/> parameter that this event receives is
	/// the text that should be displayed to the user. Use this method
	/// to display line text to the user.
	///
	/// The <see cref="DialogueUI"/> class gradually reveals the
	/// localised text of the <see cref="Line"/>, at a rate of <see
	/// cref="textSpeed"/> seconds per character. <see
	/// cref="onLineUpdate"/> will be called multiple times, each time
	/// with more text; the final call to <see cref="onLineUpdate"/>
	/// will have the entire text of the line.
	///
	/// If <see cref="MarkLineComplete"/> is called before the line has
	/// finished displaying, which indicates that the user has
	/// requested that the Dialogue UI skip to the end of the line,
	/// <see cref="onLineUpdate"/> will be called once more, to display
	/// the entire text.
	///
	/// If <see cref="textSpeed"/> is `0`, <see cref="onLineUpdate"/>
	/// will be called just once, to display the entire text all at
	/// once.
	///
	/// After the final call to <see cref="onLineUpdate"/>, <see
	/// cref="onLineFinishDisplaying"/> will be called to indicate that
	/// the line has finished appearing.
	/// </remarks>
	/// <seealso cref="textSpeed"/>
	/// <seealso cref="onLineFinishDisplaying"/>
	public DialogueRunner.StringUnityEvent onLineUpdate;

	/// <summary>
	/// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
	/// when a line has finished displaying, and should be removed from
	/// the screen.
	/// </summary>
	/// <remarks>
	/// This method is called after the <see cref="MarkLineComplete"/>
	/// has been called. Use this method to dismiss the line's UI
	/// elements.
	///
	/// After this method is called, the next piece of dialogue content
	/// will be presented, or the dialogue will end.
	/// </remarks>
	public UnityEngine.Events.UnityEvent onLineEnd;

	/// <summary>
	/// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
	/// when an <see cref="OptionSet"/> has been displayed to the user.
	/// 
	/// </summary>
	/// <remarks>
	/// Before this method is called, the <see cref="Button"/>s in <see
	/// cref="optionButtons"/> are enabled or disabled (depending on
	/// how many options there are), and the <see cref="Text"/> or <see
	/// cref="TMPro.TextMeshProUGUI"/> is updated with the correct
	/// text.
	///
	/// Use this method to ensure that the active <see
	/// cref="optionButtons"/>s are visible, such as by enabling the
	/// object that they're contained in.
	/// </remarks>
	public UnityEngine.Events.UnityEvent onOptionsStart;

	/// <summary>
	/// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
	/// when an option has been selected, and the <see
	/// cref="optionButtons"/> should be hidden.
	/// </summary>
	/// <remarks>
	/// This method is called after one of the <see
	/// cref="optionButtons"/> has been clicked, or the <see
	/// cref="SelectOption(int)"/> method has been called.
	///
	/// Use this method to hide all of the <see cref="optionButtons"/>,
	/// such as by disabling the object they're contained in. (The
	/// DialogueUI won't hide them for you individually.)
	/// </remarks>
	public UnityEngine.Events.UnityEvent onOptionsEnd;
	#endregion

	internal void Awake() {
		// Start by hiding the container
		ds = GetComponent<DialogScreen>();
	}

	/// Runs a line.
	/// <inheritdoc/>
	public override Dialogue.HandlerExecutionType RunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onLineComplete) {
		// Start displaying the line; it will call onComplete later
		// which will tell the dialogue to continue
		StartCoroutine(DoRunLine(line, localisationProvider, onLineComplete));
		return Dialogue.HandlerExecutionType.PauseExecution;
	}

	/// Show a line of dialogue, gradually        
	private IEnumerator DoRunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onComplete) {
		onLineStart?.Invoke();

		userRequestedNextLine = false;

		// The final text we'll be showing for this line.
		string text = localisationProvider.GetLocalisedTextForLine(line);

		if (text == null) {
			Debug.LogWarning($"Line {line.ID} doesn't have any localised text.");
			text = line.ID;
		}


		string[] split = text.Split('^');

		bool eventualEnd = end;
		end = false;

		for (int i = 0; i < split.Length; i++) {
			if (split[i][0] == '&') {
				ds.AddImage(split[i].Remove(0, 1));
			}
			else {
				ds.AddToDialogText(currentSpeakerName, split[i], textAlign);
			}


			if (i == split.Length - 1) {
				end = eventualEnd;
			}

			// We're now waiting for the player to move on to the next line
			userRequestedNextLine = false;

			// Indicate to the rest of the game that the line has finished being delivered
			onLineFinishDisplaying?.Invoke();

			while (userRequestedNextLine == false) {
				yield return null;
			}
		}
		
		// Hide the text and prompt
		onLineEnd?.Invoke();

		onComplete();

	}

	/// Runs a set of options.
	/// <inheritdoc/>
	public override void RunOptions(Yarn.OptionSet optionSet, ILineLocalisationProvider localisationProvider, System.Action<int> onOptionSelected) {
		StartCoroutine(DoRunOptions(optionSet, localisationProvider, onOptionSelected));
	}

	/// Show a list of options, and wait for the player to make a
	/// selection.
	private IEnumerator DoRunOptions(Yarn.OptionSet optionsCollection, ILineLocalisationProvider localisationProvider, System.Action<int> selectOption) 
	{
		ds.ClearOptions();
		end = false;

		waitingForOptionSelection = true;

		currentOptionSelectionHandler = selectOption;

		foreach (var optionString in optionsCollection.Options) 
		{
			string text = localisationProvider.GetLocalisedTextForLine(optionString.Line);
			ds.AddChoice(text, () => SelectOption(optionString.ID));
		}

		onOptionsStart?.Invoke();

		// Wait until the chooser has been used and then removed 
		while (waitingForOptionSelection) {
			yield return null;
		}
		
		onOptionsEnd?.Invoke();

		
	}

	/// Runs a command.
	/// <inheritdoc/>
	public override Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, System.Action onCommandComplete) {
		// Dispatch this command via the 'On Command' handler.
		onCommand?.Invoke(command.Text);

		// Signal to the DialogueRunner that it should continue
		// executing. (This implementation of RunCommand always signals
		// that execution should continue, and never calls
		// onCommandComplete.)
		return Dialogue.HandlerExecutionType.ContinueExecution;
	}

	/// Called when the dialogue system has started running.
	/// <inheritdoc/>
	public override void DialogueStarted() {
		onDialogueStart?.Invoke();
	}

	/// Called when the dialogue system has finished running.
	/// <inheritdoc/>
	public override void DialogueComplete() {
		onDialogueEnd?.Invoke();

	}

	/// <summary>
	/// Signals that the user has finished with a line, or wishes to
	/// skip to the end of the current line.
	/// </summary>
	/// <remarks>
	/// This method is generally called by a "continue" button, and
	/// causes the DialogueUI to signal the <see
	/// cref="DialogueRunner"/> to proceed to the next piece of
	/// content.
	///
	/// If this method is called before the line has finished appearing
	/// (that is, before <see cref="onLineFinishDisplaying"/> is
	/// called), the DialogueUI immediately displays the entire line
	/// (via the <see cref="onLineUpdate"/> method), and then calls
	/// <see cref="onLineFinishDisplaying"/>.
	/// </remarks>
	public void MarkLineComplete() {
		userRequestedNextLine = true;
	}

	/// <summary>
	/// Signals that the user has selected an option.
	/// </summary>
	/// <remarks>
	/// This method is called by the <see cref="Button"/>s in the <see
	/// cref="optionButtons"/> list when clicked.
	///
	/// If you prefer, you can also call this method directly.
	/// </remarks>
	/// <param name="optionID">The <see cref="OptionSet.Option.ID"/> of
	/// the <see cref="OptionSet.Option"/> that was selected.</param>
	public void SelectOption(int optionID) {
		if (waitingForOptionSelection == false) {
			Debug.LogWarning("An option was selected, but the dialogue UI was not expecting it.");
			return;
		}
		waitingForOptionSelection = false;
		currentOptionSelectionHandler?.Invoke(optionID);
	}

	[YarnCommand("setpartnername")]
	public void SetOtherName(string name) {
		otherName = name.Replace('_', ' ');
	}

	[YarnCommand("setspeaker")]
	public void SetCurrentSpeaker(string speaker) {
		if (speaker.ToLower() == "jason") {
			currentSpeakerName = "Jason";
			textAlign = TMPro.TextAlignmentOptions.Left;
		}
		else {
			currentSpeakerName = otherName;
			textAlign = TMPro.TextAlignmentOptions.Right;
		}
	}

	[YarnCommand("showcontinue")]
	public void ShowContinueButton() {
		ds.AddContinueOption();
	}

	[YarnCommand("setend")]
	public void SetEndOfBlock() {
		end = true;
		Debug.Log("Set end to true");
	}

	public bool EndOfBlock {
		get {
			return end;
		}
	}
}
