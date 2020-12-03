using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positions : MonoBehaviour
{
	public Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		pos = PosToArray((int)transform.position.z, (int)transform.position.x);
		
    }

	public Vector2 PosToArray(int y, int x) 
	{
		////Debug.Log(Mathf.Round(((x - 3) / 6.25f)) - 0);
		return new Vector2(  Mathf.Round(((y + 3.25f) / -6.25f) - 0),  Mathf.Round(((x - 3f) / 6.25f)) - 0);
		
	}
}
