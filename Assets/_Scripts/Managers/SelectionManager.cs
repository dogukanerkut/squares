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
	ColorBase colorBase = new ColorBase();
	BlocksCreator blocksCreator = new BlocksCreator();
	List<TileInfo> newBlocks = new List<TileInfo>();
	List<TileInfo> tempBlocks = new List<TileInfo>();

	List<Tile> blocksPlaced = new List<Tile>();

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
	public RectTransform newBlocksPanel;
	private Image[] newBlockImages;

	void Awake()
	{
		newBlockImages = newBlocksPanel.GetComponentsInChildren<Image>();
	}

	void Start ()
	{

		gameState = GameState.Idle;
		GenerateBoard();

		//ColorBase colorCreator = new ColorBase();
		//TileInfo[] tileInfos = new TileInfo[Constants.ColumnCount * Constants.RowCount];
		//int count = 0;
		//for (int i = 0; i < Constants.ColumnCount; i++)
		//{
		//	for (int j = 0; j < Constants.RowCount; j++)
		//	{
		//		tileInfos[count] = blocks[j, i].GetComponent<Tile>().info;
		//		count++;
		//	}
		//}
		//colorCreator.FillColorInfo(tileInfos);
		//print(blocks[0, 0].GetComponent<Tile>().info.TileColor);
		//print(tileInfos[0].TileColor);
		//print(tileInfos[1].TileColor);
  //      blocks[0, 0].GetComponent<Tile>().SetColor(tileInfos[0].TileColor);
		//blocks[0, 1].GetComponent<Tile>().SetColor(tileInfos[1].TileColor);

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
		CreateBlocks();

	}
	/// <summary>
	/// Create new blocks, assign their colors and arrange the new block images.
	/// </summary>
	void CreateBlocks()
	{
		newBlocks.Clear();
		newBlocks = blocksCreator.GetRandomBlocks();
		colorBase.FillColorInfo(newBlocks);
		for (int i = 0; i < newBlockImages.Length; i++)
		{
			newBlockImages[i].gameObject.SetActive(false);
		}
		for (int i = 0; i < newBlocks.Count; i++)
		{
			newBlockImages[i].gameObject.SetActive(true);
			newBlockImages[i].color = newBlocks[i].TileColor;
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
			//selectedTile.SetColor(Color.red);//debug
			selectedTile.SetColor(newBlocks[selectionCount].TileColor);
			blocksPlaced.Add(selectedTile);
			selectionCount++;
		}
		
	}

	public void SetSelectedTile(Tile selectedTile)
	{
		if (blocks.IsTilesAdjacentAndAvailable(currentTile.info, selectedTile.info))
		{
			
			//selectedTile.SetColor(Color.red);//debug
			if (selectionCount < newBlocks.Count)
			{
				previousTile = currentTile;
				currentTile = selectedTile;

				selectedTile.SetColor(newBlocks[selectionCount].TileColor);
				blocksPlaced.Add(selectedTile);
				selectionCount++;
			}
		}
		else if(blocksPlaced.Contains(selectedTile) && selectedTile == previousTile)
		{
			int index = blocksPlaced.IndexOf(selectedTile);
			//for (int i = 0; i < blocksPlaced.Count; i++)
			//{
			if (index+1 < blocksPlaced.Count)
			{
				blocksPlaced[index+1].Clear();
					blocksPlaced.RemoveAt(index+1);
					selectionCount--;
			}
			//}
			currentTile = selectedTile;
			if (index > 0)
				previousTile = blocksPlaced[index - 1];

		}
	}

	public void TerminateSelection()
	{
		if (selectionCount > 0)
		{

			if (blocksPlaced.Count < newBlocks.Count)
			{
				for (int i = 0; i < blocksPlaced.Count; i++)
				{
					blocksPlaced[i].Clear();
				}
			}
			else
			{
				CreateBlocks();
			}
			selectionCount = 0;
			blocksPlaced.Clear();
			selectionStarted = false;
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
