//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
/// <summary>
/// Referring To: SelectionManager.cs
/// Referenced From: 
/// Attached To: Block
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
		if (SelectionManager.gameState == GameState.SelectionStarted) // if the selection started during OnPointerEnter event
		{
			Block selectedBlock = GetComponent<Block>();
			selectionManager.SetSelectedBlock(selectedBlock);
		}
		
	}
	public void OnPointerDown()
	{
		Block selectedBlock = GetComponent<Block>();
			selectionManager.StartSelection(selectedBlock);
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			selectionManager.TerminateSelection();
		}
	}

}
