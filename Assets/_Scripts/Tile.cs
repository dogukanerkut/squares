//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Referring To: 
/// Referenced From: EventController.cs
/// Attached To: Tile
/// Description: Holder class for TileInfo.
/// </summary>

public class Tile : MonoBehaviour {
	public TileInfo info;

	private Image tileImg;
	void Awake ()
	{
		tileImg = GetComponent<Image>();
	}
	
	public void FillInfo(int _row, int _column, Color _color)
	{
		info = new TileInfo(_row, _column, _color);
		tileImg.color = _color;
	}
	public void SetColor(Color clr)
	{
		info.TileColor = clr;
		tileImg.color = clr;
	}
	public void Clear()
	{
		info.TileColor = Color.white;
		tileImg.color = Color.white;
	}

}
