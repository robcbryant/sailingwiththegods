using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noteCheckUp : MonoBehaviour
{
    public bool canBePressed;

    //public KeyCode keyToPress;
    private Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn = GameObject.FindWithTag("UpActivator").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

       btn.onClick.AddListener(() => correctNoteHit());

        if (Input.GetKeyDown("up"))
        {
            correctNoteHit();
        }


}

	//When note is hit correctly 
    public void correctNoteHit()
    {
		if(canBePressed)
           {
            gameObject.SetActive(false);
            canBePressed = false;
            SongGameController.instance.NoteHit();
            }
    }

	private void OnTriggerEnter2D (Collider2D other)
    {
		if(other.tag == "UpActivator")
        {
            canBePressed = true;
        }
    } 

    private void OnTriggerExit2D(Collider2D other)
    {
        if (this.gameObject.activeSelf)
        {
            if (other.tag == "UpActivator")
            {
                canBePressed = false;

                SongGameController.instance.NoteMissed();
            }
        }
    }
}
