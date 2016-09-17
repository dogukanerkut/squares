//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Description: Responsible from assigning colors to newly created blocks and adjusting difficulty(increasing color palette).
/// </summary>
public class ColorBase
{
	private List<Color> currentColors = new List<Color>();
	private List<Color> difficultyColors = new List<Color>();
	public static Color defaultColor = Color.white; // default is white unless it is modified in BoardManager
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
	/// <summary>
	/// Increases Difficulty of the game(adds another color in our color pool
	/// </summary>
	public void IncreaseDifficulty()
	{
		if (difficultyColors.Count > 0)
		{
			currentColors.Add(difficultyColors[0]);
			difficultyColors.RemoveAt(0);
			isDifficultyJustIncreased = true;
		}
	}
	/// <summary>
	/// This is for loading game, we need to iterate our difficulty brackets to reach the difficulty level our loaded game.
	/// </summary>
	/// <param name="difficultyBracket">Difficulty bracket.</param>
	public void IncreaseDifficulty(int difficultyBracket)
	{
		for (int i = 0; i < difficultyBracket; i++)
			IncreaseDifficulty();
	}

	/// <summary>
	/// Fills color of given list of block infos randomly, preventing to have 4 blocks with same color and makes sure to include the new color if difficulty is just increased
	/// </summary>
	/// <param name="blockInfos"></param>
	public void FillColorInfo(List<BlockInfo> blockInfos)
	{
		for (int i = 0; i < blockInfos.Count; i++)
		{
			blockInfos[i].BlockColor = new SerializableColor(GetRandomColor());
		}
		//if blocks are 4 or more, look if all blocks are same color 
		if (blockInfos.Count >= 4)
		{
			bool isAllBlocksSameColor = true;
			for (int i = 1; i < blockInfos.Count; i++)
			{
				if (blockInfos[0].BlockColor.GetColor() != blockInfos[i].BlockColor.GetColor()) // if even one of them is not same, false allSameColor
				{
					isAllBlocksSameColor = false;
					break;
				}
			}
			IncludeNewColorOnFirstTime(blockInfos);

			PreventAllBlocksToBeSameColor(blockInfos, isAllBlocksSameColor);
		}
		//return blockInfos;
	}
	/// <summary>
	/// If the difficulty is just increased, include the override introduced color to one of our blocks if the blocks do not contain it.
	/// </summary>
	/// <param name="blockInfos"></param>
	private void IncludeNewColorOnFirstTime(List<BlockInfo> blockInfos)
	{
		if (isDifficultyJustIncreased)
		{
			bool isNewColorAdded = false;
			for (int i = 0; i < blockInfos.Count; i++)
			{
				if (currentColors[currentColors.Count - 1] == blockInfos[i].BlockColor.GetColor()) // if new color is not in list
				{
					isNewColorAdded = true;
					break;
				}
			}
			if (!isNewColorAdded) // if new color is not in list override it to a random block
				blockInfos[Random.Range(0, blockInfos.Count - 1)].BlockColor = new SerializableColor(currentColors[currentColors.Count - 1]);
			isDifficultyJustIncreased = false;
		}
	}
	/// <summary>
	/// Prevents all blocks to be same color if they are more than 4
	/// </summary>
	/// <param name="blockInfos"></param>
	/// <param name="isAllBlocksSameColor">if this is true, we have 4 blocks that all of them are same color</param>
	private void PreventAllBlocksToBeSameColor(List<BlockInfo> blockInfos, bool isAllBlocksSameColor)
	{
		while (isAllBlocksSameColor) // while all blocks are same color
		{
			int rnd = Random.Range(0, blockInfos.Count - 1);
			blockInfos[rnd].BlockColor = new SerializableColor(GetRandomColor()); // assign a random color to a random block
			for (int i = 0; i < blockInfos.Count; i++)
			{
				if (blockInfos[0].BlockColor.GetColor() != blockInfos[i].BlockColor.GetColor()) // if even one of them is not same, false allSameColor
				{
					isAllBlocksSameColor = false;
					break;
				}
			}
		}
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
