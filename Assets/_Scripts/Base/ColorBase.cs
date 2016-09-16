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
	public static Color defaultColor = Color.white; // default is white unless it is modified somewhere
	private bool isDifficultyJustIncreased;
	public ColorBase()
	{
		ResetToDefault();
	}
	/// <summary>
	/// Default Color state.
	/// </summary>
	public void ResetToDefault()
	{
		//currentColors.Clear();
		//currentColors.Add(ConvertTo1(179, 255, 135)); // green
		//currentColors.Add(ConvertTo1(116, 255, 255)); // blue
		//currentColors.Add(ConvertTo1(127, 54, 255)); // purple

		//difficultyColors.Clear();
		//difficultyColors.Add(ConvertTo1(201, 25, 96)); // red-ish
		//difficultyColors.Add(ConvertTo1(246, 255, 96)); // yellow
		//difficultyColors.Add(ConvertTo1(255, 105, 24)); // orange
		//difficultyColors.Add(ConvertTo1(245, 148, 255)); // pink
		//difficultyColors.Add(ConvertTo1(62, 62, 62)); // black-ish
		currentColors.Clear();
		currentColors.Add(ConvertTo1(164, 243, 11)); // green
		currentColors.Add(ConvertTo1(62, 199, 191)); // blue
		currentColors.Add(ConvertTo1(104, 42, 173)); // purple

		difficultyColors.Clear();
		difficultyColors.Add(ConvertTo1(201, 6, 83)); // red-ish
		difficultyColors.Add(ConvertTo1(251, 227, 22)); // yellow
		difficultyColors.Add(ConvertTo1(242, 93, 12)); // orange
		difficultyColors.Add(ConvertTo1(183, 37, 199)); // pink
		difficultyColors.Add(ConvertTo1(41, 41, 41)); // black-ish
	}

	public void IncreaseDifficulty()
	{
		if (difficultyColors.Count > 0)
		{
			currentColors.Add(difficultyColors[0]);
			difficultyColors.RemoveAt(0);
			isDifficultyJustIncreased = true;
		}
	}
	public void IncreaseDifficulty(int difficultyBracket)
	{
		for (int i = 0; i < difficultyBracket; i++)
			IncreaseDifficulty();
	}

	public void FillColorInfo(List<BlockInfo> blockInfos)
	{
		for (int i = 0; i < blockInfos.Count; i++)
		{
			blockInfos[i].BlockColor = new SerializableColor(GetRandomColor());
		}
		//include new color if it's not added to new blocks
		if (isDifficultyJustIncreased)
		{
			bool isNewColorAdded = false;
			for (int i = 0; i < blockInfos.Count; i++)
			{
				if (currentColors[currentColors.Count - 1] ==blockInfos[i].BlockColor.GetColor())
				{
					isNewColorAdded = true;
					break;
				}
            }
			if (!isNewColorAdded)
				blockInfos[0].BlockColor = new SerializableColor(currentColors[currentColors.Count - 1]);
			isDifficultyJustIncreased = false;
		}
		//return blockInfos;
	}
	public Color GetLatestColor()
	{
		return currentColors[currentColors.Count - 1];
	}

	private Color GetRandomColor()
	{
		return currentColors[Random.Range(0, currentColors.Count)];
	}
	public int GetTotalDifficulty()
	{
		return difficultyColors.Count;
	}

	private Color ConvertTo1(float r, float g, float b)
	{
		return new Color(r / 255, g / 255, b / 255);
	}

}
