using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class BoardGenerator : MonoBehaviour
{

	// Use this for initialization
	public GameObject blockPrefab;
	public int row;
	public int column;
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
				blocks[i, j] = tempBlock;

				tempBlock.GetComponent<Block>().SetBoardID(j, i);
			}
		}
	} 
	// Update is called once per frame
	void Update () {
	
	}
}
