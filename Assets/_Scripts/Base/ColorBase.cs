//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Referring To: 
/// Referenced From: 
/// Attached To: 
/// Description: Responsible from assigning colors to newly created blocks and adjusting difficulty(increasing color palette).
/// </summary>
public class ColorBase
{
	private List<Color> currentColors = new List<Color>();
	private List<Color> difficultyColors = new List<Color>();

	public ColorBase()
	{
		FillBaseColors();
	}
	/// <summary>
	/// Default Color state.
	/// </summary>
	private void FillBaseColors()
	{
		currentColors.Clear();
		currentColors.Add(Color.red);
		currentColors.Add(Color.blue);
		currentColors.Add(Color.green);

		difficultyColors.Clear();
		difficultyColors.Add(Color.cyan);
		difficultyColors.Add(Color.yellow);
		difficultyColors.Add(Color.grey);
	}

	public void IncreaseDifficulty()
	{
		if (difficultyColors.Count > 0)
		{
			currentColors.Add(difficultyColors[0]);
			difficultyColors.RemoveAt(0);
		}
	}

	private Color GetRandomColor()
	{
		return currentColors[Random.Range(0, currentColors.Count)];
	}

	public void FillColorInfo(List<TileInfo> tileInfos)
	{
		for (int i = 0; i < tileInfos.Count; i++)
		{
			tileInfos[i].TileColor = GetRandomColor();
		}
		//return tileInfos;
	}



}
