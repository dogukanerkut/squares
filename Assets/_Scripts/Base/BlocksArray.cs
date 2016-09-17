//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
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
	#region Block Availability and Adjacency control for letting user to only place blocks on adjacent blocks -------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Checks if newly selected block is both available and is adjacent to current block and updates local block variables if check is true.
	/// </summary>
	/// <param name="currentBlockInfo">Already placed block.</param>
	/// <param name="newBlockInfo">Block to be placed.</param>
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
		if (newBlockInfo.BlockColor.GetColor() == ColorBase.defaultColor) // if the new block is not empty
		{
			rBool = true;
		}
		return rBool;
	}

	/// <summary>
	/// Checks adjacent blocks(left, right, up and down) of given block1 and compares if block2 is one of them(if it's adjacent to block1).
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
		return block;
	}

	#region Detecting same color adjacent blocks and transporting that data to manager class for manager class to destroy them -------------------------------------------------------------------------------------------------------------------------------

	private List<BlockInfo> adjacentBlocksWithSameColor = new List<BlockInfo>(); //Temporary list to hold all adjacent blocks with same color

	#region Comment encapsulated because of length[giving info about reason of using nested list here]
	// persistant list to hold all lists of adjacent blocks seperately
	// reason for this is to reward player if he/she matched more than one adjacent and same color blocks
	// for example if he/she matched 3 red blocks and 4 green blocks at the same time, the reward for 3 red blocks will be different than
	// reward for 4 green blocks and total score reward will be doubled since player matched them at the same time.
	#endregion
	private List<List<Block>> matchedBlockLists = new List<List<Block>>(); 
	private int adjacencyIndex = 0;

	private bool IsBlockListHasBlocks()
	{ return matchedBlockLists.Count > 0 ? true : false; }

	public List<List<Block>> GetMatchedBlockLists()
	{ return matchedBlockLists; }

	public void ClearMatchedBlockLists()
	{ matchedBlockLists.Clear(); }

	/// <summary>
	/// Starts recursive function for all added blocks and returns true if any equal or more than 3 adjacent-same-color blocks found.
	/// </summary>
	/// <param name="blocksPlaced">List of blocks that has been placed by player.</param>
	/// <returns></returns>
	public bool CheckAdjacentBlocks(List<Block> blocksPlaced)
	{
		for (int i = 0; i < blocksPlaced.Count; i++)
			Check3Match(blocksPlaced[i].info);//Start recursive function for every single block that has been placed this turn.
											  //After recursive function does its job of obtaining and assigning all 3 or more adjacent-same-color blocks
											  //to a list seperately, check if that list contains any list(if any 3 or more adjacent-same-color blocks exist)
		return IsBlockListHasBlocks();
	}

	/// <summary>
	/// Control adjacent blocks, if they are same colors control them too(recursively), find all adjacent-same-color blocks and add them to matchedBlockLists.
	/// Note that this method's functionality has been extended to look for empty blocks efficiently.
	/// <param name="blockInfo">A single block to start adjacency control process.</param>
	/// <param name="isWhiteBlockCheck">If the intention is to check for empty blocks(to see there are any space left to place new blocks).</param>
	/// <param name="adjacentBlockCount">Number of adjacent empty block is needed in order to place all new blocks.</param>
	private void Check3Match(BlockInfo blockInfo, bool isWhiteBlockCheck = false, int adjacentBlockCount = 3) // default game rule is to find 3 or more blocks
	{
		Color blockColor = blockInfo.BlockColor.GetColor();
		//only add this block if it's not checked
		if (!blockInfo.IsChecked)
		adjacentBlocksWithSameColor.Add(blockInfo);

		//then control adjacent blocks(left, right, up and down) and add to list if there are any same colors.
		if (blockInfo.Row != Constants.RowCount - 1)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row + 1, blockInfo.Column);
			if (blockColor == checkBlock.BlockColor.GetColor() && !checkBlock.IsChecked) //adjacent block should be same color as ours and should not be checked before.
				adjacentBlocksWithSameColor.Add(checkBlock); // add it to our array
		}
		if (blockInfo.Row != 0) 
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row - 1, blockInfo.Column);
			if (blockColor == checkBlock.BlockColor.GetColor() && !checkBlock.IsChecked)
				adjacentBlocksWithSameColor.Add(checkBlock);
		}										  
		if (blockInfo.Column != Constants.ColumnCount - 1)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row, blockInfo.Column + 1);
			if (blockColor == checkBlock.BlockColor.GetColor() && !checkBlock.IsChecked)
				adjacentBlocksWithSameColor.Add(checkBlock);
		}															  
		if (blockInfo.Column != 0)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row, blockInfo.Column - 1);
			if (blockColor == checkBlock.BlockColor.GetColor() && !checkBlock.IsChecked)
				adjacentBlocksWithSameColor.Add(checkBlock);
		}
		
		if (adjacentBlocksWithSameColor.Count > 1 || (isWhiteBlockCheck && adjacentBlocksWithSameColor.Count > 0)) // if any adjacent same color blocks found
		{																									   // or code is looking for empty blocks and even 1 block should be sended
			//set all added blocks to checked to avoid adding them again
			foreach (BlockInfo info in adjacentBlocksWithSameColor)
				info.IsChecked = true;

			adjacencyIndex++; // block is checked so increase index

			//if the code is searching for colored blocks, it needs to find every single colored block
			//that are adjacent to eachother and destroy them if they are more than 3.
			if (adjacencyIndex < adjacentBlocksWithSameColor.Count) //continue only if there are more blocks to check
				Check3Match(adjacentBlocksWithSameColor[adjacencyIndex], isWhiteBlockCheck, adjacentBlockCount); //recursive!

			else if(adjacentBlocksWithSameColor.Count >= adjacentBlockCount) // if there are no more blocks to check, check if adjacent blocks are more than given block Count
			{																
				List<Block> adjacentBlockComponents = new List<Block>();
                foreach (BlockInfo info in adjacentBlocksWithSameColor) //Transfer them to Block component
					adjacentBlockComponents.Add(blocks[info.Row, info.Column].GetComponent<Block>());

				// transfer them to main list for BoardManager to destroy them all.
				matchedBlockLists.Add(adjacentBlockComponents); 
			}
			else // if there are no more blocks to check and collected adjacent blocks are less than 3
			{
				if (!isWhiteBlockCheck) // only clear checked flags if it's not empty block check because we will clear flags when all checks ended for optimization
					ClearIsCheckedFlags(adjacentBlocksWithSameColor); // clears isChecked flags
			}
		}
		else //if any adjacent same color blocks not found
		{
			if (!isWhiteBlockCheck) // only clear checked flags if it's not white empty check because we will clear flags when all checks ended for optimization
				ClearIsCheckedFlags(adjacentBlocksWithSameColor); // clears isChecked flags
		}
		PrepareForNextIteration(); // this clears all temporary lists and index
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

	#region Check all white blocks to see if any available spots left for player to put -------------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Checks all empty Blocks in board and returns immediately if a consequent adjacent place found for player to place newly created blocks
	/// <para>WARNING! This method must be used after new blocks created!</para>
	///</summary>
	///<param name="newBlocksCount">Number of new blocks created</param>
	public bool CheckEmptyBlocks(int newBlocksCount)
	{
		List<BlockInfo> emptyBlocks = new List<BlockInfo>();
		bool rBool = false;
		//get all white blocks in board
		for (int i = 0; i < Constants.ColumnCount; i++)
		{
			for (int j = 0; j < Constants.RowCount; j++)
			{
				if (blocks[j, i].GetComponent<Block>().info.BlockColor.GetColor() == ColorBase.defaultColor) // if the block is white
				{
					emptyBlocks.Add(blocks[j, i].GetComponent<Block>().info);
				}
			}
		}
		// now we need to seperate them according to their adjacency
		// we can use our slightly modified version of recursive function Check3Match and matchedBlockLists to store them seperately
		for (int i = 0; i < emptyBlocks.Count; i++)
		{
			if (!emptyBlocks[i].IsChecked) // skip already checked blocks
			{
				Check3Match(emptyBlocks[i], true, newBlocksCount); // this is a white block check so flag it and give custom block count to check accordingly and avoid overstoring!
				//Check3Match only adds to matchedBlockLists if there are more equal or more adjacent blocks than newBlocksCount
			}
		}
		//our matchedBlockLists should be filled with atleast one block of adjacent white blocks
		//so check if their count is bigger or equal than our newBlockCount
		//note that this check only works if our newBlockCount smaller than 4(because different condition appears after 4, see below comment for more info)
		if (matchedBlockLists.Count != 0) //if we have some elements to test
			{
				foreach (List<Block> adjacentBlockList in matchedBlockLists)
				{
					int additionalCount = newBlocksCount >= 4 ? 2 : 0; // if we have 4 or more new blocks to be placed, we need to add 2...
					#region Comment encapsulated because of length[reasoning explanation]
					//because safest way to be sure is to have atleast 6 adjacent empty blocks to place 4 blocks. See example below.
					//Consider & as previously placed blocks(blocks with colors) and * as empty blocks(white blocks)
					//And consider that player received 4 blocks to put
					// & & & * &
					// & & * * *
					// & & & * &
					// & & & * & 
					// & & & & &
					//before going to more complex way of doing this, only way to be sure that player can put his blocks is to have 6 empty adjacent blocks.
					//if we don't have 6 adjacent empty blocks we will need another approach to solve this problem.
					//that approach is explained in AdvancedEmptyBlockCheck(in further lines) method.
					#endregion

					if (adjacentBlockList.Count >= newBlocksCount + additionalCount) // if there are enough empty blocks for user to place new blocks
					{
						rBool = true; //new blocks can be placed to grid
						break; // this leaves from foreach loop
					}
				}
			}

		//so far we are ok as long as we get less than 4 blocks, we can control our grid properly by using our Check3Match method to see adjacent empty blocks
		//but if we get 4 blocks, there is a certain pattern that there is 4 adjacent empty blocks exist(which our code can detect) but we can't place our 4 blocks
		//in this pattern, consider following example.
		#region Comment encapsulated because of length[example]
		//Consider & as previously placed blocks(blocks with colors) and * as empty blocks(white blocks)
		//And consider that player received 4 blocks to put
		//& & & * &
		//& & * * *
		//& & & & &
		//& & & & &
		//& & & & &
		//In this case there are 4 adjacent empty blocks(that our code detects) but player can't place 4 blocks(that our code can't detect!) here.
		//-------------------------
		//& & & * &
		//& & * * *
		//& & & * &
		//& & & & & 
		//& & & & &
		//In this case there are 5 adjacent empty blocks but player still can't place 4 blocks. So a quick way around won't solve our problem
		//Therefore we need a different approach and that approach is AdvancedEmptyBlockCheck
		#endregion
		if (!rBool && newBlocksCount >= 4) //only do this if there are not empty blocks for user to place new blocks detected and our newBlocksCount is bigger than 4
		{
			if (matchedBlockLists.Count != 0) // if we have some elements to test
			{
				//Send every block info in every block list to check their patterns
				foreach (List<Block> adjacentBlockList in matchedBlockLists)
				{
					if (rBool)
						break;
					foreach (Block adjacentBlock in adjacentBlockList)
					{
						AdvancedEmptyBlockCheck(adjacentBlock.GetComponent<Block>().info, newBlocksCount);
						if (emptySpaceAvailable) //this is modified within AdvancedEmptyBlockCheck
						{
							rBool = true;
							emptySpaceAvailable = false;
							break;
						}
					}
				}
			}
		}
		//we got what we want so we can reset our variables
		ClearIsCheckedFlags(emptyBlocks);
		ClearMatchedBlockLists();
		PrepareForNextIteration();
		return rBool;
	}


	int nothingAddedCount = 0; // if nothing is added to our adjacentBlocksWithSameColor list for 2 times we terminate recursive method.
	bool emptySpaceAvailable = false; // this bool needs to be global in order for CheckEmptyBlocks to see if this method found any pattern that 4 blocks can be placed into.
	List<BlockInfo> removedBlocks = new List<BlockInfo>(); // list of our "dead end" blocks
	//The reason we cannot use Check3Match method to see all white blocks is that
	//There is a specific pattern where Check3Match method can't handle properly
	//See for visualisation of that pattern below:
	//Consider & as previously placed blocks(blocks with colors) and * as empty blocks(white blocks)
	//And consider that player received 4 blocks to put
	// & & & * &
	// & & * * *
	// & & & & &
	// & & & & & 
	// & & & & &
	//In that example and all of it's rotated variations, Check3Match will not detect that player can't put 4 blocks on this situation
	//Player can only put 3 blocks and he will be left with 1 more block to be put but can't put it, so the game will not detect game over
	//And therefore game will stuck at that point. That's why we need a different approach on detecting available empty blocks
	//This approach takes one block as "reference point" and seeks a single path from that point. Every time code comes to "dead end"
	//It turns to the last point where it left and looks for another path, this way it can detect that player can't place blocks in previous example
	/// <summary>
	/// Check if we can put our newBlocks to our adjacent empty blocks
	/// </summary>
	/// <param name="blockInfo">"Reference point" that code will look a path from.</param>
	/// <param name="newBlocksCount">Number of new blocks we have.</param>
	private void AdvancedEmptyBlockCheck(BlockInfo blockInfo, int newBlocksCount)
	{
		Color blockColor = blockInfo.BlockColor.GetColor();
		int consequentCount = adjacentBlocksWithSameColor.Count; //to ensure we added a new block to our list
		if (!adjacentBlocksWithSameColor.Contains(blockInfo)) // do not add already added blocks
			adjacentBlocksWithSameColor.Add(blockInfo);

		bool isSingleBlockAdded = false; // we want to add single block at a time per run(this way we can ensure code will look for a single path)
		bool isAdjacentToDeadEndBlock = false; // if the block we are looking is adjacent to a "dead end" block
		//control adjacent blocks and add to list if there are any same colors.
		//whole if block is for looking left, right, up and down of given blockInfo and adding a single block that is same color and
		//is not flagged as "dead end" from our previous runs
		if (blockInfo.Row != Constants.RowCount - 1 && !isSingleBlockAdded)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row + 1, blockInfo.Column);
			if (blockColor == checkBlock.BlockColor.GetColor() && !adjacentBlocksWithSameColor.Contains(checkBlock) && !checkBlock.IsDeadEnd) //only add it if its same color && it doesn't included in list already && it is not a deadend
			{
				adjacentBlocksWithSameColor.Add(checkBlock); // add it to our array
				isSingleBlockAdded = true;
			}
			else if (checkBlock.IsDeadEnd) isAdjacentToDeadEndBlock = true;
		}
		if (blockInfo.Row != 0 && !isSingleBlockAdded)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row - 1, blockInfo.Column);
			if (blockColor == checkBlock.BlockColor.GetColor() && !adjacentBlocksWithSameColor.Contains(checkBlock) && !checkBlock.IsDeadEnd)
			{
				adjacentBlocksWithSameColor.Add(checkBlock);
				isSingleBlockAdded = true;
			}
			else if (checkBlock.IsDeadEnd) isAdjacentToDeadEndBlock = true;
		}
		if (blockInfo.Column != Constants.ColumnCount - 1 && !isSingleBlockAdded)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row, blockInfo.Column + 1);
			if (blockColor == checkBlock.BlockColor.GetColor() && !adjacentBlocksWithSameColor.Contains(checkBlock) && !checkBlock.IsDeadEnd)
			{
				adjacentBlocksWithSameColor.Add(checkBlock);
				isSingleBlockAdded = true;
			}
			else if (checkBlock.IsDeadEnd) isAdjacentToDeadEndBlock = true;
		}
		if (blockInfo.Column != 0 && !isSingleBlockAdded)
		{
			BlockInfo checkBlock = RetrieveInfo(blockInfo.Row, blockInfo.Column - 1);
			if (blockColor == checkBlock.BlockColor.GetColor() && !adjacentBlocksWithSameColor.Contains(checkBlock) && !checkBlock.IsDeadEnd)
			{
				adjacentBlocksWithSameColor.Add(checkBlock);
				isSingleBlockAdded = true;
			}
			else if (checkBlock.IsDeadEnd) isAdjacentToDeadEndBlock = true;
		}

		if (consequentCount != adjacentBlocksWithSameColor.Count) // if any blocks added to list
			nothingAddedCount = 0; // reset our counter

		adjacencyIndex++; // block is checked so increase index

		if (adjacentBlocksWithSameColor.Count >= newBlocksCount) // check if adjacent blocks are more than given block Count
		{
			emptySpaceAvailable = true; // we won't continue recursive method because we got what we need
			PrepareForNextIteration(); // this clears all temporary lists and index
		}
		else if (adjacencyIndex < adjacentBlocksWithSameColor.Count) //continue only if there are more blocks to check 
			AdvancedEmptyBlockCheck(adjacentBlocksWithSameColor[adjacencyIndex], newBlocksCount); //recursive!
		else // if there are no more blocks to check and collected adjacent blocks are less than newBlocksCount
		{
			if (nothingAddedCount < 2)
			{
				nothingAddedCount++;
				//This place of code indicates that we are at a "dead end" so we need to flag this block as dead end and
				// remove it from our list because we need to search for another path.
				if (isAdjacentToDeadEndBlock)
				{
					//We flag our "dead end" block and transfer it to removedBlocks list so it won't bother us anymore for this run
					adjacentBlocksWithSameColor[adjacentBlocksWithSameColor.Count - 1].IsDeadEnd = true;
					removedBlocks.Add(adjacentBlocksWithSameColor[adjacentBlocksWithSameColor.Count - 1]);
					adjacentBlocksWithSameColor.RemoveAt(adjacentBlocksWithSameColor.Count - 1);
					adjacencyIndex = adjacentBlocksWithSameColor.Count - 1; // update adjacency index to check different blocks
				}
				AdvancedEmptyBlockCheck(adjacentBlocksWithSameColor[adjacentBlocksWithSameColor.Count - 1], newBlocksCount);
			}
			else // if nothing is added for 2 turns(referenced empty block's every possible path is calculated)
			{
				//false every blocks' isDeadEnd so we can start a clean look from a different reference point(a different block's path)
				foreach (BlockInfo block in adjacentBlocksWithSameColor)
					block.IsDeadEnd = false;
				foreach (BlockInfo block in removedBlocks)
					block.IsDeadEnd = false;
				removedBlocks.Clear();
				PrepareForNextIteration();
				nothingAddedCount = 0;
			}
		}
		//}
	}
	#endregion

}
