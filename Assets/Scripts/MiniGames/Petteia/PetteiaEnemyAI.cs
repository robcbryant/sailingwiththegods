//Paul Reichling
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PetteiaEnemyAI : MonoBehaviour
{
	public PetteiaGameController p;
	public DialogPetteia d;
	//public int[,] positions = new int[8, 8];
	public List<GameObject> pieces;
	public bool isMoving;
	public GameObject currentg;
	public int num = 0;

	public Text water;
	public Text food; 

	public GameObject winCanvas;
	script_player_controls playerShip;
	//Some variables are public for debugging and being able to be viewed in the inspector 
	// Start is called before the first frame update
	void Start()
    {
		playerShip = GameObject.FindGameObjectWithTag("playerShip").GetComponent<script_player_controls>();
		winCanvas.SetActive(false);
		isMoving = false;
		
	}

    // Update is called once per frame
    void Update()
    {
	//	Debug.Log("Numpieces" + pieces.Count);
		if (pieces.Count == 0 || Input.GetKeyDown(KeyCode.W)) {
			Debug.Log("youwin!");
			winCanvas.SetActive(true);



			water.text ="+" + playerShip.GameResultWater().ToString();
			food.text = "+" + playerShip.GameResultFood().ToString();

			//	ship.cargo[1].amount_kg < dailyProvisionsKG * ship.crewRoster.Count


		}
		if (Input.GetKeyDown(KeyCode.W)) {
			PrintBoard();
		}
		if(p.yourturn == false && isMoving == false) {
			isMoving = true;
			StartCoroutine(MakeMove()); //Checks if the user has made a move and runs the enemy move command is so
			p.lastPieceMoved = 2;
		}

		
	}
	public void LeaveButton() {
		TavernaController.BackToTavernaMenu();
	}

	IEnumerator MakeMove() {
		
		yield return new WaitForSeconds(1f); 

		string s = "";

		
		
		GameObject go = null;
		foreach (GameObject g in pieces) { //Runs through each available enemy piece and sees if it can capture another piece.
			
			currentg = g;



			//moving up loop
			for (int x = (int)g.GetComponent<Positions>().pos.x; x > 2; x--) {
				//Debug.Log("looking up");
				//Debug.Log(currentg.name);
				if (p.positions[x, (int)g.GetComponent<Positions>().pos.y] != 0 &&
					x != (int)g.GetComponent<Positions>().pos.x) {
					//Debug.Log("BREAKING");
					break;
				}
				if (p.positions[x - 1, (int)g.GetComponent<Positions>().pos.y] == 2
					&& p.positions[x - 2, (int)g.GetComponent<Positions>().pos.y] == 1
					&& p.positions[x, (int)g.GetComponent<Positions>().pos.y] == 0) {
					go = g;
					s = "up";
					num = (int)g.GetComponent<Positions>().pos.x - x;

					goto End;
				}

				if ((int)g.GetComponent<Positions>().pos.y <= 5) {
					//look right while moving 
					if (p.positions[x, 1 + (int)g.GetComponent<Positions>().pos.y] == 2
						&& p.positions[x, 2 + (int)g.GetComponent<Positions>().pos.y] == 1
						&& p.positions[x, (int)g.GetComponent<Positions>().pos.y] == 0) {
						//Debug.Log("right looking");
						go = g;
						s = "up";
						num = (int)g.GetComponent<Positions>().pos.x - x;

						goto End;
					}
				}
				if ((int)g.GetComponent<Positions>().pos.y >= 2) {
					//look left while moving 
					//Debug.Log(1 - (int)g.GetComponent<Positions>().pos.y);

					if (p.positions[x, (int)g.GetComponent<Positions>().pos.y - 1] == 2
						&& p.positions[x, (int)g.GetComponent<Positions>().pos.y - 2] == 1
						&& p.positions[x, (int)g.GetComponent<Positions>().pos.y] == 0) {
						//Debug.Log("left looking");
						go = g;
						s = "up";
						num = (int)g.GetComponent<Positions>().pos.x - x;

						goto End;
					}

				}
			}


			//////////////////////////////////////////////////////////////////////////////////////////


			//moving down loop
			for (int x = (int)g.GetComponent<Positions>().pos.x; x < 5; x++) {
				//Debug.Log("looking down");
				//Debug.Log(currentg.name);
				if (p.positions[x, (int)g.GetComponent<Positions>().pos.y] != 0 && 
					x != (int)g.GetComponent<Positions>().pos.x) {
					//Debug.Log("BREAKING");
					break;
				}
					if (p.positions[x + 1, (int)g.GetComponent<Positions>().pos.y] == 2
						&& p.positions[x + 2, (int)g.GetComponent<Positions>().pos.y] == 1
						&& p.positions[x, (int)g.GetComponent<Positions>().pos.y] == 0) {
						go = g;
						s = "down";
						num = x - (int)g.GetComponent<Positions>().pos.x;

						goto End;
					}

					if ((int)g.GetComponent<Positions>().pos.y <= 5) {
						//look right while moving 
						if (p.positions[x, 1 + (int)g.GetComponent<Positions>().pos.y] == 2
							&& p.positions[x, 2 + (int)g.GetComponent<Positions>().pos.y] == 1
							&& p.positions[x, (int)g.GetComponent<Positions>().pos.y] == 0) {
						//Debug.Log("right looking");
						go = g;
							s = "down";
							num = x - (int)g.GetComponent<Positions>().pos.x;

							goto End;
						}
					}
					if ((int)g.GetComponent<Positions>().pos.y >= 2) {
						//look left while moving 
						//Debug.Log(1 - (int)g.GetComponent<Positions>().pos.y);

						if (p.positions[x, (int)g.GetComponent<Positions>().pos.y - 1] == 2
							&& p.positions[x, (int)g.GetComponent<Positions>().pos.y - 2] == 1
							&& p.positions[x, (int)g.GetComponent<Positions>().pos.y] == 0) {
						//Debug.Log("left looking");
						go = g;
							s = "down";
							num = x - (int)g.GetComponent<Positions>().pos.x;

							goto End;
						}
					
				}
			}

			////////////////////////////////////////////////////////////////////////////////////////

			//moving right loop
			for (int y = (int)g.GetComponent<Positions>().pos.y; y < 5; y++) {
				if (p.positions[(int)g.GetComponent<Positions>().pos.x, y] != 0 &&
					y != (int)g.GetComponent<Positions>().pos.y) {
					//Debug.Log("BREAKING");
					break;
				}
				//Debug.Log("looking right");
				//Debug.Log(currentg.name);
				if (p.positions[(int)g.GetComponent<Positions>().pos.x, y + 1 ] == 2
					&& p.positions[(int)g.GetComponent<Positions>().pos.x, y + 2] == 1
					&& p.positions[(int)g.GetComponent<Positions>().pos.x, y] == 0) {
					go = g;
					s = "right";
					num =  y - (int)g.GetComponent<Positions>().pos.y ;

					goto End;
				}

				if ((int)g.GetComponent<Positions>().pos.x >= 2) {
					//look up while moving 
					if (p.positions[(int)g.GetComponent<Positions>().pos.x - 1, y] == 2
						&& p.positions[(int)g.GetComponent<Positions>().pos.x - 2, y ] == 1
						&& p.positions[(int)g.GetComponent<Positions>().pos.x, y ] == 0) {
						//Debug.Log("up looking");
						go = g;
						s = "right";
						num = y - (int)g.GetComponent<Positions>().pos.y;

						goto End;
					}
				}
				if ((int)g.GetComponent<Positions>().pos.x <=5 ) {
					//look down while moving 
					if (p.positions[(int)g.GetComponent<Positions>().pos.x + 1, y] == 2
						&& p.positions[(int)g.GetComponent<Positions>().pos.x + 2,y ] == 1
						&& p.positions[(int)g.GetComponent<Positions>().pos.x,y ] == 0) {
						//Debug.Log("down looking");
						go = g;
						s = "right";
						num = y - (int)g.GetComponent<Positions>().pos.y;

						goto End;
					}
				}
			}





			////////////////////////////////////////////////////////////////////////////////////////

			//moving left loop
			for (int y = (int)g.GetComponent<Positions>().pos.y; y > 2; y--) {
				if (p.positions[(int)g.GetComponent<Positions>().pos.x, y] != 0 &&
					y != (int)g.GetComponent<Positions>().pos.y) {
					//Debug.Log("BREAKING");
					break;
				}
				//Debug.Log("looking left");
				//Debug.Log(currentg.name);
				if (p.positions[(int)g.GetComponent<Positions>().pos.x, y - 1] == 2
					&& p.positions[(int)g.GetComponent<Positions>().pos.x, y - 2] == 1
					&& p.positions[(int)g.GetComponent<Positions>().pos.x, y] == 0) {
					go = g;
					s = "left";
					num = (int)g.GetComponent<Positions>().pos.y - y ;

					goto End;
				}

				if ((int)g.GetComponent<Positions>().pos.x >= 2) {
					//look up while moving 
					if (p.positions[(int)g.GetComponent<Positions>().pos.x - 1, y] == 2
						&& p.positions[(int)g.GetComponent<Positions>().pos.x - 2, y] == 1
						&& p.positions[(int)g.GetComponent<Positions>().pos.x, y] == 0) {
						//Debug.Log("up looking");
						go = g;
						s = "left";
						num = (int)g.GetComponent<Positions>().pos.y - y;

						goto End;
					}
				}
				if ((int)g.GetComponent<Positions>().pos.x <= 5) {
					//look down while moving 
					if (p.positions[(int)g.GetComponent<Positions>().pos.x + 1, y] == 2
						&& p.positions[(int)g.GetComponent<Positions>().pos.x + 2, y] == 1
						&& p.positions[(int)g.GetComponent<Positions>().pos.x, y] == 0) {
						//Debug.Log("down looking");
						go = g;
						s = "left";
						num = (int)g.GetComponent<Positions>().pos.y - y;

						goto End;
					}
				}
			}















		}
		End:
		if (go != null) {
			//Debug.Log("capture called with these params:");
			//Debug.Log(go.name + " " + s + " " + num);
			StartCoroutine(MovePiece(go, s, num));
			d.EnemyCaptures();
		} else {
			// Moves the piece randomly 1-3 spaces if it cannot find a capture. 
			int tries = 0;
			Rand:
			//Debug.Log("moving randomly");
			
			bool trying = false;
			num = 0;
			
			while (trying == false && tries < 50) {

				tries++;
				Debug.Log(tries);
				int direction = Random.Range(0, 4);
				go = pieces.RandomElement();
				currentg = go;
				//Simplified Version not working 

				//for (int i = 0; i < num - 2; i++) {
				//	if ((int)go.GetComponent<Positions>().pos.x - i > 0) {
				//		//Debug.Log((int)go.GetComponent<Positions>().pos.x - i);
				//		if (p.positions[(int)go.GetComponent<Positions>().pos.y ,
				//			(int)go.GetComponent<Positions>().pos.x - i] == 0) {
				//			//Debug.Log("abley" + (int)go.GetComponent<Positions>().pos.y);
				//			//Debug.Log("ablex" + ((int)go.GetComponent<Positions>().pos.x - i));
				//			able = true;
				//			useNum =  i;
				//		} else {

				//			able = false;
				//			break;
				//		}
				//	}
				//	else {
				//		able = false;
				//		break;
				//	}
				//}

				////////////////////////////////////////////////////////////////////////

				if (direction == 0) {
					s = "up";
					//Debug.Log("moving randomly up ");
					if ((int)go.GetComponent<Positions>().pos.x - 1 > 0) {
						//if (p.positions[(int)go.GetComponent<Positions>().pos.y,
						//			(int)go.GetComponent<Positions>().pos.x - 1] == 0) {
						if (p.positions[(int)go.GetComponent<Positions>().pos.x - 1,
									(int)go.GetComponent<Positions>().pos.y] == 0) {
							trying = true;
							num = 1;
							//Debug.Log("can move 1");
							////Debug.Log((int)go.GetComponent<Positions>().pos.y + ";" +
									//((int)go.GetComponent<Positions>().pos.x - 1));
							if ((int)go.GetComponent<Positions>().pos.x - 2 > 0) {
								if (p.positions[(int)go.GetComponent<Positions>().pos.x - 2,
									(int)go.GetComponent<Positions>().pos.y] == 0) {
									num = Random.Range(1, 3);
									//Debug.Log("can move 2");
									////Debug.Log((int)go.GetComponent<Positions>().pos.y + ";" +
									//((int)go.GetComponent<Positions>().pos.x - 2));
									if ((int)go.GetComponent<Positions>().pos.x - 3 > 0) {
										if (p.positions[(int)go.GetComponent<Positions>().pos.x - 3,
									(int)go.GetComponent<Positions>().pos.y] == 0) {
											num = Random.Range(1, 4);
											//Debug.Log("can move 3");
											////Debug.Log((int)go.GetComponent<Positions>().pos.y + ";" +
									//((int)go.GetComponent<Positions>().pos.x - 3));
										}
										else {
											trying = true;
											num = Random.Range(1, 3);
											break;
										}
									}
								}
								else {
									trying = true;
									num = 1;
									break;
								}
								

							}
						}
						else {
							trying = false;
							break;
						}
						
					}
				}

				
				if (direction ==1) {
					s = "left";
					//Debug.Log("moving randomly left ");
					if ((int)go.GetComponent<Positions>().pos.y - 1 > 0) {
						//if (p.positions[(int)go.GetComponent<Positions>().pos.y - 1,
						//			(int)go.GetComponent<Positions>().pos.x] == 0) {
						if (p.positions[(int)go.GetComponent<Positions>().pos.x,
									(int)go.GetComponent<Positions>().pos.y - 1] == 0) {
							trying = true;
							num = 1;
							////Debug.Log((int)go.GetComponent<Positions>().pos.y - 1 + ";" +
								//	(int)go.GetComponent<Positions>().pos.x);
							if ((int)go.GetComponent<Positions>().pos.y - 2 > 0) {
								if (p.positions[(int)go.GetComponent<Positions>().pos.x,
									(int)go.GetComponent<Positions>().pos.y - 2] == 0) {
									num = Random.Range(1, 3);
									//Debug.Log("can move 2");
								//	//Debug.Log((int)go.GetComponent<Positions>().pos.y - 2 + ";" +
								//	(int)go.GetComponent<Positions>().pos.x);
									if ((int)go.GetComponent<Positions>().pos.y - 3 > 0) {
										if (p.positions[(int)go.GetComponent<Positions>().pos.x,
									(int)go.GetComponent<Positions>().pos.y - 3] == 0) {
											num = Random.Range(1, 4);
											//Debug.Log("can move 3");
										//	//Debug.Log((int)go.GetComponent<Positions>().pos.y - 3 + ";" +
									//(int)go.GetComponent<Positions>().pos.x);
										}
										else {
											trying = true;
											num = Random.Range(1, 3);
											break;
										}
									}
								}
								else {
									trying = true;
									num = 1;
									break;
								}
								

							}
						}
						else {
							trying = false;
							break;
						}
						
					}
				}
			
				if (direction == 2) {
					s = "right";
					//Debug.Log("moving randomly right ");
					if ((int)go.GetComponent<Positions>().pos.y + 1 < 7) {
						//if (p.positions[(int)go.GetComponent<Positions>().pos.y - 1,
						//			(int)go.GetComponent<Positions>().pos.x] == 0) {
						if (p.positions[(int)go.GetComponent<Positions>().pos.x,
									(int)go.GetComponent<Positions>().pos.y + 1] == 0) {
							trying = true;
							num = 1;
							////Debug.Log((int)go.GetComponent<Positions>().pos.y - 1 + ";" +
							//	(int)go.GetComponent<Positions>().pos.x);
							if ((int)go.GetComponent<Positions>().pos.y + 2 < 7) {
								if (p.positions[(int)go.GetComponent<Positions>().pos.x,
									(int)go.GetComponent<Positions>().pos.y + 2] == 0) {
									num = Random.Range(1, 3);
									//Debug.Log("can move 2");
									//	//Debug.Log((int)go.GetComponent<Positions>().pos.y - 2 + ";" +
									//	(int)go.GetComponent<Positions>().pos.x);
									if ((int)go.GetComponent<Positions>().pos.y + 3 < 7) {
										if (p.positions[(int)go.GetComponent<Positions>().pos.x, 
									(int)go.GetComponent<Positions>().pos.y + 3] == 0) {
											num = Random.Range(1, 4);
											//Debug.Log("can move 3");
											//	//Debug.Log((int)go.GetComponent<Positions>().pos.y - 3 + ";" +
											//(int)go.GetComponent<Positions>().pos.x);
										}
										else {
											trying = true;
											num = Random.Range(1, 3);
											break;
										}
									}
								}
								else {
									trying = true;
									num = 1;
									break;
								}


							}
						}
						else {
							trying = false;
							break;
						}

					}
				}


				if (direction == 3) {
					s = "down";
					
					//Debug.Log("moving randomly down ");
					if ((int)go.GetComponent<Positions>().pos.x + 1 < 7) {
						//if (p.positions[(int)go.GetComponent<Positions>().pos.y,
						//			(int)go.GetComponent<Positions>().pos.x - 1] == 0) {
						if (p.positions[(int)go.GetComponent<Positions>().pos.x + 1,
									(int)go.GetComponent<Positions>().pos.y] == 0) {
							trying = true;
							num = 1;
							//Debug.Log("can move 1");
							////Debug.Log((int)go.GetComponent<Positions>().pos.y + ";" +
							//((int)go.GetComponent<Positions>().pos.x - 1));
							if ((int)go.GetComponent<Positions>().pos.x + 2 < 7) {
								if (p.positions[(int)go.GetComponent<Positions>().pos.x + 2,
									(int)go.GetComponent<Positions>().pos.y] == 0) {
									num = Random.Range(1, 3);
									//Debug.Log("can move 2");
									////Debug.Log((int)go.GetComponent<Positions>().pos.y + ";" +
									//((int)go.GetComponent<Positions>().pos.x - 2));
									if ((int)go.GetComponent<Positions>().pos.x + 3 < 7) {
										if (p.positions[(int)go.GetComponent<Positions>().pos.x + 3,
									(int)go.GetComponent<Positions>().pos.y] == 0) {
											num = Random.Range(1, 4);
											//Debug.Log("can move 3");
											////Debug.Log((int)go.GetComponent<Positions>().pos.y + ";" +
											//((int)go.GetComponent<Positions>().pos.x - 3));
										}
										else {
											trying = true;
											num = Random.Range(1, 3);
											break;
										}
									}
								}
								else {
									trying = true;
									num = 1;
									break;
								}


							}
						}
						else {
							trying = false;
							break;
						}

					}
				}

				

			}
			if (num == 0) {
				if (tries >= 50) {
					StartCoroutine(MovePiece(go, s, num)); //Move cant be found - pass turn
					Debug.Log("passing my turn");
					//Need some dialouge here like "I pass my turn TODO"
				}
				else {
					goto Rand; //Needs to make sure that the piece is not trying to move zero squares, since this isn't a legal move
				}
			}
			else {
				//Debug.Log("random called with these params:");
				//Debug.Log(go.name + " " + s + " " + num);
				StartCoroutine(MovePiece(go, s, num));
				Debug.Log(tries);
			}
		}


		//find out which piece we want to move AND why and where it needs to go 


		p.moveSound.pitch = Random.Range(0.7f, 1.1f);
		p.moveSound.Play();
		yield return new WaitForSeconds(1f);
		p.yourturn = true;
		
		isMoving = false;
	}
	IEnumerator MovePiece(GameObject piece, string dir, int dist) 
		{
		
		int x, y;
		////Debug test
		x = (int)piece.GetComponent<Positions>().pos.x;
		y = (int)piece.GetComponent<Positions>().pos.y;
		p.positions[x, y] = 0;
		//Debug.Log((int)piece.GetComponent<Positions>().pos.x);
		//Debug.Log((int)piece.GetComponent<Positions>().pos.y);

		//piece.transform.Translate(Vector3.back * 6.25f);

		if (dir == "up") {
			piece.transform.Translate(Vector3.forward * 6.25f * dist);
		}
		if (dir == "left") {
			piece.transform.Translate(Vector3.left * 6.25f * dist);
		}
		if (dir == "right") {
			piece.transform.Translate(Vector3.right * 6.25f * dist);
		}
		if (dir == "down") {
			piece.transform.Translate(Vector3.back * 6.25f * dist);
		}







		yield return new WaitForSeconds(0.5f);


		x = (int)piece.GetComponent<Positions>().pos.x;
		y = (int)piece.GetComponent<Positions>().pos.y;
		p.positions[x, y] = 1;
		//Debug.Log((int)piece.GetComponent<Positions>().pos.x);
		//Debug.Log((int)piece.GetComponent<Positions>().pos.y);
		//Debug.Log(currentg.name);
		PrintBoard();
	}




	void PrintBoard() {
		string s = "  ";
		for (int i = 0; i < 8; i++) {
			s += "\n\n";
			for (int j = 0; j < 8; j++) {
				s += "  ";
				s += p.positions[i, j];
			}
		}

		Debug.Log(s);
		p.boardText = s;
	}



}
