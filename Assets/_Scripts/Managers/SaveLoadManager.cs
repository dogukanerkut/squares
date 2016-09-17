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
			BlockInfo[,] loadedBlockInfos = (BlockInfo[,])SaveLoad.LoadGame(ref position);
			List<BlockInfo> currentBlockInfos = (List<BlockInfo>)SaveLoad.LoadGame(ref position);
			GameVariables gameVariables = (GameVariables)SaveLoad.LoadGame(ref position);
			List<BlockInfo> hintBlockInfos = (List<BlockInfo>)SaveLoad.LoadGame(ref position);

			BlocksArray blocksToBeLoaded = boardManager.GetBlocksArray();

			for (int i = 0; i < Constants.ColumnCount; i++)
			{
				for (int j = 0; j < Constants.RowCount; j++)
				{
					Block block = blocksToBeLoaded[j, i].GetComponent<Block>();
					block.info = loadedBlockInfos[j, i];
					block.SetColor(loadedBlockInfos[j, i].BlockColor.GetColor());
				}
			}

			boardManager.FillBlocksArray(blocksToBeLoaded, currentBlockInfos);
			boardManager.SetGameVariables(gameVariables);
			if (gameVariables.isHintUsed)
				boardManager.CreateHintBlocksFromSave(hintBlockInfos);

		}

	}
	private void SaveGame()
	{
		if (saveData)
		{
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
