//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// Referring To: 
/// Referenced From: SelectionManager.cs
/// Attached To: BoardManager
/// Description: (Singleton)Manages main board activities like generating a board, holding board data, returning tile data with given index etc.
/// </summary>

public class BoardManager : MonoBehaviour
{
	private static BoardManager instance;
	private BoardManager() { }
	public static BoardManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(BoardManager)) as BoardManager;
			}
			return instance;
		}
	}

	public GameObject blockPrefab;
	public static int row = 5;
	public static int column = 5;
	public RectTransform gamePanel;
	private GameObject[,] blocks;
	void Start ()
	{
		GenerateBoard(row, column);
	}

	/// <summary>
	/// Generates a Board with given row and column parameters and assigns them to [blocks] 2D array.
	/// </summary>
	/// <param name="_row">Row count.</param>
	/// <param name="_column"> Column count.</param>
	void GenerateBoard(int _row, int _column)
	{
		blocks = new GameObject[_row, _column];
		//Instantiate prefabs and assign them to blocks 2D array.
		for (int i = 0; i < _column; i++)
		{
			for (int j = 0; j < _row; j++)
			{
				GameObject tempBlock = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				tempBlock.transform.SetParent(gamePanel);
				tempBlock.transform.localScale = Vector3.one; // Instantiated objects has different scale value than 1 somehow.
				blocks[j, i] = tempBlock;

				tempBlock.GetComponent<TileHolder>().SetupTile(j, i, Color.white); //Set object's row and column
			}
		}
	}
	/// <summary>
	/// Returns BaseTile with given row and column index.
	/// </summary>
	/// <param name="_row"></param>
	/// <param name="_column"></param>
	/// <returns></returns>
	public BaseTile GetTileInfo(int _row, int _column)
	{
		BaseTile tile = null;
		if (_row <= row - 1 && _row >= 0 && _column <= column - 1 && _column >= 0) //safecheck
		{
			tile = blocks[_row, _column].GetComponent<TileHolder>().tile;
		}
		else
		{
			Debug.LogError("Index out of Range!"); // for debugging purpose.
		}
		return tile;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
