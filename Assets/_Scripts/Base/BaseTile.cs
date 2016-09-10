//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
/// <summary>
/// Referring To: 
/// Referenced From: 
/// Attached To: 
/// Description: Base class for holding primitive tile data.
/// </summary>
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
