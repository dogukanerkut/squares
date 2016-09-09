using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	// Use this for initialization
	private int row;
	private int column;

	public int Row
	{
		get
		{
			return row;
		}

		set
		{
			row = value;
		}
	}

	public int Column
	{
		get
		{
			return column;
		}

		set
		{
			column = value;
		}
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetBoardID(int _row, int _column)
	{
		Row = _row;
		Column = _column;
	}
}
