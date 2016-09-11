﻿//Created by Doğukan Erkut.
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
public class BlockInfo
{
	public BlockInfo()
	{
		Row = -1;
		Column = -1;
		blockColor = Color.white;
	}
	public BlockInfo(int _row, int _column, Color _color)
	{
		Row = _row;
		Column = _column;
		BlockColor = _color;
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
	private Color blockColor;
	public Color BlockColor
	{
		get
		{
			return blockColor;
		}

		set
		{
			blockColor = value;
		}
	}

	private bool tested;
	public bool IsChecked
	{
		get
		{
			return tested;
		}

		set
		{
			tested = value;
		}
	}



}