//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
/// <summary>
/// Referring To: SelectionManager.cs
/// Referenced From: 
/// Attached To: Tile
/// Description: Controls the Event System.
/// </summary>

public class EventController : MonoBehaviour
{
	
	private SelectionManager selectionManager; // Assigned in Editor

	void Awake()
	{
		selectionManager = GameObject.FindGameObjectWithTag(Constants.Tag_SelectionManager).GetComponent<SelectionManager>();
	}

	public void OnPointerEnter()
	{
		print(selectionManager.SelectionStarted);
		if (selectionManager.SelectionStarted) // if the selection started during OnPointerEnter event
		{
			TileHolder selectedTile = GetComponent<TileHolder>();

			if (selectedTile.tile.TileColor == Color.white)
			{
				//selectionManager.SetSelectedTile(selectedTile);
				if (selectionManager.IsTilesAdjacentAndAvailable(selectedTile))
				{
					selectedTile.SetColor(Color.red);
				}
				
			}
			//print(selectionManager.PreviousTile());
			//print(selectionManager.CurrentTile());
		}
		
	}
	public void OnPointerDown()
	{
		TileHolder selectedTile = GetComponent<TileHolder>();
		if (selectedTile.tile.TileColor == Color.white)
		{
			selectionManager.StartSelection(selectedTile);
			selectedTile.SetColor(Color.red);
		}
	}
	public void OnPointerExit()
	{
		//selectionManager.EndSelection();
	}

}
