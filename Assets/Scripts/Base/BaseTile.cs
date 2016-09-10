using UnityEngine;
using System.Collections;

public class BaseTile
{
	public BaseTile(int _row, int _column, Color _color)
	{
		Row = _row;
		Column = _column;
		TileColor = _color;
	}
	private int column;
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

	private int row;
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
	private Color tileColor;
	public Color TileColor
	{
		get
		{
			return tileColor;
		}

		set
		{
			tileColor = value;
		}
	}
}
