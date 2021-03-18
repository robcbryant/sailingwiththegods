using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetteiaMovePiece : MonoBehaviour
{
	public float mouseThreshold = 50f;

	public Vector3 mouseStartPos,mouseEndPos;
	public Vector3 pieceStartPos;
	public Camera cam;
	public Transform g;
	public bool lockedx,lockedy, isMoving;
	public float timer;
	public PetteiaGameController p;
	public MeshRenderer real;
	public GameObject empty;

	public GameObject dummy,dummySpawned;

	public PetteiaIKHelper ik;
	// Start is called before the first frame update
	void Start()
    {
		empty = GameObject.Find("empty");
		//real = GameObject.Find("Sphere").GetComponent<MeshRenderer>();
		real.enabled = true;
		g = GetComponent<Transform>();
		lockedx = false;
		lockedy = false;
		pieceStartPos = g.position;
	}

	// Update is called once per frame
	void Update() {
		timer += Time.deltaTime;
		if (!Input.GetKey(KeyCode.Mouse0)) {
			mouseStartPos = Input.mousePosition;
			mouseEndPos = Input.mousePosition;
		}
	}
	void FixedUpdate()
    {
		if (p.yourturn) {
			if (Mathf.Abs(mouseStartPos.x - mouseEndPos.x) > mouseThreshold && lockedy == false) {
				if (mouseEndPos.x < mouseStartPos.x) {
					////Debug.Log("moving left");


					mouseStartPos = Input.mousePosition;
					pieceStartPos = p.MovePiece(g, "left", pieceStartPos, g.tag);
					lockedx = true;
				}
				else {
					////Debug.Log("moving right");

					mouseStartPos = Input.mousePosition;
					pieceStartPos = p.MovePiece(g, "right", pieceStartPos, g.tag);
					lockedx = true;
				}
			}

			if (Mathf.Abs(mouseStartPos.y - mouseEndPos.y) > mouseThreshold && lockedx == false) {

				if (mouseEndPos.y > mouseStartPos.y) {
					////Debug.Log("moving up");


					mouseStartPos = Input.mousePosition;
					pieceStartPos = p.MovePiece(g, "up", pieceStartPos, g.tag);
					lockedy = true;
				}
				else {
					////Debug.Log("moving down");

					mouseStartPos = Input.mousePosition;
					pieceStartPos = p.MovePiece(g, "down", pieceStartPos, g.tag);
					lockedy = true;
				}
			}
		}
	}
	void OnMouseDown() 
	{
		//	//Debug.Log(gameObject.name);
		if (real != null) {
			mouseStartPos = Input.mousePosition;
			pieceStartPos = g.position;
			ik.SetPiece(real.gameObject);
			ik.SetInital(g);
			real.enabled = false;
			//Cursor.visible = false;
			SpawnDummy();
		}


	}
	void SpawnDummy() 
	{
		
		dummySpawned = Instantiate(dummy, empty.transform);

		dummySpawned.transform.position = g.transform.position;

	}
	void DelDummy() {
		Destroy(dummySpawned);
	}

	void OnMouseUp() 
	{
		if (real != null) {
			//unlock function
			if (isMoving) {
				ik.SetFinal(real.gameObject.transform);
				//ik.onThemMove = true;
			}
			lockedx = false;
			lockedy = false;
			mouseEndPos = mouseStartPos;
			Cursor.visible = true;
			real.enabled = true;
			DelDummy();
		}


	}
	private void OnMouseDrag() {
		////Debug.Log("DHAWJLREGHaljsdhflaksjdfh");
		mouseEndPos = Input.mousePosition;
	}
	


}
