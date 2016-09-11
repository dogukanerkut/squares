﻿//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Referring To: 
/// Referenced From: EventController.cs
/// Attached To: Block
/// Description: Holder class for BlockInfo.
/// </summary>

public class Block : MonoBehaviour {
	public BlockInfo info;

	private Image blockImg;

	void Awake ()
	{
		blockImg = GetComponent<Image>();
	}
	
	public void FillInfo(int _row, int _column, Color _color)
	{
		info = new BlockInfo(_row, _column, _color);
		blockImg.color = _color;
	}
	public void SetColor(Color clr)
	{
		info.BlockColor = clr;
		blockImg.color = clr;
	}
	public void Clear()
	{
		info.BlockColor = Color.white;
		blockImg.color = Color.white;
		info.IsChecked = false;
	}

}