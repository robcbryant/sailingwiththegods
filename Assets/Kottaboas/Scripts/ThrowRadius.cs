using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The UI display for the trajectory system
public class ThrowRadius : MonoBehaviour
{
    public LineRenderer horizontalAxis, verticalAxis, trajectory;
    public Material lineMat;

    //Each axis are a child of a parent so this controls when they are turned off or on
    public GameObject[] axisDisplay;

    private int segments = 180;

    private float radius = 5.0f;

    //Controls the point of where droplets will go
    public int posHorizontal = 90;
    public int posVertical = 45;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < axisDisplay.Length; i++)
        {
            axisDisplay[i] = gameObject.transform.GetChild(i).gameObject;
        }

        horizontalAxis = LineInIt(axisDisplay[0], segments);
        verticalAxis = LineInIt(axisDisplay[1], 1);
        trajectory = LineInIt(axisDisplay[2], 1);

        InitializePoints(horizontalAxis, segments);
        //InitializePoints(verticalAxis, 0);
        InitializePoints(trajectory, 0);

        Vector3 pointc = new Vector3(0,5,0);
        verticalAxis.SetPosition(1, pointc);

        Vector3 pointb = new Vector3(0, 0, 0);
        trajectory.SetPosition(1, pointb);
    }

    /// <summary>
    /// Intializes linerenderer that represents each axis for trajectory display
    /// </summary>
    /// <param name="g">Each axis of display</param>
    /// <param name="segs">Number of lines that make up display</param>
    /// <returns></returns>
    LineRenderer LineInIt(GameObject g, int segs) 
    {
        LineRenderer l = g.AddComponent<LineRenderer>();
        l.material = lineMat;
        l.useWorldSpace = false;
        l.startWidth = 0.05f;
        l.endWidth = 0.05f;
        l.positionCount = segs + 1;
        l.loop = false;
        return l;
    }

    /// <summary>
    /// Sets up vertices for each line 
    /// </summary>
    /// <param name="l"></param>
    /// <param name="segs"></param>
    void InitializePoints(LineRenderer l, int segs)
    {
        Vector3[] points = new Vector3[segs + 1];

        for (int i = 0; i < segs + 1; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 180.0f / segs);
            points[i] = new Vector3(Mathf.Cos(rad) * radius, 0.0f, Mathf.Sin(rad) * radius);
        }
        l.SetPositions(points);
    }

    // Update is called once per frame
    void Update()
    {        
        //Sets first position of aiming line in correct spot
        Vector3 pointa = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            posHorizontal++;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            posHorizontal--;
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            posVertical++;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            posVertical--;
		}

        posHorizontal = Mathf.Clamp(posHorizontal, 0, 180);
        posVertical = Mathf.Clamp(posVertical, 0, 180);

        var radh = Mathf.Deg2Rad * (posHorizontal * 180.0f / segments);
        var radv = Mathf.Deg2Rad * (posVertical * 90.0f / segments);
        pointa = new Vector3(Mathf.Cos(radh) * radius, Mathf.Sin(radv) * radius, (Mathf.Sin(radh) * radius));
        //pointa = new Vector3(Mathf.Cos(radh) * radius, Mathf.Sin(radv) * radius, Mathf.Clamp((Mathf.Sin(radh) * radius) - (Mathf.Sin(radv) * radius),0,5));
        
        trajectory.SetPosition(0, pointa);        
    }
}
