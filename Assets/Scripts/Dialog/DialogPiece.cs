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
		if (content[0] == '/') {
			content = content.Remove(0, 1);
		}
		text.text = content;
	}

	public void SetAlignment(TextAlignmentOptions align) 
	{
		speaker.alignment = align;
		text.alignment = align;
	}
}
