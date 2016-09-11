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
	List<BlockInfo> newBlocks = new List<BlockInfo>();

	List<Block> blocksPlaced = new List<Block>();

	Block currentBlock;
	Block previousBlock;
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

				tempBlock.GetComponent<Block>().FillInfo(j, i, Color.white); //Set object's row and column
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
			newBlockImages[i].gameObject.SetActive(false);

		for (int i = 0; i < newBlocks.Count; i++)
		{
			newBlockImages[i].gameObject.SetActive(true);
			newBlockImages[i].color = newBlocks[i].BlockColor;
		}

	}
	void ResetBoard()
	{
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			for (int j = 0; j < Constants.RowCount; j++)
				blocks[j, i].GetComponent<Block>().Clear();
		}
	}

	/// <summary>
	/// When player first touches a block, Update touched block and start touching process.
	/// </summary>
	/// <param name="selectedBlock"></param>
	public void StartSelection(Block selectedBlock)
	{
		//Checks if block is white
		if (blocks.CheckIfBlockAvailable(selectedBlock.info))
		{
			gameState = GameState.SelectionStarted; //Start selection process
			currentBlock = selectedBlock;
			previousBlock = selectedBlock;

			UpdateSelectedBlock(selectedBlock);
		}
	}

	/// <summary>
	/// Appends the changes on selected block(sets it's color, adds it  to placed blocks list and updates selection count
	/// </summary>
	/// <param name="selectedBlock"></param>
	private void UpdateSelectedBlock(Block selectedBlock)
	{
		selectedBlock.SetColor(newBlocks[selectionCount].BlockColor);//Set color of selected block to queued new block
		blocksPlaced.Add(selectedBlock); // add currently selected block to the list of placed blocks(we will need it later)
		selectionCount++; // increase the selection count so next time the queued block will be placed
	}

	/// <summary>
	/// <para>When player continues to drag his finger on other blocks</para>
	/// <para>Do stuff accordingly(revert selection or add newblocks in queue).</para>
	/// </summary>
	/// <param name="selectedBlock"></param>
	public void SetSelectedBlock(Block selectedBlock)
	{
		//if player intends to add new block on queue to grid which is adjacent to previous one 
		if (blocks.IsBlocksAdjacentAndAvailable(currentBlock.info, selectedBlock.info))
		{
			// if number of selected blocks is not bigger than newly created blocks
			if (selectionCount < newBlocks.Count)
			{
				// Purpose of this to track current and previous blocks 
				// In order to revert our selection
				previousBlock = currentBlock; // Set current block as previous block as it will become previous block after this selection
				currentBlock = selectedBlock; // Set newly selected block as current block

				UpdateSelectedBlock(selectedBlock); // hover to method to see description
			}
		}
		// if player intends to revert his placed block by going backwards in placed blocks.
		else if(blocksPlaced.Contains(selectedBlock) && selectedBlock == previousBlock)
		{
			//player's finger is on previous block so remove the last placed block
			int selectedBlockIndex = blocksPlaced.IndexOf(selectedBlock);//get index of previous block

			//this check is required to prevent error when there is only one block exists and therefore index+1 is out of range
			if (selectedBlockIndex+1 < blocksPlaced.Count) // if the last placed block's index is smaller than number of blocksPlaced
			{
				//remove last placed block and update selection count accordingly
				blocksPlaced[selectedBlockIndex+1].Clear();
				blocksPlaced.RemoveAt(selectedBlockIndex+1);
				selectionCount--;
			}
			currentBlock = selectedBlock; // previously selected block is now currently selected block
			if (selectedBlockIndex > 0) //this check is required to prevent error when there is only one block exists and therefore index-1 is out of range
				previousBlock = blocksPlaced[selectedBlockIndex - 1]; // update previousBlock

		}
	}
	/// <summary>
	/// <para>This code runs when player releases finger.</para>
	/// <para>If player wants to terminate the selection(either by trying to place all queued new blocks or simply revert it's selection.</para>
	/// <para>If player places blocks, check for adjacency, if any 3 or more same color adjacent blocks found, destroy them.</para>
	/// </summary>
	public void TerminateSelection()
	{
		if (selectionCount > 0)// check if any selection made //
		{
			if (blocksPlaced.Count < newBlocks.Count) // if player didn't place all of given blocks yet
			{
				//remove all blocks that are temporarily placed to grid
				for (int i = 0; i < blocksPlaced.Count; i++)
					blocksPlaced[i].Clear();
			}
			else // player placed all of given blocks to grid
			{
				CreateBlocks(); // create new blocks to continue the game

				//Start recursive function for every single block that has been placed this turn.
				for (int i = 0; i < blocksPlaced.Count; i++)
				{
					blocks.Check3Match(blocksPlaced[i].info);
				}
				//After recursive function does its job of obtaining and assigning all 3 or more adjacent-same-color blocks
				//to a list seperately, check if that list contains any list(if any 3 or more adjacent-same-color blocks exist)
				// if that's true clear all catched 3 or more adjacent-same-color blocks
				if (blocks.IsBlockListHasBlocks())
				{
					List<List<Block>> matchedBlocksList = blocks.GetMatchedBlockLists();
					foreach (List<Block> adjacentBlocksWithSameColor in matchedBlocksList)
					{
						//score calculation goes here
						//TODO: Score Calculation!
						foreach (Block adjacentBlock in adjacentBlocksWithSameColor)
						{
							adjacentBlock.Clear();
							//score calculation goes here
						}
					}
					blocks.ClearMatchedBlockLists(); // we are done with the list and we can clear it now for further turns
				}
				// TODO:control of "if existing adjacent empty blocks' count is higher than newly introduced blocks' count" goes here
				// TODO:if not then the game is over.
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
		currentBlock = null;
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
