//Paul Reichling
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetteiaGameController : MonoBehaviour
{
	public int currentPiece;
	public int[,] positions = new int[8, 8];
	GameObject[,] colliders = new GameObject[8, 8];
	public Vector2 oldPos, curPos;
	public Vector2 curPosArray, oldPosArray;
	public bool updateOld;
	Transform currentT;
	string moveDir;
	public bool yourturn;
	public enemyAI en;
	public GameObject menucanvas;
	public int lastPieceMoved;
	public AudioSource moveSound;
	[TextArea(40, 10)]
	public string boardText = "This text will appear in a text area that automatically expands";

	//public MovePiece mp;

	//Some variables are public for debugging and being able to be viewed in the inspector 
	// Start is called before the first frame update
	void Start() {
		lastPieceMoved = 2;
		en = GetComponent<enemyAI>();
		moveDir = "";
		InitalStateSetup();
		updateOld = true;
		yourturn = true;

		for (int i = 0; i < 8; i++) {

			for (int j = 0; j < 8; j++) {
				colliders[i, j] = GameObject.Find(i.ToString() + j.ToString());
				colliders[i, j].SetActive(false);
			}
		}
	}

	// Update is called once per frame
	void Update() {
		
		if (!menucanvas.activeSelf) {
			CheckCaputre();

			if (yourturn) {
				lastPieceMoved = 1;
				curPosArray = PosToArray((int)curPos.x, (int)curPos.y);
				oldPosArray = PosToArray((int)oldPos.x, (int)oldPos.y);
				if (Input.GetKeyDown(KeyCode.Q)) {
					PrintBoard();
				}

				if (Input.GetKeyUp(KeyCode.Mouse0)) {
					if (SetPiecePosition() == "m") {

						//Debug.Log("enemyturn");
						//mp.isMoving = true;
						yourturn = false;
						moveSound.pitch = Random.Range(0.7f, 1.1f);
						moveSound.Play();
						PrintBoard();
						
					} else {
						//mp.isMoving = false;
					}
					updateOld = true;

				}
			}
			else {

				EnemyMove();

			}
		}
	}
	void EnemyMove() {
		//i,j
		//StartCoroutine(GetPiece(0, 2));
		oldPosArray = Vector2.up;
		curPosArray = Vector2.up;
		oldPos = Vector2.up;
		curPos = Vector2.up;

	}
	void InitalStateSetup() {
		for (int i = 0; i < 8; i++) {

			positions[0, i] = 1;

			positions[7, i] = 2;

		}
	}
	string SetPiecePosition() {
		bool collide = false;
		//See if piece is off board - interviewer said my code was hard to read so I made it shorter
		if ((int)curPosArray.x > 7 || (int)curPosArray.y > 7 || (int)curPosArray.x < 0 || (int)curPosArray.y < 0) { collide = true; }


		//Checking each direction to see if the piece tries to move through another piece, which is not allowed
		if (collide == false) {
			if (moveDir == "down") {
				for (int i = (int)curPosArray.x; i > (int)oldPosArray.x; i--) {
					if (positions[i, (int)curPosArray.y] != 0) {
						collide = true;
					}
				}
			}
			if (moveDir == "up") {
				for (int i = (int)curPosArray.x; i < (int)oldPosArray.x; i++) {
					if (positions[i, (int)curPosArray.y] != 0) {
						collide = true;
					}
				}
			}

			if (moveDir == "right") {
				for (int i = (int)curPosArray.y; i > (int)oldPosArray.y; i--) {
					if (positions[(int)curPosArray.x, i] != 0) {
						collide = true;
					}
				}
			}
			if (moveDir == "left") {
				for (int i = (int)curPosArray.y; i < (int)oldPosArray.y; i++) {
					if (positions[(int)curPosArray.x, i] != 0) {
						collide = true;
					}
				}
			}
		}
		





		if (collide && !updateOld) {
			MoveBack(currentT);

			//yourturn = false;
			//Debug.Log("MOVEBACK TRIGGERED");
			moveSound.pitch = Random.Range(0.7f, 1.1f);
			moveSound.Play();
			return "mb";
			
			
		}
		else {
			try {

				Transform inital, final;


				//send in original pos and new pos 



				positions[(int)curPosArray.x, (int)curPosArray.y] = currentPiece;
				positions[(int)oldPosArray.x, (int)oldPosArray.y] = 0;
			}
			catch {

			}
			if (oldPosArray != curPosArray) {
				return "m";
			} else {
				return "mb";
			}

		}
	}
	void CheckCaputre() {

		for (int y = 0; y < 8; y++) {

			for (int x = 0; x < 8; x++) {
				
				if (x != 0 && x != 7) {
					
						if (positions[x, y] == 1
						&& positions[x + 1, y] == 2
						&& positions[x - 1, y] == 2) {
							Debug.Log("CAPTURE1");
							StartCoroutine(CapturePiece(x, y));
							//Debug.Log(x.ToString() + y.ToString());
						}
					
					
						if (positions[x, y] == 2
					&& positions[x + 1, y] == 1
					&& positions[x - 1, y] == 1) {
							Debug.Log("CAPTURE2");
							StartCoroutine(CapturePiece(x, y));
							//Debug.Log(x.ToString() + y.ToString());
						}
					
				}
				if (y != 0 && y != 7) {
					
						if (positions[x, y] == 1
					&& positions[x, y + 1] == 2
					&& positions[x, y - 1] == 2) {
							Debug.Log("CAPTURE3");
							StartCoroutine(CapturePiece(x, y));
							//Debug.Log(x.ToString() + y.ToString());
						}
					
					
						if (positions[x, y] == 2
						&& positions[x, y + 1] == 1
						&& positions[x, y - 1] == 1) {
							Debug.Log("CAPTURE4");
							StartCoroutine(CapturePiece(x, y));
							//Debug.Log(x.ToString() + y.ToString());
						
					}
				}


			}
		}




		//Older function with semantics errors

		//for (int y = 0; y < 8; y++) {

		//	for (int x = 1; x < 7; x++) {

		//		if (positions[0, x] == 1 && positions[0, x - 1] == 2 && positions[0, x + 1] == 2) {
		//			//Debug.Log("CAPTUREo");
		//			StartCoroutine(CapturePiece(x, 0));
		//			//Debug.Log(x.ToString() + y.ToString());
		//		}
		//		if (positions[0, x] == 2 && positions[0, x - 1] == 1 && positions[0, x + 1] == 1) {
		//			//Debug.Log("CAPTUREo");
		//			StartCoroutine(CapturePiece(x, 0));
		//			//Debug.Log(x.ToString() + y.ToString());
		//		}
		//		if (y != 0 && y != 7) {
		//			////Debug.Log(positions[i - 1, j] + "," + positions[j, i] + "," + positions[i + 1, j]);
		//			if (positions[x, y] == 1 && positions[x - 1, y] == 2 && positions[x + 1, y] == 2) {
		//				//Debug.Log("CAPTURE");
		//				StartCoroutine(CapturePiece(x, y));
		//				//Debug.Log(x.ToString() + y.ToString());
		//			}
		//			if (positions[x, y] == 2 && positions[x - 1, y] == 1 && positions[x + 1, y] == 1) {
		//				//Debug.Log("CAPTURE");
		//				StartCoroutine(CapturePiece(x, y));
		//				//Debug.Log(x.ToString() + y.ToString());

		//			}

		//		//if (j != 0 || j != 7) {
		//			if (positions[x, y] == 1 && positions[x, y - 1] == 2 && positions[x, y + 1] == 2) {
		//				//Debug.Log("CAPTURE");
		//				StartCoroutine(CapturePiece(x, y));
		//				//Debug.Log(x.ToString() + y.ToString());

		//			}
		//			if (positions[x, y] == 2 && positions[x, y - 1] == 1 && positions[x, y + 1] == 1) {
		//				//Debug.Log("CAPTURE");
		//				StartCoroutine(CapturePiece(x, y));
		//				//Debug.Log(x.ToString() + y.ToString());

		//			}
		//		} else {
		//			if (positions[x, y] == 1 && positions[x - 1, y] == 2 && positions[x + 1, y] == 2) {
		//				//Debug.Log("CAPTURE");
		//				StartCoroutine(CapturePiece(x, y));
		//				//Debug.Log(x.ToString() + y.ToString());
		//			}
		//			if (positions[x, y] == 2 && positions[x - 1, y] == 1 && positions[x + 1, y] == 1) {
		//				//Debug.Log("CAPTURE");
		//				StartCoroutine(CapturePiece(x, y));
		//				//Debug.Log(x.ToString() + y.ToString());

		//			}
		//		}
		//	}
		//}
	}

	IEnumerator CapturePiece(int i, int j) {
		positions[i, j] = 0;
		PrintBoard();
		colliders[i, j].GetComponent<colliderMover>().destory = true;
		colliders[i, j].SetActive(true);
		//Collider needs time to check for collisions
		yield return new WaitForSeconds(0.2f);
		colliders[i, j].SetActive(false);


	}
	//IEnumerator GetPiece(int i, int j) {
	//	positions[i, j] = 0;
	//	colliders[i, j].GetComponent<colliderMover>().destory = false;
	//	colliders[i, j].SetActive(true);
		

	//	yield return new WaitForSeconds(0.2f);
	//	g = colliders[i, j].GetComponent<colliderMover>().go.transform;
	//	colliders[i, j].SetActive(false);
	//	MovePiece(g, "down", gpos, g.tag);
	//	yourturn = true;


	//}
	void PrintBoard() {
		string s = "  ";
		for (int i = 0; i < 8; i++) {
			s += "\n\n";
			for (int j = 0; j < 8; j++) {
				s += "  ";
				s += positions[i, j];
			}
		}

		Debug.Log(s);
		boardText = s;
	}
	public Vector3 MovePiece(Transform g, string dir, Vector3 startPos, string tag) {

		PrintBoard();

		moveDir = dir;
		if (updateOld == true) {
			currentT = g;
			oldPos = new Vector2(startPos.z, startPos.x);

			updateOld = false;
		}
		if (dir == "up") {
			g.position = startPos + Vector3.forward * 6.25f;
			//SetPiecePosition();
			curPos = new Vector2(g.position.z, g.position.x);

			if (tag == "PetteiaW") {
				currentPiece = 2;
			}
			else {
				currentPiece = 1;
			}
			return startPos + Vector3.forward * 6.25f;
		}
		if (dir == "down") {
			g.position = startPos + Vector3.forward * -6.25f;
			//SetPiecePosition();
			curPos = new Vector2(g.position.z, g.position.x);

			if (tag == "PetteiaW") {
				currentPiece = 2;
			}
			else {
				currentPiece = 1;
			}
			return startPos + Vector3.forward * -6.25f;
		}
		if (dir == "left") {
			g.position = startPos + Vector3.right * -6.25f;
			//SetPiecePosition();
			curPos = new Vector2(g.position.z, g.position.x);

			if (tag == "PetteiaW") {
				currentPiece = 2;
			}
			else {
				currentPiece = 1;
			}
			return startPos + Vector3.right * -6.25f;
		}
		if (dir == "right") {
			g.position = startPos + Vector3.right * 6.25f;
			//SetPiecePosition();
			curPos = new Vector2(g.position.z, g.position.x);

			if (tag == "PetteiaW") {
				currentPiece = 2;
			}
			else {
				currentPiece = 1;
			}
			return startPos + Vector3.right * 6.25f;
		}

		else {
			//Debug.Log("SOMETHING HORRIBLE HAS HAPPENED \n Just kidding. Probably a typo(use right/left/up/down only");
			return startPos;

		}

	}
	public Vector2 PosToArray(int y, int x) {
		return new Vector2(Mathf.Round(((y + 3.25f) / -6.25f) - 0), Mathf.Round(((x - 3) / 6.25f)) - 0);
		//converts the real world cordinates of the pieces to the value of the array that stores where the pieces are

	}
	void MoveBack(Transform g) {
		g.position = new Vector3(oldPos.y, 1, oldPos.x);
	}

}
