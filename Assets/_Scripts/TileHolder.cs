//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Referring To: 
/// Referenced From: EventController.cs
/// Attached To: Tile
/// Description: Holder class for BaseTile.
/// </summary>

public class TileHolder : MonoBehaviour {
	public BaseTile tile;

	private Image tileImg;
	void Awake ()
	{
		tileImg = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetupTile(int _row, int _column, Color _color)
	{
		tile = new BaseTile(_row, _column, _color);
		tileImg.color = _color;
	}
	public void SetColor(Color clr)
	{
		tile.TileColor = clr;
		tileImg.color = clr;
	}
	public void Clear()
	{
		tile.TileColor = Color.white;
		tileImg.color = Color.white;
	}

}
