using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class script_mousehover_popup : MonoBehaviour
{

	public int resourceID = 0;
	bool showPopup = false;
	public string message = "";
	string resourceName = "";
	GameVars GameVars;

	//popup window dimensions
	public float left = 0f;
	public float top = 0f;
	public float width = 100f;
	public float height = 100f;
	public List<string> allLines = new List<string>();
	public Transform bgSprite;

	void Start() {
		//Find the matching resource of this gameObject's label, and add the description here
		GameVars = Globals.GameVars;
		bgSprite = gameObject.transform.parent.GetChild(0);//bg sprite should always be the first index

		resourceName = GameVars.masterResourceList[resourceID].name;
		message = GameVars.masterResourceList[resourceID].description;

		message = resourceName + " : " + message;

		//Figure out needed pop up dimensions. Each Line of text should be no more than 40 chars
		//I went overboard with this--GUI.Labels handle word-wrapping so I don't think I need the string list
		//--but this is still useful in getting the number of 'rows' needed for the word wrap
		if (message.Length < 40) {
			width = 400f;
			height = 25f + 10f;
			bgSprite.localScale = Vector3.one;
		}
		else {
			//This will get complicated: We need to get the first 40 characters--that ends in a ' ' (space)
			//If the 40th character is not a space, then count backwards until we find one and use that index for the first slice

			for (int i = 0; i < message.Length; i += 100) {
				int indexModifier = 0;//this will be our default
				int newEndIndex = i + 100;//This will be our default
										  //First check if the character in this position is a space ' ' 
				if (message[i] != ' ') {
					//If it is, then we need to search for the first space counting back from index 40
					for (int a = 100; a >= 0; a--) {
						if (message[a] == ' ') {
							//Figure out the difference of indices we skipped
							indexModifier = newEndIndex - a;
							//Set our new split end position to 'a' index
							newEndIndex = a;
						}
					}
				}
				allLines.Add(message.Substring(i, Mathf.Min(newEndIndex, message.Length - i)));
				//Finally apply our index Modifer--essentially we rest 'i' back to the index we found the space
				//	--before it gets incremented another 40 characters
				i -= indexModifier;

			}
			width = 400f;
			height = (25f * allLines.Count) + 10f;
			bgSprite.localScale += new Vector3(0, allLines.Count, 0);
		}


	}

	void OnGUI() {

		if (showPopup) {

			GUI.Label(new Rect(left, top, width, height), message);

		}
	}

	void OnMouseOver() {
		Debug.Log(gameObject.name);
		//Set dimensions and location of background sprite
		bgSprite.localPosition = transform.localPosition;
		bgSprite.localPosition += new Vector3(4, 0, 1);

		//Set dimensions and location of GUI.Label
		Vector3 iconPosition = transform.position;
		iconPosition = GameVars.FPVCamera.GetComponent<Camera>().WorldToScreenPoint(iconPosition);
		//the GUI 0,0 is top left, the screen is bottom left--let's flip the y value--the x stays the same
		top = Screen.height - iconPosition.y;
		//let's bump about 20 pixels off to the right
		left = iconPosition.x + 20f;


		//Turn the popup on
		showPopup = true;
		//Turn on sprite
		bgSprite.gameObject.SetActive(true);

	}

	void OnMouseExit() {
		showPopup = false;
		//turn off sprite
		bgSprite.gameObject.SetActive(false);
	}

}
