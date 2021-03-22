using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongGameController : MonoBehaviour
{
    public AudioSource[] musicList;
    private int songChoiceNum;

    public static bool startPlaying;

    //End Game Manager
    public static float targetScore;
    public float gameEndTimerValue;
    private float gameEndTimerHolder;
    public static bool endGameState;

    public ArrowController arrowController;

    //score holders
    public static float currentScore;
    public int scorePerNote = 100;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;

    //text boxes
    public Text scoreText;
    public Text multiText;
    public Text lyricsText;
	private Color lyricsColor;

    //Made static so that there is only one instance of mattsGameManager at a time
    public static SongGameController instance;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        lyricsColor = lyricsText.color;  //  sets color to object
        lyricsColor.a = 0.0f; // makes the color transparent
        targetScore = 10000;


        //Resetting score at Start
        scoreText.text = "Score: 0";
        multiText.text = "Multiplier: 1x";
        currentMultiplier = 1;
        songChoiceNum = Random.Range(0, musicList.Length);

        endGameState = false;
        startPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(gameEndTimerHolder);
        if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                gameEndTimerHolder = 0;
                startPlaying = true;
                arrowController.hasStarted = true;
                musicList[songChoiceNum].Play();
            }
        }

        if (startPlaying)
        {
            gameEndTimerHolder += Time.deltaTime;
        }

        //End Game Manger
        if (gameEndTimerHolder >= gameEndTimerValue)
        {
            endGameState = true;
        }
        if (currentScore > targetScore)
        {
            endGameState = true;
        }

        //ending game 
        if (endGameState)
        {
            musicList[songChoiceNum].Stop();
        }
    }

    //what happens when we hit a note
    public void NoteHit()
    {
        //Debug.Log("Hit On Time");

        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;

            //increases multiplier at each threshold 
            //then resets tracker to 0 so we can get to next threshold
            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }

        multiText.text = "Multiplier: x:" + currentMultiplier;

        currentScore += scorePerNote * currentMultiplier;
        scoreText.text = "Score: " + currentScore;
    }

    //what happens when we miss a note
    public void NoteMissed()
    {
        //Debug.Log("Missed Note");

        currentMultiplier = 1;
        multiplierTracker = 0;

        multiText.text = "Multiplier: x:" + currentMultiplier;
    }

   // public static void ChangeOppacityOfLyrics (Text lyricsText)
	//{
      //  Color lyricsColor = lyricsText.color;  //  sets color to object
        //lyricsColor.a = targetScore % currentScore; // changes the color of alpha
		//Debug.Log(lyricsColor.a);
   // }
}
