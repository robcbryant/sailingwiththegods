using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the placement and chances of placement for the kottabos stand and table
public class RandomPlacement : MonoBehaviour
{
    public GameManager gm;

    public Transform[] pos;

    //Limit the how far the table and kottaboas stand will be displaced
    private float xMaxRange = 4.5f;
    private float xMinRange = -4.0f;
    private float zMaxRange = 4.5f;
    private float zMinRange = -4.0f;

    private static float percent = 50.0f;
    // Start is called before the first frame update
    void Start()
    {
        placementChances();

        PlaceRandomPosition();
    }

    private void Update()
    {
        //As score goes up diffculty goes up
        if (gm.scored)
        {
            percent -= 10;
        }
        gm.scored = false;
        placementChances();
    }


    /// <summary>
    /// Adds chance for more diffcult placements after a target hit
    /// </summary>
    public void placementChances() 
    {
        int rangeInt = Random.Range(1, 100);

        if (percent >= 50 && percent < rangeInt)
        {
            zMaxRange = 0;
        }
        else if (percent >= 40 && percent < rangeInt)
        {
            zMaxRange = 1.0f;
        }
        else if (percent >= 30 && percent < rangeInt)
        {
            zMaxRange = 2.0f;
            zMinRange = -3.0f;
        }
        else if (percent >= 20 && percent < rangeInt)
        {
            zMaxRange = 3.0f;
            zMinRange = -1.5f;
        }
        else if (percent >= 10 && percent < rangeInt)
        {
            zMaxRange = 4.5f;
            zMinRange = 0.0f;
        }
    }

    public void PlaceRandomPosition() 
    {
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i].transform.position = new Vector3(Random.Range(xMinRange, xMaxRange), pos[i].position.y, Random.Range(zMinRange, zMaxRange));
            if (i < pos.Length - 1 && Vector3.Distance(pos[i].transform.position, pos[i + 1].transform.position) < 3.0f)
            {
                if (pos[i].name == "Table")
                {
                    PlaceRandomPosition();
                }
            }
        }
    }
}
