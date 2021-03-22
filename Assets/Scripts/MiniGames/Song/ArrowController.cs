using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
	//hold tempo of notes
    public float noteTempo;

    //starts game
    public bool hasStarted;

    // Start is called before the first frame update
    void Start()
    {
		//how fast notes should move per second
        noteTempo = noteTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            //This is now handled in mattsGameManager
            /*
            if (Input.anyKeyDown)
            {
                hasStarted = true;
            }
			*/
        }
        else
        {
            transform.position -= new Vector3(0f, noteTempo * Time.deltaTime, 0f);
        }

		//destroys this arrow when game ends
		if (SongGameController.endGameState == true)
		{
           Destroy(this.gameObject);
        }
    }
}
