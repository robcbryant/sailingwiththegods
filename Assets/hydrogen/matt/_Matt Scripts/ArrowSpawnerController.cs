using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawnerController : MonoBehaviour
{
    // public Transform arrowToSpawn;
    public GameObject arrowToSpawn;
	//public int targetScore;
    public  GameObject spawnLoc;

    public float minTimeBetweenSpawn;
    public float maxTimeBetweenSpawn;

    //private Transform arrow;
    private GameObject arrow;
    public Transform spawnParent;
    Vector3 spawnPosition;

    private bool callFunction ;
    private bool startFunction;
    private float startFunctionTimer;
    public float startFunctionEndTime;

    // Start is called before the first frame update
    void Start()
    {
        startFunction = false;
        callFunction = true;
        startFunctionTimer = 0;
    }

    IEnumerator ArrowSpawn()
        {
        //while (GameManager.startPlaying == true && GameManager.currentScore < targetScore)
        // {

        //must be done to be able to set up GameObject as a child of the Canvas so our image renders
        // arrow = Instantiate(arrowToSpawn, spawnLoc.position, spawnLoc.rotation) as Transform;
        // arrow.parent = spawnParent;

        arrow = Instantiate(arrowToSpawn, spawnLoc.transform.position, spawnLoc.transform.rotation) as GameObject;
        arrow.transform.SetParent(spawnParent.transform);

        callFunction = false;
            yield return new WaitForSeconds(Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn));
			callFunction = true;
           // }
        }

	void Update()
    {
		//so that the coroutine runs after timer
        if(GameManager.startPlaying == true && GameManager.endGameState == false && callFunction == true && startFunction == true)
        {
            StartCoroutine(ArrowSpawn());
        }

        startFunctionTimer += Time.deltaTime;
        if (startFunctionTimer >= startFunctionEndTime)
        {
            startFunction = true;
        }
        //Debug.Log(startFunction);
    }
}
