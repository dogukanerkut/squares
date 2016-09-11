//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Referring To: 
/// Referenced From: 
/// Attached To: 
/// <para>Description: Handles all operations about board, Checking for adjacent and same color blocks, transporting that data to manager class for it to destroy them</para>
/// <para>checking block availability for placing blocks to board, letting user to place blocks on only adjacent blocks</para>
/// </summary>
public class BlocksArray
{
	private GameObject[,] blocks = new GameObject[Constants.RowCount, Constants.ColumnCount];

	public GameObject this[int row, int column]
	{
		get
		{
			try
			{
				return blocks[row, column];
			}
			catch (System.Exception exc)
			{

				throw exc;
			}
			
		}
		set
		{
			blocks[row, column] = value;
		}
	}
	#region Block Availability and Adjacency control for letting user to only place blocks on adjacent blocks

	/// <summary>
	/// Checks if newly selected block is both available and is adjacent to current block and updates local block variables if check is true.
	/// </summary>
	/// <param name="newBlockInfo"></param>
	/// <returns></returns>
	public bool IsBlocksAdjacentAndAvailable(BlockInfo currentBlockInfo, BlockInfo newBlockInfo)
	{
		bool rBool = false;
		bool isNewBlockAdjacent = CheckAdjacentBlocks(currentBlockInfo, newBlockInfo);
		bool isNewBlockAvailable = CheckIfBlockAvailable(newBlockInfo);
		if (isNewBlockAdjacent && isNewBlockAvailable)
		{
			rBool = true;
		}
		return rBool;
	}
	/// <summary>
	/// Checks if the newly selected block is available(in terms of color).
	/// </summary>
	/// <param name="newBlockInfo"></param>
	/// <returns></returns>
	public bool CheckIfBlockAvailable(BlockInfo newBlockInfo)
	{
		bool rBool = false;
		if (newBlockInfo.BlockColor == Color.white) // if the new block is not empty
		{
			rBool = true;
		}
		return rBool;
	}

	/// <summary>
	/// Checks adjacent blocks of given block1 and compares if block2 is one of them(if it's adjacent to block1).
	/// </summary>
	/// <param name="blockInfo1">Main block to check adjacency.</param>
	/// <param name="blockInfo2">Block to be compared to block1's adjacency.</param>
	/// <returns></returns>
	private bool CheckAdjacentBlocks(BlockInfo blockInfo1, BlockInfo blockInfo2)
	{
		bool rBool = false;
		List<BlockInfo> adjacentBlocks = new List<BlockInfo>();
		//Right block check
		if (blockInfo1.Row != Constants.RowCount - 1) // If it's not at the rightmost of the board
			adjacentBlocks.Add(RetrieveInfo(blockInfo1.Row + 1, blockInfo1.Column)); // Add block at the right of the block1
		//Left block check
		if (blockInfo1.Row != 0) //If it's not at the leftmost of the board
			adjacentBlocks.Add(RetrieveInfo(blockInfo1.Row - 1, blockInfo1.Column)); // Add block at the left of the block1
		//Bottom block check
		if (blockInfo1.Column != Constants.ColumnCount - 1)// If it's not at the bottom of the board
			adjacentBlocks.Add(RetrieveInfo(blockInfo1.Row, blockInfo1.Column + 1)); // Add block under block1
		 //Top block check
		if (blockInfo1.Column != 0)// If it's not at the top of the board
			adjacentBlocks.Add(RetrieveInfo(blockInfo1.Row, blockInfo1.Column - 1)); // Add block above block1

		foreach (var block in adjacentBlocks)
		{
			if (block == blockInfo2) // if any of adjacent blocks is the block that has been selected by player.
			{
				//Debug.Log("Block is Adjacent!");
				rBool = true;
			}
		}
		return rBool;
	}
	#endregion

	/// <summary>
	/// Retrieves info of a block with given row and column indexes.
	/// </summary>
	/// <param name="_row">Row index of block.</param>
	/// <param name="_column">Column index of block.</param>
	public BlockInfo RetrieveInfo(int _row, int _column)
	{
		BlockInfo block = null;
		if (_row <= Constants.RowCount - 1 && _row >= 0 && _column <= Constants.ColumnCount - 1 && _column >= 0) //safecheck
		{
			block = blocks[_row, _column].GetComponent<Block>().info;
		}
		else
		{
			Debug.LogError("Index out of Range!"); // for debugging purpose.
		}
		return block;
	}

	#region Detecting same color adjacent blocks and transporting that data to manager class for manager class to destroy them
	private List<BlockInfo> adjacentBlocksWithSameColor = new List<BlockInfo>(); //Temporary list to hold all adjacent blocks with same color

	#region Comment encapsulated because of length
	// persistant list to hold all lists of adjacent blocks seperately
	// reason for this is to reward player if he/she matched more than one adjacent and same color blocks
	// for example if he/she matched 3 red blocks and 4 green blocks at the same time, the reward for 3 red blocks will be different than
	// reward for 4 green blocks and total score reward will be doubled since player matched them at the same time.
	#endregion
	private List<List<Block>> matchedBlockLists = new List<List<Block>>(); 
	private int adjacencyIndex = 0;

	public bool IsBlockListHasBlocks()
	{ return matchedBlockLists.Count > 0 ? true : false; }

	public List<List<Block>> GetMatchedBlockLists()
	{ return matchedBlockLists; }

	public void ClearMatchedBlockLists()
	{ matchedBlockLists.Clear(); }

	/// <summary>
	/// Control adjacent blocks, if they are same colors control them too(recursively), find all adjacent-same-color blocks and add them to matchedBlockLists.
	/// </summary>
	/// <param name="blockInfo">A single block to start control process.</param>
	public void Check3Match(BlockInfo blockInfo)
	{
		Color blockColor = blockInfo.BlockColor;
		//only add this block if it's not checked
		if (!blockInfo.IsChecked)
		adjacentBlocksWithSameColor.Add(blockInfo);

		//then control adjacent blocks and add to list if there are any same colors.
		if (blockInfo.Row != Constants.RowCount - 1)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row + 1, blockInfo.Column);
			if (blockColor == checkBlock.BlockColor && !checkBlock.IsChecked) //adjacent block should be same color as ours and should not be checked before.
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row + 1, blockInfo.Column)); // add it to our array
		}
		if (blockInfo.Row != 0) 
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row - 1, blockInfo.Column);
			if (blockColor == checkBlock.BlockColor && !checkBlock.IsChecked)
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row - 1, blockInfo.Column));
		}										  
		if (blockInfo.Column != Constants.ColumnCount - 1)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row, blockInfo.Column + 1);
			if (blockColor == checkBlock.BlockColor && !checkBlock.IsChecked)
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row, blockInfo.Column + 1));
		}															  
		if (blockInfo.Column != 0)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row, blockInfo.Column - 1);
			if (blockColor == checkBlock.BlockColor && !checkBlock.IsChecked)
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row, blockInfo.Column - 1));
		}
		
		if (adjacentBlocksWithSameColor.Count > 1) // if any adjacent same color blocks found
		{
			//set all added blocks to checked to avoid adding them again
			foreach (BlockInfo info in adjacentBlocksWithSameColor)
				info.IsChecked = true;

			adjacencyIndex++; // block is checked so increase index

			if (adjacencyIndex < adjacentBlocksWithSameColor.Count) //continue only if there are more blocks to check
				Check3Match(adjacentBlocksWithSameColor[adjacencyIndex]); //recursive!

			else if(adjacentBlocksWithSameColor.Count >= 3) // if there are no more blocks to check, check if adjacent blocks are more than 3
			{
				List<Block> adjacentBlockComponents = new List<Block>();
                foreach (BlockInfo info in adjacentBlocksWithSameColor) //Transfer them to Block component
					adjacentBlockComponents.Add(blocks[info.Row, info.Column].GetComponent<Block>());

				// transfer them to main list for selectionManager to destroy them all.
				#region obsolete comments
				// note that this adds adjacentBlockComponents as new list because adjacentBlockComponents list will be removed
				// in order to prepare it for next iteration. If it wouldn't be added as new list, it would be removed from
				// matchedBlockLists also due to C#'s nature.
				#endregion
				matchedBlockLists.Add(adjacentBlockComponents); 
				PrepareForNextIteration(); // this clears all temporary lists and index
			}
			else // if there are no more blocks to check and collected adjacent blocks are less than 3
			{
				ClearIsCheckedFlags(adjacentBlocksWithSameColor); // clears isChecked flags
				PrepareForNextIteration();// this clears all temporary lists and index
			}
		}
		else //if any adjacent same color blocks not found
		{
			ClearIsCheckedFlags(adjacentBlocksWithSameColor); // clears isChecked flags
			PrepareForNextIteration(); // this clears all temporary lists and index
		}
	}

	private void ClearIsCheckedFlags(List<BlockInfo> collectedBlocks)
	{
		foreach (BlockInfo block in collectedBlocks)
			block.IsChecked = false;
	}
	/// <summary>
	/// Clears all temporary lists and index
	/// </summary>
	private void PrepareForNextIteration()
	{
		adjacentBlocksWithSameColor.Clear();
		adjacencyIndex = 0;
	}
	#endregion
}
