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
				Debug.Log("Tile is Adjacent!");
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

}
