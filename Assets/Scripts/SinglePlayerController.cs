using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SinglePlayerController : MonoBehaviour
{
	
    public GameObject yellowKing,brownKing;
	//array to store the players boxes on which they are placed
	string[] computerColliderBoxes = new string[12];
	string[] player2ColliderBoxes = new string[12];
	string[] computerKingColliderBoxes = new string[12];
	string[] player2KingColliderBoxes = new string[12];
	bool click=true,restart=false,move=false;
	bool computerMove = true;
	GameObject[] source=new GameObject[3];
	GameObject[] target=new GameObject[2];
	int pieceBlockIndex=0,pieceIndex=0,blockIndex = 0;
	GameObject box1=null,box2=null,box3=null,box4=null,box5=null,box6=null,box7=null,box8=null;
	int computerDeadPieces=0,player2DeadPieces=0;
	int noOfComputerKings=0,noOfP2Kings=0;
    int target1XY = 0, target2XY = 0, target3XY = 0, target4XY = 0, target5XY = 0, target6XY = 0, target7XY = 0, target8XY = 0, sourceXY = 0;
    string target1xy = "", target2xy = "", target3xy = "", target4xy = "", target5xy = "", target6xy = "", target7xy = "", target8xy = "";

	public Text turnText,winLoseText;
	public Button restartButton;

	//node to store current game state
	Node currentGameState=new Node();

	//start is called during initialization
	void Start()
	{
		//set the pieces initial positions
		currentGameState.SetPiecesInitialPosition ();
		restart = false;
		turnText.text = "";winLoseText.text="";
		restartButton.gameObject.SetActive (false);
		SetPlayersColliderBoxes ();
	}
	
	void Update()
	{
		if (restart) {
			restartButton.gameObject.SetActive (true);
		}
		//if game end then show winning text 
		if (computerDeadPieces == 12) {
			winLoseText.text = "Player Wins!";
			restart = true;
		} else if (player2DeadPieces == 12) {
			winLoseText.text = "computer Wins!";
			restart = true;
		} else {
			if (click == true) {
				//if its computer move
				if (computerMove == true) {
					turnText.text = "computer turn";
					//use minmax tree and make computer move
					ComputerMove ();
				} else if (computerMove == false) {
					//if its player turn then get click from the player and select piece and highlight all possible moves to the player
					turnText.text = "Player turn";
					if (Input.GetMouseButtonDown (0)) {
						//Generate ray from main camera and mouse position
						Ray R = Camera.main.ScreenPointToRay (Input.mousePosition);
						//Will store info about all intersections
						RaycastHit[] HitInfo = Physics.RaycastAll (R);
						//Debug.Log (HitInfo.Length);
						//Test to see if ray intersects with any colliders
						if (HitInfo != null && HitInfo.Length == 3) {
							if (computerMove == false) {//if it is player 2 move
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
				} 
			} else if (click == false) {
				//now the piece has been selected and now we get the target box where the player desired to move
				if (Input.GetMouseButtonDown (0)) {
					//Generate ray from main camera and mouse position
					Ray R = Camera.main.ScreenPointToRay (Input.mousePosition);
					//Will store info about all intersections
					RaycastHit[] HitInfo = Physics.RaycastAll (R);
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
							//change the gamestate in the Game node
							currentGameState.SetPlayerTurn (2);
							int startX, startY, endX, endY;
							startX = (int)Char.GetNumericValue (source [pieceBlockIndex].name [0]);
							startY = (int)Char.GetNumericValue (source [pieceBlockIndex].name [1]);
							endX = (int)Char.GetNumericValue (target [blockIndex].name [0]);
							endY = (int)Char.GetNumericValue (target [blockIndex].name [1]);

							currentGameState.MakeMove (startX, startY, endX, endY);
								
							move = true;

							int srcXY, trgXY;
							int.TryParse (source [pieceBlockIndex].name, out srcXY);
							int.TryParse (target [blockIndex].name, out trgXY);

							//if the player has reached the opponent corner then crown the player piece
							if (CheckIfP2KingPiece (source [pieceIndex]) == false) 
                            {
								if (trgXY == 11 || trgXY == 12 || trgXY == 13 || trgXY == 14 || trgXY == 15 || trgXY == 16 || trgXY == 17 || trgXY == 18) 
                                {
									GameObject p = GameObject.Find ("Player2");
									GameObject t = Instantiate (brownKing, new Vector3 (p.transform.position.x, p.transform.position.y, p.transform.position.z), Quaternion.identity)as GameObject;
									t.transform.parent = p.transform;
									t.transform.position = source [pieceIndex].transform.position;
									t.transform.transform.Rotate (270, 0, 0);
									//rename the gameobject to differentiate 
									if (noOfP2Kings == 0)
										t.name = "brownKing";
									else if (noOfP2Kings == 1)
										t.name = "brownKing 1";
									else if (noOfP2Kings == 2)
										t.name = "brownKing 2";
									else if (noOfP2Kings == 3)
										t.name = "brownKing 3";
									else if (noOfP2Kings == 4)
										t.name = "brownKing 4";
									else if (noOfP2Kings == 5)
										t.name = "brownKing 5";
									else if (noOfP2Kings == 6)
										t.name = "brownKing 6";
									else if (noOfP2Kings == 7)
										t.name = "brownKing 7";
									else if (noOfP2Kings == 8)
										t.name = "brownKing 8";
									else if (noOfP2Kings == 9)
										t.name = "brownKing 9";
									else if (noOfP2Kings == 10)
										t.name = "brownKing 10";
									else if (noOfP2Kings == 11)
										t.name = "brownKing 11";
									UpdatePlayer2ColliderBoxes (source [pieceIndex]);
									SetPlayer2KingColliderBox (target [blockIndex]);
									Destroy (GameObject.Find (source [pieceIndex].name.ToString ()));
									source [pieceIndex] = null;
								}
							}
								
							//check if player has double junp to kill the opponent player piece
							//if it has then destroy the kill piece and update the positions according to it
							if (trgXY == srcXY - 22 || trgXY == srcXY + 22 || trgXY == srcXY - 18 || trgXY == srcXY + 18 || trgXY == srcXY - 2 || trgXY == srcXY + 2 || trgXY == srcXY - 20 || trgXY == srcXY + 20)
							{
								if (trgXY == srcXY - 22) {
									int t = srcXY - 11;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									//check if player  has killed computer piece
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								} else if (trgXY == srcXY + 22) {
									int t = srcXY + 11;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								} else if (trgXY == srcXY - 18) {
									int t = srcXY - 9;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								} else if (trgXY == srcXY + 18) {
									int t = srcXY + 9;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								} else if (trgXY == srcXY - 2) {
									int t = srcXY - 1;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								} else if (trgXY == srcXY + 2) {
									int t = srcXY + 1;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								} else if (trgXY == srcXY - 20) {
									int t = srcXY - 10;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								} else if (trgXY == srcXY + 20) {
									int t = srcXY + 10;
									string t1 = t.ToString ();
									GameObject g1 = GameObject.Find (t1);
									string p = GetCheckerPieceName (g1);
									if (CheckIfComputerKingPiece (GameObject.Find (p)))
										UpdatecomputerKingColliderBoxes (GameObject.Find (p));
									else
										UpdatecomputerColliderBoxes (GameObject.Find (p));
									Destroy (GameObject.Find (p));
									computerDeadPieces++;
								}
							}
								
							//update the pieces boxes numbers i-e row-column
							if (source [pieceIndex] != null) 
							{
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
                            box1 = null; box2 = null; box3 = null; box4 = null; box5 = null; box6 = null; box7 = null; box8 = null;
                            target1XY = 0; target2XY = 0; target3XY = 0; target4XY = 0; target5XY = 0; target6XY = 0; target7XY = 0; target8XY = 0; sourceXY = 0;
                            target1xy = ""; target2xy = ""; target3xy = ""; target4xy = ""; target5xy = ""; target6xy = ""; target7xy = ""; target8xy = "";
							click = true;
							computerMove = true;
							move = false;
							turnText.text = "Computer turn";
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
			
			if(computerMove==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfComputer(box1)==true)
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
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfComputer(box2)==true)
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
						if(CheckIfNextPieceIsOfComputer(box3)==true)
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
						if(CheckIfNextPieceIsOfComputer(box4)==true)
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
						if(CheckIfNextPieceIsOfComputer(box5)==true)
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
			
			if(computerMove==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfComputer(box1)==true)
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
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfComputer(box2)==true)
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
						if(CheckIfNextPieceIsOfComputer(box3)==true)
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
						if(CheckIfNextPieceIsOfComputer(box4)==true)
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
						if(CheckIfNextPieceIsOfComputer(box5)==true)
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
			
			if(computerMove==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfComputer(box1)==true)
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
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfComputer(box2)==true)
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
						if(CheckIfNextPieceIsOfComputer(box3)==true)
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
						if(CheckIfNextPieceIsOfComputer(box4)==true)
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
						if(CheckIfNextPieceIsOfComputer(box5)==true)
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
			
			if(computerMove==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfComputer(box1)==true)
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
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfComputer(box2)==true)
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
						if(CheckIfNextPieceIsOfComputer(box3)==true)
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
						if(CheckIfNextPieceIsOfComputer(box4)==true)
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
						if(CheckIfNextPieceIsOfComputer(box5)==true)
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
			
			
			if(computerMove==false)
			{
				if(CheckIfFromAvailableBlocks(target1xy))
				{
					box1=GameObject.Find(target1xy);
					if(CheckIfCornerBlocks(target1XY)==false && CheckIfNextPieceIsOfComputer(box1)==true)
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
					if(CheckIfCornerBlocks(target2XY)==false && CheckIfNextPieceIsOfComputer(box2)==true)
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
					if(CheckIfCornerBlocks(target3XY)==false && CheckIfNextPieceIsOfComputer(box3)==true)
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
					if(CheckIfCornerBlocks(target4XY)==false && CheckIfNextPieceIsOfComputer(box4)==true)
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
						if(CheckIfNextPieceIsOfComputer(box5)==true)
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
						if(CheckIfNextPieceIsOfComputer(box6)==true)
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
						if(CheckIfNextPieceIsOfComputer(box7)==true)
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
						if(CheckIfNextPieceIsOfComputer(box8)==true)
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
		if (computerMove == false) {
			if(temp==true)
			{
				click=false;
			}
		}
	}
	//check if piece belong to player 1
	bool CheckIfcomputerPiece(GameObject g)
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
		computerColliderBoxes[0]="11";computerColliderBoxes[1]="13";computerColliderBoxes[2]="15";computerColliderBoxes[3]="17";computerColliderBoxes[4]="22";computerColliderBoxes[5]="24";computerColliderBoxes[6]="26";computerColliderBoxes[7]="28";computerColliderBoxes[8]="31";computerColliderBoxes[9]="33";computerColliderBoxes[10]="35";computerColliderBoxes[11]="37";
		player2ColliderBoxes[0]="82";player2ColliderBoxes[1]="84";player2ColliderBoxes[2]="86";player2ColliderBoxes[3]="88";player2ColliderBoxes[4]="71";player2ColliderBoxes[5]="73";player2ColliderBoxes[6]="75";player2ColliderBoxes[7]="77";player2ColliderBoxes[8]="62";player2ColliderBoxes[9]="64";player2ColliderBoxes[10]="66";player2ColliderBoxes[11]="68";
	}
	//check if there is any piece above gameobject
	bool CheckIfTargetPlaceHavePiece(GameObject g)
	{
		if (g == null)
			return false;
		for (int i=0; i<12; i++) {
			if(g.name==computerColliderBoxes[i] || g.name==player2ColliderBoxes[i] || g.name==computerKingColliderBoxes[i] || g.name==player2KingColliderBoxes[i])
				return true;
		}
		return false;
	}
	string GetCheckerPieceName(GameObject g)
	{
		for (int i=0; i<12; i++) {
			if(g.name==computerColliderBoxes[i])
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
			else if(g.name==computerKingColliderBoxes[i])
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
	string GetCheckerPieceNameAtBox(string g)
	{
		for (int i=0; i<12; i++) {
			if(g==computerColliderBoxes[i])
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
			else if(g==player2ColliderBoxes[i])
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
			else if(g==computerKingColliderBoxes[i])
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
			else if(g==player2KingColliderBoxes[i])
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
	bool CheckIfNextPieceIsOfComputer(GameObject g)
	{
		if (g == null)
			return false;
		for (int i=0; i<12; i++) {
			if(g.name==computerColliderBoxes[i] || g.name==computerKingColliderBoxes[i])
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
	bool CheckIfComputerKingPiece(GameObject g)
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
	void UpdatecomputerColliderBoxes(GameObject g)
	{
		if (g.name == "yellow")
			computerColliderBoxes [0] = "";
		else if (g.name == "yellow 1")
			computerColliderBoxes [1] = "";
		else if (g.name == "yellow 2")
			computerColliderBoxes [2] = "";
		else if (g.name == "yellow 3")
			computerColliderBoxes [3] = "";
		else if (g.name == "yellow 4")
			computerColliderBoxes [4] = "";
		else if (g.name == "yellow 5")
			computerColliderBoxes [5] = "";
		else if (g.name == "yellow 6")
			computerColliderBoxes [6] = "";
		else if (g.name == "yellow 7")
			computerColliderBoxes [7] = "";
		else if (g.name == "yellow 8")
			computerColliderBoxes [8] = "";
		else if (g.name == "yellow 9")
			computerColliderBoxes [9] = "";
		else if (g.name == "yellow 10")
			computerColliderBoxes [10] = "";
		else if (g.name == "yellow 11")
			computerColliderBoxes [11] = "";
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
	void UpdatecomputerKingColliderBoxes(GameObject g)
	{
		if (g.name == "yellowKing")
			computerKingColliderBoxes [0] = "";
		else if (g.name == "yellowKing 1")
			computerKingColliderBoxes [1] = "";
		else if (g.name == "yellowKing 2")
			computerKingColliderBoxes [2] = "";
		else if (g.name == "yellowKing 3")
			computerKingColliderBoxes [3] = "";
		else if (g.name == "yellowKing 4")
			computerKingColliderBoxes [4] = "";
		else if (g.name == "yellowKing 5")
			computerKingColliderBoxes [5] = "";
		else if (g.name == "yellowKing 6")
			computerKingColliderBoxes [6] = "";
		else if (g.name == "yellowKing 7")
			computerKingColliderBoxes [7] = "";
		else if (g.name == "yellowKing 8")
			computerKingColliderBoxes [8] = "";
		else if (g.name == "yellowKing 9")
			computerKingColliderBoxes [9] = "";
		else if (g.name == "yellowKing 10")
			computerKingColliderBoxes [10] = "";
		else if (g.name == "yellowKing 11")
			computerKingColliderBoxes [11] = "";
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
	void SetcomputerKingColliderBox(GameObject g)
	{
		computerKingColliderBoxes [noOfComputerKings] = g.name;
		noOfComputerKings++;
	}
	void SetPlayer2KingColliderBox(GameObject g)
	{
		player2KingColliderBoxes [noOfP2Kings] = g.name;
		noOfP2Kings++;
	}

	//**********************************************************AI part************************************************************************************
	public class Node
	{
		int playerTurn;
		int[,] board;
		int playerDeadPieces, computerDeadPieces;
		string currentMovedPieceOldIndex, currentMovedNewPieceIndex;
		int value;
		
		//constructor
		public Node()
		{
			currentMovedNewPieceIndex = "";
			currentMovedPieceOldIndex = "";
			playerTurn = 1;
			playerDeadPieces = 0;
			computerDeadPieces = 0;
            value = 0; 
			//we here skip zero index and make 9x9 board to make our move calculation easy
			board = new int[9, 9];
        }
		//copy constructor
		public Node(Node Node)
		{
			playerTurn = Node.GetPlayerTurn();
			//skip zero index for making calculations of moves easy
			board = new int[9,9];
			for (int y = 1; y <=8; y++)
			{
				for (int x = 1; x <=8; x++)
				{
					board[x, y] = Node.GetPieceAtCurrentPosition(x, y);
				}
			}
			Evaluate();
		}
		//here 1=yellow, 2=brown ,3=yellowKing, 4=brownKing
		public void SetPiecesInitialPosition()
		{
			playerTurn = 1;
			
			board[1, 1] = board[1, 3] = board[1, 5] = board[1, 7] = 1;
			board[2, 2] = board[2, 4] = board[2, 6] = board[2, 8] = 1;
			board[3, 1] = board[3, 3] = board[3, 5] = board[3, 7] = 1;
			
			board[6, 2] = board[6, 4] = board[6, 6] = board[6, 8] = 2;
			board[7, 1] = board[7, 3] = board[7, 5] = board[7, 7] = 2;
			board[8, 2] = board[8, 4] = board[8, 6] = board[8, 8] = 2;
		}
		public int GetPlayerTurn() 
		{
			return playerTurn;
		}
		public void SetPlayerTurn(int turn)
		{
			playerTurn = turn;
		}
		public int GetPieceAtCurrentPosition(int x, int y) 
		{
			if ((x < 0) || (y < 0) || (x > 8) || (y > 8))
			{
				return 0;
			}
			return board[x, y];
		}
        //the evaluation function will give value on the basis of player 
        //since our AI player is 1 and 3 therefore if the board contains these pieces we will give more value 
        //then different values are assigned for different moves
        //here we have make our AI player to go more for attacks than for defense
		public void Evaluate()
		{
            if (playerDeadPieces == 11)
                value += 100;
            else if (playerDeadPieces > computerDeadPieces)
                value += 50;
            else if (playerDeadPieces < computerDeadPieces)
                value -= 50;
			for (int x = 1; x <= 8; x++)
			{
				for (int y = 1; y <= 8; y++)
				{
					if (GetPieceAtCurrentPosition(x, y) == 1)
					{
                        value += 1;
                        if (GetPieceAtCurrentPosition(x + 1, y + 1) == 2 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                           value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 2 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                           value += 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 2 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                           value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 2 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y + 1) == 4 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 4 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 4 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                           value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 4 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
					}
					if (GetPieceAtCurrentPosition(x, y) == 2)
					{
						value -= 1;
						if (GetPieceAtCurrentPosition(x + 1, y + 1) == 1 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 1 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 1 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 1 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y + 1) == 3 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 3 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 3 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 3 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 10;
                        }
					}
					if (GetPieceAtCurrentPosition(x, y) == 3)
					{
						value+=3;
						if (GetPieceAtCurrentPosition(x + 1, y + 1) == 2 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 2 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                           value += 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 2 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 2 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x, y + 1) == 2 && GetPieceAtCurrentPosition(x, y + 2) == 0 && (x) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y) == 2 && GetPieceAtCurrentPosition(x - 2,y) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x, y - 1) == 2 && GetPieceAtCurrentPosition(x, y - 2) == 0 && (x) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y) == 2 && GetPieceAtCurrentPosition(x + 2,y) == 0 && (x +1) > 0 && (x + 2) > 0 && (y) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y) < 9 )
                        {
                            value += 10;
                        }

						if (GetPieceAtCurrentPosition(x + 1, y + 1) == 4 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 4 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 4 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 4 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x, y + 1) == 4 && GetPieceAtCurrentPosition(x, y + 2) == 0 && (x) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y) == 4 && GetPieceAtCurrentPosition(x - 2,y) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x, y - 1) == 4 && GetPieceAtCurrentPosition(x, y - 2) == 0 && (x) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value += 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y) == 4 && GetPieceAtCurrentPosition(x + 2,y) == 0 && (x +1) > 0 && (x + 2) > 0 && (y) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y) < 9 )
                        {
                            value += 10;
                        }
					}
					if (GetPieceAtCurrentPosition(x, y) == 4)
					{
						value-=3;
						if (GetPieceAtCurrentPosition(x + 1, y + 1) == 1 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 1 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 1 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 1 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x, y + 1) == 1 && GetPieceAtCurrentPosition(x, y + 2) == 0 && (x) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y) == 1 && GetPieceAtCurrentPosition(x - 2,y) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x, y - 1) == 1 && GetPieceAtCurrentPosition(x, y - 2) == 0 && (x) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 5;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y) == 1 && GetPieceAtCurrentPosition(x + 2,y) == 0 && (x +1) > 0 && (x + 2) > 0 && (y) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y) < 9 )
                        {
                            value -= 5;
                        }

						if (GetPieceAtCurrentPosition(x + 1, y + 1) == 3 && GetPieceAtCurrentPosition(x + 2, y + 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y + 1) == 3 && GetPieceAtCurrentPosition(x - 2, y + 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y - 1) == 3 && GetPieceAtCurrentPosition(x + 2, y - 2) == 0 && (x + 1) > 0 && (x + 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y - 1) == 3 && GetPieceAtCurrentPosition(x - 2, y - 2) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x, y + 1) == 3 && GetPieceAtCurrentPosition(x, y + 2) == 0 && (x) > 0 && (y + 1) > 0 && (y + 2) > 0 && (x) < 9 && (y + 1) < 9 && (y + 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x - 1, y) == 3 && GetPieceAtCurrentPosition(x - 2,y) == 0 && (x - 1) > 0 && (x - 2) > 0 && (y) > 0 && (x - 1) < 9 && (x - 2) < 9 && (y) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x, y - 1) == 3 && GetPieceAtCurrentPosition(x, y - 2) == 0 && (x) > 0 && (y - 1) > 0 && (y - 2) > 0 && (x) < 9 && (y - 1) < 9 && (y - 2) < 9)
                        {
                            value -= 10;
                        }
						if (GetPieceAtCurrentPosition(x + 1, y) == 3 && GetPieceAtCurrentPosition(x + 2,y) == 0 && (x +1) > 0 && (x + 2) > 0 && (y) > 0 && (x + 1) < 9 && (x + 2) < 9 && (y) < 9 )
                        {
                            value -= 10;
                        }
					}
				}
			}
		}
		public string GetMovedPieceOldPosition()
		{
			return currentMovedPieceOldIndex;
		}
		public string GetMovedPieceNewPosition()
		{
			return currentMovedNewPieceIndex;
		}
		public string CheckWin()
		{
			if (playerDeadPieces == 12)
				return "Player Wins";
			else if (computerDeadPieces == 12)
				return "Computer Wins";
			else
				return "";
		}
		public int GetValue()
		{
			return value;
		}
		public void SetValue(int _value)
		{
			value = _value;
		}
		public bool CheckValidMove(int startingPointX, int startingPointY, int endingPointX, int endingPointY)
		{
			// if starting point does not contain  a piece 
			if (GetPieceAtCurrentPosition(startingPointX, startingPointY) == 0)
			{
				return false;
			}
			//if ending point contains a piece
			if (GetPieceAtCurrentPosition(endingPointX, endingPointY) != 0)
			{
				return false;
			}
			//if its yellow piece turn
			if(playerTurn==1)
			{
				if(GetPieceAtCurrentPosition(startingPointX,startingPointY)==1)
				{
					if ((endingPointX == startingPointX + 1 || endingPointX == startingPointX - 1) && (endingPointY == startingPointY + 1 || endingPointY == startingPointY - 1))
						return true;
					if(endingPointY==startingPointY+2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY + 1) == 2) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY + 1) == 2))
						{
							return true;
						}
					}
					if (endingPointY == startingPointY - 2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY - 1) == 2) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY - 1) == 2))
						{
							return true;
						}
					}
				}
				if(GetPieceAtCurrentPosition(startingPointX,startingPointY)==3)
				{
					if ((endingPointX == startingPointX + 1 || endingPointX == startingPointX - 1) && (endingPointY == startingPointY + 1 || endingPointY == startingPointY - 1))
						return true;
					if(endingPointY==startingPointY+2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY + 1) == 2) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY + 1) == 2) || (endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY + 1) == 4) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY + 1) == 4))
						{
							return true;
						}
					}
					if (endingPointY == startingPointY - 2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY - 1) == 2) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY - 1) == 2) || (endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY - 1) == 4) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY - 1) == 4))
						{
							return true;
						}
					}
					if((endingPointX==startingPointX && (endingPointY==startingPointY+1 || endingPointY==startingPointY-1))||(endingPointY==startingPointY && (endingPointX==startingPointX+1 || endingPointX==startingPointX-1)))
						return true;
					if(endingPointY==startingPointY+2)
					{
						if (GetPieceAtCurrentPosition(startingPointX, startingPointY + 1) == 2 || GetPieceAtCurrentPosition(startingPointX, startingPointY + 1) == 4)
						{
							return true;
						}
					}
					if(endingPointY==startingPointY-2)
					{
						if (GetPieceAtCurrentPosition(startingPointX, startingPointY - 1) == 2 || GetPieceAtCurrentPosition(startingPointX, startingPointY - 1) == 4)
						{
							return true;
						}
					}
					if(endingPointX==startingPointX+2)
					{
						if (GetPieceAtCurrentPosition(startingPointX+1, startingPointY) == 2 || GetPieceAtCurrentPosition(startingPointX+1, startingPointY) == 4)
						{
							return true;
						}
					}
					if(endingPointX==startingPointX-2)
					{
						if (GetPieceAtCurrentPosition(startingPointX-1, startingPointY) == 2 || GetPieceAtCurrentPosition(startingPointX-1, startingPointY) == 4)
						{
							return true;
						}
					}
				}
			}
			else if(playerTurn==2)
			{
				if (GetPieceAtCurrentPosition(startingPointX, startingPointY) == 2)
				{
					if ((endingPointX == startingPointX + 1 || endingPointX == startingPointX - 1) && (endingPointY == startingPointY + 1 || endingPointY == startingPointY - 1))
						return true;
					if (endingPointY == startingPointY + 2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY + 1) == 1) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY + 1) == 1))
						{
							return true;
						}
					}
					if (endingPointY == startingPointY - 2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY - 1) == 1) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY - 1) == 1))
						{
							return true;
						}
					}
				}
				if(GetPieceAtCurrentPosition(startingPointX,startingPointY)==4)
				{
					if ((endingPointX == startingPointX + 1 || endingPointX == startingPointX - 1) && (endingPointY == startingPointY + 1 || endingPointY == startingPointY - 1))
						return true;
					if(endingPointY==startingPointY+2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY + 1) == 1) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY + 1) == 1) || (endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY + 1) == 3) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY + 1) == 3))
						{
							return true;
						}
					}
					if (endingPointY == startingPointY - 2)
					{
						if ((endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY - 1) == 1) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY - 1) == 1) || (endingPointX == startingPointX - 2 && GetPieceAtCurrentPosition(startingPointX - 1, startingPointY - 1) == 3) || (endingPointX == startingPointX + 2 && GetPieceAtCurrentPosition(startingPointX + 1, startingPointY - 1) == 3))
						{
							return true;
						}
					}
					if((endingPointX==startingPointX && (endingPointY==startingPointY+1 || endingPointY==startingPointY-1))||(endingPointY==startingPointY && (endingPointX==startingPointX+1 || endingPointX==startingPointX-1)))
						return true;
					if(endingPointY==startingPointY+2)
					{
						if (GetPieceAtCurrentPosition(startingPointX, startingPointY + 1) == 1 || GetPieceAtCurrentPosition(startingPointX, startingPointY + 1) == 3)
						{
							return true;
						}
					}
					if(endingPointY==startingPointY-2)
					{
						if (GetPieceAtCurrentPosition(startingPointX, startingPointY - 1) == 1 || GetPieceAtCurrentPosition(startingPointX, startingPointY - 1) == 3)
						{
							return true;
						}
					}
					if(endingPointX==startingPointX+2)
					{
						if (GetPieceAtCurrentPosition(startingPointX+1, startingPointY) == 1 || GetPieceAtCurrentPosition(startingPointX+1, startingPointY) == 3)
						{
							return true;
						}
					}
					if(endingPointX==startingPointX-2)
					{
						if (GetPieceAtCurrentPosition(startingPointX-1, startingPointY) == 1 || GetPieceAtCurrentPosition(startingPointX-1, startingPointY) == 3)
						{
							return true;
						}
					}
				}
			}
			
			return false;
		}
		public void MakeMove(int startingPointX, int startingPointY, int endingPointX, int endingPointY)
		{
			if(startingPointX>0 && startingPointY>0 && endingPointX<9 && endingPointY<9)
			{

				if((GetPieceAtCurrentPosition(startingPointX,startingPointY)==1)&& ((endingPointX==8) &&( endingPointY==1 ||endingPointY==2 ||endingPointY==3 ||endingPointY==4 ||endingPointY==5 ||endingPointY==6 ||endingPointY==7 ||endingPointY==8)))
				{
					board[endingPointX, endingPointY]=3;
				}
				if((GetPieceAtCurrentPosition(startingPointX,startingPointY)==2)&& ((endingPointX==1) &&( endingPointY==1 ||endingPointY==2 ||endingPointY==3 ||endingPointY==4 ||endingPointY==5 ||endingPointY==6 ||endingPointY==7 ||endingPointY==8)))
				{
					board[endingPointX, endingPointY]=4;
				}
				else
					board[endingPointX, endingPointY] = board[startingPointX, startingPointY];

				StringBuilder tem = new StringBuilder();
				string x,y;
				x=Convert.ToString(startingPointX);
				y = Convert.ToString(startingPointY);
				tem.Append(x); tem.Append(y);
				currentMovedPieceOldIndex = tem.ToString();
				StringBuilder tem1 = new StringBuilder();
				x = Convert.ToString(endingPointX);
				y = Convert.ToString(endingPointY);
				tem1.Append(x); tem1.Append(y);
				currentMovedNewPieceIndex = tem1.ToString();
				
				board[startingPointX, startingPointY] = 0;
				
				if(endingPointX==startingPointX+2 && endingPointY==startingPointY+2)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX + 1, startingPointY + 1] = 0;
				}
				if (endingPointX == startingPointX - 2 && endingPointY == startingPointY + 2)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX - 1, startingPointY + 1] = 0;
				}
				if (endingPointX == startingPointX + 2 && endingPointY == startingPointY - 2)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX + 1, startingPointY - 1] = 0;
				}
				if (endingPointX == startingPointX - 2 && endingPointY == startingPointY - 2)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX - 1, startingPointY - 1] = 0;
				}

				if(endingPointX==startingPointX && endingPointY==startingPointY+2)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX, startingPointY + 1] = 0;
				}
				if (endingPointX == startingPointX && endingPointY == startingPointY - 2)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX, startingPointY - 1] = 0;
				}
				if (endingPointX == startingPointX + 2 && endingPointY == startingPointY)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX + 1, startingPointY] = 0;
				}
				if (endingPointX == startingPointX - 2 && endingPointY == startingPointY)
				{
					if (playerTurn == 1)
						playerDeadPieces++;
					else if (playerTurn == 2)
						computerDeadPieces++;
					board[startingPointX - 1, startingPointY] = 0;
				}

			}
			if (playerTurn == 1)
			{
				playerTurn = 2;
			}
			else
			{
				playerTurn = 1;
			}
		}
	}

    //MinMax algorithm with Alpha Beta Prunning
	Node MinMaxWithAlphaBetaPrunning(Node curr_node, int depthCurrent,int alpha,int beta)
	{
		Node bestNode = new Node();
        if (curr_node.GetPlayerTurn() == 1 || curr_node.GetPlayerTurn() == 3)
            bestNode.SetValue(alpha);
        if (curr_node.GetPlayerTurn() == 2 || curr_node.GetPlayerTurn() == 4)
            bestNode.SetValue(beta);
		bool bestNodeInit = false;
		for (int startX = 1; startX <=8; startX++)
		{
			for (int startY = 1; startY <=8; startY++)
			{
				if (curr_node.GetPieceAtCurrentPosition(startX, startY) > 0)
				{
					for (int endX = 1; endX <= 8; endX++)
					{
						for (int endY = 1; endY <= 8; endY++)
						{
							if (curr_node.CheckValidMove(startX, startY, endX, endY))
							{
								Node newNode = new Node(curr_node);
								newNode.MakeMove(startX, startY, endX, endY);
								
								if ((depthCurrent > 0) && (curr_node.CheckWin() == ""))
								{
									Node NodeTemp = new Node();
									NodeTemp = MinMaxWithAlphaBetaPrunning(newNode, depthCurrent - 1,alpha,beta);
									newNode.SetValue(NodeTemp.GetValue());	
								}
								else
								{
									newNode.Evaluate();
								}
								
								if (((curr_node.GetPlayerTurn() == 2 || curr_node.GetPlayerTurn() == 4) && (newNode.GetValue() < bestNode.GetValue())) ||
								    ((curr_node.GetPlayerTurn() == 1 || curr_node.GetPlayerTurn() == 3) && (newNode.GetValue() > bestNode.GetValue())) ||
								    (bestNodeInit == false))
								{
									bestNode = newNode;
                                    bestNode.SetValue(newNode.GetValue());
									if(curr_node.GetPlayerTurn()==1 || curr_node.GetPlayerTurn() == 3)
									{
										alpha = newNode.GetValue();
										if (alpha >= beta)
										{
											return bestNode;
										}
									}
									if (curr_node.GetPlayerTurn() == 2 || curr_node.GetPlayerTurn() == 4)
									{
										beta = newNode.GetValue();
										if (beta <= alpha)
										{
											return bestNode;
										}
									}
									bestNodeInit = true;
								}
							}
						}
					}
				}
			}
		}
		return bestNode;
	}

    //MinMax Algorithm
	Node MinMax(Node curr_node, int depthCurrent)
	{
		Node bestNode = new Node();
		bool bestNodeInit = false;
		for (int startX = 1; startX <=8; startX++)
		{
			for (int startY = 1; startY <=8; startY++)
			{
				if (curr_node.GetPieceAtCurrentPosition(startX, startY) > 0)
				{
					for (int endX = 1; endX <= 8; endX++)
					{
						for (int endY = 1; endY <= 8; endY++)
						{
							if (curr_node.CheckValidMove(startX, startY, endX, endY))
							{
								Node newNode = new Node(curr_node);
								newNode.MakeMove(startX, startY, endX, endY);
								
								if ((depthCurrent > 0) && (curr_node.CheckWin() == ""))
								{
									Node NodeTemp = new Node();
									NodeTemp = MinMax(newNode, depthCurrent - 1);
									newNode.SetValue(NodeTemp.GetValue());	
								}
								else
								{
									newNode.Evaluate();
								}
								
								if (((curr_node.GetPlayerTurn() == 2 || curr_node.GetPlayerTurn() == 4) && (newNode.GetValue() < bestNode.GetValue())) ||
								    ((curr_node.GetPlayerTurn() == 1 || curr_node.GetPlayerTurn() == 3) && (newNode.GetValue() > bestNode.GetValue())) ||
								    (bestNodeInit == false))
								{
									bestNode = newNode;
									bestNodeInit = true;
								}
							}
						}
					}
				}
			}
		}
		return bestNode;
	}

    //function which will use MinMax Algorithm to select best move for the computer player
	void ComputerMove()
	{
		Vector3 temp = new Vector3 (0, 0, -3.0f);
		currentGameState.SetPlayerTurn (1);
		
		Node m = MinMaxWithAlphaBetaPrunning(currentGameState,4,-50,50);

        //Node m = MinMax(currentGameState,3);
		 
		string movedPieceStartPosition = m.GetMovedPieceOldPosition ();
		string movedPieceNewPosition = m.GetMovedPieceNewPosition ();
		int startX,startY,endX,endY;
		startX=(int)Char.GetNumericValue(movedPieceStartPosition [0]);
		startY = (int)Char.GetNumericValue (movedPieceStartPosition [1]);
		endX = (int)Char.GetNumericValue (movedPieceNewPosition [0]);
		endY = (int)Char.GetNumericValue (movedPieceNewPosition [1]);

		currentGameState.MakeMove (startX, startY, endX, endY);
		
		string computerPieceName = GetCheckerPieceNameAtBox (movedPieceStartPosition);
		GameObject computerPiece=GameObject.Find(computerPieceName);
		GameObject newPosition = GameObject.Find (movedPieceNewPosition);

		computerPiece.transform.position = newPosition.transform.position;
		computerPiece.transform.position += temp;

		int srcXY, trgXY;
		int.TryParse (movedPieceStartPosition, out srcXY);
		int.TryParse (movedPieceNewPosition, out trgXY);

		if(currentGameState.GetPieceAtCurrentPosition(startX,startY)!=3)
		{
			if(trgXY==81 || trgXY==82 || trgXY==83 || trgXY==84 || trgXY==85 || trgXY==86 || trgXY==87 || trgXY==88)
			{
				GameObject p=GameObject.Find("Computer");
				GameObject t=Instantiate(yellowKing,new Vector3(0,0,0),Quaternion.identity)as GameObject;
				//set p to be the parent of t
				t.transform.parent=p.transform;
				//place it to the same position
				t.transform.position=computerPiece.transform.position;
				t.transform.transform.Rotate(-90,0,0);
				//rename the gameobject to differentiate 
				if(noOfComputerKings==0)
					t.name="yellowKing";
				else if(noOfComputerKings==1)
					t.name="yellowKing 1";
				else if(noOfComputerKings==2)
					t.name="yellowKing 2";
				else if(noOfComputerKings==3)
					t.name="yellowKing 3";
				else if(noOfComputerKings==4)
					t.name="yellowKing 4";
				else if(noOfComputerKings==5)
					t.name="yellowKing 5";
				else if(noOfComputerKings==6)
					t.name="yellowKing 6";
				else if(noOfComputerKings==7)
					t.name="yellowKing 7";
				else if(noOfComputerKings==8)
					t.name="yellowKing 8";
				else if(noOfComputerKings==9)
					t.name="yellowKing 9";
				else if(noOfComputerKings==10)
					t.name="yellowKing 10";
				else if(noOfComputerKings==11)
					t.name="yellowKing 11";
				UpdatecomputerColliderBoxes (computerPiece);
				SetcomputerKingColliderBox(newPosition);
				Destroy(GameObject.Find (computerPiece.name.ToString()));
				computerPiece=null;
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
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			} else if (trgXY == srcXY + 22) {
				int t = srcXY + 11;
				string t1 = t.ToString ();
				GameObject g1 = GameObject.Find (t1);
				string p = GetCheckerPieceName (g1);
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			} else if (trgXY == srcXY - 18) {
				int t = srcXY - 9;
				string t1 = t.ToString ();
				GameObject g1 = GameObject.Find (t1);
				string p = GetCheckerPieceName (g1);
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			} else if (trgXY == srcXY + 18) {
				int t = srcXY + 9;
				string t1 = t.ToString ();
				GameObject g1 = GameObject.Find (t1);
				string p = GetCheckerPieceName (g1);
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			}
			else if (trgXY == srcXY - 2) {
				int t = srcXY - 1;
				string t1 = t.ToString ();
				GameObject g1 = GameObject.Find (t1);
				string p = GetCheckerPieceName (g1);
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			} else if (trgXY == srcXY + 2) {
				int t = srcXY + 1;
				string t1 = t.ToString ();
				GameObject g1 = GameObject.Find (t1);
				string p = GetCheckerPieceName (g1);
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			} else if (trgXY == srcXY - 20) {
				int t = srcXY - 10;
				string t1 = t.ToString ();
				GameObject g1 = GameObject.Find (t1);
				string p = GetCheckerPieceName (g1);
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			} else if (trgXY == srcXY + 20) {
				int t = srcXY + 10;
				string t1 = t.ToString ();
				GameObject g1 = GameObject.Find (t1);
				string p = GetCheckerPieceName (g1);
				if(CheckIfP2KingPiece(GameObject.Find (p)))
					UpdatePlayer2KingColliderBoxes(GameObject.Find (p));
				else
					UpdatePlayer2ColliderBoxes (GameObject.Find (p));
				Destroy (GameObject.Find (p));
				player2DeadPieces++;
			}
		}
		computerMove=false;
		//update the pieces boxes numbers i-e row-column
		if(computerPiece!=null)
		{
			if (computerPiece.name == "yellow")
				computerColliderBoxes [0] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 1")
				computerColliderBoxes [1] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 2")
				computerColliderBoxes [2] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 3")
				computerColliderBoxes [3] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 4")
				computerColliderBoxes [4] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 5")
				computerColliderBoxes [5] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 6")
				computerColliderBoxes [6] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 7")
				computerColliderBoxes [7] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 8")
				computerColliderBoxes [8] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 9")
				computerColliderBoxes [9] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 10")
				computerColliderBoxes [10] = newPosition.transform.name;
			else if (computerPiece.name == "yellow 11")
				computerColliderBoxes [11] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing")
				computerKingColliderBoxes [0] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 1")
				computerKingColliderBoxes [1] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 2")
				computerKingColliderBoxes [2] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 3")
				computerKingColliderBoxes [3] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 4")
				computerKingColliderBoxes [4] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 5")
				computerKingColliderBoxes [5] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 6")
				computerKingColliderBoxes [6] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 7")
				computerKingColliderBoxes [7] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 8")
				computerKingColliderBoxes [8] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 9")
				computerKingColliderBoxes [9] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 10")
				computerKingColliderBoxes [10] = newPosition.transform.name;
			else if (computerPiece.name == "yellowKing 11")
				computerKingColliderBoxes [11] = newPosition.transform.name; 
		}
	}

}
