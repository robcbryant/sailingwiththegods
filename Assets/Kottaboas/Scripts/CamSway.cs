using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move camera to similate uncoordinated movement
public class CamSway : MonoBehaviour
{
    //amplitude that controls the rate at which camera bobs
	[SerializeField]
    private float ampX = 0.01f;
	[SerializeField]
	private float ampY = 0.01f;

	[SerializeField]
	private float pX = 40.0f;
	[SerializeField]
	private float pY = 40.0f;

    //x and y are the formulas for a sine(y) and cos(x) wave for which the camera will follow
    private float x = 0;
    private float y = 0;

	//Range of the camera
	[SerializeField]
	private float index = 0;
    private float smoothTime = 0.05f;

    // Update is called once per frame
    void Update()
    {
		index = Mathf.Clamp(index, -15f, 15f);
        //index += Time.deltaTime * (Random.Range(0, 2) * 2 - 1) * smoothTime;
        index += Time.deltaTime * smoothTime;
        //x = ampX * Mathf.Cos(pX * index);
        //y = ampY * Mathf.Sin(pY * index);

        x = ampX * Mathf.Cos(((2 * Mathf.PI) / pX) * (index));
        y = ampY * Mathf.Sin(((2 * Mathf.PI) / pY) * (index));

        transform.Rotate(new Vector3(x,y));
        //transform.Rotate(new Vector3(0,y));
        //transform.Rotate(new Vector3(x,0));
    }
}
