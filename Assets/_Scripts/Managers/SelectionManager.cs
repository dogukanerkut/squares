//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
/// <summary>
/// Referring To: BoardManager.cs(Singleton)
/// Referenced From: EventController.cs
/// Attached To: SelectionManager
/// Description: Handles selection activities(when player presses and drags).
/// </summary>
public class SelectionManager : MonoBehaviour
{

	// Use this for initialization
	BlocksArray blocks;

	Tile firstTile;
	Tile currentTile;
	Tile previousTile;
	private int selectionCount = 0;
	private GameState gameState;
	private bool selectionStarted;

	public bool SelectionStarted
	{ get {return selectionStarted;} }

	public GameObject blockPrefab;
	public RectTransform gamePanel;
	void Start ()
	{
		gameState = GameState.Idle;
		GenerateBoard();
	}

	/// <summary>
	/// Generates a Board with given row and column parameters and assigns them to [blocks] 2D array.
	/// </summary>
	/// <param name="_row">Row count.</param>
	/// <param name="_column"> Column count.</param>
	void GenerateBoard()
	{
		blocks = new BlocksArray();
		//blocks = new GameObject[_row, _column];
		//Instantiate prefabs and assign them to blocks 2D array.
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			for (int j = 0; j < Constants.RowCount; j++)
			{
				GameObject tempBlock = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				tempBlock.transform.SetParent(gamePanel);
				tempBlock.transform.localScale = Vector3.one; // Instantiated objects has different scale value than 1 somehow?
				blocks[j, i] = tempBlock;

				tempBlock.GetComponent<Tile>().FillInfo(j, i, Color.white); //Set object's row and column
			}
		}
	}
	void ResetBoard()
	{
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			for (int j = 0; j < Constants.RowCount; j++)
			{
				blocks[j, i].GetComponent<Tile>().Clear();
			}
		}
	}

	public void StartSelection(Tile selectedTile)
	{
		if (blocks.CheckIfTileAvailable(selectedTile.info))
		{
			selectionStarted = true;
			firstTile = selectedTile;
			currentTile = selectedTile;
			previousTile = selectedTile;
			selectedTile.SetColor(Color.red);//debug
			selectionCount++;
		}
		
	}

	public void SetSelectedTile(Tile selectedTile)
	{
		if (blocks.IsTilesAdjacentAndAvailable(currentTile.info, selectedTile.info))
		{
			previousTile = currentTile;
			currentTile = selectedTile;
			selectedTile.SetColor(Color.red);//debug
			selectionCount++;
		}


	}
	public void EndSelection()
	{
		selectionStarted = false;

	}
	public void ClearSelection()
	{
		firstTile = null;
		currentTile = null;
		selectionCount = 0;
	}


	//List<RaycastResult> results = new List<RaycastResult>();
//	void Update ()
//	{
//#if UNITY_EDITOR
//		if (gameState == GameState.Idle)
//		{
//			//if (Input.GetKey(KeyCode.Mouse0))
//			//{
//			//	//Code to be place in a MonoBehaviour with a GraphicRaycaster component
//			//	GraphicRaycaster gr = GameObject.Find("GameBoardCanvas").GetComponent<GraphicRaycaster>();
//			//	//Create the PointerEventData with null for the EventSystem
//			//	PointerEventData ped = new PointerEventData(null);
//			//	//Set required parameters, in this case, mouse position
//			//	ped.position = Input.mousePosition;
//			//	//Create list to receive all results
				
//			//	//Raycast it
//			//	gr.Raycast(ped, results);

//   //         }
//			//else if(Input.GetKeyUp(KeyCode.Mouse0))
//			//{
//			//	foreach (var item in results)
//			//	{
//			//		print(item.gameObject.name);
//			//	}
//			//}

			
//        }

//		#endif

//	}
}
