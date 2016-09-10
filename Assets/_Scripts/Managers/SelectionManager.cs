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
	TileHolder firstTile;
	TileHolder currentTile;
	TileHolder previousTile;
	private int selectionCount = 0;
	private GameState gameState;
	private bool selectionStarted;

	public bool SelectionStarted
	{ get {return selectionStarted;} }

	void Start ()
	{
		gameState = GameState.Idle;
	}

	public void StartSelection(TileHolder selectedTile)
	{
		selectionStarted = true;
		firstTile = selectedTile;
		currentTile = selectedTile;
		previousTile = selectedTile;
		selectionCount++;
	}

	public void SetSelectedTile(TileHolder selectedTile)
	{
		selectionCount++;
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
	void AssignLocalTiles(TileHolder newTile)
	{
		previousTile = currentTile; // assign current tile to previous tile as new tile will be current tile now
		currentTile = newTile; // new tile is assigned to currentTile
	}
	//public string PreviousTile()
	//{
	//	return previousTile.tile.Row.ToString() + previousTile.tile.Column.ToString();
	//}
	//public string CurrentTile()
	//{
	//	return currentTile.tile.Row.ToString() + currentTile.tile.Column.ToString();
	//}

	/// <summary>
	/// Checks if newly selected tile is both available and is adjacent to current tile and updates local tile variables if check is true.
	/// </summary>
	/// <param name="newTile"></param>
	/// <returns></returns>
	public bool IsTilesAdjacentAndAvailable(TileHolder newTile)
	{
		bool rBool = false;
		bool isNewTileAdjacent = CheckAdjacentTiles(currentTile.tile, newTile.tile);
		bool isNewTileAvailable = CheckIfTileAvailable(newTile.tile);
		if (isNewTileAdjacent && isNewTileAvailable)
		{
			rBool = true;
			AssignLocalTiles(newTile);
		}
		return rBool;
	}
	/// <summary>
	/// Checks if the newly selected tile is available(in terms of color).
	/// </summary>
	/// <param name="newTile"></param>
	/// <returns></returns>
	private bool CheckIfTileAvailable(BaseTile newTile)
	{
		bool rBool = false;
		if (newTile.TileColor == Color.white) // if the new tile is not empty
		{
			rBool = true;
		}
		return rBool;
	}
	/// <summary>
	/// Checks adjacent tiles of given tile1 and compares if tile2 is one of them(if it's adjacent to tile1).
	/// </summary>
	/// <param name="tile1">Main tile to check adjacency.</param>
	/// <param name="tile2">Tile to be compared to tile1's adjacency.</param>
	/// <returns></returns>
	private bool CheckAdjacentTiles(BaseTile tile1, BaseTile tile2)
	{
		bool rBool = false;
		List<BaseTile> adjacentTiles = new List<BaseTile>();
		//Right tile check
		if (tile1.Row != BoardManager.row -1) // If it's not at the rightmost of the board
			adjacentTiles.Add(BoardManager.Instance.GetTileInfo(tile1.Row + 1, tile1.Column)); // Add tile at the right of the tile1
		//Left tile check
		if (tile1.Row != 0) //If it's not at the leftmost of the board
			adjacentTiles.Add(BoardManager.Instance.GetTileInfo(tile1.Row - 1, tile1.Column)); // Add tile at the left of the tile1
		//Bottom tile check
		if (tile1.Column != BoardManager.column -1)// If it's not at the bottom of the board
			adjacentTiles.Add(BoardManager.Instance.GetTileInfo(tile1.Row, tile1.Column + 1)); // Add tile under tile1
		//Top tile check
		if (tile1.Column != 0)// If it's not at the top of the board
			adjacentTiles.Add(BoardManager.Instance.GetTileInfo(tile1.Row, tile1.Column - 1)); // Add tile above tile1

		foreach (var tile in adjacentTiles)
		{
			if (tile == tile2) // if any of adjacent tiles is the tile that has been selected by player.
			{
				print("Tile is Adjacent!");
				rBool = true;
			}
		}
		return rBool;
	}

	//List<RaycastResult> results = new List<RaycastResult>();
	void Update ()
	{
#if UNITY_EDITOR
		if (gameState == GameState.Idle)
		{
			//if (Input.GetKey(KeyCode.Mouse0))
			//{
			//	//Code to be place in a MonoBehaviour with a GraphicRaycaster component
			//	GraphicRaycaster gr = GameObject.Find("GameBoardCanvas").GetComponent<GraphicRaycaster>();
			//	//Create the PointerEventData with null for the EventSystem
			//	PointerEventData ped = new PointerEventData(null);
			//	//Set required parameters, in this case, mouse position
			//	ped.position = Input.mousePosition;
			//	//Create list to receive all results
				
			//	//Raycast it
			//	gr.Raycast(ped, results);

   //         }
			//else if(Input.GetKeyUp(KeyCode.Mouse0))
			//{
			//	foreach (var item in results)
			//	{
			//		print(item.gameObject.name);
			//	}
			//}

			
        }

		#endif

	}
}
