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
	public GameState CurGameState
	{ get { return gameState; } }




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
	}

	/// <summary>
	/// Generates a Board with prearranged Row and Column count(See Constants class) and assigns them to [blocks] 2D array.
	/// </summary>
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
	/// <summary>
	/// <para>When player first touches a block, Update touched block and start touching process.</para>
	/// </summary>
	/// <param name="selectedTile"></param>
	public void StartSelection(Tile selectedTile)
	{
		//Checks if tile is white
		if (blocks.CheckIfTileAvailable(selectedTile.info))
		{
			//selectionStarted = true;
			gameState = GameState.SelectionStarted; //Start selection process
			firstTile = selectedTile;
			currentTile = selectedTile;
			previousTile = selectedTile;

			UpdateSelectedBlock(selectedTile);
		}
		
	}

	/// <summary>
	/// Appends the changes on selected block(sets it's color, adds it  to placed blocks list and updates selection count
	/// </summary>
	/// <param name="selectedTile"></param>
	private void UpdateSelectedBlock(Tile selectedTile)
	{
		selectedTile.SetColor(newBlocks[selectionCount].TileColor);//Set color of selected block to queued new block
		blocksPlaced.Add(selectedTile); // add currently selected block to the list of placed blocks(we will need it later)
		selectionCount++; // increase the selection count so next time the queued block will be placed
	}

	/// <summary>
	/// <para>When player continues to drag his finger on other blocks</para>
	/// <para>Do stuff accordingly(revert selection or add newblocks in queue).</para>
	/// </summary>
	/// <param name="selectedTile"></param>
	public void SetSelectedTile(Tile selectedTile)
	{
		//if player intends to add new block on queue to grid which is adjacent to previous one 
		if (blocks.IsTilesAdjacentAndAvailable(currentTile.info, selectedTile.info))
		{
			// if number of selected tiles is not bigger than newly created blocks
			if (selectionCount < newBlocks.Count)
			{
				// Purpose of this to track current and previous tiles 
				// In order to revert our selection
				previousTile = currentTile; // Set current tile as previous tile as it will become previous tile after this selection
				currentTile = selectedTile; // Set newly selected tile as current tile

				UpdateSelectedBlock(selectedTile); // hover to method to see description
			}
		}
		// if player intends to revert his placed block by going backwards in placed blocks.
		else if(blocksPlaced.Contains(selectedTile) && selectedTile == previousTile)
		{
			//player's finger is on previous block so remove the last placed block
			int selectedTileIndex = blocksPlaced.IndexOf(selectedTile);//get index of previous block

			//this check is required to prevent error when there is only one tile exists and therefore index+1 is out of range
			if (selectedTileIndex+1 < blocksPlaced.Count) // if the last placed block's index is smaller than number of blocksPlaced
			{
				//remove last placed block and update selection count accordingly
				blocksPlaced[selectedTileIndex+1].Clear();
				blocksPlaced.RemoveAt(selectedTileIndex+1);
				selectionCount--;
			}
			currentTile = selectedTile; // previously selected tile is now currently selected tile
			if (selectedTileIndex > 0) //this check is required to prevent error when there is only one tile exists and therefore index-1 is out of range
				previousTile = blocksPlaced[selectedTileIndex - 1]; // update previousTile

		}
	}
	/// <summary>
	/// <para>This code runs when player releases finger.</para>
	/// <para>If player wants to terminate the selection(either by trying to place all queued new blocks or simply revert it's selection.</para>
	/// </summary>
	public void TerminateSelection()
	{
		if (selectionCount > 0)// check if any selection made
		{
			
			if (blocksPlaced.Count < newBlocks.Count) // if player didn't place all of given blocks yet
			{
				//remove all blocks that are temporarily placed to grid
				for (int i = 0; i < blocksPlaced.Count; i++)
				{
					blocksPlaced[i].Clear();
				}
			}
			else // if player placed all of given blocks to grid
			{
				CreateBlocks(); // create new blocks to continue the game

				// control of "3 or more adjacent blocks exist codes" goes here
				for (int i = 0; i < blocksPlaced.Count; i++)
				{
					blocks.Check3Match(blocksPlaced[i].info);
				}
				if (blocks.IsBlockListHasBlocks())
				{
					List<List<Tile>> matchedBlocksList = blocks.GetMatchedBlockLists();
					foreach (List<Tile> adjacentBlocksWithSameColor in matchedBlocksList)
					{
						foreach (Tile adjacentBlock in adjacentBlocksWithSameColor)
						{
							adjacentBlock.Clear();
						}
					}
					blocks.ClearMatchedBlockLists();
				}
				else
				{
					print("List Empty!");
				}
				
				// control of "if existing adjacent empty blocks' count is higher than newly introduced blocks' count" goes here
				// if not then the game is over.
			}
			//regardless of the conditions above, the player is released fingers
			//and we need to reset the selectionCount and blocksPlaced list
			//and set the gamestate back to idle
			selectionCount = 0;
			blocksPlaced.Clear();
			gameState = GameState.Idle;
		}

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
