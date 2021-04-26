using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class arrowButtonController : MonoBehaviour
{

    //so that arrow keys work when a key input is pressed
    public KeyCode keyToPress;

    private Button _button;

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyToPress))
        {
			//changes button color when key is pressed
            FadeToColor(_button.colors.pressedColor);
            //ClickButton
            _button.onClick.Invoke();
           // Debug.Log("button pressed");
        }
        else if (Input.GetKeyUp(keyToPress))
        {
            FadeToColor(_button.colors.normalColor);
        }
    }

	//To change button color 
    public void FadeToColor(Color color)
	{
    Graphic graphic = GetComponent<Graphic>();
    graphic.CrossFadeColor(color, _button.colors.fadeDuration, true, true);
	}
}
