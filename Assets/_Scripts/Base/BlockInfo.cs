//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
/// <summary>
/// Referring To: 
/// Referenced From: 
/// Attached To: 
/// Description: Base class for holding primitive block data.
/// </summary>
[System.Serializable]
public class BlockInfo
{
	public BlockInfo()
	{
		Row = -1;
		Column = -1;
		blockColor = new SerializableColor(ColorBase.defaultColor);
	}
	public BlockInfo(int _row, int _column, Color _color)
	{
		Row = _row;
		Column = _column;
		BlockColor = new SerializableColor(_color);
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
	private SerializableColor blockColor;
	public SerializableColor BlockColor
	{
		get
		{
			return blockColor;
		}

		set
		{
			blockColor =value;
		}
	}
	/// <summary>
	/// this flag is for controlling adjacent blocks
	/// </summary>
	private bool isChecked;
	public bool IsChecked
	{
		get
		{
			return isChecked;
		}

		set
		{
			isChecked = value;
		}
	}
	/// <summary>
	/// this flag is for controlling empty blocks
	/// </summary>
	private bool isDeadEnd;
	public bool IsDeadEnd
	{
		get
		{
			return isDeadEnd;
		}

		set
		{
			isDeadEnd = value;
		}
	}
	public void Clear()
	{
		isDeadEnd = false;
		isChecked = false;

	}


}
