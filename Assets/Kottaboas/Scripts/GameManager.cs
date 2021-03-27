using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the game of Kottabos
public class GameManager : MonoBehaviour
{
    public GameObject playerPos;
    private Rigidbody playerRb;
    private Vector3 playerStartPos;

    public GameObject randomPlacement;
    public Transform[] childPos;
    public Vector3 startPos;
    public Quaternion startRot;

    public bool isHit = false;

    private Throw tr;

    private static int score = 0;
    public bool scored = false;

    private static int tries = 5;
    private bool continueRound = false;

    public bool ContinueRound { get => continueRound; set => continueRound = value; }

    // Start is called before the first frame update
    void Start()
    {
        playerStartPos = playerPos.transform.position;
        playerRb = playerPos.GetComponent<Rigidbody>();

        startPos = childPos[3].position;
        startRot = childPos[3].rotation;
        
        tr = playerPos.GetComponent<Throw>();
    }

    // Update is called once per frame
    void Update()
    {
        if (continueRound)
        {            
            Debug.Log("C or B");
            if (Input.GetKey(KeyCode.C))
            {
                tr.animate.SetBool("isFlinged", false);

                //Reset
                ResetRound();
                Debug.Log("reset");
                continueRound = false;
            }
            else if (Input.GetKey(KeyCode.B) || score >= 7 || tries == 0)
            {
                //Thinking if you reached number of tries and have low amount of points you lose and get an insult
                if(tries == 0) 
                {
                    //You lose End game
                }

                //if the scored atleast 7 then you get something
                if(score >= 7) 
                {
                    //Here's your reward end game
                }

                //Get Reward and return to tavern
                Debug.Log("End game");
                continueRound = false;
            }
        }
    }

    public void addScore()
    {
        score += 1;
    }
    /// <summary>
    /// Currently ends game when you hit c after 5 misses
    /// </summary>
    public void subTries()
    {
        tries -= 1;
    }

    
    private void ResetBallPosition()
    {
        playerPos.transform.position = playerStartPos;
        playerPos.transform.rotation = Quaternion.Euler(Vector3.zero);

        playerRb.useGravity = false;
        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        for (int i = 0; i < playerPos.GetComponent<Transform>().childCount; i++) 
        {
            playerPos.transform.GetChild(i).gameObject.SetActive(true);
        }
        tr.Launch = false;
    }
    
    private void ResetTargetPosition()
    {
        randomPlacement.GetComponent<RandomPlacement>().PlaceRandomPosition();

        childPos[3].localPosition = startPos;
        childPos[3].rotation = startRot;
        //childPos[3].localRotation = startRot;

        /*
        for (int i = 0; i < childPos.Length; i++)
        {
            
           //childPos[i].position = childStartPos[i].localPosition;
           //childPos[i].rotation = childStartPos[i].rotation;
        } 
        */
        
        isHit = false;
    }

    private void ResetRound()
    {
        //ResetBallPosition
        ResetBallPosition();
        //ResetTargetPosition
        if (isHit)
        {
            ResetTargetPosition();
        }
        else 
        {
            subTries();
        }
    }
}
