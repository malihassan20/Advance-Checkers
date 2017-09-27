using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiPlayerController : MonoBehaviour
{
	public GameObject yellowKing,brownKing;
	//array to store the players boxes on which they are placed
	string[] player1ColliderBoxes = new string[12];
    string[] player2ColliderBoxes = new string[12];
    string[] player1KingColliderBoxes = new string[12];
    string[] player2KingColliderBoxes = new string[12];
	bool click=true,restart=false,move=false;
	bool player1Move = true;
    GameObject[] source = new GameObject[3];
    GameObject[] target = new GameObject[2];
	int pieceBlockIndex=0,pieceIndex=0;
    GameObject box1 = null, box2 = null, box3 = null, box4 = null, box5 = null, box6 = null, box7 = null, box8 = null;
    int player1DeadPieces = 0, player2DeadPieces = 0;
    int noOfP1Kings = 0, noOfP2Kings = 0;
    int target1XY = 0, target2XY = 0, target3XY = 0, target4XY = 0, target5XY = 0, target6XY = 0, target7XY = 0, target8XY = 0, sourceXY = 0;
    string target1xy = "", target2xy = "", target3xy = "", target4xy = "", target5xy = "", target6xy = "", target7xy = "", target8xy = "";

	public Text turnText,winLoseText;
	public Button restartButton;

	//start is called during initialization
	void Start()
	{
		restart = false;
		turnText.text = "";winLoseText.text="";
		restartButton.gameObject.SetActive (false);
		SetPlayersColliderBoxes ();
	}

	void Update()
	{
		if (restart) 
		{
			restartButton.gameObject.SetActive (true);
		}
		if (player1DeadPieces == 12) {
			winLoseText.text="Player 2 Wins!";
			restart=true;
		} else if (player2DeadPieces == 12) {
			winLoseText.text="Player 1 Wins!";
			restart=true;
		} else {
			if (click == true) {
				//if its player 1 move
				if(player1Move==true)
					turnText.text="Player 1 turn";
				else if(player1Move==false)
					turnText.text="Player 2 turn";
				if (Input.GetMouseButtonDown (0)) {
					//Generate ray from main camera and mouse position
					Ray R = Camera.main.ScreenPointToRay (Input.mousePosition);
					//Will store info about all intersections
					RaycastHit[] HitInfo = Physics.RaycastAll (R);
					//Debug.Log (HitInfo.Length);
					//Test to see if ray intersects with any colliders
					if (HitInfo != null && HitInfo.Length == 3) {
						//check if it is player 1 move
						if (player1Move == true) {
							for (int i=0; i<3; i++) {
								source [i] = HitInfo [i].collider.gameObject;
								if (CheckIfPlayer1Piece (source [i]) == true)
									pieceIndex = i;
								if (CheckIfPlayer1Piece (source [i]) != true && source [i].name != "Plane")
									pieceBlockIndex = i;
							}
							if (CheckIfPlayer1Piece (source [pieceIndex]) == true) {
								//select the piece and highlight all possible moves
								PieceSelection (source);
							}
						} else if (player1Move == false) {//if it is player 2 move
							for (int i=0; i<3; i++) {
								source [i] = HitInfo [i].collider.gameObject;
								if (CheckIfPlayer2Piece (source [i]) == true)
									pieceIndex = i;
								if (CheckIfPlayer2Piece (source [i]) != true && source [i].name != "Plane")
									pieceBlockIndex = i;
							}
							if (CheckIfPlayer2Piece (source [pieceIndex]) == true) {
								PieceSelection (source);
							}
						}
					} 
				}
			} else if (click == false) {
				//now the piece has been selected and now we get the target box where the player desired to move
				if (Input.GetMouseButtonDown (0)) {
					//Generate ray from main camera and mouse position
					Ray R = Camera.main.ScreenPointToRay (Input.mousePosition);
					//Will store info about all intersections
					RaycastHit[] HitInfo = Physics.RaycastAll (R);
					//Debug.Log (HitInfo.Length);
					int blockIndex = 0;
					//Test to see if ray intersects with any colliders
					if (HitInfo != null && HitInfo.Length == 2) {

						for (int i=0; i<2; i++) {
							target [i] = HitInfo [i].collider.gameObject;
							if (target [i].name != "Plane")
								blockIndex = i;
						} 
						Vector3 temp = new Vector3 (0, 0, -3.0f);

						//if the target place is one of the possible moves 
                        if ((target[blockIndex].name == target1xy && box1 != null) || (target[blockIndex].name == target2xy && box2 != null) || (target[blockIndex].name == target3xy && box3 != null) || (target[blockIndex].name == target4xy && box4 != null) || (target[blockIndex].name == target5xy && box5 != null) || (target[blockIndex].name == target6xy && box6 != null) || (target[blockIndex].name == target7xy && box7 != null) || (target[blockIndex].name == target8xy && box8 != null)) 
						{
							//update the piece position
							source [pieceIndex].transform.position = target [blockIndex].transform.position;
							source [pieceIndex].transform.position += temp;
							move = true;
							int srcXY, trgXY;
							int.TryParse (source [pieceBlockIndex].name, out srcXY);
							int.TryParse (target [blockIndex].name, out trgXY);

							//make king if each player reaches opponent corner
							if(player1Move==false)
							{
								if(CheckIfP1KingPiece(source[pieceIndex])==false)
								{
									if(trgXY==81 || trgXY==82 || trgXY==83 || trgXY==84 || trgXY==85 || trgXY==86 || trgXY==87 || trgXY==88)
									{
										GameObject p=GameObject.Find("Player1");
										GameObject t=Instantiate(yellowKing,new Vector3(0,0,0),Quaternion.identity)as GameObject;
										//set p to be the parent of t
										t.transform.parent=p.transform;
										//place it to the same position
										t.transform.position=source [pieceIndex].transform.position;
										t.transform.transform.Rotate(-90,0,0);
										//rename the gameobject to differentiate 
										if(noOfP1Kings==0)
											t.name="yellowKing";
										else if(noOfP1Kings==1)
											t.name="yellowKing 1";
										else if(noOfP1Kings==2)
											t.name="yellowKing 2";
										else if(noOfP1Kings==3)
											t.name="yellowKing 3";
										else if(noOfP1Kings==4)
											t.name="yellowKing 4";
										else if(noOfP1Kings==5)
											t.name="yellowKing 5";
										else if(noOfP1Kings==6)
											t.name="yellowKing 6";
										else if(noOfP1Kings==7)
											t.name="yellowKing 7";
										else if(noOfP1Kings==8)
											t.name="yellowKing 8";
										else if(noOfP1Kings==9)
											t.name="yellowKing 9";
										else if(noOfP1Kings==10)
											t.name="yellowKing 10";
										else if(noOfP1Kings==11)
											t.name="yellowKing 11";
										UpdatePlayer1ColliderBoxes (source [pieceIndex]);
										SetPlayer1KingColliderBox(target [blockIndex]);
										Destroy(GameObject.Find (source [pieceIndex].name.ToString()));
										source[pieceIndex]=null;
									}
								}
							}
							if(player1Move==true)
							{
								if(CheckIfP2KingPiece(source[pieceIndex])==false)
								{
									if(trgXY==11 || trgXY==12 || trgXY==13 || trgXY==14 || trgXY==15 || trgXY==16 || trgXY==17 || trgXY==18)
									{
										GameObject p=GameObject.Find("Player2");
										GameObject t=Instantiate(brownKing,new Vector3(p.transform.position.x,p.transform.position.y,p.transform.position.z),Quaternion.identity)as GameObject;
										t.transform.parent=p.transform;
										t.transform.position=source [pieceIndex].transform.position;
										t.transform.transform.Rotate(270,0,0);
										//rename the gameobject to differentiate 
										if(noOfP2Kings==0)
											t.name="brownKing";
										else if(noOfP2Kings==1)
											t.name="brownKing 1";
										else if(noOfP2Kings==2)
											t.name="brownKing 2";
										else if(noOfP2Kings==3)
											t.name="brownKing 3";
										else if(noOfP2Kings==4)
											t.name="brownKing 4";
										else if(noOfP2Kings==5)
											t.name="brownKing 5";
										else if(noOfP2Kings==6)
											t.name="brownKing 6";
										else if(noOfP2Kings==7)
											t.name="brownKing 7";
										else if(noOfP2Kings==8)
											t.name="brownKing 8";
										else if(noOfP2Kings==9)
											t.name="brownKing 9";
										else if(noOfP2Kings==10)
											t.name="brownKing 10";
										else if(noOfP2Kings==11)
											t.name="brownKing 11";
										UpdatePlayer2ColliderBoxes (source [pieceIndex]);
										SetPlayer2KingColliderBox(target[blockIndex]);
										Destroy(GameObject.Find (source [pieceIndex].name.ToString()));
										source[pieceIndex]=null;
									}
								}
							}

							//check if player has double junp to kill the opponent player piece
							//if it has then destroy the kill piece and update the positions according to it
							if (trgXY == srcXY - 22 || trgXY == srcXY + 22 || trgXY == srcXY - 18 || trgXY == srcXY + 18 || trgXY == srcXY - 2 || trgXY == srcXY + 2 || trgXY == srcXY - 20 || trgXY == srcXY + 20) {
								if (trgXY == srcXY - 22) {
									int t = srcXY - 11;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										//check if player 1 has killed player 2 piece
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										//if player 2 has killed player 1 piece
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								} else if (trgXY == srcXY + 22) {
									int t = srcXY + 11;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								} else if (trgXY == srcXY - 18) {
									int t = srcXY - 9;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								} else if (trgXY == srcXY + 18) {
									int t = srcXY + 9;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								}
								else if (trgXY == srcXY - 2) {
									int t = srcXY - 1;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										//check if player 1 has killed player 2 piece
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										//if player 2 has killed player 1 piece
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								} else if (trgXY == srcXY + 2) {
									int t = srcXY + 1;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								} else if (trgXY == srcXY - 20) {
									int t = srcXY - 10;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								} else if (trgXY == srcXY + 20) {
									int t = srcXY + 10;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (player1Move == true) {
										if(CheckIfP1KingPiece(GameObject.Find (p)))
											UpdatePlayer1KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer1ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player1DeadPieces++;
									} else if (player1Move == false) {
										if(CheckIfP2KingPiece(GameObject.Find (p)))
											UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
										else
											UpdatePlayer2ColliderBoxes (GameObject.Find (p));
										Destroy (GameObject.Find (p));
										player2DeadPieces++;
									}
								}
							}

							//update the pieces boxes numbers i-e row-column
							if(source[pieceIndex]!=null)
							{
								if (player1Move == false) {//player 1 had moved so update the block on which the piece is above piece
									if (source [pieceIndex].name == "yellow")
										player1ColliderBoxes [0] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 1")
										player1ColliderBoxes [1] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 2")
										player1ColliderBoxes [2] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 3")
										player1ColliderBoxes [3] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 4")
										player1ColliderBoxes [4] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 5")
										player1ColliderBoxes [5] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 6")
										player1ColliderBoxes [6] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 7")
										player1ColliderBoxes [7] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 8")
										player1ColliderBoxes [8] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 9")
										player1ColliderBoxes [9] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 10")
										player1ColliderBoxes [10] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellow 11")
										player1ColliderBoxes [11] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing")
										player1KingColliderBoxes [0] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 1")
										player1KingColliderBoxes [1] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 2")
										player1KingColliderBoxes [2] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 3")
										player1KingColliderBoxes [3] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 4")
										player1KingColliderBoxes [4] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 5")
										player1KingColliderBoxes [5] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 6")
										player1KingColliderBoxes [6] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 7")
										player1KingColliderBoxes [7] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 8")
										player1KingColliderBoxes [8] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 9")
										player1KingColliderBoxes [9] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 10")
										player1KingColliderBoxes [10] = target [blockIndex].transform.name;
									else if (source [pieceIndex].name == "yellowKing 11")
										player1KingColliderBoxes [11] = target [blockIndex].transform.name;
								} else if (player1Move == true) {
									if (source [pieceIndex].name == "brown")
										player2ColliderBoxes [0] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 1")
										player2ColliderBoxes [1] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 2")
										player2ColliderBoxes [2] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 3")
										player2ColliderBoxes [3] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 4")
										player2ColliderBoxes [4] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 5")
										player2ColliderBoxes [5] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 6")
										player2ColliderBoxes [6] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 7")
										player2ColliderBoxes [7] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 8")
										player2ColliderBoxes [8] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 9")
										player2ColliderBoxes [9] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 10")
										player2ColliderBoxes [10] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brown 11")
										player2ColliderBoxes [11] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing")
										player2KingColliderBoxes [0] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 1")
										player2KingColliderBoxes [1] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 2")
										player2KingColliderBoxes [2] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 3")
										player2KingColliderBoxes [3] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 4")
										player2KingColliderBoxes [4] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 5")
										player2KingColliderBoxes [5] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 6")
										player2KingColliderBoxes [6] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 7")
										player2KingColliderBoxes [7] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 8")
										player2KingColliderBoxes [8] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 9")
										player2KingColliderBoxes [9] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 10")
										player2KingColliderBoxes [10] = target [blockIndex].name;
									else if (source [pieceIndex].name == "brownKing 11")
										player2KingColliderBoxes [11] = target [blockIndex].name;
								}
							}

						}
						if (move == true) {
							//unhighlight every box
							source [pieceBlockIndex].GetComponent<MeshRenderer> ().material.color = Color.white;
							source [pieceBlockIndex].GetComponent<MeshRenderer> ().enabled = false;
							if (box1 != null) {
								box1.GetComponent<MeshRenderer> ().material.color = Color.white;
								box1.GetComponent<MeshRenderer> ().enabled = false;
							}
							if (box2 != null) {
								box2.GetComponent<MeshRenderer> ().material.color = Color.white;
								box2.GetComponent<MeshRenderer> ().enabled = false;
							}
							if (box3 != null) {
								box3.GetComponent<MeshRenderer> ().material.color = Color.white;
								box3.GetComponent<MeshRenderer> ().enabled = false;
							}
							if (box4 != null) {
								box4.GetComponent<MeshRenderer> ().material.color = Color.white;
								box4.GetComponent<MeshRenderer> ().enabled = false;
							}
							if (box5 != null) {
								box5.GetComponent<MeshRenderer> ().material.color = Color.white;
								box5.GetComponent<MeshRenderer> ().enabled = false;
							}
							if (box6 != null) {
								box6.GetComponent<MeshRenderer> ().material.color = Color.white;
								box6.GetComponent<MeshRenderer> ().enabled = false;
							}
							if (box7 != null) {
								box7.GetComponent<MeshRenderer> ().material.color = Color.white;
								box7.GetComponent<MeshRenderer> ().enabled = false;
							}
							if (box8 != null) {
								box8.GetComponent<MeshRenderer> ().material.color = Color.white;
								box8.GetComponent<MeshRenderer> ().enabled = false;
							}
							source [0] = null;
							source [1] = null;
							source [2] = null;
							target [0] = null;
							target [1] = null;
							box1 = null;box2 = null;box3 = null;box4 = null;box5 = null;box6 = null;box7 = null;box8 = null;
                            target1XY = 0; target2XY = 0; target3XY = 0; target4XY = 0; target5XY = 0; target6XY = 0; target7XY = 0; target8XY = 0; sourceXY = 0;
                            target1xy = ""; target2xy = ""; target3xy = ""; target4xy = ""; target5xy = ""; target6xy = ""; target7xy = ""; target8xy = "";
							click = true;
							move = false;
						}
					}
				}
			}
		}
	}

	//function to select checkers pieces and highlight all possible moves
	void PieceSelection(GameObject[] source)
	{
		int.TryParse(source[pieceBlockIndex].name,out sourceXY);
		bool temp = false;
		//to check if player select the checker which cannot move
		if(sourceXY==11 || sourceXY==21 || sourceXY==31 || sourceXY==41 || sourceXY==51 || sourceXY==61 || sourceXY==71)
		{
			target1XY=sourceXY+11;
			target2XY=sourceXY-9;
			target3XY=sourceXY+1;
			target4XY=sourceXY+10;
			target5XY=sourceXY-10;
			target1xy=target1XY.ToString();
			target2xy=target2XY.ToString();
			target3xy=target3XY.ToString();
			target4xy=target4XY.ToString();
			target5xy=target5XY.ToString();

			if(player1Move==true)
			{
				//check if next block prediction number become out of range
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP2(box1)==true)
					{
						target1XY+=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP2(box2)==true)
					{
						target2XY-=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP1KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP2(box3)==true)
						{
							target3XY+=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP2(box4)==true)
						{
							target4XY+=10;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP2(box5)==true)
						{
							target5XY-=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
			else if(player1Move==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP1(box1)==true)
					{
						target1XY+=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP1(box2)==true)
					{
						target2XY-=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP2KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP1(box3)==true)
						{
							target3XY+=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP1(box4)==true)
						{
							target4XY+=10;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP1(box5)==true)
						{
							target5XY-=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
		}
		else if(sourceXY==12 || sourceXY==13 || sourceXY==14 || sourceXY==15 || sourceXY==16 || sourceXY==17)
		{
			target1XY=sourceXY+11;
			target2XY=sourceXY+9;
			target3XY=sourceXY-1;
			target4XY=sourceXY+1;
			target5XY=sourceXY+10;
			target1xy=target1XY.ToString();
			target2xy=target2XY.ToString();
			target3xy=target3XY.ToString();
			target4xy=target4XY.ToString();
			target5xy=target5XY.ToString();

			if(player1Move==true)
			{
				//check if next block prediction number become out of range
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP2(box1)==true)
					{
						target1XY+=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP2(box2)==true)
					{
						target2XY+=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP1KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP2(box3)==true)
						{
							target3XY-=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP2(box4)==true)
						{
							target4XY+=1;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP2(box5)==true)
						{
							target5XY+=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
			else if(player1Move==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP1(box1)==true)
					{
						target1XY+=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP1(box2)==true)
					{
						target2XY+=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP2KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP1(box3)==true)
						{
							target3XY-=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP1(box4)==true)
						{
							target4XY+=1;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP1(box5)==true)
						{
							target5XY+=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
		}
		else if(sourceXY==18 || sourceXY==28 || sourceXY==38 || sourceXY==48 || sourceXY==58 || sourceXY==68 || sourceXY==78)
		{
			target1XY=sourceXY-11;
			target2XY=sourceXY+9;
			target3XY=sourceXY-1;
			target4XY=sourceXY+10;
			target5XY=sourceXY-10;
			target1xy=target1XY.ToString();
			target2xy=target2XY.ToString();
			target3xy=target3XY.ToString();
			target4xy=target4XY.ToString();
			target5xy=target5XY.ToString();

			if(player1Move==true)
			{
				//check if next block prediction number become out of range
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP2(box1)==true)
					{
						target1XY-=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP2(box2)==true)
					{
						target2XY+=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP1KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP2(box3)==true)
						{
							target3XY-=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP2(box4)==true)
						{
							target4XY+=10;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP2(box5)==true)
						{
							target5XY-=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
			else if(player1Move==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP1(box1)==true)
					{
						target1XY-=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP1(box2)==true)
					{
						target2XY+=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP2KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP1(box3)==true)
						{
							target3XY-=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP1(box4)==true)
						{
							target4XY+=10;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP1(box5)==true)
						{
							target5XY-=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
		}
		else if(sourceXY==81 || sourceXY==82 || sourceXY==83 || sourceXY==84 || sourceXY==85 || sourceXY==86 || sourceXY==87 || sourceXY==88)
		{
			target1XY=sourceXY-11;
			target2XY=sourceXY-9;
			target3XY=sourceXY+1;
			target4XY=sourceXY-1;
			target5XY=sourceXY-10;
			target1xy=target1XY.ToString();
			target2xy=target2XY.ToString();
			target3xy=target3XY.ToString();
			target4xy=target4XY.ToString();
			target5xy=target5XY.ToString();

			if(player1Move==true)
			{
				//check if next block prediction number become out of range
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP2(box1)==true)
					{
						target1XY-=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP2(box2)==true)
					{
						target2XY-=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP1KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP2(box3)==true)
						{
							target3XY+=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP2(box4)==true)
						{
							target4XY-=1;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP2(box5)==true)
						{
							target5XY-=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
			else if(player1Move==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP1(box1)==true)
					{
						target1XY-=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP1(box2)==true)
					{
						target2XY-=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP2KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target3xy))
					{
						box3=GameObject.Find(target3xy);
						if(CheckIfNextPieceIsOfP1(box3)==true)
						{
							target3XY+=1;
							target3xy=target3XY.ToString();
							if(CheckIfFromAvailableBlocks(target3xy))
							{
								box3=GameObject.Find(target3xy);
								if(!CheckIfTargetPlaceHavePiece(box3))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box3.GetComponent<MeshRenderer>().enabled=true;
									box3.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target4xy))
					{
						box4=GameObject.Find(target4xy);
						if(CheckIfNextPieceIsOfP1(box4)==true)
						{
							target4XY-=1;
							target4xy=target4XY.ToString();
							if(CheckIfFromAvailableBlocks(target4xy))
							{
								box4=GameObject.Find(target4xy);
								if(!CheckIfTargetPlaceHavePiece(box4))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box4.GetComponent<MeshRenderer>().enabled=true;
									box4.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP1(box5)==true)
						{
							target5XY-=10;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
		}
		else
		{
			target1XY=sourceXY+11;
			target2XY=sourceXY+9;
			target3XY=sourceXY-11;
			target4XY=sourceXY-9;
			target5XY=sourceXY+1;
			target6XY=sourceXY-1;
			target7XY=sourceXY+10;
			target8XY=sourceXY-10;

			target1xy=target1XY.ToString();
			target2xy=target2XY.ToString();
			target3xy=target3XY.ToString();
			target4xy=target4XY.ToString();
			target5xy=target5XY.ToString();
			target6xy=target6XY.ToString();
			target7xy=target7XY.ToString();
			target8xy=target8XY.ToString();


			if(player1Move==true)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP2(box1)==true)
					{
						target1XY+=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP2(box2)==true)
					{
						target2XY+=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target3xy))
				{
					box3=GameObject.Find(target3xy);
					if(CheckIfCornerBlocks(target3XY)==false && CheckIfNextPieceIsOfP2(box3)==true)
					{
						target3XY-=11;
						target3xy=target3XY.ToString();
						if(CheckIfFromAvailableBlocks(target3xy))
						{
							box3=GameObject.Find(target3xy);
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box3))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box3.GetComponent<MeshRenderer>().enabled=true;
							box3.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target4xy))
				{
					box4=GameObject.Find(target4xy);
					if(CheckIfCornerBlocks(target4XY)==false && CheckIfNextPieceIsOfP2(box4)==true)
					{
						target4XY-=9;
						target4xy=target4XY.ToString();
						if(CheckIfFromAvailableBlocks(target4xy))
						{
							box4=GameObject.Find(target4xy);
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box4))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box4.GetComponent<MeshRenderer>().enabled=true;
							box4.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP1KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP2(box5)==true)
						{
							target5XY+=1;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target6xy))
					{
						box6=GameObject.Find(target6xy);
						if(CheckIfNextPieceIsOfP2(box6)==true)
						{
							target6XY-=1;
							target6xy=target6XY.ToString();
							if(CheckIfFromAvailableBlocks(target6xy))
							{
								box6=GameObject.Find(target6xy);
								if(!CheckIfTargetPlaceHavePiece(box6))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box6.GetComponent<MeshRenderer>().enabled=true;
									box6.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box6))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box6.GetComponent<MeshRenderer>().enabled=true;
								box6.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target7xy))
					{
						box7=GameObject.Find(target7xy);
						if(CheckIfNextPieceIsOfP2(box7)==true)
						{
							target7XY+=10;
							target7xy=target7XY.ToString();
							if(CheckIfFromAvailableBlocks(target7xy))
							{
								box7=GameObject.Find(target7xy);
								if(!CheckIfTargetPlaceHavePiece(box7))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box7.GetComponent<MeshRenderer>().enabled=true;
									box7.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box7))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box7.GetComponent<MeshRenderer>().enabled=true;
								box7.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target8xy))
					{
						box8=GameObject.Find(target8xy);
						if(CheckIfNextPieceIsOfP2(box8)==true)
						{
							target8XY-=10;
							target8xy=target8XY.ToString();
							if(CheckIfFromAvailableBlocks(target8xy))
							{
								box8=GameObject.Find(target8xy);
								if(!CheckIfTargetPlaceHavePiece(box8))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box8.GetComponent<MeshRenderer>().enabled=true;
									box8.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box8))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box8.GetComponent<MeshRenderer>().enabled=true;
								box8.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
			else if(player1Move==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfP1(box1)==true)
					{
						target1XY+=11;
						target1xy=target1XY.ToString();
						if(CheckIfFromAvailableBlocks(target1xy))
						{
							box1=GameObject.Find(target1xy);
							if(!CheckIfTargetPlaceHavePiece(box1))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box1.GetComponent<MeshRenderer>().enabled=true;
								box1.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box1))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box1.GetComponent<MeshRenderer>().enabled=true;
							box1.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target2xy))
				{
					box2=GameObject.Find(target2xy);
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfP1(box2)==true)
					{
						target2XY+=9;
						target2xy=target2XY.ToString();
						if(CheckIfFromAvailableBlocks(target2xy))
						{
							box2=GameObject.Find(target2xy);
							if(!CheckIfTargetPlaceHavePiece(box2))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box2.GetComponent<MeshRenderer>().enabled=true;
								box2.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box2))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box2.GetComponent<MeshRenderer>().enabled=true;
							box2.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target3xy))
				{
					box3=GameObject.Find(target3xy);
					if(CheckIfCornerBlocks(target3XY)==false && CheckIfNextPieceIsOfP1(box3)==true)
					{
						target3XY-=11;
						target3xy=target3XY.ToString();
						if(CheckIfFromAvailableBlocks(target3xy))
						{
							box3=GameObject.Find(target3xy);
							if(!CheckIfTargetPlaceHavePiece(box3))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box3.GetComponent<MeshRenderer>().enabled=true;
								box3.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box3))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box3.GetComponent<MeshRenderer>().enabled=true;
							box3.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				if(CheckIfFromAvailableBlocks(target4xy))
				{
					box4=GameObject.Find(target4xy);
					if(CheckIfCornerBlocks(target4XY)==false && CheckIfNextPieceIsOfP1(box4)==true)
					{
						target4XY-=9;
						target4xy=target4XY.ToString();
						if(CheckIfFromAvailableBlocks(target4xy))
						{
							box4=GameObject.Find(target4xy);
							if(!CheckIfTargetPlaceHavePiece(box4))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box4.GetComponent<MeshRenderer>().enabled=true;
								box4.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					else
					{
						if(!CheckIfTargetPlaceHavePiece(box4))
						{
							source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
							source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
							box4.GetComponent<MeshRenderer>().enabled=true;
							box4.GetComponent<MeshRenderer>().material.color=Color.green;
							temp=true;
						}
					}
				}
				//check if king is selected
				if(CheckIfP2KingPiece(source[pieceIndex]))
				{
					if(CheckIfFromAvailableBlocks(target5xy))
					{
						box5=GameObject.Find(target5xy);
						if(CheckIfNextPieceIsOfP1(box5)==true)
						{
							target5XY+=1;
							target5xy=target5XY.ToString();
							if(CheckIfFromAvailableBlocks(target5xy))
							{
								box5=GameObject.Find(target5xy);
								if(!CheckIfTargetPlaceHavePiece(box5))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box5.GetComponent<MeshRenderer>().enabled=true;
									box5.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box5))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box5.GetComponent<MeshRenderer>().enabled=true;
								box5.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target6xy))
					{
						box6=GameObject.Find(target6xy);
						if(CheckIfNextPieceIsOfP1(box6)==true)
						{
							target6XY-=1;
							target6xy=target6XY.ToString();
							if(CheckIfFromAvailableBlocks(target6xy))
							{
								box6=GameObject.Find(target6xy);
								if(!CheckIfTargetPlaceHavePiece(box6))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box6.GetComponent<MeshRenderer>().enabled=true;
									box6.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box6))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box6.GetComponent<MeshRenderer>().enabled=true;
								box6.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target7xy))
					{
						box7=GameObject.Find(target7xy);
						if(CheckIfNextPieceIsOfP1(box7)==true)
						{
							target7XY+=10;
							target7xy=target7XY.ToString();
							if(CheckIfFromAvailableBlocks(target7xy))
							{
								box7=GameObject.Find(target7xy);
								if(!CheckIfTargetPlaceHavePiece(box7))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box7.GetComponent<MeshRenderer>().enabled=true;
									box7.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box7))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box7.GetComponent<MeshRenderer>().enabled=true;
								box7.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
					if(CheckIfFromAvailableBlocks(target8xy))
					{
						box8=GameObject.Find(target8xy);
						if(CheckIfNextPieceIsOfP1(box8)==true)
						{
							target8XY-=10;
							target8xy=target8XY.ToString();
							if(CheckIfFromAvailableBlocks(target8xy))
							{
								box8=GameObject.Find(target8xy);
								if(!CheckIfTargetPlaceHavePiece(box8))
								{
									source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
									source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
									box8.GetComponent<MeshRenderer>().enabled=true;
									box8.GetComponent<MeshRenderer>().material.color=Color.green;
									temp=true;
								}
							}
						}
						else
						{
							if(!CheckIfTargetPlaceHavePiece(box8))
							{
								source[pieceBlockIndex].GetComponent<MeshRenderer>().enabled=true;
								source[pieceBlockIndex].GetComponent<MeshRenderer>().material.color=Color.blue;
								box8.GetComponent<MeshRenderer>().enabled=true;
								box8.GetComponent<MeshRenderer>().material.color=Color.green;
								temp=true;
							}
						}
					}
				}
			}
		}
		if (player1Move == true) {
			if(temp==true)
			{
				click=false;player1Move=false;
			}
		}
		else if (player1Move == false) {
			if(temp==true)
			{
				click=false;player1Move=true;
			}
		}
	}
	//check if piece belong to player 1
	bool CheckIfPlayer1Piece(GameObject g)
	{
		if (g.name == "yellow" || g.name == "yellow 1" || g.name == "yellow 2" || g.name == "yellow 3" || g.name == "yellow 4" || g.name == "yellow 5" || g.name == "yellow 6" || g.name == "yellow 7" || g.name == "yellow 8" || g.name == "yellow 9" || g.name == "yellow 10" || g.name == "yellow 11" || g.name == "yellowKing" || g.name == "yellowKing 1" || g.name == "yellowKing 2" || g.name == "yellowKing 3" || g.name == "yellowKing 4" || g.name == "yellow 5" || g.name == "yellowKing 6" || g.name == "yellowKing 7" || g.name == "yellowKing 8" || g.name == "yellowKing 9" || g.name == "yellowKing 10" || g.name == "yellowKing 11")
			return true;
		else
			return false;
	}
	bool CheckIfPlayer2Piece(GameObject g)
	{
		if (g.name == "brown" || g.name == "brown 1" || g.name == "brown 2" || g.name == "brown 3" || g.name == "brown 4" || g.name == "brown 5" || g.name == "brown 6" || g.name == "brown 7" || g.name == "brown 8" || g.name == "brown 9" || g.name == "brown 10" || g.name == "brown 11" || g.name == "brownKing" || g.name == "brownKing 1" || g.name == "brownKing 2" || g.name == "brownKing 3" || g.name == "brownKing 4" || g.name == "brownKing 5" || g.name == "brownKing 6" || g.name == "brownKing 7" || g.name == "brownKing 8" || g.name == "brownKing 9" || g.name == "brownKing 10" || g.name == "brownKing 11")
			return true;
		else
			return false;
	}
	//set the boxex where our pieces are above of it
	void SetPlayersColliderBoxes()
	{
		player1ColliderBoxes[0]="11";player1ColliderBoxes[1]="13";player1ColliderBoxes[2]="15";player1ColliderBoxes[3]="17";player1ColliderBoxes[4]="22";player1ColliderBoxes[5]="24";player1ColliderBoxes[6]="26";player1ColliderBoxes[7]="28";player1ColliderBoxes[8]="31";player1ColliderBoxes[9]="33";player1ColliderBoxes[10]="35";player1ColliderBoxes[11]="37";
		player2ColliderBoxes[0]="82";player2ColliderBoxes[1]="84";player2ColliderBoxes[2]="86";player2ColliderBoxes[3]="88";player2ColliderBoxes[4]="71";player2ColliderBoxes[5]="73";player2ColliderBoxes[6]="75";player2ColliderBoxes[7]="77";player2ColliderBoxes[8]="62";player2ColliderBoxes[9]="64";player2ColliderBoxes[10]="66";player2ColliderBoxes[11]="68";
	}
	//check if there is any piece above gameobject
	bool CheckIfTargetPlaceHavePiece(GameObject g)
	{
		if (g == null)
			return false;
		for (int i=0; i<12; i++) {
			if(g.name==player1ColliderBoxes[i] || g.name==player2ColliderBoxes[i] || g.name==player1KingColliderBoxes[i] || g.name==player2KingColliderBoxes[i])
				return true;
		}
		return false;
	}
	string GetCheckerPieceName(GameObject g)
	{
		for (int i=0; i<12; i++) {
			if(g.name==player1ColliderBoxes[i])
			{
				if(i==0)
					return "yellow";
				else if(i==1)
					return "yellow 1";
				else if(i==2)
					return "yellow 2";
				else if(i==3)
					return "yellow 3";
				else if(i==4)
					return "yellow 4";
				else if(i==5)
					return "yellow 5";
				else if(i==6)
					return "yellow 6";
				else if(i==7)
					return "yellow 7";
				else if(i==8)
					return "yellow 8";
				else if(i==9)
					return "yellow 9";
				else if(i==10)
					return "yellow 10";
				else if(i==11)
					return "yellow 11";
			}
			else if(g.name==player2ColliderBoxes[i])
			{
				if(i==0)
					return "brown";
				else if(i==1)
					return "brown 1";
				else if(i==2)
					return "brown 2";
				else if(i==3)
					return "brown 3";
				else if(i==4)
					return "brown 4";
				else if(i==5)
					return "brown 5";
				else if(i==6)
					return "brown 6";
				else if(i==7)
					return "brown 7";
				else if(i==8)
					return "brown 8";
				else if(i==9)
					return "brown 9";
				else if(i==10)
					return "brown 10";
				else if(i==11)
					return "brown 11";
			}
			else if(g.name==player1KingColliderBoxes[i])
			{
				if(i==0)
					return "yellowKing";
				else if(i==1)
					return "yellowKing 1";
				else if(i==2)
					return "yellowKing 2";
				else if(i==3)
					return "yellowKing 3";
				else if(i==4)
					return "yellowKing 4";
				else if(i==5)
					return "yellowKing 5";
				else if(i==6)
					return "yellowKing 6";
				else if(i==7)
					return "yellowKing 7";
				else if(i==8)
					return "yellowKing 8";
				else if(i==9)
					return "yellowKing 9";
				else if(i==10)
					return "yellowKing 10";
				else if(i==11)
					return "yellowKing 11";
			}
			else if(g.name==player2KingColliderBoxes[i])
			{
				if(i==0)
					return "brownKing";
				else if(i==1)
					return "brownKing 1";
				else if(i==2)
					return "brownKing 2";
				else if(i==3)
					return "brownKing 3";
				else if(i==4)
					return "brownKing 4";
				else if(i==5)
					return "brownKing 5";
				else if(i==6)
					return "brownKing 6";
				else if(i==7)
					return "brownKing 7";
				else if(i==8)
					return "brownKing 8";
				else if(i==9)
					return "brownKing 9";
				else if(i==10)
					return "brownKing 10";
				else if(i==11)
					return "brownKing 11";
			}
		}
		return "";
	}
	//check if neighbour block have opponent piece
	bool CheckIfNextPieceIsOfP1(GameObject g)
	{
		if (g == null)
			return false;
		for (int i=0; i<12; i++) {
			if(g.name==player1ColliderBoxes[i] || g.name==player1KingColliderBoxes[i])
				return true;
		}
		return false;
	}
	bool CheckIfNextPieceIsOfP2(GameObject g)
	{
		if (g == null)
			return false;
		for (int i=0; i<12; i++) {
			if(g.name==player2ColliderBoxes[i] || g.name==player2KingColliderBoxes[i])
				return true;
		}
		return false;
	}
	bool CheckIfP1KingPiece(GameObject g)
	{
		if (g == null)
			return false;
		if(g.name=="yellowKing" || g.name=="yellowKing 1" || g.name=="yellowKing 2" || g.name=="yellowKing 3" || g.name=="yellowKing 4" || g.name=="yellowKing 5" || g.name=="yellowKing 6" || g.name=="yellowKing 7" || g.name=="yellowKing 8" || g.name=="yellowKing 9" || g.name=="yellowKing 10" || g.name=="yellowKing 11")
				return true;
		return false;
	}
	bool CheckIfP2KingPiece(GameObject g)
	{
		if (g == null)
			return false;
		if(g.name=="brownKing" || g.name=="brownKing 1" || g.name=="brownKing 2" || g.name=="brownKing 3" || g.name=="brownKing 4" || g.name=="brownKing 5" || g.name=="brownKing 6" || g.name=="brownKing 7" || g.name=="brownKing 8" || g.name=="brownKing 9" || g.name=="brownKing 10" || g.name=="brownKing 11")
				return true;
		return false;
	}
	//check if we have a boundary blocks
	bool CheckIfCornerBlocks(int num)
	{
		if (num == 11 || num == 12 || num == 13 || num == 14 || num == 15 || num == 16 || num == 17 || num == 18 || num == 21 || num == 31 || num == 41 || num == 51 || num == 61 || num == 71 || num == 81 || num == 82 || num == 83 || num == 84 || num == 85 || num == 86 || num == 87 || num == 88 || num == 28 || num == 38 || num == 48 || num == 58 || num == 68 || num == 78)
			return true;
		else
			return false;
	}
	bool CheckIfFromAvailableBlocks(string s)
	{
		if (s == "11" || s == "12" || s == "13" || s == "14" || s == "15" || s == "16" || s == "17" || s == "18" || s == "21" || s == "22" || s == "23" || s == "24" || s == "25" || s == "26" || s == "27" || s == "28" || s == "31" || s == "32" || s == "33" || s == "34" || s == "35" || s == "36" || s == "37" || s == "38" || s == "41" || s == "42" || s == "43" || s == "44" || s == "45" || s == "46" || s == "47" || s == "48" || s == "51" || s == "52" || s == "53" || s == "54" || s == "55" || s == "56" || s == "57" || s == "58" || s == "61" || s == "62" || s == "63" || s == "64" || s == "65" || s == "66" || s == "67" || s == "68" || s == "71" || s == "72" || s == "73" || s == "74" || s == "75" || s == "76" || s == "77" || s == "78" || s == "81" || s == "82" || s == "83" || s == "84" || s == "85" || s == "86" || s == "87" || s == "88")
			return true;
		else
			return false;
	}
	void UpdatePlayer1ColliderBoxes(GameObject g)
	{
		if (g.name == "yellow")
			player1ColliderBoxes [0] = "";
		else if (g.name == "yellow 1")
			player1ColliderBoxes [1] = "";
		else if (g.name == "yellow 2")
			player1ColliderBoxes [2] = "";
		else if (g.name == "yellow 3")
			player1ColliderBoxes [3] = "";
		else if (g.name == "yellow 4")
			player1ColliderBoxes [4] = "";
		else if (g.name == "yellow 5")
			player1ColliderBoxes [5] = "";
		else if (g.name == "yellow 6")
			player1ColliderBoxes [6] = "";
		else if (g.name == "yellow 7")
			player1ColliderBoxes [7] = "";
		else if (g.name == "yellow 8")
			player1ColliderBoxes [8] = "";
		else if (g.name == "yellow 9")
			player1ColliderBoxes [9] = "";
		else if (g.name == "yellow 10")
			player1ColliderBoxes [10] = "";
		else if (g.name == "yellow 11")
			player1ColliderBoxes [11] = "";
	}
	void UpdatePlayer2ColliderBoxes(GameObject g)
	{
		if (g.name == "brown")
			player2ColliderBoxes [0] = "";
		else if (g.name == "brown 1")
			player2ColliderBoxes [1] = "";
		else if (g.name == "brown 2")
			player2ColliderBoxes [2] = "";
		else if (g.name == "brown 3")
			player2ColliderBoxes [3] = "";
		else if (g.name == "brown 4")
			player2ColliderBoxes [4] = "";
		else if (g.name == "brown 5")
			player2ColliderBoxes [5] = "";
		else if (g.name == "brown 6")
			player2ColliderBoxes [6] = "";
		else if (g.name == "brown 7")
			player2ColliderBoxes [7] = "";
		else if (g.name == "brown 8")
			player2ColliderBoxes [8] = "";
		else if (g.name == "brown 9")
			player2ColliderBoxes [9] = "";
		else if (g.name == "brown 10")
			player2ColliderBoxes [10] = "";
		else if (g.name == "brown 11")
			player2ColliderBoxes [11] = "";
	}
	void UpdatePlayer1KingColliderBoxes(GameObject g)
	{
		if (g.name == "yellowKing")
			player1KingColliderBoxes [0] = "";
		else if (g.name == "yellowKing 1")
			player1KingColliderBoxes [1] = "";
		else if (g.name == "yellowKing 2")
			player1KingColliderBoxes [2] = "";
		else if (g.name == "yellowKing 3")
			player1KingColliderBoxes [3] = "";
		else if (g.name == "yellowKing 4")
			player1KingColliderBoxes [4] = "";
		else if (g.name == "yellowKing 5")
			player1KingColliderBoxes [5] = "";
		else if (g.name == "yellowKing 6")
			player1KingColliderBoxes [6] = "";
		else if (g.name == "yellowKing 7")
			player1KingColliderBoxes [7] = "";
		else if (g.name == "yellowKing 8")
			player1KingColliderBoxes [8] = "";
		else if (g.name == "yellowKing 9")
			player1KingColliderBoxes [9] = "";
		else if (g.name == "yellowKing 10")
			player1KingColliderBoxes [10] = "";
		else if (g.name == "yellowKing 11")
			player1KingColliderBoxes [11] = "";
	}
	void UpdatePlayer2KingColliderBoxes(GameObject g)
	{
		if (g.name == "brownKing")
			player2KingColliderBoxes [0] = "";
		else if (g.name == "brownKing 1")
			player2KingColliderBoxes [1] = "";
		else if (g.name == "brownKing 2")
			player2KingColliderBoxes [2] = "";
		else if (g.name == "brownKing 3")
			player2KingColliderBoxes [3] = "";
		else if (g.name == "brownKing 4")
			player2KingColliderBoxes [4] = "";
		else if (g.name == "brownKing 5")
			player2KingColliderBoxes [5] = "";
		else if (g.name == "brownKing 6")
			player2KingColliderBoxes [6] = "";
		else if (g.name == "brownKing 7")
			player2KingColliderBoxes [7] = "";
		else if (g.name == "brownKing 8")
			player2KingColliderBoxes [8] = "";
		else if (g.name == "brownKing 9")
			player2KingColliderBoxes [9] = "";
		else if (g.name == "brownKing 10")
			player2KingColliderBoxes [10] = "";
		else if (g.name == "brownKing 11")
			player2KingColliderBoxes [11] = "";
	}
	void SetPlayer1KingColliderBox(GameObject g)
	{
		player1KingColliderBoxes [noOfP1Kings] = g.name;
		noOfP1Kings++;
	}
	void SetPlayer2KingColliderBox(GameObject g)
	{
		player2KingColliderBoxes [noOfP2Kings] = g.name;
		noOfP2Kings++;
	}
}
