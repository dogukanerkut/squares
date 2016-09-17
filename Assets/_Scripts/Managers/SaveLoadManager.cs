//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Attached To: SaveLoadManager
/// Description: Manager class for our save/load operations.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
	// Use this for initialization
	private BoardManager boardManager;
	public bool saveData;
	public bool loadData;
	void Awake()
	{
		boardManager = GameObject.FindGameObjectWithTag(Constants.Tag_BoardManager).GetComponent<BoardManager>();
		if (loadData) LoadGame();

		#region safecheck
#if UNITY_EDITOR
		if (!saveData) Debug.Log("Game will not save data");
		if (!loadData) Debug.Log("Game will not load data");
#endif
		#endregion
	}

	private void LoadGame()
	{
		if (SaveLoad.IsFileExists())
		{
			long position = 0;
			BlockInfo[,] loadedBlockInfos = (BlockInfo[,])SaveLoad.LoadGame(ref position); //Load all blocks in our grid
			List<BlockInfo> currentBlockInfos = (List<BlockInfo>)SaveLoad.LoadGame(ref position); //Load new blocks
			GameVariables gameVariables = (GameVariables)SaveLoad.LoadGame(ref position); //Load game variables
			List<BlockInfo> hintBlockInfos = (List<BlockInfo>)SaveLoad.LoadGame(ref position); //Load hint blocks

			BlocksArray blocksToBeLoaded = boardManager.GetBlocksArray(); // reference of boardManager's "blocks" we will fill it with loaded data

			for (int i = 0; i < Constants.ColumnCount; i++)
			{
				for (int j = 0; j < Constants.RowCount; j++)
				{
					Block block = blocksToBeLoaded[j, i].GetComponent<Block>();
					block.info = loadedBlockInfos[j, i]; // fill infos of loaded blocks into current empty blocks
					block.SetColor(loadedBlockInfos[j, i].BlockColor.GetColor()); // visually update color of block
				}
			}

			boardManager.FillBlocksArray(blocksToBeLoaded, currentBlockInfos); //Send back loaded grid to BoardManager
			boardManager.SetGameVariables(gameVariables); //Send loaded game variables to Boardmanager
			if (gameVariables.isHintUsed) //send hint blocks if used in previous session
				boardManager.CreateHintBlocksFromSave(hintBlockInfos);
		}
	}
	private void SaveGame()
	{
		if (saveData)
		{
			//get all necessary data and save them
			BlocksArray blocks = boardManager.GetBlocksArray();
			BlockInfo[,] blockInfosToBeSaved = new BlockInfo[Constants.RowCount, Constants.ColumnCount];
			GameVariables gameVariables = boardManager.GetGameVariables();
			for (int i = 0; i < Constants.ColumnCount; i++)
			{
				for (int j = 0; j < Constants.RowCount; j++)
				{
					blockInfosToBeSaved[j, i] = blocks[j, i].GetComponent<Block>().info;
					//BlockInfo bInfo = loadedBlocks[j, i].GetComponent<BlockInfo>();
					//bInfo = loadedBlockInfos[j, i];
				}
			}
			//save all necessary data
			SaveLoad.SaveGame(blockInfosToBeSaved, boardManager.GetBlocks(BoardManager.BlockCreationType.Actual), gameVariables, boardManager.GetBlocks(BoardManager.BlockCreationType.Hint));
		}
	}
	// Update is called once per frame
#if UNITY_ANDROID
	void OnApplicationPause()
	{
		SaveGame();
	}
#endif
#if UNITY_EDITOR
	void OnApplicationQuit()
	{
		SaveGame();
	}
#endif
}
