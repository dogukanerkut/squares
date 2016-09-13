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
/// Description: Responsible from creating blocks and adjusting their random values.
/// </summary>
public class BlocksCreator
{
	public List<BlockInfo> GetRandomBlocks()
	{
		//tryout
		List<BlockInfo> blockGroup;
		int random = Random.Range(0, 100); // chance for getting one or four blocks OR two or three blocks.

		if (random < 65) //"two and three has 65% chance to be retrieved and "one or four" has 35% chance.
			blockGroup = GetTwoOrThreeBlocks();
		else
			//HACK: Temporarily disabled
			blockGroup = GetOneOrFourBlocks();

		return blockGroup;

	}
	private List<BlockInfo> GetOneOrFourBlocks()
	{
		List<BlockInfo> blockGroup;
		int random = Random.Range(0, 100); // 50% 50% chance

		if (random < 50)
			blockGroup = GetBlocks(1);
		else
			blockGroup = GetBlocks(4);
			//HACK: Temporarily disabled
		return blockGroup;
	}

	private List<BlockInfo> GetTwoOrThreeBlocks()
	{
		List<BlockInfo> blockGroup;
		int random = Random.Range(0, 100); // 50% 50% chance

		if (random < 50)
			blockGroup = GetBlocks(2);
		else
			blockGroup = GetBlocks(3);
        return blockGroup;
	}

	private List<BlockInfo> GetBlocks(int index)
	{
		List<BlockInfo> blockGroup = new List<BlockInfo>();
		for (int i = 0; i < index; i++)
		{
			blockGroup.Add(new BlockInfo());
		}
		return blockGroup;
	}

}
