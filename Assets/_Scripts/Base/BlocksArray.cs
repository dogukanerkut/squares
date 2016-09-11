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
/// Description: 
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
	/// <summary>
	/// Checks if newly selected tile is both available and is adjacent to current tile and updates local tile variables if check is true.
	/// </summary>
	/// <param name="newTileInfo"></param>
	/// <returns></returns>
	public bool IsTilesAdjacentAndAvailable(TileInfo currentTileInfo, TileInfo newTileInfo)
	{
		bool rBool = false;
		bool isNewTileAdjacent = CheckAdjacentTiles(currentTileInfo, newTileInfo);
		bool isNewTileAvailable = CheckIfTileAvailable(newTileInfo);
		if (isNewTileAdjacent && isNewTileAvailable)
		{
			rBool = true;
		}
		return rBool;
	}
	/// <summary>
	/// Checks if the newly selected tile is available(in terms of color).
	/// </summary>
	/// <param name="newTileInfo"></param>
	/// <returns></returns>
	public bool CheckIfTileAvailable(TileInfo newTileInfo)
	{
		bool rBool = false;
		if (newTileInfo.TileColor == Color.white) // if the new tile is not empty
		{
			rBool = true;
		}
		return rBool;
	}

	/// <summary>
	/// Checks adjacent tiles of given tile1 and compares if tile2 is one of them(if it's adjacent to tile1).
	/// </summary>
	/// <param name="tileInfo1">Main tile to check adjacency.</param>
	/// <param name="tileInfo2">Tile to be compared to tile1's adjacency.</param>
	/// <returns></returns>
	private bool CheckAdjacentTiles(TileInfo tileInfo1, TileInfo tileInfo2)
	{
		bool rBool = false;
		List<TileInfo> adjacentTiles = new List<TileInfo>();
		//Right tile check
		if (tileInfo1.Row != Constants.RowCount - 1) // If it's not at the rightmost of the board
			adjacentTiles.Add(RetrieveInfo(tileInfo1.Row + 1, tileInfo1.Column)); // Add tile at the right of the tile1
																							   //Left tile check
		if (tileInfo1.Row != 0) //If it's not at the leftmost of the board
			adjacentTiles.Add(RetrieveInfo(tileInfo1.Row - 1, tileInfo1.Column)); // Add tile at the left of the tile1
																							   //Bottom tile check
		if (tileInfo1.Column != Constants.ColumnCount - 1)// If it's not at the bottom of the board
			adjacentTiles.Add(RetrieveInfo(tileInfo1.Row, tileInfo1.Column + 1)); // Add tile under tile1
																							   //Top tile check
		if (tileInfo1.Column != 0)// If it's not at the top of the board
			adjacentTiles.Add(RetrieveInfo(tileInfo1.Row, tileInfo1.Column - 1)); // Add tile above tile1

		foreach (var tile in adjacentTiles)
		{
			if (tile == tileInfo2) // if any of adjacent tiles is the tile that has been selected by player.
			{
				//Debug.Log("Tile is Adjacent!");
				rBool = true;
			}
		}
		return rBool;
	}

	public TileInfo RetrieveInfo(int _row, int _column)
	{
		TileInfo tile = null;
		if (_row <= Constants.RowCount - 1 && _row >= 0 && _column <= Constants.ColumnCount - 1 && _column >= 0) //safecheck
		{
			tile = blocks[_row, _column].GetComponent<Tile>().info;
		}
		else
		{
			Debug.LogError("Index out of Range!"); // for debugging purpose.
		}
		return tile;
	}

	private List<TileInfo> adjacentBlocksWithSameColor = new List<TileInfo>();
	private List<Tile> adjacentBlockComponents = new List<Tile>();
	private List<List<Tile>> matchedBlockLists = new List<List<Tile>>();
	private int adjacencyIndex = 0;

	public bool IsBlockListHasBlocks()
	{
		return matchedBlockLists.Count > 0 ? true : false;
	}
	public List<List<Tile>> GetMatchedBlockLists()
	{
		return matchedBlockLists;
	}

	public void ClearMatchedBlockLists()
	{
		matchedBlockLists.Clear();
	}
	public void Check3Match(TileInfo blockInfo)
	{
		//List<TileInfo> adjacentBlocksWithSameColor = new List<TileInfo>();
		Color blockColor = blockInfo.TileColor;
		//only add this tile if it's not checked
		if (!blockInfo.isChecked)
		adjacentBlocksWithSameColor.Add(blockInfo); //first add this block to list

		//then control adjacent tiles and add to list if there are any same colors.
		if (blockInfo.Row != Constants.RowCount - 1) // If it's not at the rightmost of the board
		{
			TileInfo checkTile = RetrieveInfo(blockInfo.Row + 1, blockInfo.Column);
			if (blockColor == checkTile.TileColor && !checkTile.isChecked)
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row + 1, blockInfo.Column));
		}
		if (blockInfo.Row != 0) //If it's not at the leftmost of the board
		{
			TileInfo checkTile = RetrieveInfo(blockInfo.Row - 1, blockInfo.Column);
			if (blockColor == checkTile.TileColor && !checkTile.isChecked)
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row - 1, blockInfo.Column));
		}										  
		if (blockInfo.Column != Constants.ColumnCount - 1)// If it's not at the bottom of the board
		{
			TileInfo checkTile = RetrieveInfo(blockInfo.Row, blockInfo.Column + 1);
			if (blockColor == checkTile.TileColor && !checkTile.isChecked)
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row, blockInfo.Column + 1));
		}															  
		if (blockInfo.Column != 0)// If it's not at the top of the board
		{
			TileInfo checkTile = RetrieveInfo(blockInfo.Row, blockInfo.Column - 1);
			if (blockColor == checkTile.TileColor && !checkTile.isChecked)
				adjacentBlocksWithSameColor.Add(RetrieveInfo(blockInfo.Row, blockInfo.Column - 1));
		}
		
		

		if (adjacentBlocksWithSameColor.Count > 1) // if any adjacent same color blocks found
		{
			//set all added blocks to checked to avoid adding them again
			foreach (TileInfo info in adjacentBlocksWithSameColor)
			{
				info.isChecked = true;
			}
			adjacencyIndex++;

			if (adjacencyIndex < adjacentBlocksWithSameColor.Count) //continue only if there are more blocks to check
			{
				Check3Match(adjacentBlocksWithSameColor[adjacencyIndex]); //recursive!
			}
			else if(adjacentBlocksWithSameColor.Count >= 3) // if there are no more blocks to check, check if adjacent blocks are more than 3
			{
				foreach (TileInfo info in adjacentBlocksWithSameColor) //Transfer them to Tile component
				{
					adjacentBlockComponents.Add(blocks[info.Row, info.Column].GetComponent<Tile>());
				}
				matchedBlockLists.Add(new List<Tile>(adjacentBlockComponents)); // transfer them to collaborative list for selectionManager to destroy them all
				Debug.Log("Caboomination!");
				ClearTempLists();
			}
			else // if there are no more blocks to check and collected adjacent blocks are less than 3
			{
				ClearIsCheckedFlags(adjacentBlocksWithSameColor);
				ClearTempLists();
			}
		}
		else //if any adjacent same color blocks not found
		{
			ClearIsCheckedFlags(adjacentBlocksWithSameColor);
			ClearTempLists();
		}

	}

	private void ClearIsCheckedFlags(List<TileInfo> collectedBlocks)
	{
		foreach (TileInfo block in collectedBlocks)
		{
			block.isChecked = false;
		}
	}
	private void ClearTempLists()
	{
		adjacentBlockComponents.Clear();
		adjacentBlocksWithSameColor.Clear();
		adjacencyIndex = 0;
	}

}
