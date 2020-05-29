using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPiece : MonoBehaviour
{
	public TextMeshProUGUI speaker;
	public TextMeshProUGUI text;

	public void SetText(string speakerName, string content) 
	{
		speaker.text = speakerName;
		text.text = content;
	}

	public void SetAlignment(TextAlignmentOptions align) 
	{
		speaker.alignment = align;
		text.alignment = align;
	}
}
