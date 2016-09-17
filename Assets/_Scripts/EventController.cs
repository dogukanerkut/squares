//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
/// <summary>
/// Attached To: Block
/// Description: Controls the Event System(player's input)
/// </summary>
public class EventController : MonoBehaviour
{
	
	private BoardManager boardManager; // Assigned in Editor

	void Awake()
	{
		boardManager = GameObject.FindGameObjectWithTag(Constants.Tag_BoardManager).GetComponent<BoardManager>();
	}

	public void OnPointerEnter()
	{
		if (BoardManager.gameState == GameState.SelectionStarted) // if the selection started during OnPointerEnter event
		{
			Block selectedBlock = GetComponent<Block>();
			boardManager.SetSelectedBlock(selectedBlock);
		}
		
	}
	public void OnPointerDown()
	{
		Block selectedBlock = GetComponent<Block>();
			boardManager.StartSelection(selectedBlock);
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			boardManager.TerminateSelection();
		}
	}

}
