//Created by Doğukan Erkut.
//Copyright(c) 2016 Doğukan Erkut all rights reserved.
//Contact: dogukanerkut@gmail.com
using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
/// <summary>
/// Description: Save/load operations of the game.
/// </summary>
public static class SaveLoad
{
	private static string savePath = Application.persistentDataPath + "/gamesave.data"; // in android this requires external card permission
	/// <summary>
	/// Saves the current session of the game
	/// </summary>
	/// <param name="gridBlockInfos">Current grid state.</param>
	/// <param name="currentBlockInfos">Current new blocks.</param>
	/// <param name="gameVariables">Game variables.</param>
	/// <param name="hintBlocks">List of hint blocks</param>
	public static void SaveGame(BlockInfo[,] gridBlockInfos, List<BlockInfo> currentBlockInfos, GameVariables gameVariables, List<BlockInfo> hintBlocks)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate);
		bf.Serialize(fs, gridBlockInfos);
		bf.Serialize(fs, currentBlockInfos);
		bf.Serialize(fs, gameVariables);
		bf.Serialize(fs, hintBlocks);
		fs.Close();
	}
	/// <summary>
	/// Load the saved game.
	/// </summary>
	/// <param name="position">FileStream's current position</param>
	/// <returns></returns>
	public static object LoadGame(ref long position)
	{
		object obj = null;
		if (File.Exists(savePath))
		{
			FileStream fs = new FileStream(savePath, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			fs.Seek(position, SeekOrigin.Begin);
			obj = bf.Deserialize(fs);
			position = fs.Position;
			fs.Close();
        }
		return obj;
	}
	public static bool IsFileExists()
	{
		return File.Exists(savePath);
	}
}
