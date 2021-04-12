using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PetteiaMovePiece : MonoBehaviour
{
	public Vector3 mouseStartPos,mouseEndPos;
	public Vector2Int pieceStartPos;
	public Camera cam;
	public bool lockedx,lockedy, isMoving;
	public float timer;
	public PetteiaGameController pController;
	public MeshRenderer real;
	public GameObject dummyParent;
	public GameObject highlight;

	public GameObject dummy,dummySpawned;

	public PetteiaIKHelper ik;
	private int mask;
	private bool active = false;

	private Vector2Int potentialPos;
	private List<PetteiaColliderMover> validMoves = new List<PetteiaColliderMover>();
	[HideInInspector] public static bool showHighlight;

	void Start()
    {
		//real = GameObject.Find("Sphere").GetComponent<MeshRenderer>();
		real.enabled = true;
		lockedx = false;
		lockedy = false;
		showHighlight = true;

		mask = LayerMask.GetMask("GameSquare");
	}
	
	void Update() {
		timer += Time.deltaTime;
		if (!Input.GetKey(KeyCode.Mouse0)) {
			mouseStartPos = Input.mousePosition;
			mouseEndPos = Input.mousePosition;
		}
	}

	void FixedUpdate()
    {
		if (showHighlight && !highlight.activeSelf) {
			highlight.SetActive(true);
		}
		if (!showHighlight && highlight.activeSelf) {
			highlight.SetActive(false);
		}

		if (pController.yourTurn && active) {
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, 100f, mask, QueryTriggerInteraction.Collide)) 
			{
				PetteiaColliderMover pcm = hit.collider.GetComponent<PetteiaColliderMover>();
				if (validMoves.Contains(pcm)) 
				{
					potentialPos = pcm.position;
					transform.position = hit.transform.position;
				}
			}

			//if (Mathf.Abs(mouseStartPos.x - mouseEndPos.x) > mouseThreshold && lockedy == false) {
			//	if (mouseEndPos.x < mouseStartPos.x) {
			//		////Debug.Log("moving left");


			//		mouseStartPos = Input.mousePosition;
			//		pieceStartPos = p.MovePiece(g, "left", pieceStartPos, g.tag);
			//		lockedx = true;
			//	}
			//	else {
			//		////Debug.Log("moving right");

			//		mouseStartPos = Input.mousePosition;
			//		pieceStartPos = p.MovePiece(g, "right", pieceStartPos, g.tag);
			//		lockedx = true;
			//	}
			//}

			//if (Mathf.Abs(mouseStartPos.y - mouseEndPos.y) > mouseThreshold && lockedx == false) {

			//	if (mouseEndPos.y > mouseStartPos.y) {
			//		////Debug.Log("moving up");


			//		mouseStartPos = Input.mousePosition;
			//		pieceStartPos = p.MovePiece(g, "up", pieceStartPos, g.tag);
			//		lockedy = true;
			//	}
			//	else {
			//		////Debug.Log("moving down");

			//		mouseStartPos = Input.mousePosition;
			//		pieceStartPos = p.MovePiece(g, "down", pieceStartPos, g.tag);
			//		lockedy = true;
			//	}
			//}

		}
	}

	void OnMouseDown() 
	{
		if (enabled) {

			if (EventSystem.current.IsPointerOverGameObject()) {
				return;
			}

			if (pController.yourTurn) 
			{
				showHighlight = false;
				active = true;
				if (real != null) {
					mouseStartPos = Input.mousePosition;
					validMoves = PopulateValidMovesList(pieceStartPos);
					foreach (PetteiaColliderMover p in validMoves) {
						p.HighlightSpace(true);
					}
					//pieceStartPos = g.position;
					//ik.SetPiece(real.gameObject);
					//ik.SetInital(g);
					real.enabled = false;
					//Cursor.visible = false;
					SpawnDummy();
				}
			}
		}
	}

	void SpawnDummy() 
	{
		dummySpawned = Instantiate(dummy, dummyParent.transform);
		dummySpawned.transform.position = transform.position;
	}

	void DelDummy() {
		Destroy(dummySpawned);
	}

	void OnMouseUp() 
	{
		if (enabled) 
		{
			if (EventSystem.current.IsPointerOverGameObject()) {
				return;
			}

			//Debug.Log("PetteiaMovePiece OnMouseUp: " + name);
			active = false;
			if (real != null) {
				//Debug.Log("Starting the ending function");
				//unlock function
				if (isMoving) {
					ik.SetFinal(real.gameObject.transform);
					//ik.onThemMove = true;
				}
				lockedx = false;
				lockedy = false;
				//mouseEndPos = mouseStartPos;
				//Debug.Log("Checking if the piece was moved or just dropped");
				//end of turn
				if (!(pieceStartPos.x == potentialPos.x && pieceStartPos.y == potentialPos.y)) {
					//Debug.Log("Dropped piece after moving it, preparing to change turn");
					pController.MovePiece(pieceStartPos, potentialPos, "PetteiaW");
					pieceStartPos = potentialPos;
					//pController.CheckCapture();
					//Debug.Log("Preparing to switch turn off of player");
					pController.SwitchTurn();
					pController.PlayMoveSound();
				}
				else {
					//Debug.Log("Dropped piece without moving it");
					showHighlight = true;
				}

				foreach (PetteiaColliderMover p in validMoves) {
					p.HighlightSpace(false);
				}
				real.enabled = true;
				DelDummy();
			}
		}

	}

	private void OnMouseDrag() {
		////Debug.Log("DHAWJLREGHaljsdhflaksjdfh");
		mouseEndPos = Input.mousePosition;
	}

	private List<PetteiaColliderMover> PopulateValidMovesList(Vector2Int startPos) 
	{
		List<PetteiaColliderMover> possibleMoves = new List<PetteiaColliderMover>();
		possibleMoves.Add(pController.BoardSquares[startPos.x, startPos.y]);

		//start at the current position
		//go up one at a time, decreasing y until it's at 0 OR until you hit one occupied square
		for (int y = startPos.y - 1; y >= 0; y--) {
			if (!pController.BoardSquares[startPos.x, y].occupied) {
				possibleMoves.Add(pController.BoardSquares[startPos.x, y]);
			}
			else {
				break;
			}
		}
		//go down one at a time, increasing y until it's at 7 OR until you hit one occupied square
		for (int y = startPos.y + 1; y < pController.BoardSquares.GetLength(1); y++) {
			if (!pController.BoardSquares[startPos.x, y].occupied) {
				possibleMoves.Add(pController.BoardSquares[startPos.x, y]);
			}
			else {
				break;
			}
		}
		//go left one at a time, decreasing x until it's at 0 OR until you hit one occupied square
		for (int x = startPos.x - 1; x >= 0; x--) {
			if (!pController.BoardSquares[x, startPos.y].occupied) {
				possibleMoves.Add(pController.BoardSquares[x, startPos.y]);
			}
			else {
				break;
			}
		}
		//go right one at a time, increasing x until it's at 7 OR until you hit one occupied square
		for (int x = startPos.x + 1; x < pController.BoardSquares.GetLength(0); x++) {
			if (!pController.BoardSquares[x, startPos.y].occupied) {
				possibleMoves.Add(pController.BoardSquares[x, startPos.y]);
			}
			else {
				break;
			}
		}

		return possibleMoves;
	}
	
}
